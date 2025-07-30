using BattleTech;
using CustAmmoCategories;

namespace BTX_ExpansionPack
{
    public static class ArrowIVHelper
    {
        [HarmonyPatch(typeof(UnitSpawnPointGameLogic), "initializeActor")]
        public static class UnitSpawnPointGameLogic_initializeActor
        {
            [HarmonyPrefix]
            public static void Prefix(AbstractActor actor)
            {
                if (actor == null || actor.ammoBoxes == null || actor.StatCollection == null) return;

                bool foundHomingAmmo = false;
                foreach (AmmunitionBox ammo in actor.ammoBoxes)
                {
                    if (ammo.ammunitionBoxDef.AmmoID == "Ammunition_ArrowIV_Homing")
                    {
                        foundHomingAmmo = true;
                        break;
                    }
                }

                actor.StatCollection.GetOrCreateStatisic("HasHomingArrowIV", foundHomingAmmo);
            }
        }

        public static bool IsHomingArrowIV(this Weapon weapon)
        {
            return weapon != null &&
                   weapon.mode()?.Id == "ARTY_Guided" &&
                   weapon.ammo()?.Id == "Ammunition_ArrowIV_Homing";
        }

        public static bool HasActiveHomingArrowIV(this AbstractActor actor)
        {
            if (actor == null || actor.StatCollection == null) return false;
            if (!actor.StatCollection.GetValue<bool>("HasHomingArrowIV"))
            {
                return false;
            }

            if (actor.Weapons == null) return false;
            foreach (Weapon weapon in actor.Weapons)
            {
                if (weapon.CanFire &&
                    weapon.mode()?.Id == "ARTY_Guided" &&
                    weapon.ammo()?.Id == "Ammunition_ArrowIV_Homing")
                {
                    return true;
                }
            }
            return false;
        }

        public static bool IsTAGed(this ICombatant target)
        {
            return target != null && target.StatCollection != null &&
                   target.StatCollection.GetValue<float>("TAGCount") +
                   target.StatCollection.GetValue<float>("TAGCountClan") > 0f;
        }
    }
}
