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
            if (fn == null)
                throw new Exception("<tr>\n" +
                "\t<td>Error Semantico</td>\n" +
                "\t<td>El id de la funcion: '" + this.id + "' no existe</td>\n" +
                "\t<td>" + this.line + "</td>\n" +
                "\t<td>" + this.column + "</td>\n</tr>\n\n");

            if (fn.parametros.Count != this.expParams.Count) // VRIFICO SI EL TAMAÑO DELOS PARAMETROS CON LA DECLARACIONES ES EL MISMO
            {
                throw new Exception("<tr>\n" +
                "\t<td>Error Semantico</td>\n" +
                "\t<td>los parametros de la funcion No coinciden funcion: '" + this.id + "'</td>\n" +
                "\t<td>" + this.line + "</td>\n" +
                "\t<td>" + this.column + "</td>\n</tr>\n\n");
            }

            List<retorno> valsParams = new List<retorno>();
            foreach (Expresion expParam in this.expParams) //EJECUTAMOS LOS VALORES DE LOS PARAMETROS
            {
                if (expParam is Acceso acceso)
                    valsParams.Add(acceso.getReferenciaSintetizado(ent, programClass));
                else 
                    valsParams.Add(expParam.getValorSintetizado(ent, programClass));
            }
            
            Entorno nvoEntorno = new Entorno(ent, fn.idFuncion); // NVO ENTORNO

            for (int i = 0; i < fn.parametros.Count; i++)//VALIDACION Y DECLARACION DE PARAMETROS DE LA FUNCION CON SUS RESPECTIVOS VALORRES
            {
                if (fn.parametros[i].type == valsParams[i].type)
                    if (validarTiposDatos(fn.parametros[i].valor, valsParams[i].value))
                        declararParametro(fn.parametros[i], valsParams[i], nvoEntorno);
                else
                    throw new Exception("<tr>\n" +
                    "\t<td>Error Semantico</td>\n" +
                    "\t<td>El Paramentro '" + fn.parametros[i].type + "' no es compatible con el valor " + valsParams[i].type + " </td>\n" +
                    "\t<td>" + this.line + "</td>\n" +
                    "\t<td>" + this.column + "</td>\n</tr>\n\n");

            }

            foreach(Instruccion instFunc in fn.instruccionesFunc) //EJECTO SUS FUNCIONES
            {
                retorno valorRetorno = instFunc.execute(nvoEntorno, programClass);
                if (instFunc is Exit)
                {
                    if (validarRetornoFunc(valorRetorno, fn.tipo, programClass))
                        return valorRetorno;
                    else
                        throw new Exception("<tr>\n" +
                        "\t<td>Error Semantico</td>\n" +
                        "\t<td>El tipo de retorno debe ser '" + fn.tipo + "' </td>\n" +
                        "\t<td>" + this.line + "</td>\n" +
                        "\t<td>" + this.column + "</td>\n</tr>\n\n");
                }
            }
            retorno ret;
            ret.value = null;
            ret.type = tiposPrimitivos.VOID;
            return ret;

        }
       
        public void declararParametro(Simbolo param,  retorno valparam, Entorno nvoEntorno)
        {
            if (valparam.value is Simbolo valSimbolo)
            {
                if (param.temporal)
                {
                    nvoEntorno.addVariable(valSimbolo.idVariable, valSimbolo);
                }
                else
                {
                    param.valor = valSimbolo.valor;
                    nvoEntorno.addVariable(param.idVariable, param);
                }
            }
            else
            {
                param.valor = valparam.value;
                nvoEntorno.addVariable(param.idVariable, param);
            }
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



