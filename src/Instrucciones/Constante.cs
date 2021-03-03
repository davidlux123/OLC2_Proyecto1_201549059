using _OLC2_Proyecto1.src.Ambientes;
using _OLC2_Proyecto1.src.Expresiones;
using _OLC2_Proyecto1.src.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace _OLC2_Proyecto1.src.Instrucciones.InstGlobales
{
    class Constante : Instruccion
    {
        public int line { get ; set ; }
        public int column { get; set; }

        private string id;
        private Expresion valor;

        public Constante(int line, int column, string id, Expresion valor)
        {
            this.line = line;
            this.column = column;
            this.id = id;
            this.valor = valor;
        }

        public void execute(Entorno ent, ProgramClass programClass)
        {
            if (!ent.existeID(this.id) && !programClass.existeID(this.id))
            {
                retorno resultado = valor.execute(ent, programClass);
                ent.addVariable(resultado.value, this.id, resultado.type, this.line, this.column, true);
            }
            else
            {
                throw new Exception("<tr>\n" +
                "\t<td>Error Semantico</td>\n" +
                "\t<td>El id:'" + this.id + "' ya esta en uso</td>\n" +
                "\t<td>" + this.line + "</td>\n" +
                "\t<td>" + this.column + "</td>\n</tr>");
            }

        }
    }
}
