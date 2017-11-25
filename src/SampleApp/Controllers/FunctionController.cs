using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebHooks;
using Microsoft.AspNetCore.WebHooks.Metadata;
using Newtonsoft.Json.Linq;

namespace SampleApp.Controllers
{
    public class FunctionController : ControllerBase
    {
        [Duplicate]
        [Consumes("application/json", "text/json")]
        [GeneralWebHook(BodyType = WebHookBodyType.Json)]
        public async Task<IActionResult> Execute(string receiverName, string id, string[] events, JObject data)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var entry = (FunctionsRouteTable.Entry)ControllerContext.ActionDescriptor.Properties[typeof(FunctionsRouteTable.Entry)];
            await entry.Execute(ControllerContext.HttpContext);

            return Ok();
        }

        [Duplicate]
        [Consumes("application/x-www-form-urlencoded", "multipart/form-data")]
        [GeneralWebHook(BodyType = WebHookBodyType.Form)]
        public async Task<IActionResult> Execute(string receiverName, string id, string[] events, IFormCollection data)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var entry = (FunctionsRouteTable.Entry)ControllerContext.ActionDescriptor.Properties[typeof(FunctionsRouteTable.Entry)];
            await entry.Execute(ControllerContext.HttpContext);

            return Ok();
        }

        [Duplicate]
        [Consumes("application/xml", "text/xml")]
        [GeneralWebHook(BodyType = WebHookBodyType.Xml)]
        public async Task<IActionResult> Execute(string receiverName, string id, string[] events, XElement data)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var entry = (FunctionsRouteTable.Entry)ControllerContext.ActionDescriptor.Properties[typeof(FunctionsRouteTable.Entry)];
            await entry.Execute(ControllerContext.HttpContext);

            return Ok();
        }
    }
}
