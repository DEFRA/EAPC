using System.ComponentModel.DataAnnotations;

namespace Forestry.Eapc.External.Web.Models.Application
{
    public class Quantity
    {
        public decimal Amount { get; set; }

        public QuantityUnit? Unit { get; set; }

        [StringLength(DataValueConstants.DefaultSingleLineTextMaxLength)]
        public string? OtherText { get; set; }

        public string CreateCertificateString()
        {
            if (Unit == null || Amount == 0)
            {
                return string.Empty;
            }

            if (Unit == QuantityUnit.Other && string.IsNullOrWhiteSpace(OtherText))
            {
                return string.Empty;
            }

            switch (Unit)
            {
                case QuantityUnit.KG:
                    return $"{Amount} KG";
                case QuantityUnit.KGNet:
                    return $"{Amount} KG (net)";
                case QuantityUnit.KGGross:
                    return $"{Amount} KG (gross)";
                case QuantityUnit.M3:
                    return $"{Amount} CUBIC METRES";
                default:
                    return $"{Amount} {OtherText?.Trim()}";
            }
        }
    }

    public enum QuantityUnit
    {
        KG,
        KGNet,
        KGGross,
        M3,
        Other
    }
}