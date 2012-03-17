using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Liberty.Reach
{
    public sealed class WeaponEditing
    {
        /// <summary>
        /// Returns a UIElement/Util.IAmmoDisplay that can be used to accurately display the weapon's ammo.
        /// </summary>
        /// <param name="saveData">The save data to read from</param>
        /// <param name="weapon">The weapon object to display</param>
        /// <param name="name">The name of the weapon</param>
        /// <returns>A UIElement/Util.IAmmoDisplay corresponding to the weapon</returns>
        public static UIElement GetAmmoDisplay(Reach.CampaignSave saveData, Reach.WeaponObject weapon, string name)
        {
            name = name.ToLower();

            UIElement display;
            if (name.Contains("plasma") || name.Contains("energy") || name.Contains("laser") || name.Contains("focus"))
                display = new plasmaAmmoDisplay(weapon);
            else if (name.Contains("target") || name.Contains("locator"))
                display = new targetLocatorDisplay(saveData);
            else
                display = new regularAmmoDisplay(weapon);

            return display;
        }
    }
}
