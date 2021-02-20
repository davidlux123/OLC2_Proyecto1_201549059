using System;
using System.Collections.Generic;
using System.Text;

namespace _OLC2_Proyecto1.src.Interfaces
{
    public interface Instruccion
    {
        public int line { get; set; }
        public int column { get; set; }
        public void execute();
    }
}
