using _OLC2_Proyecto1.src.Ambientes;
using _OLC2_Proyecto1.src.Expresiones;
using _OLC2_Proyecto1.src.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace _OLC2_Proyecto1.src.Instrucciones.InstLocales
{
    class While : Instruccion
    {
        public int line { get ; set ; }
        public int column { get; set; }

        private Expresion condicion;

        Instruccion bloqueWhile;

        public While(int line, int column, Expresion condicion, Instruccion bloqueWhile)
        {
            this.line = line;
            this.column = column;
            this.condicion = condicion;
            this.bloqueWhile = bloqueWhile;
        }

        public retorno execute(Entorno ent, ProgramClass programClass)
        {
            retorno condi = condicion.getValorSintetizado(ent, programClass);
            if (condi.type != tiposPrimitivos.BOOLEAN)
            {
                throw new Exception("<tr>\n" +
                "\t<td>Error Semantico</td>\n" +
                "\t<td>La expresion de la condicion es '" + condi.type + "' y debe ser de tipo BOOLEAN</td>\n" +
                "\t<td>" + this.line + "</td>\n" +
                "\t<td>" + this.column + "</td>\n</tr>\n\n");
            }

            while ((bool)condi.value)
            {
                retorno valorRetorno = bloqueWhile.execute(ent, programClass);
                if (valorRetorno.value != null && valorRetorno.type != tiposPrimitivos.VOID)
                    return valorRetorno;

                condi = condicion.getValorSintetizado(ent, programClass);
                if (condi.type != tiposPrimitivos.BOOLEAN)
                {
                    throw new Exception("<tr>\n" +
                    "\t<td>Error Semantico</td>\n" +
                    "\t<td>La expresion de la condicion es '" + condi.type + "' y debe ser de tipo BOOLEAN</td>\n" +
                    "\t<td>" + this.line + "</td>\n" +
                    "\t<td>" + this.column + "</td>\n</tr>\n\n");
                }
            }

            retorno ret;
            ret.value = null;
            ret.type = tiposPrimitivos.VOID;
            return ret;
        }
    }
}
