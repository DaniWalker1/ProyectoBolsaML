using ProyectoBolsaML.Models;

namespace ProyectoBolsaML.Services.Interfaces
{
    public interface IStockDataService
    {
        Task<List<StockData>> GetHistoricalDataAsync(string ticker);
        Task<float> GetCurrentWorldStabilityVixAsync();
    }
}