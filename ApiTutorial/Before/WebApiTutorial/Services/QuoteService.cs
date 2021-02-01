using System;
using System.Threading.Tasks;
using WebApiTutorial.Models;

namespace WebApiTutorial.Services
{
    public interface IQuoteService
    {
        Task<StockPrice> GetLivePrice(string symbol);
        Task<StockPrice> GetDelayedPrice(string symbol);
    }
    
    public class QuoteService : IQuoteService
    {
        private readonly Random rng = new Random();
        
        public Task<StockPrice> GetLivePrice(string symbol)
        {
            var price = new StockPrice
            {
                Description = "Live",
                Symbol = symbol,
                Price = (decimal) (rng.NextDouble() * 20),
                When = DateTime.Now
            };

            return Task.FromResult(price);
        }

        public async Task<StockPrice> GetDelayedPrice(string symbol)
        {
            var price = await GetLivePrice(symbol);
            price.Description = "Delayed";

            price.When = price.When.AddHours(-2);

            return price;
        }
    }
}