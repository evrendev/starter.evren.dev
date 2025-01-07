using System.Globalization;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;

namespace EvrenDev.PublicApi.Localization;

public class JsonStringLocalizer : IStringLocalizer
{
    private readonly IDistributedCache _cache;
    private readonly JsonSerializer _serializer = new();

    public JsonStringLocalizer(IDistributedCache cache)
    {
        _cache = cache;
    }

    public LocalizedString this[string name]
    {
        get
        {
            string? value = GetString(name);
            return new LocalizedString(name, value ?? name, value == null);
        }
    }

    public LocalizedString this[string name, params object[] arguments]
    {
        get
        {
            LocalizedString actualValue = this[name];
            return !actualValue.ResourceNotFound
                ? new LocalizedString(name, string.Format(actualValue.Value, arguments), false)
                : actualValue;
        }
    }

    public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
    {
        string filePath = $"Resources/{CultureInfo.CurrentCulture.Name}.json";

        if (!File.Exists(filePath))
            yield break;

        using var str = File.OpenRead(filePath);
        using var sReader = new StreamReader(str);
        using var reader = new JsonTextReader(sReader);

        while (reader.Read())
        {
            if (reader.TokenType != JsonToken.PropertyName)
                continue;

            string? key = (string?)reader.Value;
            reader.Read();
            string? value = _serializer.Deserialize<string>(reader);
            yield return new LocalizedString(key ?? string.Empty, value ?? string.Empty, false);
        }
    }

    private string? GetString(string key)
    {
        string filePath = $"Resources/{CultureInfo.CurrentCulture.Name}.json";
        if (!File.Exists(filePath))
            return null;

        string cacheKey = $"locale_{CultureInfo.CurrentCulture.Name}_{key}";
        string? cachedValue = _cache.GetString(cacheKey);
        if (!string.IsNullOrEmpty(cachedValue))
            return cachedValue;

        string? result = GetValueFromJSON(key, filePath);
        if (!string.IsNullOrEmpty(result))
            _cache.SetString(cacheKey, result);

        return result;
    }

    private string? GetValueFromJSON(string propertyName, string filePath)
    {
        if (string.IsNullOrEmpty(propertyName) || string.IsNullOrEmpty(filePath))
            return null;

        using var str = File.OpenRead(filePath);
        using var sReader = new StreamReader(str);
        using var reader = new JsonTextReader(sReader);

        while (reader.Read())
        {
            if (reader.TokenType == JsonToken.PropertyName && (string?)reader.Value == propertyName)
            {
                reader.Read();
                return _serializer.Deserialize<string>(reader);
            }
        }

        return null;
    }
}
