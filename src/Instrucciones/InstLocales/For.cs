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
        List<Instruccion> instsFor;

        public For(int line, int column, string id, Expresion valorAsig, Expresion valorLim, List<Instruccion> instsFor)
        {
            this.line = line;
            this.column = column;
            this.id = id;
            this.valorAsig = valorAsig;
            this.valorLim = valorLim;
            this.instsFor = instsFor;
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
                string errors = "";
                for (int i = (int)valor.value; i <= (int)lim.value; i++)
                {
                    simbo.valor = i;
                    ent.actualizaVariable(simbo.idVariable, simbo);

                    foreach (Instruccion inst in this.instsFor)
                    {
                        try
                        {
                            inst.execute(ent, programClass);
                        }
                        catch (Exception error)
                        {
                            errors += error.Message + "\n|\n";
                        }
                    }
                }
                if (errors != "")
                    throw new Exception(errors);
                
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
