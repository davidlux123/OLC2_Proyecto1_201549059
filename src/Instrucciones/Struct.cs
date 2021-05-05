using _OLC2_Proyecto1.src.Ambientes;
using _OLC2_Proyecto1.src.Expresiones;
using _OLC2_Proyecto1.src.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace _OLC2_Proyecto1.src.Instrucciones
{
    public class Struct : Instruccion, ICloneable
    {
        public int line { get; set; }
        public int column { get; set; }
        public string idStruct { get; set; }
        public Entorno entStruct { get; }
        
        private List<Declaracion> variables;

        public Struct(int line, int column, List<Declaracion> variables)
        {
            this.line = line;
            this.column = column;
            this.entStruct = new Entorno(null);
            this.variables = variables;

        }

        private Struct(int line, int column, string idStruct)
        {
            this.line = line;
            this.column = column;
            this.idStruct = idStruct;
            this.entStruct = entStruct;
        }

        public void declararVars(ProgramClass programClass)
        {
            //se inicializan las variables del struct
            foreach (Declaracion declaracion in this.variables)
            {
                declaracion.execute(this.entStruct, programClass);
            }
        }

        public retorno execute(Entorno ent, ProgramClass programClass)
        {
            if (!programClass.existeIDtypes(this.idStruct))
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

            retorno ret;
            ret.value = null;
            ret.type = tiposPrimitivos.VOID;
            return ret;
        }

        public object Clone()
        {
            Struct clone = new Struct(this.line, this.column,  this.idStruct);
            return clone;
        }

    }
}
