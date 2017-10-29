using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.EventReceivers;
using System.Reflection;

namespace firsttestWeb.Services
{
    public class AppEventReceiver : IRemoteEventService
    {
        public SPRemoteEventResult ProcessEvent(SPRemoteEventProperties properties)
        {
            SPRemoteEventResult result = new SPRemoteEventResult();
            if (properties.EventType == SPRemoteEventType.AppInstalled)
            {
                using (ClientContext clientContext = TokenHelper.CreateAppEventClientContext(properties, false))
                {
                    if (clientContext != null)
                    {
                        //Get reference to the host web list by itsname
                        var documentsList = clientContext.Web.Lists.GetByTitle("firstlist");
                        clientContext.Load(documentsList);
                        clientContext.ExecuteQuery();
                        string remoteUrl = "http://azurewebappname.azurewebsites.net/Services/RemoteEventReceiver1.svc";
                        //Create the remote event receiver definition
                        EventReceiverDefinitionCreationInformation newEventReceiver = new EventReceiverDefinitionCreationInformation()
                        {
                            EventType = EventReceiverType.ItemAdded,
                            ReceiverAssembly = Assembly.GetExecutingAssembly().FullName,
                            ReceiverName = "RemoteEventReceiver1",
                            ReceiverClass = "RemoteEventReceiver1",
                            ReceiverUrl = remoteUrl,
                            SequenceNumber = 15000
                        };
                        //Add the remote event receiver to the host web list
                        documentsList.EventReceivers.Add(newEventReceiver);
                        clientContext.ExecuteQuery();
                    }
                }
            }
            else if (properties.EventType == SPRemoteEventType.AppUninstalling)
            {
                using (ClientContext clientContext = TokenHelper.CreateAppEventClientContext(properties, false))
                {
                    var list = clientContext.Web.Lists.GetByTitle("firstlist");
                    clientContext.Load(list);
                    clientContext.ExecuteQuery();
                    EventReceiverDefinitionCollection erdc = list.EventReceivers;
                    clientContext.Load(erdc);
                    clientContext.ExecuteQuery();
                    List<EventReceiverDefinition> toDelete = new List<EventReceiverDefinition>();
                    foreach (EventReceiverDefinition erd in erdc)
                    {
                        if (erd.ReceiverName == "RemoteEventReceiver1")
                        {
                            toDelete.Add(erd);
                        }
                    }
                    //Delete the remote event receiver from the list, when the app gets uninstalled
                    foreach (EventReceiverDefinition item in toDelete)
                    {
                        item.DeleteObject();
                        clientContext.ExecuteQuery();
                    }
                }
            }
            return result;
        }
        public void ProcessOneWayEvent(SPRemoteEventProperties properties)
        {
            // This method is not used by app events.
        }
    }
}
