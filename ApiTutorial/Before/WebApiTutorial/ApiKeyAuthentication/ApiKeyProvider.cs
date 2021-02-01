using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AspNetCore.Authentication.ApiKey;

namespace WebApiTutorial.ApiKeyAuthentication
{
    public static class SubscriptionLevels
    {
        public const string Pro = "pro";
        public const string Standard = "std";
        public const string Free = "free";
    }
    
    public class ApiKeyProvider : IApiKeyProvider
    {
        private Dictionary<string, string> apiKeyToSubscriptionMap = new Dictionary<string, string>
        {
            ["123"] = SubscriptionLevels.Pro,
            ["456"] = SubscriptionLevels.Standard,
            ["789"] = SubscriptionLevels.Free
        };
        
        public Task<IApiKey> ProvideAsync(string key)
        {
            if (!apiKeyToSubscriptionMap.TryGetValue(key, out string subscription))
            {
                subscription = SubscriptionLevels.Free;
            }

            var apiKey = new ApiKey(key, $"{subscription}User", new[]
            {
                new Claim("subscriptionLevel", subscription)
            });

            return Task.FromResult((IApiKey) apiKey);
        }
    }
}