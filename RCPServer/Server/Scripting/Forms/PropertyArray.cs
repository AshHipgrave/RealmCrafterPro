using System;
using System.Collections.Generic;
using System.Text;

namespace Scripting.Forms
{
    /// <summary>
    /// Array container for control properties for callback-based updates.
    /// </summary>
    /// <typeparam name="T">Array type</typeparam>
    public class PropertyArray<T>
    {
        IPropertyArrayBase<T> parent;
        List<T> items;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="inParent">Parent object to call on update.</param>
        /// <param name="inItems">Item list to maintain.</param>
        public PropertyArray(IPropertyArrayBase<T> inParent, List<T> inItems)
        {
            parent = inParent;
            items = inItems;
        }

        /// <summary>
        /// Gets or Sets the value at the specified index.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public T this[int index]
        {
            get
            {
                if (index < 0 || index >= items.Count)
                    throw new ArgumentOutOfRangeException("", "Index must be in range of 0 to Items.Count - 1");

                return items[index];
            }

            set
            {
                if (index < 0 || index >= items.Count)
                    throw new ArgumentOutOfRangeException("", "Index must be in range of 0 to Items.Count - 1");

                items[index] = value;

                parent.UpdateIndexedProperty(items, index); 
            }
        }

        /// <summary>
        /// Add an item to the list.
        /// </summary>
        /// <param name="value"></param>
        public void Add(T value)
        {
            parent.AddProperty(items, new T[] { value });
        }

        /// <summary>
        /// Add an array of items to the list.
        /// </summary>
        /// <param name="values"></param>
        public void AddRange(T[] values)
        {
            parent.AddProperty(items, values);
        }

        /// <summary>
        /// Remove an element from the list.
        /// </summary>
        /// <param name="value"></param>
        public void Remove(T value)
        {
            int Idx = -1;
            int Count = 0;

            foreach (T I in items)
            {
                if (I.Equals(value))
                {
                    Idx = Count;
                    break;
                }
                ++Count;
            }

            if(Idx != -1)
                parent.RemoveProperty(items, Idx);
        }

        /// <summary>
        /// Remove an element from the list.
        /// </summary>
        /// <param name="index"></param>
        public void Remove(int index)
        {
            if (index >= 0 && index < items.Count)
                parent.RemoveProperty(items, index);
        }

        /// <summary>
        /// Get the number of elements in the list.
        /// </summary>
        public int Count
        {
            get { return items.Count; }
        }
    }
}
