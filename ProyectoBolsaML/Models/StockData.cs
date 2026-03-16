namespace ProyectoBolsaML.Models
{
    public class StockData
    {
        public DateTime Date { get; set; }
        public float Open { get; set; }
        public float Close { get; set; }
        public float High { get; set; }
        public float Low { get; set; }
        public float Volume { get; set; }

        public float SMA50 { get; set; }
        public float RSI { get; set; }
    }
}