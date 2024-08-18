using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Scripting;

namespace RCPServer
{
    public class DroppedItem
    {
        #region Members
        public Area.AreaInstance ServerHandle = null;
        public Scripting.Math.SectorVector Position = new Scripting.Math.SectorVector();
        public Area.InstanceSector Sector = null;
        public ItemInstance Item = null;
        public int Amount = 0;
        public int AllocID = 0;
        #endregion

        #region Methods
        public DroppedItem(Area.InstanceSector sector)
        {
            ++LastAllocID;
            AllocID = LastAllocID;
            Sector = sector;
            sector.DroppedItems.AddLast(this);
            DroppedItemList.AddLast(this);
        }
        #endregion

        #region Static Members
        protected static int LastAllocID = 0;
        public static LinkedList<DroppedItem> DroppedItemList = new LinkedList<DroppedItem>();
        public static LinkedList<DroppedItem> DroppedItemDelete = new LinkedList<DroppedItem>();
        #endregion

        #region Static Methods
        public static DroppedItem FromAllocID(int id)
        {
            foreach (DroppedItem Di in DroppedItemList)
                if (Di.AllocID == id)
                    return Di;
            return null;
        }


        public static void Delete(DroppedItem item)
        {
            DroppedItemDelete.AddLast(item);
        }

        public static void Clean()
        {
            foreach (DroppedItem A in DroppedItemDelete)
            {
                DroppedItemList.Remove(A);
                if (A.Sector != null)
                    A.Sector.DroppedItems.Remove(A);
            }
            DroppedItemDelete.Clear();
        }
        #endregion
    }
}
