using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Forestry.Eapc.External.Web.Models.Application
{
    public class Section1
    {
        [DisplayName("Name of Exporter")]
        [StringLength(DataValueConstants.DefaultSingleLineTextMaxLength)]
        public string? ExporterName { get; set; }

        public Address ExporterAddress { get; set; } = new();
    }
}