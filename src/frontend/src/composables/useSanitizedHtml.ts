import DOMPurify from "dompurify";

export function useSanitizedHtml() {
  const sanitize = (html: string | undefined): string => {
    if (!html) return "";
    return DOMPurify.sanitize(html, {
      ALLOWED_TAGS: [
        "p",
        "br",
        "strong",
        "em",
        "u",
        "h1",
        "h2",
        "h3",
        "h4",
        "h5",
        "h6",
        "ul",
        "ol",
        "li",
        "a",
        "img",
        "blockquote",
        "code",
        "pre",
        "table",
        "thead",
        "tbody",
        "tr",
        "td",
        "th",
      ],
      ALLOWED_ATTR: ["href", "target", "src", "alt", "title"],
    });
  };

  return {
    sanitize,
  };
}
