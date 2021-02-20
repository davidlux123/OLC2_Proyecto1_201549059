using _OLC2_Proyecto1.src.Interfaces;
using _OLC2_Proyecto1.src.Instrucciones;
using _OLC2_Proyecto1.src.Expresiones;
using Irony.Parsing;
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Globalization;
using System.Windows.Forms;

namespace _OLC2_Proyecto1.src.Gramatica
{
    class GeneradorAST
    {

        public AST ast { get; set; }

        private ParseTree arbolIrony;
        private string astDot;
        private int contador;

        public GeneradorAST(ParseTree ArbolIrony)
        {
            this.arbolIrony = ArbolIrony;
            generarAST(this.arbolIrony.Root);
        }



        private void generarAST(ParseTreeNode raizArbolIrony)
        {
            this.ast = (AST)construirNodo(raizArbolIrony);
        }

        private object construirNodo(ParseTreeNode actual)
        {
            if (nodeNameIsEqual(actual, "INIT"))
            {
                return construirNodo(actual.ChildNodes[3]);
            }
            else if (nodeNameIsEqual(actual, "INSTRUCCIONES"))
            {
                LinkedList<Instruccion> instrucciones = new LinkedList<Instruccion>();
                foreach (ParseTreeNode hijo in actual.ChildNodes)
                {
                    instrucciones.AddLast((Instruccion)construirNodo(hijo));
                }

                return new AST(instrucciones);
            }
            
            /*CONSTRUCCION DE INSTRUCCIONES*/
            else if (nodeNameIsEqual(actual, "INSTRUCCION"))
            {
                return construirNodo(actual.ChildNodes[0]);
            }
            else if (nodeNameIsEqual(actual, "WRITE"))
            {
                bool Writeln = false;
                if (nodeNameIsEqual(actual.ChildNodes[0], "writeln"))
                    Writeln = true;

                if (actual.ChildNodes.Count == 2)
                
                    return new WRITE(actual.ChildNodes[0].Token.Location.Line + 1, actual.ChildNodes[0].Token.Location.Column + 1,
                         (Expresion)construirNodo(actual.ChildNodes[1]), Writeln);

                else if (actual.ChildNodes.Count == 1)
                
                    return new WRITE( actual.ChildNodes[0].Token.Location.Line + 1, actual.ChildNodes[0].Token.Location.Column + 1,
                         new Primitivo(actual.ChildNodes[0].Token.Location.Line, 1, "", tiposPrimitivos.STRING), Writeln);
                
            }

