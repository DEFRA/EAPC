using System;
using NodaTime;
using NodaTime.Extensions;

namespace Forestry.Eapc.External.Web.Services
{
    public static class ClockHelper
    {
        public static DateTime GetToday(IClock clock)
        {
            DateTimeZone timeZone = DateTimeZoneProviders.Tzdb.GetZoneOrNull("Europe/London")!;
            var today = clock.InZone(timeZone).GetCurrentDate().ToDateTimeUnspecified();
            return today;
        }
    }
}
