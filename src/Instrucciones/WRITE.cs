using _OLC2_Proyecto1.src.Interfaces;
using _OLC2_Proyecto1.src.Expresiones;
using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;

namespace _OLC2_Proyecto1.src.Instrucciones
{
    class WRITE : Instruccion
    {
        public int line { get; set; }
        public int column { get; set; }
        private Expresion expresion { get; set; }

        private bool writeln;

        public WRITE(int line, int column, Expresion expresion, bool writeln)
        {
            this.line = line;
            this.column = column;
            this.expresion = expresion;
            this.writeln = writeln;
        }

        public void execute()
        {
            retorno valorExp = expresion.execute();

            if (valorExp.type != tiposPrimitivos.error && valorExp.value != null)
            {
                if (writeln)
                   Form1.ConsoleRichText.AppendText(valorExp.value.ToString() + "\n");
                else
                    Form1.ConsoleRichText.AppendText(valorExp.value.ToString());
            }
            else
                Form1.ConsoleRichText.AppendText("Error semantico: no se puede imprimir un valor 'null'");

        }
    }
}
