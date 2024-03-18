using BarryFileMan.Rename.Providers;

namespace BarryFileMan.Rename.Models
{
    public class RenameResult
    {
        public string Value { get; }
        public IEnumerable<RenameTag> Tags { get; }
        public IEnumerable<string>? Errors { get; }

        public RenameResult(string value, IEnumerable<RenameTag>  tags, IEnumerable<string>? errors = null)
        {
            Value = value;
            Tags = tags;
            Errors = errors;
        }
    }
}
