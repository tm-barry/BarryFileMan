using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarryFileMan.Interfaces
{
    public interface IPresetViewModel
    {
        string Name { get; set; }
        bool IsSystem { get; set; }
    }
}
