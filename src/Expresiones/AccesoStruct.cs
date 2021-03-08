using _OLC2_Proyecto1.src.Ambientes;
using _OLC2_Proyecto1.src.Instrucciones.InstGlobales;
using _OLC2_Proyecto1.src.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace _OLC2_Proyecto1.src.Expresiones
{
    public class AccesoStruct : Expresion
    {
        public int line { get; set ; }
        public int column { get ; set ; }

        private string id;
        private List<string> accesos;
        

        public AccesoStruct(int line, int column, string id, List<string> accesos)
        {
            this.line = line;
            this.column = column;
            this.id = id;
            this.accesos = accesos;
            
        }

        public retorno execute(Entorno ent, ProgramClass programClass)
        {
            Simbolo variableFind = ent.getVariable(this.id);
            if (variableFind != null)
            {
                if (variableFind.valor is Struct)
                {
                    Struct structVariableFind = (Struct)variableFind.valor;
                    if (structVariableFind.entStruct.isEmptyVars())
                        structVariableFind.declararVariables(programClass);
                    
                    return getValor(structVariableFind, 0, programClass);

                }else
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

        private retorno getValor(Struct structFind, int i, ProgramClass programClass)
        {
            Simbolo variableFind = structFind.entStruct.getVariable(this.accesos[i]);
            if (variableFind != null)
            {
                if (i + 1 < this.accesos.Count)
                {
                    if (variableFind.valor is Struct)
                    {
                        Struct structVariableFind = (Struct)variableFind.valor;
                        if (structVariableFind.entStruct.isEmptyVars())
                            structVariableFind.declararVariables(programClass);

                        return getValor(structVariableFind, i+1, programClass);
                        
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
                    retorno resultado;
                    resultado.value = variableFind.valor;
                    resultado.type = variableFind.type;
                    return resultado;
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
