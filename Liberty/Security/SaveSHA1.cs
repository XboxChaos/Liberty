/*
* Liberty - http://xboxchaos.com/
*
* Copyright (C) 2011 XboxChaos
* Copyright (C) 2011 ThunderWaffle/AMD
* Copyright (C) 2011 Xeraxic
*
* Liberty is free software; you can redistribute it and/or modify
* it under the terms of the GNU General Public License as published
* by the Free Software Foundation; either version 2 of the License,
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
using System.Security.Cryptography;

namespace Liberty.Security
{
    /// <summary>
    /// Computes salted SHA-1 digests by using the algorithm used in mmiof.bmf.
    /// </summary>
    class SaveSHA1 : SHA1Managed
    {
        /// <summary>
        /// Constructs and initializes a new SaveSHA1 object.
        /// </summary>
        /// <seealso cref="Initialize"/>
        public SaveSHA1()
        {
            Initialize();
        }

        /// <summary>
        /// Initializes the hash by transforming the salt.
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();
            base.TransformBlock(_salt, 0, 34, _salt, 0);
        }

        /// <summary>
        /// The salt used to verify the validity of saves
        /// </summary>
        private static byte[] _salt = new byte[]
        {
            0xED, 0xD4, 0x30, 0x09, 0x66, 0x6D, 0x5C, 0x4A, 0x5C, 0x36, 0x57,
            0xFA, 0xB4, 0x0E, 0x02, 0x2F, 0x53, 0x5A, 0xC6, 0xC9, 0xEE, 0x47,
            0x1F, 0x01, 0xF1, 0xA4, 0x47, 0x56, 0xB7, 0x71, 0x4F, 0x1C, 0x36,
            0xEC
        };
    }
}
