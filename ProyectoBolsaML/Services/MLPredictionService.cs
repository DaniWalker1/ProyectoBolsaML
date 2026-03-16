using System.Collections.Generic;
using System.Linq;
using Microsoft.ML;
using Microsoft.ML.Transforms.TimeSeries;
using ProyectoBolsaML.Models;
using ProyectoBolsaML.Services.Interfaces;

namespace ProyectoBolsaML.Services
{

    public class MLPredictionService : IMLPredictionService
    {
        private readonly MLContext _mlContext;

        public MLPredictionService()
        {
            _mlContext = new MLContext(seed: 1);
        }

        public StockPrediction EvaluateStock(List<StockData> historicalData, int horizon)
        {
            IDataView dataView = _mlContext.Data.LoadFromEnumerable(historicalData);


            var forecastPipeline = _mlContext.Forecasting.ForecastBySsa(
                outputColumnName: nameof(StockForecast.ForecastedClosePrices),
                inputColumnName: nameof(StockData.Close),
                windowSize: 30,
                seriesLength: historicalData.Count,
                trainSize: historicalData.Count,
                horizon: horizon);

            var forecastModel = forecastPipeline.Fit(dataView);
            var forecastingEngine = forecastModel.CreateTimeSeriesEngine<StockData, StockForecast>(_mlContext);
            var forecast = forecastingEngine.Predict();
            var anomalyPipeline = _mlContext.Transforms.DetectIidSpike(
                outputColumnName: nameof(AnomalyResult.AnomalyVector),
                inputColumnName: nameof(StockData.Close),
                confidence: 95.0, 
                pvalueHistoryLength: historicalData.Count / 4);

            var anomalyModel = anomalyPipeline.Fit(dataView);
            var transformedData = anomalyModel.Transform(dataView);


            var anomalies = _mlContext.Data.CreateEnumerable<AnomalyResult>(transformedData, reuseRowObject: false).ToList();
            bool isCurrentAnomaly = anomalies.Last().AnomalyVector[0] == 1.0;

            var currentPrice = historicalData.Last().Close;
            var targetPrice = forecast.ForecastedClosePrices.Last();
            var expectedGrowth = ((targetPrice - currentPrice) / currentPrice) * 100;

            string action = "MANTENER";
            if (expectedGrowth > 2.0) action = "COMPRAR";
            else if (expectedGrowth < -2.0) action = "VENDER";

            return new StockPrediction
            {
                ForecastedPrices = forecast.ForecastedClosePrices,
                Action = action,
                ExpectedGrowthPercentage = expectedGrowth,
                HorizonDays = horizon,
                IsAnomalyDetected = isCurrentAnomaly 
            };
        }
    }
}