namespace BarryFileMan.Rename.Exceptions
{
    public class InvalidRegexException : Exception
    {
        public string? Field { get; set; }

        public InvalidRegexException() : base() { }
        public InvalidRegexException(string? message) : base(message) { }
    }
}
