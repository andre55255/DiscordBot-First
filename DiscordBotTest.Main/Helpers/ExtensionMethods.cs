namespace DiscordBotTest.Main.Helpers
{
    public static class ExtensionMethods
    {
        public static string RemoveStringFromStart(this string input, string subStringToRemove)
        {
            if (input.StartsWith(subStringToRemove))
                return input.Substring(subStringToRemove.Length);

            return input;
        }
    }
}
