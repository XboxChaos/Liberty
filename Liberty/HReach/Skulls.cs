using System;

namespace Liberty.Reach
{
    /// <summary>
    /// Skull bits used in the savedata's skulls bitfield
    /// </summary>
    [Flags]
    public enum Skulls : uint
    {
        None = 0,
        Iron = 0x1,
        BlackEye = 0x2,
        ToughLuck = 0x4,
        Catch = 0x8,
        Cloud = 0x10,
        Famine = 0x20,
        Thunderstorm = 0x40,
        Tilt = 0x80,
        Mythic = 0x100,
        Assassain = 0x200,  // trolololol
        Blind = 0x400,
        Cowbell = 0x800,
        GruntBirthday = 0x1000,
        IWHBYD = 0x2000,
        // RGB skulls probably follow, but they're pointless for campaign
        LASO = Iron | BlackEye | ToughLuck | Catch | Cloud | Famine | Thunderstorm | Tilt | Mythic | Cowbell | GruntBirthday | IWHBYD
    }
}