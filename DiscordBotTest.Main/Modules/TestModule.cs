using Discord.Commands;

namespace DiscordBotTest.Main.Modules
{
    public class TestModule : ModuleBase<SocketCommandContext>
    {
        [Command("ping")]
        public async Task PingAsync()
        {
            await ReplyAsync($"Pong!");
        }

        [Command("salve")]
        public async Task HelloAsync()
        {
            await ReplyAsync($"Salve {Context.User.GlobalName}!");
        }
    }
}
