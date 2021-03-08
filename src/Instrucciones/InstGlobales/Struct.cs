using _OLC2_Proyecto1.src.Ambientes;
using _OLC2_Proyecto1.src.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace _OLC2_Proyecto1.src.Instrucciones.InstGlobales
{
    public class Struct : Instruccion, ICloneable
    {
        public int line { get ; set ; }
        public int column { get ; set ; }
        public string idStruct { get; }
        public Entorno entStruct { get; set; }
        private List<Declaracion> variables;



        public Struct(int line, int column, string idStruct, List<Declaracion> declaraciones)
        {
            this.line = line;
            this.column = column;
            this.idStruct = idStruct;
            this.entStruct = new Entorno(null, idStruct);
            this.variables = declaraciones;
        }

        public void execute(Entorno ent, ProgramClass programClass)
        {
            if (programClass.existeIDtypes(this.idStruct))
            {
                programClass.addType(this.idStruct, this);
            }
            else 
            {
                throw new Exception("<tr>\n" +
                "\t<td>Error Semantico</td>\n" +
                "\t<td>El id: '" + this.idStruct + "' ya esta en uso</td>\n" +
                "\t<td>" + this.line + "</td>\n" +
                "\t<td>" + this.column + "</td>\n</tr>\n\n");
            }

        }

        public void declararVariables(ProgramClass programClass)
        {
            if (this.entStruct == null)
                this.entStruct = new Entorno(null,this.idStruct);

            foreach (Declaracion declaracion in this.variables)
            {
                declaracion.execute(this.entStruct, programClass);
            }

        }

        public object Clone()
        {
            Struct clone = new Struct(line, column, idStruct, variables);
            return clone;
        }
    }
}
