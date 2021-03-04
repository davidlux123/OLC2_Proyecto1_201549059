using _OLC2_Proyecto1.src.Ambientes;
using _OLC2_Proyecto1.src.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace _OLC2_Proyecto1.src.Instrucciones
{
    class GraficarTS : Instruccion
    {
        public int line { get; set; }
        public int column { get ; set; }

        public GraficarTS(int line, int column)
        {
            this.line = line;
            this.column = column;
        }

        public void execute(Entorno ent, ProgramClass programClass)
        {
            string simbolosHTML = "<title>Tabla de Simbolos</title>\n\n" +

             "<style type = \"text / css\">\n\n" +

             "\ttable, th, td {\n" +
             "\t\tborder: 1px solid black;\n" +
             "\t\tborder-collapse: collapse;\n" +
             "\t}\n\n" +
             "\tth, td{\n" +
             "\t\tpadding: 10px;\n" +
             "\t}\n\n" +

             "</style>\n\n" +

             "<h1  style=\"text-align: center;\">Listado de Variables</h1>\n\n" +

             "<table border = 1.5 width = 100%>\n" +
             "<head>\n" +
             "\t<tr bgcolor = blue >\n" +
             "\t\t<th>Nombre</th>\n" +
             "\t\t<th>Tipo</th>\n" +
             "\t\t<th>Ambito</th>\n" +
             "\t\t<th>Linea</th>\n" +
             "\t\t<th>Columna</th>\n" +
             "\t</tr>\n" +
             "</head>\n\n";

            simbolosHTML += ent.getAllVariables();

            //CREAMOS EL ARCHIVO DOT Y ESCRIBIMOS EN EL EL CODIGO GENERADO
            File.WriteAllText("C:\\compiladores2\\TablaSimbolos.HTML", simbolosHTML);

            if (File.Exists("C:\\compiladores2\\ListaErrores.HTML"))
            {
                string comandoDot = "C:\\compiladores2\\TablaSimbolos.HTML";
                var comando = string.Format(comandoDot);
                var procStart = new System.Diagnostics.ProcessStartInfo("cmd", "/C" + comando);
                var proc = new System.Diagnostics.Process();
                proc.StartInfo = procStart;
                proc.Start();
                proc.WaitForExit();
            }
            else
                MessageBox.Show("ERROR: el archivo C:\\compiladores2\\TablaSimbolos.HTM no existe :(");

        }
    }
}
