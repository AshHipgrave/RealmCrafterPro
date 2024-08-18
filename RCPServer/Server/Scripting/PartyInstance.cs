using System;
using System.Collections.Generic;
using System.Text;

namespace Scripting
{
    public class PartyInstance
    {        

        public virtual List<Actor> GetCurrentMembers()
        {
            return null;
        }

        public virtual byte[] GetData() { return null; }
        public virtual void SetData(byte[] data) { }

    }
}
