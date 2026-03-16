using ProyectoBolsaML.Models;

namespace ProyectoBolsaML.Services.Interfaces
{
    public interface IMLPredictionService
    {
        StockPrediction EvaluateStock(List<StockData> historicalData, int horizon);
    }
}