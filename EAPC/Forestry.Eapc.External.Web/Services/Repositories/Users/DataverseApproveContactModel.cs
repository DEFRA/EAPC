using System.Text.Json.Serialization;

namespace Forestry.Eapc.External.Web.Services.Repositories.Users
{
    /// <summary>
    /// This is a cut-down version of the <see cref="DataverseContactModel"/> to be used when setting the 'cr671_approvedbyprofessionaloperator'
    /// flag as part of the account approval process. Using this model means that we send and update only the field required.
    /// </summary>
    public class DataverseApproveContactModel
    {
        [JsonPropertyName("cr671_approvedbyprofessionaloperator")]
        public bool IsApprovedByProfessionalOperator { get; set; }
    }
}