            /*CONSTRUCCION DE EXPRESIONES*/
            else if (nodeNameIsEqual(actual, "EXP"))
            {
                return construirNodo(actual.ChildNodes[0]);
            }
            else if (nodeNameIsEqual(actual, "EXPARIT"))
            {
                if (actual.ChildNodes.Count == 3)
                {
                    string opcionA = getLexemeNode(actual.ChildNodes[1]);

                    if (opcionA == "+")
                        return new Aritmetica(actual.ChildNodes[1].Token.Location.Line + 1, actual.ChildNodes[1].Token.Location.Column + 1,
                            (Expresion)construirNodo(actual.ChildNodes[0]), (Expresion)construirNodo(actual.ChildNodes[2]), opcionAritmetica.SUMAR, opcionA);
                    else if (opcionA == "-")
                        return new Aritmetica(actual.ChildNodes[1].Token.Location.Line + 1, actual.ChildNodes[1].Token.Location.Column + 1,
                            (Expresion)construirNodo(actual.ChildNodes[0]), (Expresion)construirNodo(actual.ChildNodes[2]), opcionAritmetica.RESTAR, opcionA);
                    else if (opcionA == "*")
                        return new Aritmetica(actual.ChildNodes[1].Token.Location.Line + 1, actual.ChildNodes[1].Token.Location.Column + 1,
                            (Expresion)construirNodo(actual.ChildNodes[0]), (Expresion)construirNodo(actual.ChildNodes[2]), opcionAritmetica.MULTIPLICAR, opcionA);
                    else if (opcionA == "-")
                        return new Aritmetica(actual.ChildNodes[1].Token.Location.Line + 1, actual.ChildNodes[1].Token.Location.Column + 1,
                            (Expresion)construirNodo(actual.ChildNodes[0]), (Expresion)construirNodo(actual.ChildNodes[2]), opcionAritmetica.DIVIDIR, opcionA);
                    else if (opcionA == "%")
                        return new Aritmetica(actual.ChildNodes[1].Token.Location.Line + 1, actual.ChildNodes[1].Token.Location.Column + 1,
                            (Expresion)construirNodo(actual.ChildNodes[0]), (Expresion)construirNodo(actual.ChildNodes[2]), opcionAritmetica.RESTO, opcionA);

                }
                else if (actual.ChildNodes.Count == 2)
                {
                    return new AritmeticaUnaria(actual.ChildNodes[0].Token.Location.Line + 1, actual.ChildNodes[0].Token.Location.Column + 1,
                        (Expresion)construirNodo(actual.ChildNodes[1]));
                }

            }
            else if (nodeNameIsEqual(actual, "EXPREL"))
            {
                string opcionR = getLexemeNode(actual.ChildNodes[1]);

                if (opcionR == "<")
                    return new Relacional(actual.ChildNodes[1].Token.Location.Line + 1, actual.ChildNodes[1].Token.Location.Column + 1,
                        (Expresion)construirNodo(actual.ChildNodes[0]), (Expresion)construirNodo(actual.ChildNodes[2]), opcionRelacional.MENOR, opcionR);
                else if (opcionR == "<=")
                    return new Relacional(actual.ChildNodes[1].Token.Location.Line + 1, actual.ChildNodes[1].Token.Location.Column + 1,
                        (Expresion)construirNodo(actual.ChildNodes[0]), (Expresion)construirNodo(actual.ChildNodes[2]), opcionRelacional.MENORIGUAL, opcionR);
                else if (opcionR == ">")
                    return new Relacional(actual.ChildNodes[1].Token.Location.Line + 1, actual.ChildNodes[1].Token.Location.Column + 1,
                        (Expresion)construirNodo(actual.ChildNodes[0]), (Expresion)construirNodo(actual.ChildNodes[2]), opcionRelacional.MAYOR, opcionR);
                else if (opcionR == ">=")
                    return new Relacional(actual.ChildNodes[1].Token.Location.Line + 1, actual.ChildNodes[1].Token.Location.Column + 1,
                        (Expresion)construirNodo(actual.ChildNodes[0]), (Expresion)construirNodo(actual.ChildNodes[2]), opcionRelacional.MAYORIGUAL, opcionR);
                else if (opcionR == "=")
                    return new Relacional(actual.ChildNodes[1].Token.Location.Line + 1, actual.ChildNodes[1].Token.Location.Column + 1,
                        (Expresion)construirNodo(actual.ChildNodes[0]), (Expresion)construirNodo(actual.ChildNodes[2]), opcionRelacional.IGUAL, opcionR);
                else if (opcionR == "<>")
                    return new Relacional(actual.ChildNodes[1].Token.Location.Line + 1, actual.ChildNodes[1].Token.Location.Column + 1,
                        (Expresion)construirNodo(actual.ChildNodes[0]), (Expresion)construirNodo(actual.ChildNodes[2]), opcionRelacional.DIFERENCIACION, opcionR);
                
            }
            else if (nodeNameIsEqual(actual, "EXPLOG"))
            {
                if (actual.ChildNodes.Count == 3)
                {
                    string opcionL = getLexemeNode(actual.ChildNodes[1]);

                    if (opcionL == "AND")
                        return new Logica(actual.ChildNodes[1].Token.Location.Line + 1, actual.ChildNodes[1].Token.Location.Column + 1,
                            (Expresion)construirNodo(actual.ChildNodes[0]), (Expresion)construirNodo(actual.ChildNodes[2]), opcionLogica.AND, opcionL);
                    else if (opcionL == "OR")
                        return new Logica(actual.ChildNodes[1].Token.Location.Line + 1, actual.ChildNodes[1].Token.Location.Column + 1,
                            (Expresion)construirNodo(actual.ChildNodes[0]), (Expresion)construirNodo(actual.ChildNodes[2]), opcionLogica.OR, opcionL);
                    
                }
                else if (actual.ChildNodes.Count == 2)
                {
                    return new LogicaUnaria(actual.ChildNodes[0].Token.Location.Line + 1, actual.ChildNodes[0].Token.Location.Column + 1,
                        (Expresion)construirNodo(actual.ChildNodes[1]));
                }
                
            }
            else if (nodeNameIsEqual(actual, "DATO"))
            {
                if (nodeNameIsEqual(actual.ChildNodes[0], "EXP"))
                {
                    return construirNodo(actual.ChildNodes[0]);
                }
                else 
                {
                    string valor = getLexemeNode(actual.ChildNodes[0]);

                    if (nodeNameIsEqual(actual.ChildNodes[0], "numeroEntero"))
                        return new Primitivo(actual.ChildNodes[0].Token.Location.Line + 1, actual.ChildNodes[0].Token.Location.Column + 1,
                            int.Parse(valor), tiposPrimitivos.INT);
                    else if (nodeNameIsEqual(actual.ChildNodes[0], "numeroDecimal"))
                        return new Primitivo(actual.ChildNodes[0].Token.Location.Line + 1, actual.ChildNodes[0].Token.Location.Column + 1,
                            float.Parse(valor, CultureInfo.GetCultureInfo("en-US")), tiposPrimitivos.REAL);
                    else if (nodeNameIsEqual(actual.ChildNodes[0], "cadena"))
                        return new Primitivo(actual.ChildNodes[0].Token.Location.Line + 1, actual.ChildNodes[0].Token.Location.Column + 1,
                            valor, tiposPrimitivos.STRING);
                    else if (nodeNameIsEqual(actual.ChildNodes[0], "True") || nodeNameIsEqual(actual.ChildNodes[0], "False"))
                        return new Primitivo(actual.ChildNodes[0].Token.Location.Line + 1, actual.ChildNodes[0].Token.Location.Column + 1,
                           bool.Parse(valor), tiposPrimitivos.BOOLEAN);

                }

            }
            
