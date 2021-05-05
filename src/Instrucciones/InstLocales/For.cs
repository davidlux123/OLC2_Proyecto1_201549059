using _OLC2_Proyecto1.src.Ambientes;
using _OLC2_Proyecto1.src.Expresiones;
using _OLC2_Proyecto1.src.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace _OLC2_Proyecto1.src.Instrucciones.InstLocales
{
    class For : Instruccion
    {
        public int line { get ; set ; }
        public int column { get ; set ; }

        private string id;
        private Expresion valorAsig;
        private Expresion valorLim;
        Instruccion bloqueFor;

        public For(int line, int column, string id, Expresion valorAsig, Expresion valorLim, Instruccion bloqueFor)
        {
            this.line = line;
            this.column = column;
            this.id = id;
            this.valorAsig = valorAsig;
            this.valorLim = valorLim;
            this.bloqueFor = bloqueFor;
        }

        public retorno execute(Entorno ent, ProgramClass programClass)
        {
            Simbolo simbo = ent.getVariable(this.id);
            if (simbo == null)
            {
                throw new Exception("<tr>\n" +
                "\t<td>Error Semantico</td>\n" + "\t<td>el id " + this.id + "no existe</td>\n" +
                "\t<td>" + this.line + "</td>\n" +"\t<td>" + this.column + "</td>\n</tr>\n\n");
            }

            retorno valor = valorAsig.getValorSintetizado(ent, programClass);
            retorno lim = valorLim.getValorSintetizado(ent, programClass);
            if (simbo.type == tiposPrimitivos.INT && valor.type == tiposPrimitivos.INT && lim.type == tiposPrimitivos.INT)
            {
                for (int i = (int)valor.value; i <= (int)lim.value; i++)
                {
                    simbo.valor = i;
                    ent.actualizaVariable(simbo.idVariable, simbo);

                    retorno valorRetorno = bloqueFor.execute(ent, programClass);
                    if (valorRetorno.value != null && valorRetorno.type != tiposPrimitivos.VOID)
                        return valorRetorno;
                }
            }
            else
                throw new Exception("<tr>\n" +
                "\t<td>No se puede asignar un tipo de tado " + simbo.type + " con un tipo " + valor.type + "</td>\n" +
                "\t<td>" + this.line + "</td>\n" + "\t<td>" + this.column + "</td>\n</tr>\n\n");


            retorno ret1;
            ret1.value = null;
            ret1.type = tiposPrimitivos.VOID;
            return ret1;
        }
    }
}
