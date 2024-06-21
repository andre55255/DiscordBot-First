using DiscordBotTest.Main.Services.Interfaces;
using DotnetGeminiSDK.Client.Interfaces;
using System.Text;

namespace DiscordBotTest.Main.Services
{
    public class GeminiService : IGeminiService
    {
        private readonly IGeminiClient _geminiClient;

        public GeminiService(IGeminiClient geminiClient)
        {
            _geminiClient = geminiClient;
        }

        public async Task<string> ProcessPromptAsync(string prompt)
        {
            try
            {
                var countTokens = await _geminiClient.CountTokens(prompt);
                Console.WriteLine($"Mensagem: {prompt}\nCount tokens: {countTokens.TotalTokens}");

                var response = await _geminiClient.TextPrompt(prompt);

                var parts = response.Candidates.Select(x => x.Content.Parts);

                var results = new StringBuilder();

                foreach (var part in parts)
                {
                    foreach (var item in part)
                    {
                        results.AppendLine(item.Text);
                    }
                }

                Console.WriteLine($"Resultados: {results.ToString()}");
                return results.ToString();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro no processamento do prompt enviado. Ex: {ex.Message}");
                return $"Erro inesperado no processamento do prompt enviado";
            }
        }
    }
}
