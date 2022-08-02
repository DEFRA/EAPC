using System.ComponentModel;

namespace Forestry.Eapc.External.Web.Models.Application
{
    public class Confirmation
    {
        [DisplayName("I confirm that to the best of my knowledge and belief, the information I have given is true and I accept responsibility for ensuring that the appropriate payment for the services provided is made to the Forestry Commission. I understand that it is an offence to provide false information.")]
        public bool AcceptTermsAndConditions { get; set; }
    }
}