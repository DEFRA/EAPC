using System.Text.Json.Serialization;

namespace Forestry.Eapc.External.Web.Models.Repository
{
    public class QueryMultipleResponseModel<T> where T: class
    {
        [JsonPropertyName("@odata.context")]
        public string Context { get; set; }

        [JsonPropertyName("value")]
        public T[] Values { get; set; }
    }
}