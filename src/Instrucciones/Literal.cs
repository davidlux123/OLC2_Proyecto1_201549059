using _OLC2_Proyecto1.src.Ambientes;
using _OLC2_Proyecto1.src.Expresiones;
using _OLC2_Proyecto1.src.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace _OLC2_Proyecto1.src.Instrucciones
{
    public class Literal : Instruccion
    {
        public int line { get ; set ; }
        public int column { get ; set; }

        private string idLiteral;
        private object type;

        public Literal(int line, int column, string idLiteral, object type)
        {
            this.line = line;
            this.column = column;
            this.idLiteral = idLiteral;
            this.type = type;
        }

        public retorno execute(Entorno ent, ProgramClass programClass)
        {
            if (!programClass.existeIDtypes(this.idLiteral))
            {
                if (this.type is string)
                {
                    object valorType = programClass.getValorType((string)this.type);
                    if (valorType != null)
                        programClass.addType(this.idLiteral, valorType);
                    else
                        throw new Exception("<tr>\n" +
                    "\t<td>Error Semantico</td>\n" +
                    "\t<td>No se ah declarado ningun Type con el id " + "'" + this.type + "'" + "</td>\n" +
                    "\t<td>" + this.line + "</td>\n" +
                    "\t<td>" + this.column + "</td>\n</tr>\n\n");


                }
                else
                {
                    programClass.addType(this.idLiteral, this.type);
                }
            }
            else
            {
                throw new Exception("<tr>\n" +
                "\t<td>Error Semantico</td>\n" +
                "\t<td>El id: '" + this.idLiteral + "' ya esta en uso</td>\n" +
                "\t<td>" + this.line + "</td>\n" +
                "\t<td>" + this.column + "</td>\n</tr>\n\n");
            }

            retorno ret;
            ret.value = null;
            ret.type = tiposPrimitivos.VOID;
            return ret;
        }
    }
}
