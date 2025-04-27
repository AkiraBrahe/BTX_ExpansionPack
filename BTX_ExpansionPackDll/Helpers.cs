using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BTX_ExpansionPack
{
    internal class Helpers
    {
        private static bool HasArtemis(Mech mech)
        {
            foreach (MechComponent mechComponent in mech.allComponents)
            {
                if (mechComponent.componentDef.Description.Id == "Gear_Addon_Artemis4" || mechComponent.componentDef.Description.Id == "Gear_Addon_Artemis5")
                {
                    return true;
                }
            }
            return false;
        }
    }
}
