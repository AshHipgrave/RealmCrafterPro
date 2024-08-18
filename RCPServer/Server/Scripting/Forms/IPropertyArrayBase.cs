using System;
using System.Collections.Generic;
using System.Text;

namespace Scripting.Forms
{
    /// <summary>
    /// Base class for controls which rely on a PropertyArray.
    /// </summary>
    public interface IPropertyArrayBase<T>
    {
        /// <summary>
        /// Called when an item is modified in items.
        /// </summary>
        /// <param name="items"></param>
        /// <param name="index"></param>
        void UpdateIndexedProperty(List<T> items, int index);

        /// <summary>
        /// Called when items are added
        /// </summary>
        /// <param name="items"></param>
        /// <param name="values"></param>
        void AddProperty(List<T> items, T[] values);

        /// <summary>
        /// Called when items are removed
        /// </summary>
        /// <param name="items"></param>
        /// <param name="index"></param>
        void RemoveProperty(List<T> items, int index);
    }
}
