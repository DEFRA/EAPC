using System.Collections.Generic;
using System.Linq;
using Forestry.Eapc.External.Web.Models.Application;
using Forestry.Eapc.External.Web.Services.Repositories.DataVerse;
using Xunit;

namespace Forestry.Eapc.External.Web.Tests.Services.DataVerse
{
    public class ModelMapTests
    {
        [Fact]
        public void ParsesEmptyListOfQuantitiesToBlankCrmString()
        {
            var result = ModelMap.ParseConsignmentQuantityToCrmString(new List<Quantity>());
            Assert.Equal(string.Empty, result);
        }

        [Theory]
        [InlineData(1, QuantityUnit.KG, "1 KG")]
        [InlineData(2, QuantityUnit.M3, "2 M3")]
        public void ParsesOneQuantityWithUnitToCrmString(int amount, QuantityUnit unit, string expectedValue)
        {
            var quantity = new Quantity {Amount = amount, Unit = unit};
            var result = ModelMap.ParseConsignmentQuantityToCrmString(new List<Quantity> {quantity});
            Assert.Equal(expectedValue, result);
        }

        [Fact]
        public void ParsesOneQuantityWithOtherUnitToCrmString()
        {
            var quantity = new Quantity {Amount = 7, Unit = QuantityUnit.Other, OtherText = " Flobbles "};
            var result = ModelMap.ParseConsignmentQuantityToCrmString(new List<Quantity> {quantity});
            Assert.Equal("7 Flobbles", result);
        }

        [Fact]
        public void ParsesMultipleQuantitiesToCrmString()
        {
            var result = ModelMap.ParseConsignmentQuantityToCrmString(new List<Quantity>
            {
                new Quantity {Amount = 1, Unit = QuantityUnit.KG},
                new Quantity {Amount = 2, Unit = QuantityUnit.M3},
                new Quantity {Amount = 7, Unit = QuantityUnit.Other, OtherText = "Flobbles"},
            });
            Assert.Equal("1 KG\r\n2 M3\r\n7 Flobbles", result);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("\n")]
        [InlineData("\r")]
        [InlineData("\r\n")]
        [InlineData("\t\t")]
        public void ParsesEmptyCrmStringToEmptyListOfQuantities(string value)
        {
            var result = ModelMap.ParseCrmConsignmentQuantityString(value);
            Assert.Empty(result);
        }

        [Theory]
        [InlineData("1 KG", 1, QuantityUnit.KG, null)]
        [InlineData("1 kg", 1, QuantityUnit.KG, null)]
        [InlineData(" 2 M3 ", 2, QuantityUnit.M3, null)]
        [InlineData(" 3   M3", 3, QuantityUnit.M3, null)]
        [InlineData("7 Flobbles", 7, QuantityUnit.Other, "Flobbles")]
        public void ParsesCrmStringToSingleQuantity(string value, int expectedAmount, QuantityUnit expectedUnit, string? expectedOtherText)
        {
            var result = ModelMap.ParseCrmConsignmentQuantityString(value);
            var quantity = result.Single();
            Assert.Equal(expectedAmount, quantity.Amount);
            Assert.Equal(expectedUnit, quantity.Unit);
            Assert.Equal(expectedOtherText, quantity.OtherText);
        }

        [Fact]
        public void ParseMultipleLinesOfTextToQuantities()
        {
            var inputText = "1 KG\r\n 2 M3 \r\n\r\n  7  Flobbles ";
            var result = ModelMap.ParseCrmConsignmentQuantityString(inputText);
            Assert.Equal(3, result.Count);
            Assert.Contains(result, x => x.Unit == QuantityUnit.KG && x.Amount == 1 && x.OtherText == null);
            Assert.Contains(result, x => x.Unit == QuantityUnit.M3 && x.Amount == 2 && x.OtherText == null);
            Assert.Contains(result, x => x.Unit == QuantityUnit.Other && x.Amount == 7 && x.OtherText == "Flobbles");

        }
    }
}
