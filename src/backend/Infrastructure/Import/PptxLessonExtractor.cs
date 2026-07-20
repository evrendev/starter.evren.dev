using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using DocumentFormat.OpenXml.Packaging;
using EvrenDev.Domain.Catalog;
using A = DocumentFormat.OpenXml.Drawing;
using P = DocumentFormat.OpenXml.Presentation;

namespace EvrenDev.Infrastructure.Import;

public record ExtractedSlideMedia(byte[] Bytes, string Extension, string ContentType);

public class ExtractedSlide
{
    public required string Title { get; init; }
    public required string ContentHtml { get; init; }
    public required PageContentType ContentType { get; init; }
    public string? EmbedUrl { get; init; }
    public ExtractedSlideMedia? Media { get; init; }
}

// Scaffold-stage-only intentional simplification: PPTX distinguishes "likely video,
// but not embedded and not a recognized hyperlink" (spec case e) from plain text (case
// d) — both end up ContentType.Text with NeedsReview=true (the caller sets that flag
// unconditionally for every imported page), so there is no observable difference and
// no separate detection path is implemented for case (e).
public static class PptxLessonExtractor
{
    private static readonly string[] VideoEmbedDomains = ["youtube.com", "youtu.be", "vimeo.com"];

    public static IEnumerable<(int SlideNumber, SlidePart SlidePart)> OpenSlides(PresentationDocument document)
    {
        var presentationPart = document.PresentationPart
            ?? throw new InvalidOperationException("The .pptx file has no presentation part.");

        var slideIds = presentationPart.Presentation.SlideIdList?.Elements<P.SlideId>().ToList() ?? [];

        var index = 0;
        foreach (var slideId in slideIds)
        {
            index++;
            var relationshipId = slideId.RelationshipId?.Value;
            if (relationshipId is null)
                continue;

            yield return (index, (SlidePart)presentationPart.GetPartById(relationshipId));
        }
    }

    public static ExtractedSlide ExtractSlide(SlidePart slidePart, int slideNumber)
    {
        var shapes = slidePart.Slide.Descendants<P.Shape>().ToList();

        var title = ExtractTitle(shapes) ?? $"Slide {slideNumber}";
        var contentHtml = ExtractBodyHtml(shapes);

        var parts = slidePart.Parts.Select(p => p.OpenXmlPart).ToList();

        // a) Embedded video: any part in this slide with a video/* content type
        var videoPart = parts.FirstOrDefault(p =>
            p.ContentType.StartsWith("video/", StringComparison.OrdinalIgnoreCase));
        if (videoPart is not null)
        {
            return new ExtractedSlide
            {
                Title = title,
                ContentHtml = contentHtml,
                ContentType = PageContentType.Video,
                Media = ReadMedia(videoPart),
            };
        }

        // b) Hyperlink to a known video-hosting domain
        var embedUrl = slidePart.HyperlinkRelationships
            .Select(r => r.Uri.ToString())
            .FirstOrDefault(uri =>
                VideoEmbedDomains.Any(domain => uri.Contains(domain, StringComparison.OrdinalIgnoreCase)));
        if (embedUrl is not null)
        {
            return new ExtractedSlide
            {
                Title = title,
                ContentHtml = contentHtml,
                ContentType = PageContentType.Embed,
                EmbedUrl = embedUrl,
            };
        }

        // c) A dominant single image, only when there isn't much competing text
        var imageParts = parts.OfType<ImagePart>().ToList();
        if (imageParts.Count == 1 && CountWords(contentHtml) < 40)
        {
            return new ExtractedSlide
            {
                Title = title,
                ContentHtml = contentHtml,
                ContentType = PageContentType.Image,
                Media = ReadMedia(imageParts[0]),
            };
        }

        // d) / e) default: plain text
        return new ExtractedSlide
        {
            Title = title,
            ContentHtml = contentHtml,
            ContentType = PageContentType.Text,
        };
    }

    private static ExtractedSlideMedia ReadMedia(OpenXmlPart part)
    {
        using var stream = part.GetStream();
        using var memory = new MemoryStream();
        stream.CopyTo(memory);

        return new ExtractedSlideMedia(memory.ToArray(), GuessExtension(part.ContentType), part.ContentType);
    }

    private static string? ExtractTitle(List<P.Shape> shapes)
    {
        var titleShape = shapes.FirstOrDefault(IsTitlePlaceholder);
        var text = titleShape is null ? null : GetShapeText(titleShape);

        return string.IsNullOrWhiteSpace(text) ? null : text.Trim();
    }

    private static string ExtractBodyHtml(List<P.Shape> shapes)
    {
        var sb = new StringBuilder();

        foreach (var shape in shapes.Where(s => !IsTitlePlaceholder(s)))
        {
            if (shape.TextBody is null)
                continue;

            AppendParagraphsAsHtml(sb, shape.TextBody.Elements<A.Paragraph>());
        }

        return sb.ToString();
    }

    private static bool IsTitlePlaceholder(P.Shape shape)
    {
        var placeholder = shape.NonVisualShapeProperties?.ApplicationNonVisualDrawingProperties
            ?.GetFirstChild<P.PlaceholderShape>();
        var type = placeholder?.Type?.Value;

        return type == P.PlaceholderValues.Title || type == P.PlaceholderValues.CenteredTitle;
    }

    private static void AppendParagraphsAsHtml(StringBuilder sb, IEnumerable<A.Paragraph> paragraphs)
    {
        var inList = false;

        foreach (var paragraph in paragraphs)
        {
            var text = string.Concat(paragraph.Descendants<A.Text>().Select(t => t.Text));
            if (string.IsNullOrWhiteSpace(text))
                continue;

            // Preserve bullet vs. plain-paragraph structure where the slide XML makes it
            // explicit; anything else (indentation levels, numbering styles, etc.) is
            // intentionally not modeled — "olabildiğince koru", not a full PPTX->HTML engine
            var isBullet = paragraph.ParagraphProperties?.Descendants<A.CharacterBullet>().Any() == true
                || paragraph.ParagraphProperties?.Descendants<A.AutoNumberedBullet>().Any() == true;

            if (isBullet)
            {
                if (!inList)
                {
                    sb.Append("<ul>");
                    inList = true;
                }

                sb.Append("<li>").Append(WebUtility.HtmlEncode(text.Trim())).Append("</li>");
            }
            else
            {
                if (inList)
                {
                    sb.Append("</ul>");
                    inList = false;
                }

                sb.Append("<p>").Append(WebUtility.HtmlEncode(text.Trim())).Append("</p>");
            }
        }

        if (inList)
            sb.Append("</ul>");
    }

    private static string GetShapeText(P.Shape shape) =>
        shape.TextBody is null
            ? string.Empty
            : string.Concat(shape.TextBody.Descendants<A.Text>().Select(t => t.Text));

    private static int CountWords(string html) =>
        Regex.Replace(html, "<[^>]+>", " ", RegexOptions.None, TimeSpan.FromSeconds(1))
            .Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries)
            .Length;

    private static string GuessExtension(string contentType) => contentType switch
    {
        "image/png" => ".png",
        "image/jpeg" => ".jpg",
        "image/gif" => ".gif",
        "video/mp4" => ".mp4",
        "video/quicktime" => ".mov",
        "video/webm" => ".webm",
        _ => "",
    };
}
