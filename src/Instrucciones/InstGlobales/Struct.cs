using _OLC2_Proyecto1.src.Ambientes;
using _OLC2_Proyecto1.src.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace _OLC2_Proyecto1.src.Instrucciones.InstGlobales
{
    class Struct : Instruccion
    {
        public int line { get ; set ; }
        public int column { get ; set ; }

        public void execute(Entorno ent, ProgramClass programClass)
        {
            throw new NotImplementedException();
        }
    }
}
