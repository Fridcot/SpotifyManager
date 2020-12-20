using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.Extensions.Logging;

namespace SpotifyManager.Services
{
    public class AccessTokenProvider : IAccessTokenProvider
    {
        private readonly HttpClient _client;
        private readonly ILogger<AccessTokenProvider> _logger;

        public AccessTokenProvider(IHttpClientFactory httpClientFactory, ILogger<AccessTokenProvider> logger)
        {
            _client = httpClientFactory.CreateClient("AuthClient");
            _logger = logger;
        }

        public async ValueTask<AccessTokenResult> RequestAccessToken()
        {
            try
            {
                var now = DateTimeOffset.Now;
                var res = await _client.PostAsync("", new StringContent("grant_type=client_credentials", Encoding.UTF8, "application/x-www-form-urlencoded"));
                if (res.IsSuccessStatusCode)
                {
                    var resss = await res.Content.ReadFromJsonAsync<RawToken>();
                    var token = new AccessToken { Value = resss.access_token, GrantedScopes = resss.scope.Split(','), Expires = now.AddSeconds(resss.expires_in) };
                    return new AccessTokenResult(AccessTokenResultStatus.Success, token, "");
                }
                else
                {
                    throw new Exception(res.ReasonPhrase);
                }
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.ToString());
                return null;
            }
        }

        public ValueTask<AccessTokenResult> RequestAccessToken(AccessTokenRequestOptions options)
            => RequestAccessToken();

        private class RawToken
        {
            public string access_token { get; set; }
            public string token_type { get; set; }
            public int expires_in { get; set; }
            public string scope { get; set; }
        }
    }
}
