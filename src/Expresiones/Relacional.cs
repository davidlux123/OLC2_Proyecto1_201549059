using _OLC2_Proyecto1.src.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace _OLC2_Proyecto1.src.Expresiones
{
    enum opcionRelacional
    {
        IGUAL,
        DIFERENCIACION,
        MENOR,
        MENORIGUAL,
        MAYOR,
        MAYORIGUAL
    }
    class Relacional : Expresion
    {
        public int line { get ; set ; }
        public int column { get ; set; }

        private Expresion hijoIzq;
        private Expresion hijoDer;
        private opcionRelacional type;
        private string simbolo;


        public Relacional(int line, int column, Expresion hijoIzq, Expresion hijoDer, opcionRelacional type, string simbolo)
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
                if (valorIzq.type == valorDer.type && this.type == opcionRelacional.IGUAL)
                {
                    if (valorIzq.value == valorDer.value)
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
                else if (valorIzq.type == valorDer.type && this.type == opcionRelacional.DIFERENCIACION)
                {
                    if (valorIzq.value != valorDer.value)
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
                else if ((valorIzq.type == tiposPrimitivos.REAL || valorIzq.type == tiposPrimitivos.INT) &&
                         (valorDer.type == tiposPrimitivos.INT || valorDer.type == tiposPrimitivos.REAL))
                {
                    if (valorIzq.type == tiposPrimitivos.INT)
                        valorIzq.value = float.Parse(valorIzq.value.ToString(), CultureInfo.GetCultureInfo("en-US"));
                        if (valorDer.type == tiposPrimitivos.INT)
                            valorDer.value = float.Parse(valorDer.value.ToString(), CultureInfo.GetCultureInfo("en-US"));
                    else if (valorDer.type == tiposPrimitivos.INT)
                        valorDer.value = float.Parse(valorDer.value.ToString(), CultureInfo.GetCultureInfo("en-US"));

                    if (this.type == opcionRelacional.MENOR)
                    {
                        if ((float)valorIzq.value < (float)valorDer.value)
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
                    else if (this.type == opcionRelacional.MENORIGUAL)
                    {
                        if ((float)valorIzq.value <= (float)valorDer.value)
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
                    else if (this.type == opcionRelacional.MAYOR)
                    {
                        if ((float)valorIzq.value > (float)valorDer.value)
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
                    else //sí, this.type == opcionRelacional.MAYORIGUAL
                    {
                        if ((float)valorIzq.value >= (float)valorDer.value)
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
                    //error que no se puede bool > bool
                    resultado.type = tiposPrimitivos.error;
                    resultado.value = null;
                    Form1.ConsoleRichText.AppendText("Error semantico: " + "en la fila: " + this.line + " y en la columna: " + this.column +
                    " Descrip: no se puede operar '" + this.simbolo + "' de un tipo " + valorIzq.type.ToString() + " con tipo " + valorDer.type.ToString() + "\n");

                }
            }
            

            return resultado;
        }
    }
}
