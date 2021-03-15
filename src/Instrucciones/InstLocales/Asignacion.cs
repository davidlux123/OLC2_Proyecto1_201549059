using _OLC2_Proyecto1.src.Ambientes;
using _OLC2_Proyecto1.src.Expresiones;
using _OLC2_Proyecto1.src.Gramatica;
using _OLC2_Proyecto1.src.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace _OLC2_Proyecto1.src.Instrucciones.InstLocales
{
    public struct posArr
    {
        public string id;
        public List<int> posicion;
    }

    public class Asignacion : Instruccion
    {
        public int line { get ; set ; }
        public int column { get ; set ; }

        private List<expPosArr> expAccesos;
        private List<posArr> accesos;
        private Expresion valor;

        public Asignacion(int line, int column, List<expPosArr> expAccesos, Expresion valor)
        {
            this.line = line;
            this.column = column;
            this.expAccesos = expAccesos;
            this.valor = valor;
        }

        private List<posArr> validarPosiciones(Entorno ent, ProgramClass programClass)
        {
            List<posArr> accesos = new List<posArr>();

            foreach (expPosArr acceso in this.expAccesos)
            {
                List<int> posiciones = new List<int>();

                foreach (Expresion expPos in acceso.posicion)
                {
                    retorno pos = expPos.getValorSintetizado(ent, programClass);
                    if (pos.type != tiposPrimitivos.INT)
                        throw new Exception("<tr>\n" +
                        "\t<td>Error Semantico</td>\n" +
                        "\t<td>La Posicion el resutado de la posicion tiene que ser de tipo integer</td>\n" +
                        "\t<td>" + this.line + "</td>\n" +
                        "\t<td>" + this.column + "</td>\n</tr>");

                    posiciones.Add((int)pos.value);

                }

                posArr nvoAcceso;
                nvoAcceso.id = acceso.id;
                nvoAcceso.posicion = posiciones;
                accesos.Add(nvoAcceso);
            }

            return accesos;
        }

        public retorno execute(Entorno ent, ProgramClass programClass)
        {
            this.accesos = validarPosiciones(ent, programClass);
            asignarValor(ent, 0, ent, programClass);


            retorno ret;
            ret.value = null;
            ret.type = tiposPrimitivos.VOID;
            return ret;
        }

        private void asignarValor(Entorno entActual, int i, Entorno ent, ProgramClass programClass)
        {
            Simbolo variableFind = entActual.getVariable(this.accesos[i].id);
            if (variableFind != null)
            {
                object valorFind;
                if (variableFind.valor is Arreglo && this.accesos[i].posicion.Count > 0)
                {
                    if(i + 1 < this.accesos.Count)
                        valorFind = getValor((Arreglo)variableFind.valor, this.accesos[i].posicion);
                    else
                    {
                        retorno valorSintetizado = this.valor.getValorSintetizado(ent, programClass);
                        valorFind = actualizarValor((Arreglo)variableFind.valor, this.accesos[i].posicion, valorSintetizado);
                    } 
                }
                else
                    valorFind = variableFind.valor;

                if (i + 1 < this.accesos.Count)
                {
                    if (valorFind is Struct)
                    {
                        Struct structVariableFind = (Struct)valorFind;// se le asigna el struct encontrado que esta dentro del struct
                        asignarValor(structVariableFind.entStruct, i+1, ent, programClass);

                        if (variableFind.valor is Arreglo && this.accesos[i].posicion.Count > 0)
                            variableFind.valor = actualizarValor((Arreglo)variableFind.valor, this.accesos[i].posicion, structVariableFind);
                        else
                            variableFind.valor = structVariableFind;

                        entActual.actualizaVariable(this.accesos[i].id, variableFind);
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
                    {
                        variableFind.valor = valorFind;
                        entActual.actualizaVariable(this.accesos[i].id, variableFind);
                    }
                    else
                    {
                        retorno valorSintetizado = this.valor.getValorSintetizado(ent, programClass);
                        if (variableFind.type == valorSintetizado.type) // y que los tipos sean iguales
                        {
                            variableFind.valor = setearValor(variableFind.valor, valorSintetizado.value);
                            entActual.actualizaVariable(this.accesos[i].id, variableFind);
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

        //--------------------------------------------------------------------------------------------------------------------------------
        private object getValor(Arreglo arreglo, List<int> posiciones)
        {
            return getValorRecursivo(arreglo.contenido, arreglo.dimensiones, posiciones, 0, 0);
        }

        private object getValorRecursivo(object [] arrActual, List<int[]> dimsArrActual, List<int> posiciones, int P ,int A)
        {
            int posReal = posiciones[P] - dimsArrActual[A][1];

            if (posReal >= 0 && posReal < arrActual.Length)
            {
                object posActual = arrActual[posReal];

                if (A + 1 < dimsArrActual.Count)
                {
                    object[] arrFind = (object[])posActual;

                    if(P + 1 < posiciones.Count)
                        return getValorRecursivo(arrFind, dimsArrActual, posiciones, P + 1, A + 1);
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
                            return getValorRecursivo(arreglo.contenido, arreglo.dimensiones, posiciones, P + 1, 0);
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
                        return posActual;
                    }
                }
            }
            else
            {
                throw new Exception("<tr>\n" +
                "\t<td>Error Semantico</td>\n" +
                "\t<td>La posicion '"+ posiciones[P] + "' no se encuentra en el rango especificado</td>\n" +
                "\t<td>" + this.line + "</td>\n" +
                "\t<td>" + this.column + "</td>\n</tr>");
            }
            
        }

        //-------------------------------------------------------------------------------------------------------------------------------
        private object actualizarValor(Arreglo arreglo, List<int> posiciones, object resultado)
        {
            retorno resultAux;
            if (resultado is Struct)
            {
                resultAux.value = resultado;
                resultAux.type = tiposPrimitivos.STRUCT;
            }
            else
                resultAux = (retorno)resultado;

            actualizarValorRecursivo(arreglo.contenido, arreglo.dimensiones, arreglo.typeArrglo, posiciones, resultAux, 0, 0);
            return arreglo;
        }

        private void actualizarValorRecursivo(object[] arrActual, List<int[]> dimsArrActual, object tipoArrActual, List<int> posiciones, retorno resultado, int P, int A)
        {
            int posReal = posiciones[P] - dimsArrActual[A][1];

            if (posReal >= 0 && posReal < arrActual.Length)
            {
                object posActual = arrActual[posReal];

                if (A + 1 < dimsArrActual.Count)
                {
                    object[] arrFind = (object[])posActual;

                    if (P + 1 < posiciones.Count)
                    {
                        actualizarValorRecursivo(arrFind, dimsArrActual, tipoArrActual, posiciones, resultado, P + 1, A + 1);
                        arrActual[posReal] = arrFind;
                    }
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
                            Arreglo arregloFind = (Arreglo)posActual;
                            actualizarValorRecursivo(arregloFind.contenido, arregloFind.dimensiones, arregloFind.typeArrglo, posiciones,  resultado,  P + 1, 0);
                            arrActual[posReal] = arregloFind;
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
                        if (getType(posActual) == resultado.type)
                        {
                            arrActual[posReal] = setearValor(posActual, resultado.value);
                        }
                        else
                        {
                            throw new Exception("<tr>\n" +
                            "\t<td>Error Semantico</td>\n" +
                            "\t<td>Tipo de datos incopatible, no se puede asignar una expresion de tipo '" + getType(posActual) + "' a una variable de tipo '" + tipoArrActual + "'</td>\n" +
                            "\t<td>" + this.line + "</td>\n" +
                            "\t<td>" + this.column + "</td>\n</tr>");
                        }
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
        //-----------------------------------------------------------------------------------------------------------------------------------------
       
        private tiposPrimitivos getType(object posActual)
        {
            if (posActual is Arreglo)
            {
                return tiposPrimitivos.ARRAEGLO;
            }
            else if (posActual is  Struct)
            {
                return tiposPrimitivos.STRUCT;
            }
            else if (posActual is int)
            {
                return tiposPrimitivos.INT;
            }
            else if (posActual is string)
            {
                return tiposPrimitivos.STRING;
            }
            else if (posActual is bool)
            {
                return tiposPrimitivos.BOOLEAN;
            }
            else
            {
                return tiposPrimitivos.REAL;
            }
        }

        private object setearValor(object posActual, object resultado)
        {
            if (posActual is Arreglo && resultado is Arreglo)
            {
                Arreglo arrAct = (Arreglo)posActual;
                Arreglo arrNvo = (Arreglo)resultado;

                if (validarTiposArreglo(arrAct, arrNvo))
                {
                    arrAct = (Arreglo)arrNvo.Clone();
                    return arrAct;
                }
                else
                    throw new Exception("<tr>\n" +
                    "\t<td>Error Semantico</td>\n" +
                    "\t<td>Tipo de Arreglo '" + arrAct.typeArrglo + "' no coincide con " + arrAct.typeArrglo + "</td>\n" +
                    "\t<td>" + this.line + "</td>\n" +
                    "\t<td>" + this.column + "</td>\n</tr>");
            }
            else if (posActual is Struct && resultado is Struct)
            {
                Struct struAct = (Struct)posActual;
                Struct struNvo = (Struct)resultado;

                if (validartiposStruct(struAct, struNvo))
                    return struNvo;
                else
                    throw new Exception("<tr>\n" +
                    "\t<td>Error Semantico</td>\n" +
                    "\t<td>Tipo de Struct '" + struAct.idStruct + "' no coincide con " + struNvo.idStruct + "</td>\n" +
                    "\t<td>" + this.line + "</td>\n" +
                    "\t<td>" + this.column + "</td>\n</tr>");
            }
            else
                return resultado;
                

        }

        private bool validarTiposArreglo(Arreglo arreglo, Arreglo arrAux)
        {
            if (arreglo.idArreglo == "" || arrAux.idArreglo == "")
            {
                if (arreglo.dimensiones.Count == arrAux.dimensiones.Count && validarTipos (arreglo.typeArrglo, arrAux.typeArrglo))
                {
                    for (int i = 0; i < arreglo.dimensiones.Count; i++)
                    {
                        bool valido = arreglo.dimensiones[i][0] == arrAux.dimensiones[i][0] && arreglo.dimensiones[i][1] == arrAux.dimensiones[i][1];
                        if (!valido)
                            return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            else
            {
                if (arreglo.idArreglo != arrAux.idArreglo)
                    return false;
            }
            return true;
        }

        private bool validarTipos(object tipo1, object tipo2)
        {
            if (tipo1 is tiposPrimitivos && tipo2 is tiposPrimitivos)
            {
                if ((tiposPrimitivos)tipo1 == (tiposPrimitivos)tipo2)
                    return true;

            }else if (tipo1 is string && tipo2 is string)
            {
                if ((string)tipo1 == (string)tipo2)
                    return true;
            }

            return false;
        }

        private bool validartiposStruct(Struct vaiableFind, Struct valorSint)
        {
            return vaiableFind.idStruct == valorSint.idStruct;
        }

        
    }

}
