using BattleTech;
using BTX_ExpansionPack.Helpers;
using CustAmmoCategories;
using System;
using UnityEngine;

namespace BTX_ExpansionPack.Helpers
{
    public static class ArtilleryHelpers
    {
        /// <summary>
        /// Gets the Targeting-Tracking System (TTS) level of an artillery weapon.
        /// </summary>
        public static int ArtilleryTTSLevel(this Weapon weapon) => (int)weapon.GetStatisticFloat("AMSAttractiveness");

        /// <summary>
        /// Determines if a target position is outside the minimum and forbidden ranges of an artillery weapon.
        /// </summary>
        public static bool IsOutsideSafeRange(this Weapon weapon, Vector3 attackerPosition, Vector3 targetPosition, out float unsafeRange)
        {
            float distance = Vector3.Distance(attackerPosition, targetPosition);
            float minRange = weapon.MinRange;
            float forbiddenRange = Mathf.Max(weapon.ForbiddenRange(), weapon.AOERange());
            unsafeRange = Math.Max(forbiddenRange, minRange);
            return distance >= unsafeRange;
        }
    }
}