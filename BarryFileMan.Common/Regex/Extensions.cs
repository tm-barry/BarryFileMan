namespace BarryFileMan.Common.Regex
{
    public static class Extensions
    {
        public static bool IsValidRegex(this string? pattern, out System.Text.RegularExpressions.Regex regex, out string? error)
        {
            pattern ??= string.Empty;
            try
            {
                regex = new System.Text.RegularExpressions.Regex(pattern);
                error = null;

                return true;
            }
            catch (Exception ex)
            {
                regex = new System.Text.RegularExpressions.Regex(string.Empty);
                error = ex?.Message ?? "Invalid regex!";
            }

            return false;
        }
    }
}
