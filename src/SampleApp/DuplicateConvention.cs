using System.Linq;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.WebHooks.Routing;

namespace SampleApp
{
    public class DuplicateConvention : IControllerModelConvention
    {
        private readonly FunctionsRouteTable _routeTable;

        public DuplicateConvention(FunctionsRouteTable routeTable)
        {
            _routeTable = routeTable;
        }

        public void Apply(ControllerModel controller)
        {
            var actions = controller.Actions.ToArray();
            foreach (var action in actions)
            {
                Apply(action);
            }
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

                    // This is the magic route-value key used by webhooks
                    newAction.RouteValues.Add("webHookReceiver", entry.Reciever);

                    var selector = newAction.Selectors.Single();
                    selector.AttributeRouteModel.Template = entry.Template;

                    // Workaround for the case where the reciever name is not part of the URL.
                    for (var i = selector.ActionConstraints.Count - 1; i >= 0; i--)
                    {
                        if (selector.ActionConstraints[i] is WebHookReceiverExistsConstraint ||
                            selector.ActionConstraints[i] is WebHookEventMapperConstraint)
                        {
                            selector.ActionConstraints.RemoveAt(i);
                        }
                    }

                    newAction.Properties.Add(typeof(FunctionsRouteTable.Entry), entry);

                    action.Controller.Actions.Add(newAction);
                }
            }
        }
    }
}
