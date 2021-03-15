using _OLC2_Proyecto1.src.Ambientes;
using _OLC2_Proyecto1.src.Interfaces;
using _OLC2_Proyecto1.src.Expresiones;
using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;

namespace _OLC2_Proyecto1.src.Instrucciones
{
    public class Declaracion : Instruccion
    {
        public int line { get; set ; }
        public int column { get; set; }

        private string idDec;
        private object tipo;
        private Expresion valor;
        private bool constante;

        public Declaracion(int line, int column, string idDec, object tipo, Expresion valor, bool constante)
        {
            this.line = line;
            this.column = column;
            this.idDec = idDec;
            this.tipo = tipo;
            this.valor = valor;
            this.constante = constante;
        }

        public Declaracion(int line, int column, string idDec, object tipo)
        {
            this.line = line;
            this.column = column;
            this.idDec = idDec;
            this.tipo = tipo;
            this.valor = null;
            this.constante = false;
        }


        public retorno execute(Entorno ent, ProgramClass programClass)
        {
            bool noExiste;
            if (ent.nombreAmbito == "GLOBAL")
                noExiste = !ent.existeID(this.idDec) && !programClass.existeIDProcFunc(this.idDec);
            else
                noExiste = !ent.existeID(this.idDec);

            if (noExiste)
            {
                if (valor != null)// de que no contenga valor
                {
                    retorno resultado = valor.getValorSintetizado(ent, programClass);
                    if (this.tipo.ToString() != "")//varifica que sino es una constante
                        if (getTipo(programClass) != resultado.type)//sino es una constante verifica que el los tipos sean iguales
                            throw new Exception("<tr>\n" +
                            "\t<td>Error Semantico</td>\n" +
                            "\t<td>Tipo de datos incopatible, no se puede asignar una expresion de tipo '" + resultado.type + "' a una variable de tipo '" + this.tipo + "'</td>\n" +
                            "\t<td>" + this.line + "</td>\n" +
                            "\t<td>" + this.column + "</td>\n</tr>\n\n");
                        
                    ent.addVariable(resultado.value, this.idDec, resultado.type, this.line, this.column, this.constante);// agrega la constamte
                }
                else
                    ent.addVariable(getValorxDefecto(ent, programClass), this.idDec, getTipo(programClass), this.line, this.column, false);// agrega la y las variables sin valor
            }
            else
                throw new Exception("<tr>\n" +
                "\t<td>Error Semantico</td>\n" +
                "\t<td>El id: '" + this.idDec + "' ya esta en uso</td>\n" +
                "\t<td>" + this.line + "</td>\n" +
                "\t<td>" + this.column + "</td>\n</tr>\n\n");


            retorno ret;
            ret.value = null;
            ret.type = tiposPrimitivos.VOID;
            return ret;
        }
        private tiposPrimitivos getTipo(ProgramClass programClass)
        {
            if (this.tipo is string)
            {
                tiposPrimitivos type = programClass.getType((string)this.tipo);
                if (type != tiposPrimitivos.error)
                    return type;
                else
                    throw new Exception("<tr>\n" +
                    "\t<td>Error Semantico</td>\n" +
                    "\t<td>No se ah declarado ningun Type con el id " + "'" + this.tipo + "'" + "</td>\n" +
                    "\t<td>" + this.line + "</td>\n" +
                    "\t<td>" + this.column + "</td>\n</tr>\n\n");
            }
            else if (this.tipo is Arreglo)
            {
                return tiposPrimitivos.ARRAEGLO;
            }
            else
            {
                return (tiposPrimitivos)this.tipo;
            }
            
        }
        private object getValorxDefecto (Entorno ent, ProgramClass programClass)
        {
            if (this.tipo is string)
            {
                object clonValor = programClass.getValorType((string)this.tipo);
                if (clonValor != null)
                    if (clonValor is Struct || clonValor is Arreglo)
                        return clonValor;
                    else
                        return getValorPrimitivo((tiposPrimitivos)clonValor);
                else
                    throw new Exception("<tr>\n" +
                    "\t<td>Error Semantico</td>\n" +
                    "\t<td>No se ah declarado ningun Type con el id " + "'" + this.tipo + "'" + "</td>\n" +
                    "\t<td>" + this.line + "</td>\n" +
                    "\t<td>" + this.column + "</td>\n</tr>\n\n");
            }
            else if (this.tipo is Arreglo)
            {
                Arreglo nvoArreglo = (Arreglo)this.tipo;
                nvoArreglo.execute(ent, programClass);
                return nvoArreglo;
            }
            else
            {
                return getValorPrimitivo((tiposPrimitivos)this.tipo);
            }
                
        }
        private object getValorPrimitivo(tiposPrimitivos type)
        {
            if (type == tiposPrimitivos.INT)
                return int.Parse("0");
            else if (type == tiposPrimitivos.REAL)
                return float.Parse("0.0", CultureInfo.GetCultureInfo("en-US"));
            else if (type == tiposPrimitivos.BOOLEAN)
                return bool.Parse("false");
            else //(type == tiposPrimitivos.STRING)
                return "";
        }
    }
}
