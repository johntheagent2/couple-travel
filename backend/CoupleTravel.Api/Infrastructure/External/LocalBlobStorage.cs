namespace CoupleTravel.Api.Infrastructure.External;

public interface IBlobStorage
{
    Task<(string objectKey, string presignedUrl)> GenerateUploadUrlAsync(string prefix, string contentType, CancellationToken ct = default);
    string GetPublicUrl(string objectKey);
}

public class LocalBlobStorage(IConfiguration config, IWebHostEnvironment env) : IBlobStorage
{
    private readonly string _basePath = config["Blob:LocalPath"] ?? Path.Combine(env.ContentRootPath, "uploads");

    public Task<(string objectKey, string presignedUrl)> GenerateUploadUrlAsync(string prefix, string contentType, CancellationToken ct = default)
    {
        var key = $"{prefix}/{Guid.NewGuid()}{MimeToExt(contentType)}";
        Directory.CreateDirectory(Path.Combine(_basePath, Path.GetDirectoryName(key)!));
        var url = $"/v1/blobs/upload?key={Uri.EscapeDataString(key)}";
        return Task.FromResult((key, url));
    }

    public string GetPublicUrl(string objectKey) => $"/v1/blobs/{objectKey}";

    private static string MimeToExt(string mime) => mime switch
    {
        "image/jpeg" => ".jpg",
        "image/png" => ".png",
        "image/webp" => ".webp",
        "image/heic" => ".heic",
        _ => ".bin",
    };
}
