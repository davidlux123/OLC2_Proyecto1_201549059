using _OLC2_Proyecto1.src.Interfaces;
using _OLC2_Proyecto1.src.Expresiones;
using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using _OLC2_Proyecto1.src.Ambientes;

namespace _OLC2_Proyecto1.src.Instrucciones.InstLocales
{
    class Write : Instruccion
    {
        public int line { get; set; }
        public int column { get; set; }
        private Expresion expresion { get; set; }

        private bool writeln;

        public Write(int line, int column, Expresion expresion, bool writeln)
        {
            this.line = line;
            this.column = column;
            this.expresion = expresion;
            this.writeln = writeln;
        }

        public void execute(Entorno ent, ProgramClass ast)
        {
            retorno valorExp = expresion.execute(ent, ast);

            if (this.writeln)
                Form1.ConsoleRichText.AppendText(Convert.ToString(valorExp.value, CultureInfo.CreateSpecificCulture("en-US")).Replace("'", "") + "\n");
            else
                Form1.ConsoleRichText.AppendText(Convert.ToString(valorExp.value, CultureInfo.CreateSpecificCulture("en-US")).Replace("'", ""));
            

        }
    }
}
