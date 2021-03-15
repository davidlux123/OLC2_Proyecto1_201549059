using _OLC2_Proyecto1.src.Ambientes;
using _OLC2_Proyecto1.src.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace _OLC2_Proyecto1.src.Expresiones
{
    public enum opcionLogica
    {
        OR,
        AND,
    }

    public class Logica : Expresion
    {
        public int line { get; set ; }
        public int column { get ; set; }

        private Expresion hijoIzq;
        private Expresion hijoDer;
        private opcionLogica type;
        private string simbolo;

        public Logica(int line, int column, Expresion hijoIzq, Expresion hijoDer, opcionLogica type, string simbolo)
        {
            this.line = line;
            this.column = column;
            this.hijoIzq = hijoIzq;
            this.hijoDer = hijoDer;
            this.type = type;
            this.simbolo = simbolo;
        }

        public retorno getValorSintetizado(Entorno ent, ProgramClass programClass)
        {
            retorno resultado;
            retorno valorIzq = this.hijoIzq.getValorSintetizado(ent, programClass);
            retorno valorDer = this.hijoDer.getValorSintetizado(ent, programClass);

            if (valorIzq.type == tiposPrimitivos.BOOLEAN && valorDer.type == tiposPrimitivos.BOOLEAN)
            {
                if (this.type == opcionLogica.AND)
                {
                    if ((bool)valorIzq.value && (bool)valorDer.value)
                    {
                        resultado.type = tiposPrimitivos.BOOLEAN;
                        resultado.value = (bool)true;
                    }
                    else
                    {
                        resultado.type = tiposPrimitivos.BOOLEAN;
                        resultado.value = (bool)false;
                    }

                }
                else //sí, this.type == opcionLogica.OR
                {
                    if ((bool)valorIzq.value || (bool)valorDer.value)
                    {
                        resultado.type = tiposPrimitivos.BOOLEAN;
                        resultado.value = (bool)true;
                    }
                    else
                    {
                        resultado.type = tiposPrimitivos.BOOLEAN;
                        resultado.value = (bool)false;
                    }
                }

            }
            else
            {
                Form1.ConsoleRichText.AppendText("Error semantico: " + "en la fila: " + this.line + " y en la columna: " + this.column +
                " Descrip: no se puede operar '" + this.simbolo + "' de un tipo " + valorIzq.type.ToString() + " con tipo " + valorDer.type.ToString() + "\n");

                throw new Exception("<tr>\n" +
                "\t<td>Error Semantico</td>\n" +
                "\t<td>" + "No se puede operar '" + this.simbolo + "' de un tipo " + valorIzq.type.ToString() + " con tipo " + valorDer.type.ToString() + " </td>\n" +
                "\t<td>" + this.line + "</td>\n" +
                "\t<td>" + this.column + "</td>\n</tr>");

            }

            return resultado;
        }
    }

    public class LogicaUnaria : Expresion
    {
        public int line { get ; set; }
        public int column { get ; set ; }
        public Expresion hijo { get; set; }

        public LogicaUnaria(int line, int column, Expresion hijo)
        {
            this.line = line;
            this.column = column;
            this.hijo = hijo;
        }

        public retorno getValorSintetizado(Entorno ent, ProgramClass programClass)
        {
            retorno resultado;
            retorno valorHijo = this.hijo.getValorSintetizado(ent, programClass);

            if (valorHijo.type == tiposPrimitivos.BOOLEAN)
            {
                if ((bool)valorHijo.value)
                {
                    resultado.type = tiposPrimitivos.BOOLEAN;
                    resultado.value = (bool)false;
                }
                else //sí 
                {
                    resultado.type = tiposPrimitivos.BOOLEAN;
                    resultado.value = (bool)true;
                }
            }
            else
            {
                Form1.ConsoleRichText.AppendText("Error semantico: " + "en la fila: " + this.line + " y en la columna: " + this.column +
                "Descrip: El tipo de la expresion '" + valorHijo.value.ToString() + "' debe ser Booleano " + "\n");

                throw new Exception("<tr>\n" +
                "\t<td>Error Semantico</td>\n" +
                "\t<td>" + "El tipo de la expresion '" + valorHijo.value.ToString() + "' debe ser Booleano " + "</td>\n" +
                "\t<td>" + this.line + "</td>\n" +
                "\t<td>" + this.column + "</td>\n</tr>");

            }

            return resultado;

        }
    }

}
