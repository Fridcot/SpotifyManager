using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SpotifyManager.Services;

namespace SpotifyManager
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");

            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
            builder.Services.AddHttpClient("AuthClient", x =>
            {
                x.BaseAddress = new Uri(builder.Configuration.GetConnectionString("SpotifyAuthUri"));
                var clientId = builder.Configuration["SpotifyApiTokens:ClientId"];
                var clientSecret = builder.Configuration["SpotifyApiTokens:ClientSecret"];
                var encodedSecret = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{clientId}:{clientSecret}"));
                x.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", encodedSecret);
            });
            //builder.Services.AddHttpClient("ApiClient", x =>
            //{
            //    x.BaseAddress = new Uri(builder.Configuration.GetConnectionString("SpotifyApiUri"));
            //    var clientId = builder.Configuration["SpotifyApiTokens:ClientId"];
            //    var clientSecret = builder.Configuration["SpotifyApiTokens:ClientSecret"];
            //    var encodedSecret = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{clientId}:{clientSecret}"));
            //    x.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", encodedSecret);
            //});
            builder.Services.AddSingleton<IAccessTokenProvider, AccessTokenProvider>();
            builder.Services.AddLogging();

            await builder.Build().RunAsync();
        }
    }
}
