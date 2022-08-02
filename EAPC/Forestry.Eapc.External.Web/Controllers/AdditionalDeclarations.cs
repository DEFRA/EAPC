using System;
using Forestry.Eapc.External.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Forestry.Eapc.External.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AdditionalDeclarations : ControllerBase
    {
        private readonly AdditionalDeclarationsProvider _provider;

        public AdditionalDeclarations(AdditionalDeclarationsProvider provider)
        {
            _provider = provider ?? throw new ArgumentNullException(nameof(provider));
        }

        [HttpGet]
        public IActionResult Get([FromQuery]AdditionalDeclarationQuery query)
        {
            query ??= new AdditionalDeclarationQuery();
            var queryResults = _provider.GetSuggestions(query);
            return Ok(queryResults);
        }
    }
}
