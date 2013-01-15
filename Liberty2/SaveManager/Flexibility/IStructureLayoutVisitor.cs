﻿/* Copyright 2012 Aaron Dierking, TJ Tunnell, Jordan Mueller, Alex Reed
 * 
 * This file is part of ExtryzeDLL.
 * 
 * Extryze is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * Extryze is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with ExtryzeDLL.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Liberty.SaveManager.Flexibility
{
    /// <summary>
    /// Defines the interface for a class which acts as a visitor for fields in
    /// a structure.
    /// </summary>
    public interface IStructureLayoutVisitor
    {
        /// <summary>
        /// Called when a basic structure field is visited.
        /// </summary>
        /// <param name="name">The name of the field.</param>
        /// <param name="type">The type of the field's value.</param>
        /// <param name="offset">The offset (in bytes) of the field from the beginning of the structure.</param>
        void VisitBasicField(string name, StructureValueType type, int offset);

        /// <summary>
        /// Called when an array structure field is visited.
        /// </summary>
        /// <param name="name">The name of the array field.</param>
        /// <param name="offset">The offset (in bytes) of the field from the beginning of the structure.</param>
        /// <param name="count">The number of elements in the array.</param>
        /// <param name="entrySize">The size (in bytes) of each array element.</param>
        /// <param name="entryLayout">The layout of each element in the array.</param>
        void VisitArrayField(string name, int offset, int count, int entrySize, StructureLayout entryLayout);
    }
}
