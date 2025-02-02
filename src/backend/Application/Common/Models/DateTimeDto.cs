using System.Globalization;

namespace EvrenDev.Application.Common.Models;

public class DateTimeDto
{
    public DateTime UtcDateTime { get; set; }
    public int Year { get; set; }
    public int Month { get; set; }
    public int Day { get; set; }
    public int Hour { get; set; }
    public int Minute { get; set; }
    public MonthNameFormats? MonthNames { get; set; }
    public string? ShortYear { get; set; }
    public string? ShortDate { get; set; }
    public string? LongDate { get; set; }
    public string? FullShortDate { get; set; }
    public string? FullLongDate { get; set; }
    public string? PluginDate { get; set; }
    public string? DisplayDate { get; set; }
    public string? DisplayDateWithTime { get; set; }
    public string? RawDate { get; set; }

    private DateTimeDto() { }

    public static class Create
    {
        public static DateTimeDto? FromLocal(DateTime? localTime)
        {
            if (!localTime.HasValue) return null;

            var dateTime = localTime.Value;
            var currentCulture = CultureInfo.CurrentCulture;

            var monthNames = new MonthNameFormats
            {
                Short = dateTime.ToString("MMM", currentCulture),
                Long = dateTime.ToString("MMMM", currentCulture)
            };

            return new DateTimeDto
            {
                UtcDateTime = dateTime.ToUniversalTime(),
                Year = dateTime.Year,
                Month = dateTime.Month,
                Day = dateTime.Day,
                Hour = dateTime.Hour,
                Minute = dateTime.Minute,
                MonthNames = monthNames,
                ShortYear = dateTime.ToString("yy", currentCulture),
                ShortDate = dateTime.ToString("d", currentCulture),
                LongDate = dateTime.ToString("D", currentCulture),
                FullShortDate = dateTime.ToString("g", currentCulture),
                FullLongDate = dateTime.ToString("f", currentCulture),
                PluginDate = dateTime.ToString("yyyy-MM-dd", currentCulture),
                DisplayDate = dateTime.ToString("dd MMM yyyy", currentCulture),
                DisplayDateWithTime = dateTime.ToString("dd MMM yyyy HH:mm", currentCulture),
                RawDate = dateTime.ToString("yyyy-MM-ddTHH:mm:ss.fffZ", CultureInfo.InvariantCulture)
            };
        }

        public static DateTimeDto? FromUtc(DateTime? utcTime)
        {
            if (!utcTime.HasValue) return null;

            var dateTime = utcTime.Value;
            var currentCulture = CultureInfo.CurrentCulture;

            var monthNames = new MonthNameFormats
            {
                Short = dateTime.ToString("MMM", currentCulture),
                Long = dateTime.ToString("MMMM", currentCulture)
            };

            return new DateTimeDto
            {
                UtcDateTime = dateTime.ToUniversalTime(),
                Year = dateTime.Year,
                Month = dateTime.Month,
                Day = dateTime.Day,
                Hour = dateTime.Hour,
                Minute = dateTime.Minute,
                MonthNames = monthNames,
                ShortYear = dateTime.ToString("yy", currentCulture),
                ShortDate = dateTime.ToString("d", currentCulture),
                LongDate = dateTime.ToString("D", currentCulture),
                FullShortDate = dateTime.ToString("g", currentCulture),
                FullLongDate = dateTime.ToString("f", currentCulture),
                PluginDate = dateTime.ToString("yyyy-MM-dd", currentCulture),
                DisplayDate = dateTime.ToString("dd MMM yyyy", currentCulture),
                DisplayDateWithTime = dateTime.ToString("dd MMM yyyy HH:mm", currentCulture),
                RawDate = dateTime.ToString("yyyy-MM-ddTHH:mm:ss.fffZ", CultureInfo.InvariantCulture)
            };
        }
    }
}

public class MonthNameFormats
{
    public string? Short { get; set; }
    public string? Long { get; set; }
}
