using CoupleTravel.Api.Domain.Entities;
using CoupleTravel.Api.Infrastructure.Data;
using CoupleTravel.Api.Infrastructure.External;
using Microsoft.EntityFrameworkCore;

namespace CoupleTravel.Api.Features.Photos;

public class PhotosService(AppDbContext db, IBlobStorage blob)
{
    public async Task<PresignedUploadResponse> CreatePresignedUploadAsync(
        Guid tripId, Guid coupleId, PresignedUploadRequest req, CancellationToken ct)
    {
        var tripExists = await db.Trips.AnyAsync(
            t => t.Id == tripId && t.CoupleId == coupleId && t.DeletedAt == null, ct);
        if (!tripExists) throw new KeyNotFoundException("Trip not found");

        var (objectKey, uploadUrl) = await blob.GenerateUploadUrlAsync($"trips/{tripId}/photos", req.ContentType, ct);
        var (thumbKey, thumbUrl) = await blob.GenerateUploadUrlAsync($"trips/{tripId}/thumbs", "image/jpeg", ct);

        var maxOrder = await db.Photos
            .Where(p => p.TripId == tripId && p.DeletedAt == null)
            .MaxAsync(p => (int?)p.OrderIndex, ct) ?? -1;

        var photo = new Photo
        {
            TripId = tripId,
            CoupleId = coupleId,
            ObjectKey = objectKey,
            ThumbKey = thumbKey,
            Width = req.Width,
            Height = req.Height,
            OrderIndex = maxOrder + 1,
            CreatedAt = DateTime.UtcNow,
        };
        db.Photos.Add(photo);
        await db.SaveChangesAsync(ct);

        return new PresignedUploadResponse(photo.Id, uploadUrl, thumbUrl);
    }

    public async Task<bool> DeleteAsync(Guid photoId, Guid coupleId, CancellationToken ct)
    {
        var photo = await db.Photos
            .Include(p => p.Trip)
            .FirstOrDefaultAsync(p => p.Id == photoId && p.CoupleId == coupleId && p.DeletedAt == null, ct);
        if (photo is null) return false;
        photo.DeletedAt = DateTime.UtcNow;
        await db.SaveChangesAsync(ct);
        return true;
    }

    public async Task ReceiveLocalUploadAsync(string key, Stream body, CancellationToken ct)
    {
        var photo = await db.Photos.FirstOrDefaultAsync(p => p.ObjectKey == key || p.ThumbKey == key, ct);
        if (photo is null) throw new KeyNotFoundException("Unknown key");

        var dir = Path.GetDirectoryName(key)!;
        Directory.CreateDirectory(Path.Combine("uploads", dir));
        var path = Path.Combine("uploads", key);
        await using var fs = File.Create(path);
        await body.CopyToAsync(fs, ct);
    }
}
