using System.ComponentModel.DataAnnotations;

namespace Forestry.Eapc.External.Web.Models.Application
{
    public enum TransportType
    {
        [Display(Name = "Sea Freight")]
        SeaFreight,
        
        [Display(Name = "Air Freight")]
        AirFreight,

        [Display(Name = "Road (Roll-on / Roll-off Freight)")]
        Road,

        Other
    }

    public static class TransportTypeExtensions
    {
        public static string ToDisplayString(this TransportType value)
        {
            return value switch
            {
                TransportType.SeaFreight => "Sea Freight",
                TransportType.AirFreight => "Air Freight",
                TransportType.Road => "Road",
                _ => "Other"
            };
        }

        public static string CreateCertificateString(TransportType? value, string? otherTreatmentTypeText)
        {
            return value switch
            {
                null => string.Empty,
                TransportType.Other => otherTreatmentTypeText ?? string.Empty,
                _ => ToDisplayString(value.Value)
            };
        }
    }
}