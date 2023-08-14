using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XenonautsModPatcher
{
    public interface ISettings
    {
        string XenonautsInstallDirectory { get; }

        string ModsDirectory { get; }
    }
}
