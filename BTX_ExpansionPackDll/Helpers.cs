using BattleTech;

namespace BTX_ExpansionPack
{
    internal class Helpers
    {
        public static bool AnyAllyHasTAG(Mech mech)
        {
            if (mech?.team == null) return false;

            foreach (ICombatant ally in mech.team.units)
            {
                if (ally is Mech allyMech && allyMech != mech)
                {
                    foreach (Weapon weapon in allyMech.Weapons)
                    {
                        if (weapon.defId.StartsWith("Weapon_TAG"))
                        {
                            return true;
                        }
                    }
                }
                else if (ally is Vehicle allyVehicle)
                {
                    foreach (Weapon weapon in allyVehicle.Weapons)
                    {
                        if (weapon.defId.StartsWith("Weapon_TAG"))
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        public static bool HasArtemis(Mech mech)
        {
            foreach (MechComponent mechComponent in mech.allComponents)
            {
                if (mechComponent.defId.StartsWith("Gear_Addon_Artemis"))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
