using System;

namespace WebApiTutorial.Models
{
    public class StockPrice
    {
        public string Symbol { get; internal set; }
        public string Description { get; internal set; }
        public DateTime When { get; internal set; }
        public decimal Price { get; internal set; }
    }
}