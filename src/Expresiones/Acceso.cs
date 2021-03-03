using _OLC2_Proyecto1.src.Ambientes;
using _OLC2_Proyecto1.src.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace _OLC2_Proyecto1.src.Expresiones
{
    class Acceso : Expresion
    {
        public int line { get ; set; }
        public int column { get; set; }

        private string id;

        public Acceso(int line, int column, string id)
        {
            this.line = line;
            this.column = column;
            this.id = id;
        }

        public retorno execute(Entorno ent, ProgramClass programClass)
        {
            retorno resultado;
            Simbolo variable = ent.getVariable(this.id);

            if (variable != null)
            {
                resultado.value = variable.valor;
                resultado.type = variable.type;
                return resultado;
            }
            else
            {
                throw new Exception("<tr>\n" +
                "\t<td>Error Semantico</td>\n" +
                "\t<td>El id:'" + this.id + "' no existe</td>\n" +
                "\t<td>" + this.line + "</td>\n" +
                "\t<td>" + this.column + "</td>\n</tr>");
            }
        }
    }
}
