using System.Text.Json.Serialization;

namespace LMS.Web.Services;

public class ApiService
{
    private readonly HttpClient _http;
    private readonly IHttpContextAccessor _ctx;

    public ApiService(IHttpClientFactory factory, IHttpContextAccessor ctx)
    {
        _http = factory.CreateClient("LmsApi");
        _ctx = ctx;
    }

    private void AttachToken()
    {
        var token = _ctx.HttpContext?.Session.GetString("jwt");
        _http.DefaultRequestHeaders.Authorization = string.IsNullOrEmpty(token) ? null
            : new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
    }

    public async Task<T?> GetAsync<T>(string url)
    {
        AttachToken();
        var res = await _http.GetAsync(url);
        return res.IsSuccessStatusCode ? await res.Content.ReadFromJsonAsync<T>() : default;
    }

    public async Task<List<T>> GetListAsync<T>(string url)
    {
        AttachToken();
        var res = await _http.GetAsync(url);
        if (!res.IsSuccessStatusCode) return new List<T>();

        var jsonStr = await res.Content.ReadAsStringAsync();
        using var doc = System.Text.Json.JsonDocument.Parse(jsonStr);
        var options = new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        
        if (doc.RootElement.ValueKind == System.Text.Json.JsonValueKind.Array)
        {
            return System.Text.Json.JsonSerializer.Deserialize<List<T>>(jsonStr, options) ?? new List<T>();
        }
        else if (doc.RootElement.ValueKind == System.Text.Json.JsonValueKind.Object)
        {
            if (doc.RootElement.TryGetProperty("value", out var valueProp) && valueProp.ValueKind == System.Text.Json.JsonValueKind.Array)
            {
                return System.Text.Json.JsonSerializer.Deserialize<List<T>>(valueProp.GetRawText(), options) ?? new List<T>();
            }
        }

        return new List<T>();
    }

    public async Task<(bool Ok, string Body)> PostAsync<T>(string url, T body)
    {
        AttachToken();
        var res = await _http.PostAsJsonAsync(url, body);
        return (res.IsSuccessStatusCode, await res.Content.ReadAsStringAsync());
    }

    public async Task<(bool Ok, string Body)> PutAsync<T>(string url, T body)
    {
        AttachToken();
        var res = await _http.PutAsJsonAsync(url, body);
        return (res.IsSuccessStatusCode, await res.Content.ReadAsStringAsync());
    }

    public async Task<bool> DeleteAsync(string url)
    {
        AttachToken();
        return (await _http.DeleteAsync(url)).IsSuccessStatusCode;
    }
}

public class ODataResult<T>
{
    [JsonPropertyName("value")]
    public List<T> Value { get; set; } = new();
}
