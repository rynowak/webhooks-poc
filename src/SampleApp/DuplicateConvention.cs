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
                    var selector = newAction.Selectors.Single();

                    // !!! entry.Template would be fine as-is if route (or route template) contains {webHookReceiver}
                    // !!! and {id} route values. WebHooks constraints and filters read these route values.
                    // !!!
                    // !!! A Functions constraint that maps from the unique function path to the necessary route
                    // !!! values would also work. That constraint must have Order < -500.
                    // !!!
                    // !!! Would not need Entry.Reciever or Entry.Id if GetFullTemplate(...) were unnecessary.
                    selector.AttributeRouteModel.Template = GetFullTemplate(entry.Template, entry.Reciever, entry.Id);

                    newAction.Properties.Add(typeof(FunctionsRouteTable.Entry), entry);

                    action.Controller.Actions.Add(newAction);
                }
            }
        }

        private static string AddTrailingSlash(string template)
        {
            if (string.IsNullOrEmpty(template))
            {
                return template;
            }

            if (template[template.Length - 1] != '/')
            {
                return template + "/";
            }

            return template;
        }

        private static string GetFullTemplate(string template, string receiver, string id)
        {
            if (!_receiverMatcher.IsMatch(template))
            {
                template = AddTrailingSlash(template);
                template += $"{{{WebHookConstants.ReceiverKeyName}={receiver}}}";
            }

            if (!string.IsNullOrEmpty(id) && !_idMatcher.IsMatch(template))
            {
                template = AddTrailingSlash(template);
                template += $"{{{WebHookConstants.IdKeyName}={id}}}";
            }

            return template;
        }
    }
}
