using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebHooks;
using Microsoft.Extensions.Logging;

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
        [GeneralWebHook]
        public Task<IActionResult> Execute(string receiverName, string receiverId)
        {
            _logger.LogInformation(
                0,
                $"{nameof(Execute)} handling WebHook request with receiver '{{ReceiverName}}' and id '{{Id}}'.",
                receiverName,
                receiverId);

            _logger.LogInformation(
                1,
                $"{nameof(Execute)} received Content-Type {{ContentType}} data.",
                Request.ContentType);

            var entry = (FunctionsRouteTable.Entry)ControllerContext.ActionDescriptor.Properties[
                typeof(FunctionsRouteTable.Entry)];
            Debug.Assert(string.Equals(receiverName, entry.Receiver, System.StringComparison.OrdinalIgnoreCase));
            Debug.Assert(string.Equals(receiverId, entry.Id, System.StringComparison.OrdinalIgnoreCase));

            return entry.Execute(ControllerContext.HttpContext);
        }
    }
}
