using _OLC2_Proyecto1.src.Ambientes;
using _OLC2_Proyecto1.src.Expresiones;
using _OLC2_Proyecto1.src.Instrucciones.InstLocales;
using _OLC2_Proyecto1.src.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace _OLC2_Proyecto1.src.Instrucciones
{
    class Bloque : Instruccion
    {
        public int line { get ; set; }
        public int column { get ; set ; }
        private List<Instruccion> Instrucciones;

        public Bloque(int line, int column, List<Instruccion> Instrucciones)
        {
            this.line = line;
            this.column = column;
            this.Instrucciones = Instrucciones;
        }

        public retorno execute(Entorno ent, ProgramClass programClass)
        {
            foreach (Instruccion inst in this.Instrucciones)
            {
                if (inst is Exit)
                    return inst.execute(ent, programClass);

                inst.execute(ent, programClass);
            }

            retorno ret;
            ret.value = null;
            ret.type = tiposPrimitivos.VOID;
            return ret;
        }
    }
}
