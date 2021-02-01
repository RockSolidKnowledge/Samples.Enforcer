using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApiTutorial.Services;

namespace WebApiTutorial.Controllers
{
    [ApiController]
    [Authorize]
    [Route("quote")]
    public class QuoteController : ControllerBase
    {
        private readonly IQuoteService quoteService;

        public QuoteController(IQuoteService quoteService)
        {
            this.quoteService = quoteService;
        }

        [HttpGet]
        [Route("{symbol}/live")]
        public async Task<ObjectResult> GetLive(string symbol)
        {
            return Ok(await quoteService.GetLivePrice(symbol));
        }
        
        [HttpGet]
        [Route("{symbol}")]
        public async Task<ObjectResult> GetDelayed(string symbol)
        {
            return Ok(await quoteService.GetDelayedPrice(symbol));
        }
    }
}