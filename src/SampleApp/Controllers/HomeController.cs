using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using SampleApp.Models;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace SampleApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly FunctionsRouteTable _routeTable;

        public HomeController(FunctionsRouteTable routeTable)
        {
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
            var templates = entriesText.Split(new char[] { '\r', '\n', '\t', ' ' }, StringSplitOptions.RemoveEmptyEntries);

            var entries = templates.Select(t => new FunctionsRouteTable.Entry(t, "github", async (c) =>
            {
                await c.Response.WriteAsync($"Hello! I'm {t}");
            })).ToArray();

            _routeTable.Update(entries);

            var viewModel = new IndexViewModel() { Entries = entries, };
            return View(viewModel);
        }

        public IActionResult DumpRoutes([FromServices] IActionDescriptorCollectionProvider actions)
        {
            var action = actions.ActionDescriptors.Items;
            return Ok();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
