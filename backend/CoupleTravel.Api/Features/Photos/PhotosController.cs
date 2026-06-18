using CoupleTravel.Api.Infrastructure.Middleware;
using Microsoft.AspNetCore.Mvc;

namespace CoupleTravel.Api.Features.Photos;

[ApiController]
[ServiceFilter(typeof(RequireAuthFilter))]
public class PhotosController(PhotosService photos) : ControllerBase
{
    [HttpPost("v1/trips/{tripId:guid}/photos")]
    public async Task<IActionResult> CreateUpload(Guid tripId, [FromBody] PresignedUploadRequest req, CancellationToken ct)
    {
        var coupleId = Guid.Parse(HttpContext.Session.GetString("coupleId")!);
        try
        {
            var result = await photos.CreatePresignedUploadAsync(tripId, coupleId, req, ct);
            return Ok(result);
        }
        catch (KeyNotFoundException) { return NotFound(); }
    }

    [HttpDelete("v1/photos/{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var coupleId = Guid.Parse(HttpContext.Session.GetString("coupleId")!);
        return await photos.DeleteAsync(id, coupleId, ct) ? NoContent() : NotFound();
    }

    // Local dev blob upload endpoint
    [HttpPut("v1/blobs/upload")]
    [IgnoreAntiforgeryToken]
    public async Task<IActionResult> BlobUpload([FromQuery] string key, CancellationToken ct)
    {
        try
        {
            await photos.ReceiveLocalUploadAsync(key, Request.Body, ct);
            return Ok();
        }
        catch (KeyNotFoundException) { return NotFound(); }
    }

    // Local dev blob serve endpoint
    [HttpGet("v1/blobs/{**key}")]
    public IActionResult BlobServe(string key)
    {
        var path = Path.Combine("uploads", key);
        if (!System.IO.File.Exists(path)) return NotFound();
        var ext = Path.GetExtension(path).ToLower();
        var mime = ext switch { ".jpg" or ".jpeg" => "image/jpeg", ".png" => "image/png", ".webp" => "image/webp", _ => "application/octet-stream" };
        return PhysicalFile(Path.GetFullPath(path), mime);
    }
}
