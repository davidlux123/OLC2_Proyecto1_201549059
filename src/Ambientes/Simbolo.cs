using _OLC2_Proyecto1.src.Expresiones;
using System;
using System.Collections.Generic;
using System.Text;

namespace _OLC2_Proyecto1.src.Ambientes
{
    public class Simbolo
    {
        public object valor { get; set; }
        public string id { get; set; }
        public tiposPrimitivos type { get; set; }
        public int linea { get; set; }
        public int columna { get; set; }
        public bool constante { get; set; }

        public Simbolo(object valor, string id, tiposPrimitivos type, int linea, int columna, bool constante)
        {
            this.valor = valor;
            this.id = id;
            this.type = type;
            this.linea = linea;
            this.columna = columna;
            this.constante = constante;
        }
    }
}
