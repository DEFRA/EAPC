using Microsoft.AspNetCore.Builder;

namespace Forestry.Eapc.External.Web.Infrastructure.TestUser
{
    /// <summary>
    /// Extension methods for registering the <see cref="TestUserMiddleware"/>.
    /// </summary>
    public static class TestUserMiddlewareExtensions
    {
        /// <summary>
        /// Registers an instance of the <see cref="TestUserMiddleware"/> as middleware on the provided
        /// <paramref name="builder"/>.
        /// </summary>
        /// <param name="builder">The application builder used as part of the startup pipeline.</param>
        /// <returns>The provided builder.</returns>
        public static IApplicationBuilder UseTestUser(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<TestUserMiddleware>();
        }
    }
}
