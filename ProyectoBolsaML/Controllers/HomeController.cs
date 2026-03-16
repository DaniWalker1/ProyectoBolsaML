using Microsoft.AspNetCore.Mvc;
using ProyectoBolsaML.Models;
using ProyectoBolsaML.Services.Interfaces;

namespace ProyectoBolsaML.Controllers
{
    public class HomeController : Controller
    {
        private readonly IStockDataService _stockService;
        private readonly IMLPredictionService _mlService;

        public HomeController(IStockDataService stockService, IMLPredictionService mlService)
        {
            _stockService = stockService;
            _mlService = mlService;
        }

        public IActionResult Index()
        {
            return View(new DashboardViewModel());
        }

        [HttpPost]

        public async Task<IActionResult> Index(string ticker, int horizon = 7)
        {
            if (string.IsNullOrEmpty(ticker)) return View(new DashboardViewModel());

            var viewModel = new DashboardViewModel { Ticker = ticker.ToUpper(), Horizon = horizon };

            try
            {
                viewModel.HistoricalData = await _stockService.GetHistoricalDataAsync(ticker);

                if (viewModel.HistoricalData.Any())
                {
                    viewModel.CurrentPrice = viewModel.HistoricalData.Last().Close;


                    viewModel.Prediction = _mlService.EvaluateStock(viewModel.HistoricalData, horizon);

                    viewModel.ChartLabels = viewModel.HistoricalData.Select(d => d.Date.ToString("yyyy-MM-dd")).ToList();
                    viewModel.ChartHistoricalPrices = viewModel.HistoricalData.Select(d => (float?)d.Close).ToList();


                    viewModel.ChartHistoricalPrices.AddRange(Enumerable.Repeat((float?)null, horizon));

                    var lastDate = viewModel.HistoricalData.Last().Date;
                    for (int i = 1; i <= horizon; i++)
                    {
                        viewModel.ChartLabels.Add(lastDate.AddDays(i).ToString("yyyy-MM-dd"));
                    }

                    viewModel.ChartForecastPrices = Enumerable.Repeat((float?)null, viewModel.HistoricalData.Count - 1).ToList();
                    viewModel.ChartForecastPrices.Add(viewModel.CurrentPrice);
                    viewModel.ChartForecastPrices.AddRange(viewModel.Prediction.ForecastedPrices.Select(p => (float?)p));


                    viewModel.ChartSMA50 = viewModel.HistoricalData.Select(d => d.SMA50 == 0 ? (float?)null : d.SMA50).ToList();
                    viewModel.ChartSMA50.AddRange(Enumerable.Repeat((float?)null, horizon));

                    viewModel.ChartRSI = viewModel.HistoricalData.Select(d => d.RSI == 0 ? (float?)null : d.RSI).ToList();
                    viewModel.ChartRSI.AddRange(Enumerable.Repeat((float?)null, horizon));


                    float vix = await _stockService.GetCurrentWorldStabilityVixAsync();
                    viewModel.Prediction.GlobalVolatilityVIX = vix;

                    if (vix < 15) viewModel.Prediction.GlobalStabilityStatus = "ESTABLE (Mercado Confiado)";
                    else if (vix >= 15 && vix < 25) viewModel.Prediction.GlobalStabilityStatus = "PRECAUCIÓN (Volatilidad Normal)";
                    else viewModel.Prediction.GlobalStabilityStatus = "INESTABLE (Miedo en el Mercado)";

                    
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Error al obtener datos. Detalle: {ex.Message}";
            }

            return View(viewModel);
        }
    }

    public class DashboardViewModel
    {
        public string Ticker { get; set; }
        public int Horizon { get; set; } = 7; 
        public List<StockData> HistoricalData { get; set; } = new List<StockData>();
        public StockPrediction Prediction { get; set; }
        public float CurrentPrice { get; set; }

        public List<string> ChartLabels { get; set; } = new List<string>();
        public List<float?> ChartHistoricalPrices { get; set; } = new List<float?>();
        public List<float?> ChartForecastPrices { get; set; } = new List<float?>();

        public List<float?> ChartSMA50 { get; set; } = new List<float?>();
        public List<float?> ChartRSI { get; set; } = new List<float?>();


    }
}