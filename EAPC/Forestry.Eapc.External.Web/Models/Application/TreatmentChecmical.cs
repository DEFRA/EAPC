using System;
using System.ComponentModel.DataAnnotations;

namespace Forestry.Eapc.External.Web.Models.Application
{
    public enum TreatmentChemical
    {
        [Display(Name = "Methyl Bromide")]
        MethylBromide,
        [Display(Name = "Phosphine")]
        Phosphine,
        [Display(Name = "Sulfuryl Fluoride")]
        SulfurylFluoride,
        [Display(Name = "None")]
        None,
        [Display(Name = "Other (please specify)")]
        Other
    }

    public static class TreatmentChemicalExtensions
    {
        public static string ToDisplayString(this TreatmentChemical value)
        {
            return value switch
            {
                TreatmentChemical.MethylBromide => "Methyl Bromide",
                TreatmentChemical.Phosphine => "Phosphine",
                TreatmentChemical.SulfurylFluoride => "Sulfuryl Fluoride",
                TreatmentChemical.None => "None",
                _ => "Other"
            };
        }

        public static string CreateCertificateString(TreatmentChemical? value, string? otherTreatmentChemicalText)
        {
            return value switch
            {
                null => string.Empty,
                TreatmentChemical.Other => otherTreatmentChemicalText ?? string.Empty,
                _ => ToDisplayString(value.Value)
            };
        }

        public static (TreatmentChemical? enumValue, string? otherText) FromDisplayString(string? value)
        {
            if (string.IsNullOrWhiteSpace(value)) return (null, null);

            value = value.Trim();

            if (TreatmentChemical.MethylBromide.ToDisplayString().Equals(value, StringComparison.InvariantCultureIgnoreCase))
                return (TreatmentChemical.MethylBromide, null);

            if (TreatmentChemical.Phosphine.ToDisplayString().Equals(value, StringComparison.InvariantCultureIgnoreCase))
                return (TreatmentChemical.Phosphine, null);

            if (TreatmentChemical.SulfurylFluoride.ToDisplayString().Equals(value, StringComparison.InvariantCultureIgnoreCase))
                return (TreatmentChemical.SulfurylFluoride, null);
            
            if (TreatmentChemical.None.ToDisplayString().Equals(value, StringComparison.InvariantCultureIgnoreCase))
                return (TreatmentChemical.None, null);

            return (TreatmentChemical.Other, value);
        }
    }
}
