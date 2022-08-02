using System;

namespace Forestry.Eapc.External.Web.Services
{
    public class UseCaseExecutionException : Exception
    {
        public UseCaseExecutionException()
        {
            
        }

        public UseCaseExecutionException(string message) : base(message)
        {
            
        }

        public UseCaseExecutionException(string message, Exception innerException) : base(message, innerException)
        {
            
        }
    }
}
