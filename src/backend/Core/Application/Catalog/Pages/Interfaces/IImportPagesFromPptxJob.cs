using System.ComponentModel;

namespace EvrenDev.Application.Catalog.Pages.Interfaces;

public interface IImportPagesFromPptxJob : IScopedService
{
    [DisplayName("Import pages from an uploaded .pptx file")]
    Task ExecuteAsync(Guid importJobId, Guid courseId, string filePath, string userId,
        List<string>? slidesHtml, CancellationToken cancellationToken);
}
