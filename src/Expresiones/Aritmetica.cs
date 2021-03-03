using _OLC2_Proyecto1.src.Ambientes;
using _OLC2_Proyecto1.src.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace _OLC2_Proyecto1.src.Expresiones
{
    public enum opcionAritmetica
    {
        SUMAR,    
        RESTAR,
        MULTIPLICAR,    
        DIVIDIR,    
        RESTO 
    }

    public class Aritmetica : Expresion
    {
        public int line { get ; set ; }
        public int column { get ; set ; }

        private Expresion hijoIzq;
        private Expresion hijoDer;
        private opcionAritmetica type;
        private string simbolo;

        public Aritmetica(int line, int column, Expresion hijoIzq, Expresion hijoDer, opcionAritmetica type, string simbolo)
        {
            this.line = line;
            this.column = column;
            this.hijoIzq = hijoIzq;
            this.hijoDer = hijoDer;
            this.type = type;
            this.simbolo = simbolo;
        }

        public retorno execute(Entorno ent, ProgramClass programClass)
        {
            retorno resultado;
            retorno valorIzq = this.hijoIzq.execute(ent, programClass);
            retorno valorDer = this.hijoDer.execute(ent, programClass);

            if ((valorIzq.type == tiposPrimitivos.STRING || valorDer.type == tiposPrimitivos.STRING) && this.type == opcionAritmetica.SUMAR)
            {
                resultado.type = tiposPrimitivos.STRING;
                resultado.value = Convert.ToString(valorIzq.value, CultureInfo.CreateSpecificCulture("en-US")) + Convert.ToString(valorDer.value, CultureInfo.CreateSpecificCulture("en-US"));
            }
            else if (valorIzq.type == tiposPrimitivos.INT && valorDer.type == tiposPrimitivos.INT)
            {
                if (this.type == opcionAritmetica.SUMAR)
                {
                    resultado.type = tiposPrimitivos.INT;
                    resultado.value = (int)valorIzq.value + (int)valorDer.value;

                }
                else if (this.type == opcionAritmetica.RESTAR)
                {
                    resultado.type = tiposPrimitivos.INT;
                    resultado.value = (int)valorIzq.value - (int)valorDer.value;

                }
                else if (this.type == opcionAritmetica.MULTIPLICAR)
                {
                    resultado.type = tiposPrimitivos.INT;
                    resultado.value = (int)valorIzq.value * (int)valorDer.value;

                }
                else if (this.type == opcionAritmetica.DIVIDIR)
                {
                    int deno = (int)valorDer.value;

                    if (deno > 0)
                    {
                        resultado.type = tiposPrimitivos.INT;
                        resultado.value = (int)valorIzq.value / deno;
                    }
                    else
                    {
                        resultado.type = tiposPrimitivos.error;
                        resultado.value = null;
                        Form1.ConsoleRichText.AppendText("Error semantico: " + "en la fila: " + this.line + " y en la columna: " + this.column +
                        "Descrip: no se puede dividir un tipo menor que cero" + "\n");

                        throw new Exception("<tr>\n" +
                        "\t<td>Error Semantico</td>\n" +
                        "\t<td>No se puede operar la division, el demominador '"+ deno.ToString() +"' tiene que ser mayor a cero</td>\n" +
                        "\t<td>" + this.line + "</td>\n" +
                        "\t<td>" + this.column + "</td>\n</tr>");
                    }
                }
                else // para la opcion del Modulo '%'
                {
                    resultado.type = tiposPrimitivos.INT;
                    resultado.value = (int)valorIzq.value % (int)valorDer.value;
                }

            }
            else if ((valorIzq.type == tiposPrimitivos.INT && valorDer.type == tiposPrimitivos.REAL) ||
                (valorIzq.type == tiposPrimitivos.REAL && valorDer.type == tiposPrimitivos.INT) ||
                (valorIzq.type == tiposPrimitivos.REAL && valorDer.type == tiposPrimitivos.REAL))
            {
                if (valorIzq.type == tiposPrimitivos.INT)
                    valorIzq.value = float.Parse(valorIzq.value.ToString(), CultureInfo.GetCultureInfo("en-US"));
                else if (valorDer.type == tiposPrimitivos.INT)
                    valorDer.value = float.Parse(valorDer.value.ToString(), CultureInfo.GetCultureInfo("en-US"));

                if (this.type == opcionAritmetica.SUMAR)
                {
                    resultado.type = tiposPrimitivos.REAL;
                    resultado.value = (float)valorIzq.value + (float)valorDer.value;

                }
                else if (this.type == opcionAritmetica.RESTAR)
                {
                    resultado.type = tiposPrimitivos.REAL;
                    resultado.value = (float)valorIzq.value - (float)valorDer.value;

                }
                else if (this.type == opcionAritmetica.MULTIPLICAR)
                {
                    resultado.type = tiposPrimitivos.REAL;
                    resultado.value = (float)valorIzq.value * (float)valorDer.value;

                }
                else if (this.type == opcionAritmetica.DIVIDIR)
                {
                    double deno = (float)valorDer.value;

                    if (deno > 0)
                    {
                        resultado.type = tiposPrimitivos.REAL;
                        resultado.value = (float)valorIzq.value / deno;
                    }
                    else
                    {
                        resultado.type = tiposPrimitivos.error;
                        resultado.value = null;
                        Form1.ConsoleRichText.AppendText("Error semantico: " + "en la fila: " + this.line + " y en la columna: " + this.column +
                        " Descrip: no se puede dividir un tipo menor que cero" + "\n");

                        throw new Exception("<tr>\n" +
                        "\t<td>Error Semantico</td>\n" +
                        "\t<td>No se puede operar la division, el demominador '" + deno.ToString() + "' tiene que ser mayor a cero</td>\n" +
                        "\t<td>" + this.line + "</td>\n" +
                        "\t<td>" + this.column + "</td>\n</tr>");

                        
                    }
                }
                else // para la opcion del Modulo '%'
                {
                    resultado.type = tiposPrimitivos.error;
                    resultado.value = null;
                    Form1.ConsoleRichText.AppendText("Error semantico: " + "en la fila: " + this.line + " y en la columna: " + this.column +
                    " Descrip: no se puede operar el modulo un tipo " + valorIzq.type.ToString() + " con el tipo " + valorDer.type.ToString() + "\n");

                    throw new Exception("<tr>\n" +
                    "\t<td>Error Semantico</td>\n" +
                    "\t<td>No se puede operar el modulo un tipo " + valorIzq.type.ToString() + " con el tipo " + valorDer.type.ToString() + "</td>\n" +
                    "\t<td>" + this.line + "</td>\n" +
                    "\t<td>" + this.column + "</td>\n</tr>");
                }

            }
            else 
            {
                resultado.type = tiposPrimitivos.error;
                resultado.value = null;
                Form1.ConsoleRichText.AppendText("Error semantico: " + "en la fila: " + this.line + " y en la columna: " + this.column +
                " Descrip: no se puede operar '" + this.simbolo + "' de un tipo " + valorIzq.type.ToString() + " con tipo " + valorDer.type.ToString() + "\n");

                throw new Exception("<tr>\n" +
                "\t<td>Error Semantico</td>\n" +
                "\t<td>" + "No se puede operar '" + this.simbolo + "' de un tipo " + valorIzq.type.ToString() + " con tipo " + valorDer.type.ToString()+" </td>\n" +
                "\t<td>" + this.line + "</td>\n" +
                "\t<td>" + this.column + "</td>\n</tr>");

            }

            return resultado;

        }
    }

    public class AritmeticaUnaria: Expresion
    {
        public int line { get; set; }
        public int column { get; set; }
        public Expresion hijo { get; set; }

        public AritmeticaUnaria(int line, int column, Expresion hijo)
        {
            this.line = line;
            this.column = column;
            this.hijo = hijo;
        }

        public retorno execute(Entorno ent, ProgramClass programClass)
        {
            retorno resultado;
            retorno valorHijo = this.hijo.execute(ent, programClass);

            if (valorHijo.type == tiposPrimitivos.INT)
            {
                resultado.type = tiposPrimitivos.INT;
                resultado.value = (int)valorHijo.value * -1;
            }
            else if (valorHijo.type == tiposPrimitivos.REAL)
            {
                resultado.type = tiposPrimitivos.REAL;
                resultado.value = (float)valorHijo.value * -1;
            }
            else
            {
                resultado.type = tiposPrimitivos.error;
                resultado.value = null;
                Form1.ConsoleRichText.AppendText("Error semantico: " + "en la fila: " + this.line + " y en la columna: " + this.column +
                "Descrip: El tipo de la expresion '" + valorHijo.value.ToString() + "' debe ser Numerico " + "\n");

                throw new Exception("<tr>\n" +
                "\t<td>Error Semantico</td>\n" +
                "\t<td>" + "El tipo de la expresion '"+ valorHijo.value.ToString() +"' debe ser Numerico " + "</td>\n" +
                "\t<td>" + this.line + "</td>\n" +
                "\t<td>" + this.column + "</td>\n</tr>");

            }

            return resultado;
        }
    }



}
