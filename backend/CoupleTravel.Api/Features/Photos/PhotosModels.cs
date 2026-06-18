namespace CoupleTravel.Api.Features.Photos;

public record PresignedUploadRequest(string ContentType, int Width, int Height);

public record PresignedUploadResponse(Guid PhotoId, string UploadUrl, string ThumbUploadUrl);
