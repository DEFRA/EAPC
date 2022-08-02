using System;
using System.ComponentModel.DataAnnotations;

namespace Forestry.Eapc.External.Web.Models.Application
{
    public enum TreatmentType
    {
        [Display(Name = "Chemical treatment")]
        Chemical,
        [Display(Name = "Bark Free")]
        BarkFree,
        [Display(Name = "De-barked")]
        Debarked,
        [Display(Name = "Fumigated")]
        Fumigation,
        [Display(Name = "Heat Treated")]
        HeatTreated,
        [Display(Name = "Kiln Dried")]
        KilnDried,
        [Display(Name = "None")]
        None,
        [Display(Name = "Other (please specify)")]
        Other
    }

    public static class TreatmentTypeExtensions
    {
        public static string ToDisplayString(this TreatmentType value)
        {
            return value switch
            {
                TreatmentType.Chemical => "Chemical",
                TreatmentType.Fumigation => "Fumigation",
                TreatmentType.KilnDried => "Kiln Dried",
                TreatmentType.HeatTreated => "Heat Treated",
                TreatmentType.BarkFree => "Bark Free",
                TreatmentType.Debarked => "Debarked",
                TreatmentType.None => "None",
                _ => "Other"
            };
        }

        public static string CreateCertificateString(TreatmentType? value, string? otherTreatmentTypeText)
        {
            return value switch
            {
                null => string.Empty,
                TreatmentType.Other => otherTreatmentTypeText ?? string.Empty,
                _ => ToDisplayString(value.Value)
            };
        }

        public static (TreatmentType? enumValue, string? otherText) FromDisplayString(string? value)
        {
            if (string.IsNullOrWhiteSpace(value)) return (null, null);

            value = value.Trim();

            if (TreatmentType.Chemical.ToDisplayString().Equals(value, StringComparison.InvariantCultureIgnoreCase))
                return (TreatmentType.Chemical, null);

            if (TreatmentType.Fumigation.ToDisplayString().Equals(value, StringComparison.InvariantCultureIgnoreCase))
                return (TreatmentType.Fumigation, null);

            if (TreatmentType.KilnDried.ToDisplayString().Equals(value, StringComparison.InvariantCultureIgnoreCase))
                return (TreatmentType.KilnDried, null);

            if (TreatmentType.HeatTreated.ToDisplayString().Equals(value, StringComparison.InvariantCultureIgnoreCase))
                return (TreatmentType.HeatTreated, null);

            if (TreatmentType.BarkFree.ToDisplayString().Equals(value, StringComparison.InvariantCultureIgnoreCase))
                return (TreatmentType.BarkFree, null);

            if (TreatmentType.Debarked.ToDisplayString().Equals(value, StringComparison.InvariantCultureIgnoreCase))
                return (TreatmentType.Debarked, null);

            if (TreatmentType.None.ToDisplayString().Equals(value, StringComparison.InvariantCultureIgnoreCase))
                return (TreatmentType.None, null);

            return (TreatmentType.Other, value);
        }
    }
}
