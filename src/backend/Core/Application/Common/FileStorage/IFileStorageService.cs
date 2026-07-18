namespace EvrenDev.Application.Common.FileStorage;

public interface IFileStorageService : ITransientService
{
    Task<string> UploadAsync<T>(FileUploadRequest? request, FileType supportedFileType,
        CancellationToken cancellationToken = default)
        where T : class;

    // Separate from UploadAsync<T> (base64-in-JSON, sized for small images): streams an
    // already-decoded file (e.g. from a multipart upload) straight to a temp working
    // directory. Returns the full path; callers are responsible for removing it via Remove().
    Task<string> SaveTempFileAsync(Stream content, string fileName, CancellationToken cancellationToken = default);

    void Remove(string? path);
}
