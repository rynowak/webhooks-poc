using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.WebHooks;

namespace SampleApp
{
    public class DuplicateConvention : IActionModelConvention
    {
        // These are the magic route-value keys used by WebHooks.
        private static readonly Regex _idMatcher = new Regex(
            $"/{{{WebHookConstants.IdKeyName}\\W",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex _receiverMatcher = new Regex(
            $"/{{{WebHookConstants.ReceiverKeyName}\\W",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private readonly FunctionsRouteTable _routeTable;

        public DuplicateConvention(FunctionsRouteTable routeTable)
        {
            _routeTable = routeTable;
        }

        public void Apply(ActionModel action)
        {
            var entries = _routeTable.GetEntries();

            if (action.Attributes.OfType<DuplicateAttribute>().Any())
            {
                action.Controller.Actions.Remove(action);

                foreach (var entry in entries)
                {
                    var newAction = new ActionModel(action);
                    UpdateValues(newAction.RouteValues, entry);

                    var selector = newAction.Selectors.Single();
                    selector.AttributeRouteModel.Template = entry.Template;

                    newAction.Properties.Add(typeof(FunctionsRouteTable.Entry), entry);

                    action.Controller.Actions.Add(newAction);
                }
            }
        }

        // entry.Template alone is sufficient for WebHooks if it contains {webHookReceiver} and {id} route values.
        // WebHook constraints and filters read these route values.
        //
        // If the template is insufficient, add missing route values to the ActionModel. WebHook constraints read these
        // from the ActionDescriptor and filters execute after the values are copied to the request's RouteData.
        //
        // A Functions constraint that maps from the unique function path to the necessary route values would also
        // work. That constraint must have Order less than -500 i.e. execute before any WebHook constraint.
        private static void UpdateValues(IDictionary<string, string> routeValues, FunctionsRouteTable.Entry entry)
        {
            var template = entry.Template;
            if (!_receiverMatcher.IsMatch(template))
            {
                routeValues.Add(WebHookConstants.ReceiverKeyName, entry.Receiver);
            }

            var id = entry.Id;
            if (!string.IsNullOrEmpty(id) && !_idMatcher.IsMatch(template))
            {
                routeValues.Add(WebHookConstants.IdKeyName, id);
            }
        }
    }
}
