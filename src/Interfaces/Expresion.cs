using System;
using System.Collections.Generic;
using System.Text;
using _OLC2_Proyecto1.src.Expresiones;
using _OLC2_Proyecto1.src.Ambientes;

namespace _OLC2_Proyecto1.src.Interfaces
{
    public struct retorno
    {
        public tiposPrimitivos type;
        public object value;
    }

    public interface Expresion
    {
        public int line { get; set; }
        public int column { get; set; }
        public retorno execute(Entorno ent, ProgramClass programClass);
     
    }
}
