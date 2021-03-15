using _OLC2_Proyecto1.src.Ambientes;
using _OLC2_Proyecto1.src.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace _OLC2_Proyecto1.src.Expresiones
{
    public enum tiposPrimitivos
    {
        STRING,
        INT,
        REAL,
        BOOLEAN,
        STRUCT,
        ARRAEGLO,
        VOID,
        error
    }

    public class Primitivo : Expresion
    {
        public int line { get; set; }
        public int column { get; set; }
        public object valor { get; set; }
        public tiposPrimitivos tipo { get; set; }

        public Primitivo(int line, int column, object value, tiposPrimitivos tipo)
        {
            this.line = line;
            this.column = column;
            this.valor = value;
            this.tipo = tipo;
        }

        public retorno getValorSintetizado(Entorno ent, ProgramClass programClass)
        {
            retorno resultado;

            if (this.tipo == tiposPrimitivos.INT)
            {
                resultado.type = tiposPrimitivos.INT;
                resultado.value = this.valor;

            }else if (this.tipo == tiposPrimitivos.REAL)
            {
                resultado.type = tiposPrimitivos.REAL;
                resultado.value = this.valor;

            }
            else if (this.tipo == tiposPrimitivos.STRING)
            {
                resultado.type = tiposPrimitivos.STRING;
                resultado.value = this.valor;

            }
            else //this.tipo == tiposPrimitivos.BOOLEAN
            {
                resultado.type = tiposPrimitivos.BOOLEAN;
                resultado.value = this.valor; 
            }

            return resultado;
        }
    }
}
