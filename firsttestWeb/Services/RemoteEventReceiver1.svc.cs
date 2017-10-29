using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.EventReceivers;

namespace firsttestWeb.Services
{
    public class RemoteEventReceiver1 : IRemoteEventService
    {
        /// <summary>
        /// Handles events that occur before an action occurs, such as when a user adds or deletes a list item.
        /// </summary>
        /// <param name="properties">Holds information about the remote event.</param>
        /// <returns>Holds information returned from the remote event.</returns>
        public SPRemoteEventResult ProcessEvent(SPRemoteEventProperties properties)
        {
            SPRemoteEventResult result = new SPRemoteEventResult();
            using (ClientContext clientContext = TokenHelper.CreateRemoteEventReceiverClientContext(properties))
            {
                if (clientContext != null)
                {
                    clientContext.Load(clientContext.Web);
                    clientContext.ExecuteQuery();
                }
            }
            return result;
        }
        public void ProcessOneWayEvent(SPRemoteEventProperties properties)
        {
            // On Item Added event, the list item creation executes
            if (properties.EventType == SPRemoteEventType.ItemAdded)
            {
                using (ClientContext clientContext = TokenHelper.CreateRemoteEventReceiverClientContext(properties))
                {
                    if (clientContext != null)
                    {
                        try
                        {
                            clientContext.Load(clientContext.Web);
                            clientContext.ExecuteQuery();
                            List imageLibrary = clientContext.Web.Lists.GetByTitle("secondlist");
                            ListItemCreationInformation itemCreateInfo = new ListItemCreationInformation();
                            ListItem oListItem = imageLibrary.AddItem(itemCreateInfo);
                            oListItem["Title"] = "A NEW ITEM WAS CREATED IN ANOTHER LIST";
                            oListItem.Update();
                            clientContext.ExecuteQuery();
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                    }
                }
            }
        }


        /// <summary>
        /// Handles events that occur after an action occurs, such as after a user adds an item to a list or deletes an item from a list.
        /// </summary>
        /// <param name="properties">Holds information about the remote event.</param>

    }
}
