using _OLC2_Proyecto1.src.Ambientes;
using _OLC2_Proyecto1.src.Interfaces;
using _OLC2_Proyecto1.src.Expresiones;
using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;

namespace _OLC2_Proyecto1.src.Instrucciones.InstGlobales
{
    class Variable : Instruccion
    {
        public int line { get; set ; }
        public int column { get; set; }

        private List<string> ids;
        private tiposPrimitivos tipo;
        private Expresion valor;

        public Variable(int line, int column, List<string> ids, string tipo, Expresion valor)
        {
            this.line = line;
            this.column = column;
            this.ids = ids;
            this.valor = valor;
            this.tipo = getTipo(tipo);
        }

        public void execute(Entorno ent, ProgramClass programClass)
        {   
            if (this.ids.Count > 1)//cuando hay un listado de ids
            {
                string errores = "";
                foreach (string id in this.ids)
                {
                    if (!ent.existeID(id) && !programClass.existeID(id))
                    {
                        ent.addVariable(ValorxDefecto(this.tipo), id, this.tipo, this.line, this.column, false);
                    }
                    else
                    {
                        errores += "<tr>\n" +
                        "\t<td>Error Semantico</td>\n" +
                        "\t<td>El id:'" + id + "' ya esta en uso</td>\n" +
                        "\t<td>" + this.line + "</td>\n" +
                        "\t<td>" + this.column + "</td>\n</tr>\n|\n";
                    } 

                }

                if (errores != "")
                    throw new Exception(errores);

            }
            else if (this.ids.Count == 1)// cuando hay solo un id 
            {
                if (!ent.existeID(this.ids[0]) && !programClass.existeID(ids[0]))
                {
                    if (valor != null)
                    {
                        retorno resultado = valor.execute(ent, programClass);
                        if (this.tipo == resultado.type)
                        {
                            ent.addVariable(resultado.value, this.ids[0], resultado.type, this.line, this.column, false);
                        }
                        else
                        {
                            throw new Exception("<tr>\n" +
                            "\t<td>Error Semantico</td>\n" +
                            "\t<td>Tipo de datos incopatible, no se puede asignar una expresion de tipo '" + resultado.type + "' " + "</td>\n" +
                            "\t<td>" + this.line + "</td>\n" +
                            "\t<td>" + this.column + "</td>\n</tr>\n\n");
                        }
                    }
                    else
                    {
                        ent.addVariable(ValorxDefecto(tipo), this.ids[0], this.tipo, this.line, this.column, false);
                    }
                }
                else
                {
                    throw new Exception("<tr>\n" +
                    "\t<td>Error Semantico</td>\n" +
                    "\t<td>El id:'" + ids[0] + "' ya esta en uso</td>\n" +
                    "\t<td>" + this.line + "</td>\n" +
                    "\t<td>" + this.column + "</td>\n</tr>\n\n");
                }

            }

        }

        private tiposPrimitivos getTipo(string tipo)
        {
            tipo = tipo.ToLower();

            if (tipo == "integer")
            {
                return tiposPrimitivos.INT;
            }
            else if (tipo == "real")
            {
                return tiposPrimitivos.REAL;
            }
            else if (tipo == "string")
            {
                return tiposPrimitivos.STRING;
            }
            else
            {
                return tiposPrimitivos.BOOLEAN; 
            }
        }

        private object ValorxDefecto (tiposPrimitivos tipo)
        {
            if (tipo == tiposPrimitivos.INT)
                return int.Parse("0");
            else if (tipo == tiposPrimitivos.REAL)
                return float.Parse("0.0", CultureInfo.GetCultureInfo("en-US"));
            else if (tipo == tiposPrimitivos.STRING)
                return "";
            else
                return bool.Parse("false");
        }
    }
}
