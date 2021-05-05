using _OLC2_Proyecto1.src.Ambientes;
using _OLC2_Proyecto1.src.Expresiones;
using _OLC2_Proyecto1.src.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace _OLC2_Proyecto1.src.Instrucciones.InstLocales
{
    class IF : Instruccion
    {
        public int line { get ; set; }
        public int column { get ; set ; }

        private Expresion expCondicion;
        private Instruccion bloqueIf;
        private List<IF> listaElseifs;
        private Instruccion bloqueElse;

        public IF(int line, int column, Expresion expCondicion, Instruccion bloqueIf, List<IF> listaElseifs, Instruccion bloqueElse)
        {
            this.line = line;
            this.column = column;
            this.expCondicion = expCondicion;
            this.bloqueIf = bloqueIf;
            this.listaElseifs = listaElseifs;
            this.bloqueElse = bloqueElse;
        }

        public IF(int line, int column, Expresion expCondicion, Instruccion bloqueIf)
        {
            this.line = line;
            this.column = column;
            this.expCondicion = expCondicion;
            this.bloqueIf = bloqueIf;
            this.listaElseifs = new List<IF>();//se inicializan vacias
            this.bloqueElse = null;//bloque vacio
        }


        public retorno execute(Entorno ent, ProgramClass programClass)
        {
            retorno condi = expCondicion.getValorSintetizado(ent, programClass);
            if (condi.type != tiposPrimitivos.BOOLEAN)
                throw new Exception("<tr>\n" +
                "\t<td>Error Semantico</td>\n" +
                "\t<td>La expresion de la condicion es '"+condi.type+"' y debe ser de tipo BOOLEAN</td>\n" +
                "\t<td>" + this.line + "</td>\n" +
                "\t<td>" + this.column + "</td>\n</tr>\n\n");

            
            if ((bool)condi.value)
            {
                retorno valorRetorno = this.bloqueIf.execute(ent, programClass);
                if (valorRetorno.value != null && valorRetorno.type != tiposPrimitivos.VOID)
                    return valorRetorno;
            }
            else
            {
                foreach (IF If in this.listaElseifs)
                {
                    condi = If.expCondicion.getValorSintetizado(ent, programClass);
                    if (condi.type != tiposPrimitivos.BOOLEAN)
                        throw new Exception("<tr>\n" +
                        "\t<td>Error Semantico</td>\n" +
                        "\t<td>La expresion de la condicion es '" + condi.type + "' y debe ser de tipo BOOLEAN</td>\n" +
                        "\t<td>" + this.line + "</td>\n" +
                        "\t<td>" + this.column + "</td>\n</tr>\n\n");

                    if ((bool)condi.value)
                    {
                        retorno valorRetorno = If.bloqueIf.execute(ent, programClass);
                        if (valorRetorno.value != null && valorRetorno.type != tiposPrimitivos.VOID)
                            return valorRetorno;

                        return valorRetorno;// este return es muy importante ya que sin entra en algun else if no pase hacer el else 
                    }
                }

                if (bloqueElse != null)
                {
                    retorno valorRet = this.bloqueElse.execute(ent, programClass);
                    if (valorRet.value != null && valorRet.type != tiposPrimitivos.VOID)
                        return valorRet;
                }
            }

            retorno ret;
            ret.value = null;
            ret.type = tiposPrimitivos.VOID;
            return ret;
        }
    }
}
