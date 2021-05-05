using _OLC2_Proyecto1.src.Ambientes;
using _OLC2_Proyecto1.src.Instrucciones;
using _OLC2_Proyecto1.src.Instrucciones.InstLocales;
using _OLC2_Proyecto1.src.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace _OLC2_Proyecto1.src.Expresiones
{
    class LlamadaFunc : Expresion, Instruccion
    {
        public int line { get; set; }
        public int column { get; set; }

        private string id;
        private List<Expresion> expParams;

        public LlamadaFunc(int line, int column, string id, List<Expresion> expParams)
        {
            this.line = line;
            this.column = column;
            this.id = id;
            this.expParams = expParams;
        }

        public retorno getValorSintetizado(Entorno ent, ProgramClass programClass)
        {
            Funcion fn = programClass.getProcFunc(this.id);//LLAMO LA FUNCION
            if (fn != null)
            {
                if (fn.decParams.Count == this.expParams.Count) // VRIFICO EL TAMAÑO DE LOS VALORES LLAMADA DE FUNCION Y LAS DECLARACIONES SEAN ES EL MISMO
                {
                    List<retorno> valsParams = new List<retorno>();
                    foreach (Expresion expParam in this.expParams) //EJECUTAMOS LOS VALORES DE LOS DE LLAMADA DE FUNCION
                    {
                        if (expParam is Acceso acceso)
                            valsParams.Add(acceso.getReferenciaSintetizado(ent, programClass));
                        else
                            valsParams.Add(expParam.getValorSintetizado(ent, programClass));
                    }

                    Entorno nvoEntorno = new Entorno(ent, fn.idFuncion); //CREAMOS NVO ENTORNO

                    foreach (Instruccion decParam in fn.decParams)// DECLARAMOS LOS PARAMETROS EN EL NUEVO ENTORNO
                        decParam.execute(nvoEntorno, programClass);

                    for (int i = 0; i < fn.decParams.Count; i++)
                    {
                        Declaracion decParam = (Declaracion)fn.decParams[i];
                        Simbolo param = nvoEntorno.getVariable(decParam.idDec);//OBTENEMOS LA VARIABLE DECLARADA CON ANTERIORIDAD Y LA COMPARAMOS CON EL VALOR 
                        if (param.type == valsParams[i].type)
                            if (validarTiposDatos(param.valor, valsParams[i].value)) //validamos tipos
                                actualizarParametro(param, valsParams[i], nvoEntorno);//actualizamos variables
                            else
                                throw new Exception("<tr>\n" +
                                "\t<td>Error Semantico</td>\n" +
                                "\t<td>El Paramentro '" + param.type + "' no es compatible con el valor " + valsParams[i].type + " </td>\n" +
                                "\t<td>" + this.line + "</td>\n" +
                                "\t<td>" + this.column + "</td>\n</tr>\n\n");
                    }

                    retorno valorRetorno = fn.bloque.execute(nvoEntorno, programClass); //EJECUTAMOS EL BLOQUE DE ISNTRUCCIONES
                    if (validarRetornoFunc(valorRetorno, fn.tipo, programClass))// VALIDAMOS TIPOS DE DATOS, RETURN Y FUNC
                        return valorRetorno;
                    else
                        throw new Exception("<tr>\n" +
                        "\t<td>Error Semantico</td>\n" +
                        "\t<td>El tipo de retorno debe ser '" + fn.tipo + "' </td>\n" +
                        "\t<td>" + this.line + "</td>\n" +
                        "\t<td>" + this.column + "</td>\n</tr>\n\n");
                }
                else
                    throw new Exception("<tr>\n" +
                    "\t<td>Error Semantico</td>\n" +
                    "\t<td>los parametros de la funcion No coinciden funcion: '" + this.id + "'</td>\n" +
                    "\t<td>" + this.line + "</td>\n" +
                    "\t<td>" + this.column + "</td>\n</tr>\n\n");
            }
            else
                throw new Exception("<tr>\n" +
                "\t<td>Error Semantico</td>\n" +
                "\t<td>El id de la funcion: '" + this.id + "' no existe</td>\n" +
                "\t<td>" + this.line + "</td>\n" +
                "\t<td>" + this.column + "</td>\n</tr>\n\n");

        }
       
        public void actualizarParametro(Simbolo param,  retorno valparam, Entorno nvoEntorno)
        {
            if (valparam.value is Simbolo valSimbolo)
                if (param.isReference)
                {
                    nvoEntorno.addVariable(param.idVariable, valSimbolo);
                    return;
                }
            
            param.valor = valparam.value;
            nvoEntorno.addVariable(param.idVariable, param);
        }
        public bool validarTiposDatos(object tipo1, object valParam)
        {
            object tipo2;
            if (valParam is Simbolo valSimbolo)
                tipo2 = valSimbolo.valor;
            else
                tipo2 = valParam;

            if (tipo1 is Arreglo && tipo2 is Arreglo)
            {
                Arreglo arrAct = (Arreglo)tipo1;
                Arreglo arrNvo = (Arreglo)tipo2;

                if (validarTiposArreglo(arrAct, arrNvo))
                    return true;
                else
                    throw new Exception("<tr>\n" +
                    "\t<td>Error Semantico</td>\n" +
                    "\t<td>Tipo de Arreglo '" + arrAct.typeArrglo + "' no coincide con " + arrAct.typeArrglo +"</td>\n" +
                    "\t<td>" + this.line + "</td>\n" +
                    "\t<td>" + this.column + "</td>\n</tr>");
            }
            else if (tipo1 is Struct && tipo2 is Struct)
            {
                Struct struAct = (Struct)tipo1;
                Struct struNvo = (Struct)tipo2;

                if (validartiposStruct(struAct, struNvo))
                    return true;
                else
                    throw new Exception("<tr>\n" +
                    "\t<td>Error Semantico</td>\n" +
                    "\t<td>Tipo de Struct '"+struAct.idStruct+ "' no coincide con "+struNvo.idStruct+"</td>\n" +
                    "\t<td>" + this.line + "</td>\n" +
                    "\t<td>" + this.column + "</td>\n</tr>");
            }
            else
            {
                return true;
            }
                
        }
        private bool validarTiposArreglo(Arreglo arreglo, Arreglo arrAux)
        {
            if (arreglo.idArreglo == "" || arrAux.idArreglo == "")
            {
                if (arreglo.dimensiones.Count == arrAux.dimensiones.Count && validarTipos(arreglo.typeArrglo, arrAux.typeArrglo))
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

            }
            else if (tipo1 is string && tipo2 is string)
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
        private bool validarRetornoFunc(retorno valorRet, object tipo, ProgramClass programClass)
        {
            if (valorRet.type == getTipo(programClass, tipo))
            {
                if (valorRet.value != null)
                {
                    if (valorRet.value is Arreglo arr)
                    {
                        if (!validarTipos(arr.idArreglo, tipo))
                            return false;
                    }
                    else if (valorRet.value is Struct st)
                    {
                        if (!validarTipos(st.idStruct, tipo))
                            return false;
                    }
                }
                return true;
            }
            return false;

        }
        private tiposPrimitivos getTipo(ProgramClass programClass, object tipo)
        {
            if (tipo is string @string)
            {
                tiposPrimitivos type = programClass.getType(@string);
                if (type != tiposPrimitivos.error)
                    return type;
                else
                    throw new Exception("<tr>\n" +
                    "\t<td>Error Semantico</td>\n" +
                    "\t<td>No se ah declarado ningun Type con el id " + "'" + @string + "'" + "</td>\n" +
                    "\t<td>" + this.line + "</td>\n" +
                    "\t<td>" + this.column + "</td>\n</tr>\n\n");
            }
            else
            {
                return (tiposPrimitivos)tipo;
            }

        }
       
        public retorno execute(Entorno ent, ProgramClass programClass)
        {
            retorno ret = getValorSintetizado(ent, programClass);
            if (ret.value == null && ret.type == tiposPrimitivos.VOID)
            {
                return ret;
            }

            throw new Exception("<tr>\n" +
            "\t<td>Error Semantico</td>\n" +
            "\t<td>Un metodo void no puede Retornar un valor</td>\n" +
            "\t<td>" + this.line + "</td>\n" +
            "\t<td>" + this.column + "</td>\n</tr>\n\n");

        }



    }
}



