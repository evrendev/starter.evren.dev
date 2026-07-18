using System.ComponentModel;

namespace EvrenDev.Application.Catalog.Lessons.Interfaces;

public interface IImportLessonsFromPptxJob : IScopedService
{
    [DisplayName("Import lessons from an uploaded .pptx file")]
    Task ExecuteAsync(Guid courseId, string filePath, string userId, CancellationToken cancellationToken);
}
