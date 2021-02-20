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

        public retorno execute()
        {
            retorno resultado;
            resultado.value = null;
            resultado.type = tiposPrimitivos.error;

            retorno valorIzq = this.hijoIzq.execute();
            retorno valorDer = this.hijoDer.execute();

            if (valorIzq.type != tiposPrimitivos.error && valorIzq.value != null && valorDer.type != tiposPrimitivos.error && valorDer.value != null)
            {
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
                    //los tipos de valores no son bools
                    resultado.type = tiposPrimitivos.error;
                    resultado.value = null;
                    Form1.ConsoleRichText.AppendText("Error semantico: " + "en la fila: " + this.line + " y en la columna: " + this.column +
                    " Descrip: no se puede operar '" + this.simbolo + "' de un tipo " + valorIzq.type.ToString() + " con tipo " + valorDer.type.ToString() + "\n");
                }
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

        public retorno execute()
        {
            retorno resultado;
            resultado.value = null;
            resultado.type = tiposPrimitivos.error;

            retorno valorHijo = this.hijo.execute();

            if (valorHijo.value != null && valorHijo.type != tiposPrimitivos.error)
            {
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
                    //ERROR DE TIPOS
                    resultado.type = tiposPrimitivos.error;
                    resultado.value = null;
                    Form1.ConsoleRichText.AppendText("Error semantico: " + "en la fila: " + this.line + " y en la columna: " + this.column +
                    " Descrip: el tipo debe ser Boolean" + "\n");
                }
            }

            return resultado;

        }
    }

}
