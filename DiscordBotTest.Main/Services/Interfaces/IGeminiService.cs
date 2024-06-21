namespace DiscordBotTest.Main.Services.Interfaces
{
    public interface IGeminiService
    {
        public Task<string> ProcessPromptAsync(string prompt);
    }
}
