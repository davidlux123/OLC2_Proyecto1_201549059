using _OLC2_Proyecto1.src.Ambientes;
using _OLC2_Proyecto1.src.Instrucciones.InstGlobales;
using _OLC2_Proyecto1.src.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace _OLC2_Proyecto1.src.Instrucciones.InstLocales
{
    public class AsignacionStruct : Instruccion
    {
        public int line { get ; set ; }
        public int column { get ; set ; }

        private string id;
        private List<string> accesos;
        private Expresion valor;

        public AsignacionStruct(int line, int column, string id, List<string> accesos, Expresion valor)
        {
            this.line = line;
            this.column = column;
            this.id = id;
            this.accesos = accesos;
            this.valor = valor;
        }

        public void execute(Entorno ent, ProgramClass programClass)
        {
            Simbolo variableFind = ent.getVariable(this.id);
            if (variableFind != null)
            {
                if (variableFind.valor is Struct)
                {
                    retorno valorSintetizado = this.valor.execute(ent, programClass);

                    Struct structVariableFind = (Struct)variableFind.valor;
                    if (structVariableFind.entStruct.isEmptyVars())
                        structVariableFind.declararVariables(programClass);

                    asignarValor(structVariableFind, 0, valorSintetizado, programClass);

                    variableFind.valor = structVariableFind;
                    ent.actualizaVariable(this.id, variableFind);
                }
                else
                {
                    throw new Exception("<tr>\n" +
                    "\t<td>Error Semantico</td>\n" +
                    "\t<td>El id: '" + this.id + "' no es de tipo Struct </td>\n" +
                    "\t<td>" + this.line + "</td>\n" +
                    "\t<td>" + this.column + "</td>\n</tr>");
                }

            }
            else
            {
                throw new Exception("<tr>\n" +
                "\t<td>Error Semantico</td>\n" +
                "\t<td>El id: '" + this.id + "' no existe</td>\n" +
                "\t<td>" + this.line + "</td>\n" +
                "\t<td>" + this.column + "</td>\n</tr>");
            }
        }

        private void asignarValor(Struct structFind, int i, retorno valorSintetizado,ProgramClass programClass)
        {
            Simbolo variableFind = structFind.entStruct.getVariable(this.accesos[i]);
            if (variableFind != null)
            {
                if (i + 1 < this.accesos.Count)
                {
                    if (variableFind.valor is Struct)
                    {
                        Struct structVariableFind = (Struct)variableFind.valor;// se le asigna el struct encontrado que esta dentro del struct
                        if (structVariableFind.entStruct.isEmptyVars())
                            structVariableFind.declararVariables(programClass);

                         asignarValor(structVariableFind, i+1, valorSintetizado, programClass);

                        variableFind.valor = structVariableFind;
                        structFind.entStruct.actualizaVariable(accesos[i], variableFind);

                    }
                    else
                    {
                        throw new Exception("<tr>\n" +
                        "\t<td>Error Semantico</td>\n" +
                        "\t<td>no se puede acceder al id '" + this.accesos[i] + "' porque no es de tipo Struct</td>\n" +
                        "\t<td>" + this.line + "</td>\n" +
                        "\t<td>" + this.column + "</td>\n</tr>");
                    }
                }
                else
                {
                    if (variableFind.type == valorSintetizado.type) // y que los tipos sean iguales
                    {
                        variableFind.valor = valorSintetizado.value;
                        structFind.entStruct.actualizaVariable(accesos[i], variableFind);
                    }
                    else
                    {
                        throw new Exception("<tr>\n" +
                        "\t<td>Error Semantico</td>\n" +
                        "\t<td>Tipo de datos incopatible, no se puede asignar una expresion de tipo '" + valorSintetizado.type + "' a una variable de tipo '" + variableFind.type + "'</td>\n" +
                        "\t<td>" + this.line + "</td>\n" +
                        "\t<td>" + this.column + "</td>\n</tr>");
                    }
                }

            }
            else
            {
                throw new Exception("<tr>\n" +
                "\t<td>Error Semantico</td>\n" +
                "\t<td>El id '" + this.accesos[i] + "' no existe declarado como variable en el Struct" + structFind.idStruct + "</td>\n" +
                "\t<td>" + this.line + "</td>\n" +
                "\t<td>" + this.column + "</td>\n</tr>");
            }

        }
    }
}