            return null;

        }

        private bool nodeNameIsEqual(ParseTreeNode nodo, string nombre)
        {
            return nodo.Term.Name.Equals(nombre, System.StringComparison.InvariantCultureIgnoreCase);  
        }

        private string getLexemeNode(ParseTreeNode nodo){
            return nodo.Token.Text;
        }

        public void generarASTdot()
        {
            //GENERAMS EL CODIGO DEL DOT
            this.astDot = "digraph G{";
            this.astDot += "nodo0[label = \"" + escapar(arbolIrony.Root.ToString()) + "\"];\n";
            this.contador = 1;
            construirASTdot("nodo0", arbolIrony.Root);
            this.astDot += "}";

            //CREAMOS EL ARCHIVO DOT Y ESCRIBIMOS EN EL EL CODIGO GENERADO
            File.WriteAllText("C:\\compiladores2\\AST.dot", this.astDot);

            //EJECUTAMOS EL COMANDO PARA COMPILAR Y GENERAR EL DOT a jpg
            string comandoDot = "dot.exe -Tpng C:\\compiladores2\\AST.dot -o C:\\compiladores2\\AST.jpg";
            var comando = string.Format(comandoDot);
            var procStart = new System.Diagnostics.ProcessStartInfo("cmd", "/C" + comando);
            var proc = new System.Diagnostics.Process();
            proc.StartInfo = procStart;
            proc.Start();
            proc.WaitForExit();

        }

        private void construirASTdot(string padre, ParseTreeNode actual)
        {
            foreach (ParseTreeNode hijo in actual.ChildNodes)
            {
                string nombreHijo = "nodo" + contador.ToString();
                this.astDot += nombreHijo + "[label=\"" + escapar(hijo.ToString()) + "\"];\n";
                this.astDot += padre + "->" + nombreHijo + ";\n";
                this.contador++;

                construirASTdot(nombreHijo, hijo);

            }
        }

        private String escapar(String cadena)
        {
            cadena = cadena.Replace("\\", "\\\\");
            cadena = cadena.Replace("\"", "\\\"");
            return cadena;
        }

    }
}
