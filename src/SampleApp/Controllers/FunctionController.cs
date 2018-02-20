using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebHooks;
using Microsoft.AspNetCore.WebHooks.Metadata;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace SampleApp.Controllers
{
    public class FunctionController : ControllerBase
    {
        private readonly ILogger _logger;

        public FunctionController(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<FunctionController>();
        }

        [Duplicate]
        [Consumes("application/json", "application/*+json", "text/json")]
        [GeneralWebHook(BodyType = WebHookBodyType.Json)]
        public Task<IActionResult> Execute(JContainer data)
        {
            _logger.LogInformation(0, "Received {DataType}: {Data}", nameof(JContainer), data.ToString());
            return Execute();
        }

        [Duplicate]
        [Consumes("application/x-www-form-urlencoded", "multipart/form-data")]
        [GeneralWebHook(BodyType = WebHookBodyType.Form)]
        public Task<IActionResult> Execute(IFormCollection data)
        {
            _logger.LogInformation(1, "Received {DataType}:", nameof(IFormCollection));
            foreach (var kvp in data)
            {
                _logger.LogInformation(2, "{DataKey}: '{DataValues}'", kvp.Key, kvp.Value.ToString());
            }

            return Execute();
        }

        [Duplicate]
        [Consumes("application/xml", "application/*+xml", "text/xml")]
        [GeneralWebHook(BodyType = WebHookBodyType.Xml)]
        public Task<IActionResult> Execute(XElement data)
        {
            _logger.LogInformation(3, "Received {DataType}: {Data}", nameof(XElement), data.ToString());
            return Execute();
        }

        private async Task<IActionResult> Execute()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var entry = (FunctionsRouteTable.Entry)ControllerContext.ActionDescriptor.Properties[
                typeof(FunctionsRouteTable.Entry)];
            await entry.Execute(ControllerContext.HttpContext);

            return Ok();
        }
    }
}
