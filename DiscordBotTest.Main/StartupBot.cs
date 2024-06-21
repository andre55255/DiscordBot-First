using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DiscordBotTest.Main.Extensions;
using DiscordBotTest.Main.Helpers;
using DiscordBotTest.Main.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace DiscordBotTest.Main
{
    public class StartupBot
    {
        private DiscordSocketClient _clientDiscord;
        private CommandService _commandsDiscord;
        private IServiceProvider _servicesProvider;

        public async Task RunAsync()
        {
            var configuration =
                new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .Build();

            string botToken = configuration[ConstantsAppSettings.BOT_DISCORD_TOKEN];
            if (string.IsNullOrEmpty(botToken))
            {
                Console.WriteLine(BuildLogString($"Não foi encontrado o token do bot"));
                return;
            }

            _clientDiscord = new DiscordSocketClient(new DiscordSocketConfig
            {
                GatewayIntents = GatewayIntents.AllUnprivileged | GatewayIntents.MessageContent,
            });

            _commandsDiscord = new CommandService(new CommandServiceConfig
            {
                LogLevel = LogSeverity.Debug,
                CaseSensitiveCommands = true
            });

            _servicesProvider =
               new ServiceCollection()
                   .AddSingleton(_clientDiscord)
                   .AddSingleton(_commandsDiscord)
                   .AddGeminiApp(configuration)
                   .BuildServiceProvider();

            _clientDiscord.Log += LogAsync;

            await RegisterCommandsAsync();

            await _clientDiscord.LoginAsync(TokenType.Bot, botToken);
            await _clientDiscord.StartAsync();

            await Task.Delay(-1);
        }

        private Task LogAsync(LogMessage message)
        {
            Console.WriteLine(BuildLogString(message.Message));
            return Task.CompletedTask;
        }

        private string BuildLogString(string message)
        {
            return $"Date {DateTime.Now.ToString("dd/MM/yyyy HH:mm")} - {message}";
        }

        private async Task RegisterCommandsAsync()
        {
            _clientDiscord.MessageReceived += HandleCommandAsync;
            await _commandsDiscord.AddModulesAsync(Assembly.GetEntryAssembly(), _servicesProvider);
        }

        private async Task HandleCommandAsync(SocketMessage socketMessage)
        {
            var message = socketMessage as SocketUserMessage;
            if (message == null)
            {
                Console.WriteLine(BuildLogString($"Não foi possível obter dados de mensagem recebida"));
                return;
            }

            var context = new SocketCommandContext(_clientDiscord, message);

            if (message.Author.IsBot) return;

            if (string.IsNullOrEmpty(message.Content))
            {
                Console.Write(BuildLogString($"Mensagem vazia"));
                return;
            }

            int argPositionIndex = 0;
            if (message.Content.StartsWith("!prompt"))
            {
                var geminiService = _servicesProvider.GetService<IGeminiService>();

                var result = await geminiService.ProcessPromptAsync(message.Content.RemoveStringFromStart("!prompt").Trim());

                await context.Channel.SendMessageAsync(result);
            }
            else if (message.HasStringPrefix("!", ref argPositionIndex))
            {
                var isCommandStr = ModulesDiscord.IsValidCommandForTheModuleProcess(message.Content);

                if (!isCommandStr)
                {
                    Console.WriteLine(BuildLogString($"Não é um comando válido a string: {message.Content}"));
                    await context.Channel.SendMessageAsync($"Comando inválido, lista de comandos válidos: {ModulesDiscord.GetCommandsValids()}");
                    return;
                }

                var result = await _commandsDiscord.ExecuteAsync(context, argPositionIndex, _servicesProvider);
                if (!result.IsSuccess)
                {
                    Console.WriteLine(BuildLogString($"Erro na execução de comando: {result.ErrorReason}"));
                    await context.Channel.SendMessageAsync($"Erro na execução de comando: {result.ErrorReason}");
                }
            }
        }
    }
}
