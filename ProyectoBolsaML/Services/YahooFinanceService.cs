using Newtonsoft.Json.Linq;
using ProyectoBolsaML.Models;
using ProyectoBolsaML.Services.Interfaces;

namespace ProyectoBolsaML.Services
{
    public class YahooFinanceService : IStockDataService
    {
        private readonly HttpClient _httpClient;

        public YahooFinanceService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/122.0.0.0 Safari/537.36");
            _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
        }

        public async Task<List<StockData>> GetHistoricalDataAsync(string ticker)
        {
            var url = $"https://query1.finance.yahoo.com/v8/finance/chart/{ticker}?range=1y&interval=1d";
            var response = await _httpClient.GetStringAsync(url);

            var json = JObject.Parse(response);
            var result = json["chart"]["result"][0];
            var timestamps = result["timestamp"].ToObject<List<long>>();
            var quote = result["indicators"]["quote"][0];

            var open = quote["open"].ToObject<List<float?>>();
            var close = quote["close"].ToObject<List<float?>>();
            var high = quote["high"].ToObject<List<float?>>();
            var low = quote["low"].ToObject<List<float?>>();
            var volume = quote["volume"].ToObject<List<float?>>();

            var historicalData = new List<StockData>();

            for (int i = 0; i < timestamps.Count; i++)
            {
                if (close[i] == null) continue;

                var date = DateTimeOffset.FromUnixTimeSeconds(timestamps[i]).DateTime;
                historicalData.Add(new StockData
                {
                    Date = date,
                    Open = open[i] ?? 0,
                    Close = close[i] ?? 0,
                    High = high[i] ?? 0,
                    Low = low[i] ?? 0,
                    Volume = volume[i] ?? 0
                });
            }

            CalculateIndicators(historicalData);

            return historicalData;
        }

        private void CalculateIndicators(List<StockData> data)
        {
            if (data.Count < 50) return;


            for (int i = 49; i < data.Count; i++)
            {
                data[i].SMA50 = data.Skip(i - 49).Take(50).Average(d => d.Close);
            }


            if (data.Count < 15) return;
            double gain = 0, loss = 0;

            for (int i = 1; i <= 14; i++)
            {
                float diff = data[i].Close - data[i - 1].Close;
                if (diff >= 0) gain += diff;
                else loss -= diff;
            }
            gain /= 14;
            loss /= 14;
            data[14].RSI = loss == 0 ? 100 : (float)(100 - (100 / (1 + (gain / loss))));

            for (int i = 15; i < data.Count; i++)
            {
                float diff = data[i].Close - data[i - 1].Close;
                float currentGain = diff >= 0 ? diff : 0;
                float currentLoss = diff < 0 ? -diff : 0;

                gain = ((gain * 13) + currentGain) / 14;
                loss = ((loss * 13) + currentLoss) / 14;

                data[i].RSI = loss == 0 ? 100 : (float)(100 - (100 / (1 + (gain / loss))));
            }
        }

        public async Task<float> GetCurrentWorldStabilityVixAsync()
        {
            try
            {
                var url = $"https://query1.finance.yahoo.com/v8/finance/chart/^VIX?range=1d&interval=1d";
                var response = await _httpClient.GetStringAsync(url);
                var json = Newtonsoft.Json.Linq.JObject.Parse(response);

                var quote = json["chart"]["result"][0]["indicators"]["quote"][0];
                var close = quote["close"].ToObject<List<float?>>();

                return close.LastOrDefault(c => c != null) ?? 0f;
            }
            catch
            {
                return 20f; 
            }
        }
    }
}