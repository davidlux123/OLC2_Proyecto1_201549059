using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using _OLC2_Proyecto1.src.Interfaces;

namespace _OLC2_Proyecto1.src
{
    public class ProgramClass
    {
        private Hashtable Structs;
        private Hashtable ProcFunc;

        public ProgramClass()
        {
            this.Structs = new Hashtable();
            this.ProcFunc = new Hashtable();
        }

        public bool existeID(string id)
        {
            if (Structs.Contains(id))
            {
                return true;
            }
            else if (ProcFunc.Contains(id))
            {
                return true;
            }
            return false;
        }

    }
}
