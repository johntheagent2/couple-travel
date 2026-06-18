using System.Text.Json;
using System.Text.Json.Serialization;

namespace CoupleTravel.Api.Infrastructure.External;

public record NominatimResult(
    int PlaceId,
    string DisplayName,
    double Lat,
    double Lon,
    string Type,
    string Class,
    NominatimAddress Address
);

public record NominatimAddress(
    string? City,
    string? Town,
    string? Village,
    string? State,
    string? Country,
    string? CountryCode
);

public class NominatimClient(HttpClient http, IConfiguration config)
{
    private static readonly JsonSerializerOptions JsonOpts = new()
    {
        PropertyNameCaseInsensitive = true,
        NumberHandling = JsonNumberHandling.AllowReadingFromString,
    };

    public async Task<List<NominatimResult>> SearchAsync(string query, CancellationToken ct = default)
    {
        var encoded = Uri.EscapeDataString(query);
        var url = $"https://nominatim.openstreetmap.org/search?q={encoded}&format=json&addressdetails=1&limit=5&featuretype=city";

        var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Add("User-Agent", config["Nominatim:UserAgent"] ?? "couple-travel/1.0");
        request.Headers.Add("Accept-Language", "en");

        var resp = await http.SendAsync(request, ct);
        resp.EnsureSuccessStatusCode();

        var json = await resp.Content.ReadAsStringAsync(ct);
        var raw = JsonSerializer.Deserialize<List<NominatimRaw>>(json, JsonOpts) ?? [];

        return raw.Select(r => new NominatimResult(
            r.PlaceId,
            r.DisplayName,
            double.Parse(r.Lat),
            double.Parse(r.Lon),
            r.Type,
            r.Class,
            r.Address
        )).ToList();
    }

    private class NominatimRaw
    {
        [JsonPropertyName("place_id")] public int PlaceId { get; set; } = 0;
        [JsonPropertyName("display_name")] public string DisplayName { get; set; } = "";
        [JsonPropertyName("lat")] public string Lat { get; set; } = "";
        [JsonPropertyName("lon")] public string Lon { get; set; } = "";
        [JsonPropertyName("type")] public string Type { get; set; } = "";
        [JsonPropertyName("class")] public string Class { get; set; } = "";
        [JsonPropertyName("address")] public NominatimAddress Address { get; set; } = new(null, null, null, null, null, null);
    }
}
