using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BZRModManager.ModItem
{
    public enum InstallStatus
    {
        Unknown,
        Uninstalled,
        ForceDisabled,
        ForceEnabled,
        Linked,
        Collision,
        Missing,
    }
}
