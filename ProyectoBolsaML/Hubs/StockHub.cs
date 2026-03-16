using Microsoft.AspNetCore.SignalR;

namespace ProyectoBolsaML.Hubs
{
    public class StockHub : Hub
    {

        public async Task SendLiveUpdate(string ticker, float newPrice)
        {
            await Clients.All.SendAsync("ReceivePriceUpdate", ticker, newPrice);
        }
    }
}