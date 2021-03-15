using _OLC2_Proyecto1.src.Expresiones;
using System;
using System.Collections.Generic;
using System.Text;

namespace _OLC2_Proyecto1.src.Ambientes
{
    public class Simbolo
    {
        public string idVariable { get; set; }
        public object valor { get; set; }
        public tiposPrimitivos type { get; set; }
        public int linea { get; set; }
        public int columna { get; set; }
        public bool constante { get; set; }
        public bool temporal { get; set; }

        public Simbolo(string idVariable ,object valor, tiposPrimitivos type, int linea, int columna, bool constante, bool temporal)
        {
            this.idVariable = idVariable;
            this.valor = valor;
            this.type = type;
            this.linea = linea;
            this.columna = columna;
            this.constante = constante;
            this.temporal = temporal;
        }

        public Simbolo(string idVariable, object valor, tiposPrimitivos type, int linea, int columna, bool constante)
        {
            this.idVariable = idVariable;
            this.valor = valor;
            this.type = type;
            this.linea = linea;
            this.columna = columna;
            this.constante = constante;
            this.temporal = false;
        }
    }
}
