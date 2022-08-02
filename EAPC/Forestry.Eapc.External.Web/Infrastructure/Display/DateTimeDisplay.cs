using System;

namespace Forestry.Eapc.External.Web.Infrastructure.Display
{
    public static class DateTimeDisplay
    {
        public static string GetDateDisplayString(DateTime? value)
        {
            return value.HasValue
                ? value.Value.ToString("dd MMMM yyyy")
                : " - ";
        }

        public static string GetDateTimeDisplayString(DateTime? value)
        {
            return value.HasValue
                ? value.Value.ToString("dd MMMM yyyy HH:mm")
                : " - ";
        }
    }
}
