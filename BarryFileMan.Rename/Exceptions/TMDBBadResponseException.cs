namespace BarryFileMan.Rename.Exceptions
{
    public class TMDBBadResponseException : Exception
    {
        public TMDBBadResponseException() : base() { }
        public TMDBBadResponseException(string message, int statusCode) : base($"{ message } ({ statusCode })") { }
    }
}
