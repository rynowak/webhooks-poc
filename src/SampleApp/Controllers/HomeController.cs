using System;
using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.WebHooks;
using Microsoft.Extensions.Logging;
using SampleApp.Models;

namespace SampleApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger _logger;
        private readonly FunctionsRouteTable _routeTable;

        public HomeController(FunctionsRouteTable routeTable, ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<HomeController>();
            _routeTable = routeTable;
        }

        public IActionResult Index()
        {
            var entries = _routeTable.GetEntries();
            var viewModel = new IndexViewModel() { Entries = entries, };
            return View(viewModel);
        }

        [HttpPost]
        public IActionResult Index([FromForm(Name = "entries")] string entriesText)
        {
            var templates = entriesText.Split(
                new char[] { '\r', '\n', '\t', ' ' },
                StringSplitOptions.RemoveEmptyEntries);

            var entries = templates
                .Select(t => new FunctionsRouteTable.Entry(
                    t,
                    GitHubConstants.ReceiverName,
                    id: null,
                    execute: async c =>
                    {
                        await c.Response.WriteAsync($"Hello! I'm {t}");
                        return Ok();
                    }))
                .ToArray();

            _routeTable.Update(entries);

            var viewModel = new IndexViewModel() { Entries = entries, };
            return View(viewModel);
        }

        public IActionResult DumpRoutes([FromServices] IActionDescriptorCollectionProvider actions)
        {
            var items = actions.ActionDescriptors.Items;
            foreach (var item in items)
            {
                if (!item.DisplayName.Contains(nameof(FunctionController), StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                _logger.LogInformation(
                    0,
                    "{DescriptorName} ({DescriptorId}): {Template}",
                    item.DisplayName,
                    item.Id,
                    item.AttributeRouteInfo.Template);
            }

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
