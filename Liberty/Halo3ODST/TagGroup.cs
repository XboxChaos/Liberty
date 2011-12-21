using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Liberty.Halo3ODST
{
    public enum TagGroup
    {
        Bipd = 0,
        Vehi = 1,
        Weap = 2,
        Eqip = 3,
        Term = 4,
        Proj = 5,
        Scen = 6,
        Mach = 7,
        Ctrl = 8,
        Ssce = 9,
        Bloc = 10,
        Crea = 11,
        Unk1 = 12,
        Efsc = 13,

        Unknown = 255    // Not actually a valid value, this is just for use as a placeholder.
    }
}
