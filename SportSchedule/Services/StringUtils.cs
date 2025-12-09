namespace SportSchedule.Services
{
    public static class StringUtils
    {
        public static string ConvertToSentenceCase(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) { return input; }
            var result = input.ToLowerInvariant();
            return char.ToUpperInvariant(result[0]) + result.Substring(1).Trim();
        }
    }
}
