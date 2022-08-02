namespace Forestry.Eapc.External.Web.Models.Repository
{
    public class ApiErrorResponse
    {
        public ApiError Error { get; set; }

        public class ApiError
        {
            public string Code { get; set; }

            public string Message { get; set; }
        }
    }
}