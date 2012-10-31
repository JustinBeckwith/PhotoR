using System;
using System.Collections.Generic;
using System.Web;
using SignalR.Hubs;

namespace PhotoR.Hubs
{
    /// <summary>
    /// Summary description for ClassName
    /// </summary>
    public class PicHub : Hub
    {
        public void Update(string url)
        {
            Clients.addImage(url);
        }
    }
}
