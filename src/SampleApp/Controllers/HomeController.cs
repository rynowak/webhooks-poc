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
                        return new EmptyResult();
                    }))
                .ToArray();

            _routeTable.Update(entries);

            var viewModel = new IndexViewModel() { Entries = entries, };
            return View(viewModel);
        }

        public IActionResult DumpRoutes([FromServices] IActionDescriptorCollectionProvider actions)
        {
            var actionDescriptors = actions.ActionDescriptors.Items;
            foreach (var actionDescriptor in actionDescriptors)
            {
                if (!actionDescriptor.DisplayName.Contains(
                    nameof(FunctionController),
                    StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                _logger.LogInformation(
                    0,
                    "{DescriptorName} ({DescriptorId}): {Template}",
                    actionDescriptor.DisplayName,
                    actionDescriptor.Id,
                    actionDescriptor.AttributeRouteInfo.Template);
                foreach (var kvp in actionDescriptor.RouteValues)
                {
                    _logger.LogInformation(
                        1,
                        "Route Value '{RouteValueKey}': '{RouteValueValue}'",
                        kvp.Key,
                        kvp.Value);
                }

                foreach (var constraint in actionDescriptor.ActionConstraints)
                {
                    _logger.LogInformation(2, "Constraint: '{ConstraintType}'", constraint.GetType());
                }

                foreach (var filter in actionDescriptor.FilterDescriptors)
                {
                    if (filter.Filter is ServiceFilterAttribute serviceFilter)
                    {
                        _logger.LogInformation(3, "Filter: '{FilterType}'", serviceFilter.ServiceType);
                    }
                    else
                    {
                        _logger.LogInformation(4, "Filter: '{FilterType}'", filter.Filter.GetType());
                    }
                }
            }

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
