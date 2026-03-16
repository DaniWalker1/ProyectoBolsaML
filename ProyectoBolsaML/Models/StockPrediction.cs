using Microsoft.ML.Data;

namespace ProyectoBolsaML.Models
{
    public class StockForecast
    {
        public float[] ForecastedClosePrices { get; set; }
    }

    public class AnomalyResult
    {
        [VectorType(3)]
        public double[] AnomalyVector { get; set; } 
    }

    public class StockPrediction
    {
        public string Action { get; set; }
        public float[] ForecastedPrices { get; set; }
        public float ExpectedGrowthPercentage { get; set; }
        public int HorizonDays { get; set; }

        public bool IsAnomalyDetected { get; set; } 
        public float GlobalVolatilityVIX { get; set; }
        public string GlobalStabilityStatus { get; set; } 
    }
}