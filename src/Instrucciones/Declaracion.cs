using _OLC2_Proyecto1.src.Ambientes;
using _OLC2_Proyecto1.src.Interfaces;
using _OLC2_Proyecto1.src.Expresiones;
using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using _OLC2_Proyecto1.src.Instrucciones.InstGlobales;

namespace _OLC2_Proyecto1.src.Instrucciones
{
    public class Declaracion : Instruccion
    {
        public int line { get; set ; }
        public int column { get; set; }

        private List<string> ids;
        private string tipo;
        private Expresion valor;
        private bool constante;

        public Declaracion(int line, int column, List<string> ids, string tipo, Expresion valor, bool constante)
        {
            this.line = line;
            this.column = column;
            this.ids = ids;
            this.tipo = tipo;
            this.valor = valor;
            this.constante = constante;
        }

        public Declaracion(int line, int column, List<string> ids, string tipo)
        {
            this.line = line;
            this.column = column;
            this.ids = ids;
            this.tipo = tipo;
            this.valor = null;
            this.constante = false;
        }


        public void execute(Entorno ent, ProgramClass programClass)
        {   
            if (this.ids.Count > 1)//cuando hay un listado de ids
            {
                string errores = "";
                foreach (string id in this.ids)
                {
                    bool noExiste;
                    if (ent.nombreAmbito == "GLOBAL")
                        noExiste = !ent.existeID(id) && !programClass.existeIDProcFunc(id);
                    else
                        noExiste = !ent.existeID(id);

                    if (noExiste)
                        ent.addVariable(getValorxDefecto(programClass), id, getTipo(programClass), this.line, this.column, false);
                    else
                        errores += "<tr>\n" +
                        "\t<td>Error Semantico</td>\n" +
                        "\t<td>El id: '" + id + "' ya esta en uso</td>\n" +
                        "\t<td>" + this.line + "</td>\n" +
                        "\t<td>" + this.column + "</td>\n</tr>\n|\n";
                }

                if (errores != "")
                    throw new Exception(errores);

            }
            else if (this.ids.Count == 1)// cuando hay solo un id 
            {
                bool noExiste;
                if (ent.nombreAmbito == "GLOBAL")
                    noExiste = !ent.existeID(this.ids[0]) && !programClass.existeIDProcFunc(this.ids[0]);
                else
                    noExiste = !ent.existeID(this.ids[0]);

                if (noExiste)
                       
                    if (valor != null)
                    {
                        retorno resultado = valor.execute(ent, programClass);
                        if (this.tipo != "")//varifica que sino es una constante
                            if (getTipo(programClass) != resultado.type)//sino es una constante verifica que el tipo del resultado sea igual al de la varaiable
                                throw new Exception("<tr>\n" +
                                "\t<td>Error Semantico</td>\n" +
                                "\t<td>Tipo de datos incopatible, no se puede asignar una expresion de tipo '" + resultado.type + "' a una variable de tipo '" + this.tipo + "'</td>\n" +
                                "\t<td>" + this.line + "</td>\n" +
                                "\t<td>" + this.column + "</td>\n</tr>\n\n");

                        ent.addVariable(resultado.value, this.ids[0], resultado.type, this.line, this.column, this.constante);

                    }
                    else
                        ent.addVariable(getValorxDefecto(programClass), this.ids[0], getTipo(programClass), this.line, this.column, false);
                else
                    throw new Exception("<tr>\n" +
                    "\t<td>Error Semantico</td>\n" +
                    "\t<td>El id: '" + this.ids[0] + "' ya esta en uso</td>\n" +
                    "\t<td>" + this.line + "</td>\n" +
                    "\t<td>" + this.column + "</td>\n</tr>\n\n");
                

            }

        }

        private tiposPrimitivos getTipo(ProgramClass programClass)
        {
            tipo = this.tipo.ToLower();

            if (tipo == "integer")
                return tiposPrimitivos.INT;
            else if (tipo == "real")
                return tiposPrimitivos.REAL;
            else if (tipo == "boolean")
                return tiposPrimitivos.BOOLEAN;
            else if (tipo == "string")
                return tiposPrimitivos.STRING;
            else
            {
                tiposPrimitivos type = programClass.getType(this.tipo);
                if (type != tiposPrimitivos.error)
                    return type;
                else
                    throw new Exception("<tr>\n" +
                    "\t<td>Error Semantico</td>\n" +
                    "\t<td>No se ah declarado ningun Type con el id " + "'" + this.tipo + "'" + "</td>\n" +
                    "\t<td>" + this.line + "</td>\n" +
                    "\t<td>" + this.column + "</td>\n</tr>\n\n");

            }
                
        }

        private object getValorxDefecto (ProgramClass programClass)
        {
            tipo = this.tipo.ToLower();

            if (tipo == "integer")
                return int.Parse("0");
            else if (tipo == "real")
                return float.Parse("0.0", CultureInfo.GetCultureInfo("en-US"));
            else if (tipo == "string")
                return "";
            else if (tipo == "boolean")
                return bool.Parse("false");
            else
            {
                object valorType = programClass.getValorType(this.tipo);
                if (valorType != null)
                    return valorType;
                else
                    throw new Exception("<tr>\n" +
                    "\t<td>Error Semantico</td>\n" +
                    "\t<td>No se ah declarado ningun Type con el id " + "'" + this.tipo + "'" + "</td>\n" +
                    "\t<td>" + this.line + "</td>\n" +
                    "\t<td>" + this.column + "</td>\n</tr>\n\n");
            }
                
        }
    }
}
