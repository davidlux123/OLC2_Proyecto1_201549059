using _OLC2_Proyecto1.src.Expresiones;
using _OLC2_Proyecto1.src.Instrucciones;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace _OLC2_Proyecto1.src.Ambientes
{
    public class Parametro
    {
        public int line { get; set; }
        public int column { get; set; }

        private string id;
        private object tipo;
        private bool temporal;

        public Parametro(int line, int column, string id, object tipo, bool temporal)
        {
            this.line = line;
            this.column = column;
            this.id = id;
            this.tipo = tipo;
            this.temporal = temporal;
        }

        public Simbolo convertirSimbolo(ProgramClass programClass)
        {
            return new Simbolo(id, getValorxDefecto(programClass), getTipo(programClass), this.line, this.column, false, this.temporal);
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
            else
            {
                return (tiposPrimitivos)this.tipo;
            }

        }
        private object getValorxDefecto(ProgramClass programClass)
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
