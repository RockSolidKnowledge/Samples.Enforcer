using System.Collections.Generic;
using System.Security.Claims;
using AspNetCore.Authentication.ApiKey;

namespace WebApiTutorial.ApiKeyAuthentication
{
    public class ApiKey : IApiKey
    {
        public ApiKey(string key, string ownerName, IReadOnlyCollection<Claim> claims)
        {
            Key = key;
            OwnerName = ownerName;
            Claims = claims;
        }

        public string Key { get; }
        public string OwnerName { get; }
        public IReadOnlyCollection<Claim> Claims { get; }
    }
}