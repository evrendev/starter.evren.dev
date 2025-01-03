
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Infrastructure.Data.Helper;
public static class ValueConverterHelper<T>
{
    public static ValueConverter<List<T>?, string?> CreateJsonValueConverter()
    {
        return new ValueConverter<List<T>?, string?>(
            v => v != null ? JsonConvert.SerializeObject(v) : null,
            v => v != null ? JsonConvert.DeserializeObject<List<T>>(v) : null);
    }

    public static ValueComparer<List<T>?> CreateJsonValueComparer()
    {
        return new ValueComparer<List<T>?>(
            (c1, c2) => c1!.SequenceEqual(c2!),
            c => c!.Aggregate(0, (a, v) => HashCode.Combine(a, v!.GetHashCode())),
            c => c!.ToList());
    }
}
