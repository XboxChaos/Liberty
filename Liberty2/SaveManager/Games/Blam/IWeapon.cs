using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Liberty.SaveManager.Games.Blam
{
    public interface IWeapon
    {
        short Ammo { get; set; }
        short ClipAmmo { get; set; }
        float Energy { get; set; }
    }
}
