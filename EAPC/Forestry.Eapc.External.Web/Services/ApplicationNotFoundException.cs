using System;

namespace Forestry.Eapc.External.Web.Services
{
    public class ApplicationNotFoundException : Exception
    {
        public ApplicationNotFoundException()
        {
            
        }

        public ApplicationNotFoundException(string? message) : base(message)
        {
            
        }
    }
}
