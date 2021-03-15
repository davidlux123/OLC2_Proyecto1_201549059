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
        private List<Instruccion> instsIf;
        private List<IF> listaElseifs;
        private List<Instruccion> instsElse;

        public IF(int line, int column, Expresion expCondicion, List<Instruccion> instsIf, List<IF> listaElseifs, List<Instruccion> instsElse)
        {
            this.line = line;
            this.column = column;
            this.expCondicion = expCondicion;
            this.instsIf = instsIf;
            this.listaElseifs = listaElseifs;
            this.instsElse = instsElse;
        }

        public IF(int line, int column, Expresion expCondicion, List<Instruccion> instsIf)
        {
            this.line = line;
            this.column = column;
            this.expCondicion = expCondicion;
            this.instsIf = instsIf;
            this.listaElseifs = new List<IF>();
            this.instsElse = new List<Instruccion>();
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

            string errors = "";
            if ((bool)condi.value)
            {
                foreach (Instruccion inst in this.instsIf)
                {
                    try
                    {
                        inst.execute(ent, programClass);
                    }
                    catch (Exception error)
                    {
                        errors += error.Message + "\n|\n";
                    }
                }

                if (errors != "")
                    throw new Exception(errors);

                retorno ret;
                ret.value = null;
                ret.type = tiposPrimitivos.VOID;
                return ret;
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
                        errors = "";
                        foreach (Instruccion inst in If.instsIf)
                        {
                            try
                            {
                                inst.execute(ent, programClass);
                            }
                            catch (Exception error)
                            {
                                errors += error.Message + "\n|\n";
                            }
                        }

                        if (errors != "")
                            throw new Exception(errors);

                        retorno ret1;
                        ret1.value = null;
                        ret1.type = tiposPrimitivos.VOID;
                        return ret1;

                    }
                }

                errors = "";
                foreach (Instruccion inst in this.instsElse)
                {
                    try
                    {
                        inst.execute(ent, programClass);
                    }
                    catch (Exception error)
                    {
                        errors += error.Message + "\n|\n";
                    }
                }

                if (errors != "")
                    throw new Exception(errors);

                retorno ret;
                ret.value = null;
                ret.type = tiposPrimitivos.VOID;
                return ret;

            }
        }
    }
}
