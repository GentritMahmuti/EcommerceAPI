using Core.Hubs.IHubs;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Hubs
{
    public class InventoryHub : Hub<IStockClient>
    {
        public async Task GetStock(int stock)
        {
            await Clients.All.SendAsync(stock);
        }
    }
}
