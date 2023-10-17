using BarryFileMan.Rename.Interfaces;

namespace BarryFileMan.Rename.Models
{
    public class BaseRenameMatchGroupValue : IRenameMatchGroupValue
    {
        public string Value { get; }

        public BaseRenameMatchGroupValue(string value)
        {  
            Value = value; 
        }
    }
}
