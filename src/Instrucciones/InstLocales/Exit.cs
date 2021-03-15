using _OLC2_Proyecto1.src.Ambientes;
using _OLC2_Proyecto1.src.Expresiones;
using _OLC2_Proyecto1.src.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace _OLC2_Proyecto1.src.Instrucciones.InstLocales
{
    public class Exit : Instruccion
    {
        public int line { get ; set; }
        public int column { get; set; }

        Expresion expresion;

        public Exit(int line, int column, Expresion expresion)
        {
            this.line = line;
            this.column = column;
            this.expresion = expresion;
        }

        public Exit(int line, int column)
        {
            this.line = line;
            this.column = column;
            this.expresion = null;
        }

        public retorno execute(Entorno ent, ProgramClass programClass)
        {
            if (this.expresion != null)
            {
                return expresion.getValorSintetizado(ent, programClass);
            }

            retorno ret;
            ret.value = null;
            ret.type = tiposPrimitivos.VOID;
            return ret;
        }
    }
}
