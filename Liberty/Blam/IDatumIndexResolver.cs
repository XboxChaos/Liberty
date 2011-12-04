using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Liberty.Blam
{
    public interface IDatumIndexResolver<T>
    {
        /// <summary>
        /// Resolves a datum index and retrieves its object reference.
        /// </summary>
        /// <param name="index">The datum index to resolve.</param>
        /// <returns>The object reference if found, or null otherwise.</returns>
        T ResolveIndex(DatumIndex index);
    }
}
