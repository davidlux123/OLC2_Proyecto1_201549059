using _OLC2_Proyecto1.src.Ambientes;
using _OLC2_Proyecto1.src.Expresiones;
using _OLC2_Proyecto1.src.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace _OLC2_Proyecto1.src.Instrucciones
{
    public class Funcion : Instruccion
    {
        public int line { get ; set; }
        public int column { get ; set ; }
        public string idFuncion { get; set; }
        public object tipo { get; set; }
        public List<Instruccion> decParams { get; set; }
        public Instruccion bloque { get; set; }

        public Funcion(int line, int column, string idFuncion, object tipo, List<Instruccion> decParams, Instruccion bloque)
        {
            this.line = line;
            this.column = column;
            this.idFuncion = idFuncion;
            this.tipo = tipo;
            this.decParams = decParams;
            this.bloque = bloque;
        }

        public retorno execute(Entorno ent, ProgramClass programClass)
        {
            if (!programClass.existeIDProcFunc(this.idFuncion) && ent.existeIDGlobal(this.idFuncion))
            {
                programClass.addProcFunc(this.idFuncion, this);
            }
            else
            {
                throw new Exception("<tr>\n" +
                "\t<td>Error Semantico</td>\n" +
                "\t<td>El id: '" + this.idFuncion + "' ya esta en uso</td>\n" +
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
