using _OLC2_Proyecto1.src.Ambientes;
using _OLC2_Proyecto1.src.Gramatica;
using _OLC2_Proyecto1.src.Instrucciones;
using _OLC2_Proyecto1.src.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace _OLC2_Proyecto1.src.Expresiones
{
    public class Acceso : Expresion
    {
        public int line { get; set ; }
        public int column { get ; set ; }

        private List<expPosArr> accesos;
        
        public Acceso(int line, int column, List<expPosArr> accesos)
        {
            this.line = line;
            this.column = column;
            this.accesos = accesos;
            
        }

        public retorno getReferenciaSintetizado(Entorno ent, ProgramClass programClass)
        {
            return getValorReferencia(ent, 0, ent, programClass);
        }

        private retorno getValorReferencia(Entorno entActual, int i, Entorno ent, ProgramClass programClass)
        {
            Simbolo variableFind = entActual.getVariable(this.accesos[i].id);
            if (variableFind != null)
            {
                retorno valorFind;
                if (variableFind.valor is Arreglo && this.accesos[i].posicion.Count > 0)
                    valorFind = getValor((Arreglo)variableFind.valor, this.accesos[i].posicion, ent, programClass);
                else
                {
                    valorFind.value = variableFind.valor;
                    valorFind.type = variableFind.type;
                }

                if (i + 1 < this.accesos.Count)
                {
                    if (valorFind.value is Struct)
                    {
                        Struct structVariableFind = (Struct)valorFind.value;
                        return getValorAccesos(structVariableFind.entStruct, i + 1, ent, programClass);
                    }
                    else
                    {
                        throw new Exception("<tr>\n" +
                        "\t<td>Error Semantico</td>\n" +
                        "\t<td>no se puede acceder al id '" + this.accesos[i].id + "' porque no es de tipo Struct</td>\n" +
                        "\t<td>" + this.line + "</td>\n" +
                        "\t<td>" + this.column + "</td>\n</tr>");
                    }
                }
                else
                {
                    if (variableFind.valor is Arreglo && this.accesos[i].posicion.Count > 0)
                        return valorFind;

                    retorno val;
                    val.value = variableFind;
                    val.type = variableFind.type;
                    return val;
                }

            }
            else
            {
                throw new Exception("<tr>\n" +
                "\t<td>Error Semantico</td>\n" +
                "\t<td>El id '" + this.accesos[i].id + "' no existe declarado variable en el entorno de " + entActual.nombreAmbito + "</td>\n" +
                "\t<td>" + this.line + "</td>\n" +
                "\t<td>" + this.column + "</td>\n</tr>");
            }

        }

        public retorno getValorSintetizado(Entorno ent, ProgramClass programClass)
        {
            return getValorAccesos(ent, 0, ent, programClass);
        }
       
        private retorno getValorAccesos(Entorno entActual, int i, Entorno ent, ProgramClass programClass)
        {
            Simbolo variableFind = entActual.getVariable(this.accesos[i].id);
            if (variableFind != null)
            {
                retorno valorFind;
                if (variableFind.valor is Arreglo && this.accesos[i].posicion.Count > 0)
                    valorFind = getValor((Arreglo)variableFind.valor, this.accesos[i].posicion, ent, programClass);
                else
                {
                    valorFind.value = variableFind.valor;
                    valorFind.type = variableFind.type;
                }
                
                if (i + 1 < this.accesos.Count)
                {
                    if (valorFind.value is Struct)
                    {
                        Struct structVariableFind = (Struct)valorFind.value;
                        return getValorAccesos(structVariableFind.entStruct, i+1, ent, programClass);
                    }
                    else
                    {
                        throw new Exception("<tr>\n" +
                        "\t<td>Error Semantico</td>\n" +
                        "\t<td>no se puede acceder al id '" + this.accesos[i].id + "' porque no es de tipo Struct</td>\n" +
                        "\t<td>" + this.line + "</td>\n" +
                        "\t<td>" + this.column + "</td>\n</tr>");
                    }
                }
                else
                {
                    if (variableFind.valor is Arreglo && this.accesos[i].posicion.Count > 0)
                        return valorFind;

                    retorno val;
                    val.value = variableFind.valor;
                    val.type = variableFind.type;
                    return val;
                }

            }
            else
            {
                throw new Exception("<tr>\n" +
                "\t<td>Error Semantico</td>\n" +
                "\t<td>El id '" + this.accesos[i].id + "' no existe declarado variable en el entorno de " + entActual.nombreAmbito + "</td>\n" +
                "\t<td>" + this.line + "</td>\n" +
                "\t<td>" + this.column + "</td>\n</tr>");
            }

        }

        private retorno getValor(Arreglo arreglo, List<Expresion> posiciones, Entorno ent, ProgramClass programClass)
        {
           return getValorRecursivo(arreglo.contenido, arreglo.dimensiones, arreglo.typeArrglo, posiciones, 0, 0, ent, programClass);
        }

        private retorno getValorRecursivo(object[] arrActual, List<int[]> dimsActual, object tipoArrActual, List<Expresion> posiciones, int P, int A, Entorno ent, ProgramClass programClass)
        {
            retorno pos = posiciones[P].getValorSintetizado(ent, programClass);
            if (pos.type != tiposPrimitivos.INT)
                throw new Exception("<tr>\n" +
                "\t<td>Error Semantico</td>\n" +
                "\t<td>La Posicion el resutado de la posicion tiene que ser de tipo integer</td>\n" +
                "\t<td>" + this.line + "</td>\n" +
                "\t<td>" + this.column + "</td>\n</tr>");

            int posReal = (int)pos.value - dimsActual[A][1];

            if (posReal >= 0 && posReal < arrActual.Length)
            {
                object posActual = arrActual[posReal];

                if (A + 1 < dimsActual.Count)
                {
                    object[] arrFind = (object[])posActual;

                    if (P + 1 < posiciones.Count)
                        return getValorRecursivo(arrFind, dimsActual, tipoArrActual, posiciones,  P + 1, A + 1, ent, programClass);
                    else
                        throw new Exception("<tr>\n" +
                        "\t<td>Error Semantico</td>\n" +
                        "\t<td>La Posicion " + String.Join(",", posiciones) + " solicitida no es valida</td>\n" +
                        "\t<td>" + this.line + "</td>\n" +
                        "\t<td>" + this.column + "</td>\n</tr>");
                }
                else
                {
                    if (P + 1 < posiciones.Count)
                    {
                        if (posActual is Arreglo)
                        {
                            Arreglo arreglo = (Arreglo)posActual;
                            return getValorRecursivo(arreglo.contenido, arreglo.dimensiones, arreglo.typeArrglo, posiciones,  P + 1, 0, ent, programClass);
                        }
                        else
                        {
                            throw new Exception("<tr>\n" +
                            "\t<td>El tipo de dato en la posicion '" + String.Join(",", posiciones) + "' no es arreglo </td>\n" +
                            "\t<td></td>\n" +
                            "\t<td>" + this.line + "</td>\n" +
                            "\t<td>" + this.column + "</td>\n</tr>");
                        }
                    }
                    else
                    {
                        retorno resultado;
                        if (tipoArrActual is string)
                        {
                            if (posActual is Arreglo)
                            {
                                resultado.value = posActual;
                                resultado.type = tiposPrimitivos.ARRAEGLO;
                            }
                            else
                            {
                                resultado.value = posActual;
                                resultado.type = tiposPrimitivos.STRUCT;
                            }
                        }
                        else
                        {
                            resultado.value = posActual;
                            resultado.type = (tiposPrimitivos)tipoArrActual;
                        }
                        return resultado;
                    }
                }
            }
            else
            {
                throw new Exception("<tr>\n" +
                "\t<td>Error Semantico</td>\n" +
                "\t<td>La posicion '" + posiciones[P] + "' no se encuentra en el rango especificado</td>\n" +
                "\t<td>" + this.line + "</td>\n" +
                "\t<td>" + this.column + "</td>\n</tr>");
            }

        }

    }
}
