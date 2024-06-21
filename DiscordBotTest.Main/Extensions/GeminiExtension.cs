using DiscordBotTest.Main.Helpers;
using DiscordBotTest.Main.Services;
using DiscordBotTest.Main.Services.Interfaces;
using DotnetGeminiSDK;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DiscordBotTest.Main.Extensions
{
    public static class GeminiExtension
    {
        public static IServiceCollection AddGeminiApp(this IServiceCollection services, IConfiguration configuration)
        {
            var strGeminiToken = configuration[ConstantsAppSettings.GEMINI_TOKEN];

            if (string.IsNullOrEmpty(strGeminiToken))
                throw new Exception($"Não foi encontrado o token gemini nas configurações");

            services.AddGeminiClient(x =>
            {
                x.ApiKey = strGeminiToken;
            });

            services.AddScoped<IGeminiService, GeminiService>();

            return services;
        }
    }
}
