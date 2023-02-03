
using EcommerceAPI.Hubs.IHubs;
using Microsoft.AspNetCore.SignalR;

namespace EcommerceAPI.Hubs

{
    public class InventoryHub : Hub <IStockClient>
    {
        public async Task GetStock(int stock)
        {
            await Clients.All.SendAsync(stock);
        }
    }
}
