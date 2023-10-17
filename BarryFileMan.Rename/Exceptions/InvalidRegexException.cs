namespace BarryFileMan.Rename.Exceptions
{
    public class InvalidRegexException : Exception
    {
        public InvalidRegexException() : base() { }
        public InvalidRegexException(string? message) : base(message) { }
    }
}
