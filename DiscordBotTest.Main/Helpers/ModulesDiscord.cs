namespace DiscordBotTest.Main.Helpers
{
    public abstract class ModulesDiscord
    {
        private static List<string> modulesNameList = new(new string[]
        {
            "!ping",
            "!salve",
            "!prompt"
        });

        public static bool IsValidCommandForTheModuleProcess(string message)
        {
            var command = ExtractCommand(message);

            if (string.IsNullOrEmpty(command))
                return false;

            return modulesNameList.Any(x => x == command);
        }

        public static string GetCommandsValids()
        {
            return string.Join(", ", modulesNameList);
        }

        private static string ExtractCommand(string input)
        {
            if (!string.IsNullOrEmpty(input) && input.StartsWith("!"))
            {
                int spaceIndex = input.IndexOf(' ');
                if (spaceIndex > 0)
                {
                    return input.Substring(0, spaceIndex);
                }
                else
                {
                    return input;
                }
            }
            return string.Empty;
        }
    }
}
