using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Rsk.Enforcer.AspNetCore.PIP;
using Rsk.Enforcer.PDP;
using Rsk.Enforcer.PIP;
using Rsk.Enforcer.PolicyModels;
using WebApiTutorial.ApiKeyAuthentication;

namespace WebApiTutorial.PIP
{
    public class RateLimitingAttributeValueProvider : RecordAttributeValueProvider<RateLimits>
    {
        private readonly PolicyAttribute subscriptionLevel = new PolicyAttribute("subscriptionLevel", PolicyValueType.String,
            PolicyAttributeCategories.Subject);
     
        private readonly PolicyAttribute currentRequestsFromQuery = new PolicyAttribute("currentRequests", PolicyValueType.String,
            HttpRequestAttributeCategories.RequestQuery);

        private readonly Dictionary<string, long> rateLimitMap = new Dictionary<string, long>
        {
            [SubscriptionLevels.Pro] = long.MaxValue,
            [SubscriptionLevels.Standard] = long.MaxValue,
            [SubscriptionLevels.Free] = 50,
        };
        
        protected override async Task<RateLimits> GetRecordValue(IAttributeResolver attributeResolver)
        {
            IReadOnlyCollection<string> subscriptionValues = await attributeResolver.Resolve<string>(subscriptionLevel);

            if (subscriptionValues.Count == 0)
            {
                return new RateLimits();
            }

            IReadOnlyCollection<string> currentRequestValues = await attributeResolver.Resolve<string>(currentRequestsFromQuery);

            long currentRequests = 0;
            if (currentRequestValues.Count > 0)
            {
                if (long.TryParse(currentRequestValues.First(), out long parsedValue))
                {
                    currentRequests = parsedValue;
                }
            }
            
            long maxRequests = rateLimitMap[subscriptionValues.First()];

            return new RateLimits
            {
                MaxRequestsPerDay = maxRequests,
                CurrentRequestsPerDay = currentRequests
            };
        }
    }

    public class RateLimits
    {
        private const string QuotesCategory = "urn:acmecorp-quotes";
        private const string MaxRequestsId = "maxQuotes";
        private const string CurrentRequestsId = "currentQuotes";
        
        [PolicyAttributeValue(QuotesCategory, MaxRequestsId, Sensitivity = PolicyAttributeSensitivity.NonSensitive)]
        public long? MaxRequestsPerDay { get; set; }
        [PolicyAttributeValue(QuotesCategory, CurrentRequestsId, Sensitivity = PolicyAttributeSensitivity.NonSensitive)]
        public long? CurrentRequestsPerDay { get; set; }
    }
}