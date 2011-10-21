/*
* Liberty - http://xboxchaos.com/
*
* Copyright (C) 2011 XboxChaos
* Copyright (C) 2011 ThunderWaffle/AMD
* Copyright (C) 2011 Xeraxic
*
* Liberty is free software; you can redistribute it and/or modify
* it under the terms of the GNU General Public License as published
* by the Free Software Foundation; either version 3 of the License,
* or (at your option) any later version.
*
* Liberty is distributed in the hope that it will be useful, but
* WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
* General Public License for more details.
*
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Liberty.classInfo
{
    class loadPackageData
    {
        public static string convertClassToString(Reach.TagGroup type)
        {
            return type.ToString().ToLower();
        }

        public static Reach.TagGroup convertStringToClass(string type)
        {
            switch (type)
            {
                case "bipd":
                    return Reach.TagGroup.Bipd;
                case "vehi":
                    return Reach.TagGroup.Vehi;
                case "weap":
                    return Reach.TagGroup.Weap;
                case "eqip":
                    return Reach.TagGroup.Eqip;
                case "term":
                    return Reach.TagGroup.Term;
                case "scen":
                    return Reach.TagGroup.Scen;
                case "mach":
                    return Reach.TagGroup.Mach;
                case "ctrl":
                    return Reach.TagGroup.Ctrl;
                case "ssce":
                    return Reach.TagGroup.Ssce;
                case "bloc":
                    return Reach.TagGroup.Bloc;
                case "crea":
                    return Reach.TagGroup.Crea;
                case "efsc":
                    return Reach.TagGroup.Efsc;
                case "proj":
                    return Reach.TagGroup.Proj;
            }
            return Reach.TagGroup.Unknown;
        }
    }
}
