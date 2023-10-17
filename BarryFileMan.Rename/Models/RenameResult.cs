namespace BarryFileMan.Rename.Models
{
    public class RenameResult
    {
        public string Value { get; }
        public IEnumerable<string>? Errors { get; }

        public RenameResult(string value, IEnumerable<string>? errors = null)
        {
            Value = value;
            Errors = errors;
        }
    }
}
