using _OLC2_Proyecto1.src.Ambientes;
using _OLC2_Proyecto1.src.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace _OLC2_Proyecto1.src.Instrucciones.InstLocales
{
    class AsignacionVar : Instruccion
    {
        public int line { get; set; }
        public int column { get; set ; }

        private string id;
        private Expresion valor;

        public AsignacionVar(int line, int column, string id, Expresion valor)
        {
            this.line = line;
            this.column = column;
            this.id = id;
            this.valor = valor;
        }

        public void execute(Entorno ent, ProgramClass ast)
        {
            Simbolo varCatch = ent.getVariable(this.id);

            if (varCatch != null)//que exista
            {
                if (!varCatch.constante)//que no sea un aconstante
                {
                    retorno resultado = this.valor.execute(ent, ast);

                    if (varCatch.type == resultado.type) // y que los tipos sean iguales
                    {
                        varCatch.valor = resultado.value;
                        ent.actualizaVariable(this.id, varCatch);
                    }
                    else
                    {
                        throw new Exception("<tr>\n" +
                        "\t<td>Error Semantico</td>\n" +
                        "\t<td>Tipo de datos incopatible, no se puede asignar una expresion de tipo '" + resultado.type + "' a una variable de tipo '" + varCatch.type + "'</td>\n" +
                        "\t<td>" + this.line + "</td>\n" +
                        "\t<td>" + this.column + "</td>\n</tr>");
                    }
                }
                else
                {
                    throw new Exception("<tr>\n" +
                    "\t<td>Error Semantico</td>\n" +
                    "\t<td>No se puede asignar un valor a la variable '"+ this.id +"' porque es una  constante " + "</td>\n" +
                    "\t<td>" + this.line + "</td>\n" +
                    "\t<td>" + this.column + "</td>\n</tr>");
                }
            }
            else
            {
                throw new Exception("<tr>\n" +
                "\t<td>Error Semantico</td>\n" +
                "\t<td>El id:'" + this.id + "' no existe</td>\n" +
                "\t<td>" + this.line + "</td>\n" +
                "\t<td>" + this.column + "</td>\n</tr>");
            }
            

        }
    }
}
