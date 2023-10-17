using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarryFileMan.Rename.Models.Regex
{
    public class RegexRenameProviderMatchOptions
    {
        public string RegexPattern { get; set; }

        public RegexRenameProviderMatchOptions() : this(string.Empty) { }
        public RegexRenameProviderMatchOptions(string regexPattern)
        {
            RegexPattern = regexPattern;
        }
    }
}
