﻿using _OLC2_Proyecto1.src.Interfaces;
using _OLC2_Proyecto1.src.Expresiones;
using Irony.Parsing;
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Globalization;
using System.Windows.Forms;
using _OLC2_Proyecto1.src.Instrucciones.InstGlobales;
using _OLC2_Proyecto1.src.Instrucciones.InstLocales;
using _OLC2_Proyecto1.src.Instrucciones;

namespace _OLC2_Proyecto1.src.Gramatica
{
    class GeneradorAST
    {
        public List<string> Errores { get; set; }
        public List<Instruccion> Structs { get; set; }
        public List<Instruccion> ProcsFuncs { get; set; }
        public List<Instruccion> Instrucciones { get; set; }
        public ParseTree arbolIrony { get; }

        private string astDot;
        private int contador;

        public GeneradorAST(ParseTree ArbolIrony, List<string> LexSyntErrores)
        {
            this.Errores = LexSyntErrores;
            this.Structs = new List<Instruccion>();
            this.ProcsFuncs = new List<Instruccion>();

            this.arbolIrony = ArbolIrony;
            if (this.arbolIrony.Root != null)
                generarAST(this.arbolIrony.Root);
        }

        public GeneradorAST(ParseTree ArbolIrony)
        {
            this.Errores = new List<string>();
            this.Structs = new List<Instruccion>();
            this.ProcsFuncs = new List<Instruccion>();

            this.arbolIrony = ArbolIrony;
            if (this.arbolIrony.Root != null)
                generarAST(this.arbolIrony.Root);
        }

        private void generarAST(ParseTreeNode raizArbolIrony)
        {
            this.Instrucciones = (List<Instruccion>)construirNodo(raizArbolIrony);
        }

        private object construirNodo(ParseTreeNode actual)
        {
            if (nodeNameIsEqual(actual, "INIT"))
            {
                List<Instruccion> Instrucciones = (List<Instruccion>)construirNodo(actual.ChildNodes[2]);// aca devuelve las variables Globales

                Instrucciones.AddRange((List<Instruccion>)construirNodo(actual.ChildNodes[3]));// aca agrega las instruccones del bloque 

                return Instrucciones;
            }

            /*CONSTRUCCION DE INSTRUCCIONES GLOBALES*/
            else if (nodeNameIsEqual(actual, "INSTSGLOBAL"))
            {
                List<Instruccion> Instrucciones = new List<Instruccion>();
                 
                foreach (ParseTreeNode hijo in actual.ChildNodes)
                {
                    object inst = construirNodo(hijo);

                    if (inst is List<Instruccion>)
                        Instrucciones.AddRange((List<Instruccion>)inst);
                    /*else if (inst is Struct)
                        this.Structs.Add((Instruccion)inst);
                    else if (inst is Procedimiento || inst is Funcion)
                        this.ProcsFuncs.Add((Instruccion)inst);*/

                }

                return Instrucciones;
            }
            else if (nodeNameIsEqual(actual, "INSTGLOBAL"))
            {
                return construirNodo(actual.ChildNodes[0]);
            }
            else if (nodeNameIsEqual(actual, "DECLARACION"))
            {
                return construirNodo(actual.ChildNodes[1]);
            }
            else if (nodeNameIsEqual(actual, "CONSTANTES") || nodeNameIsEqual(actual, "VARIABLES"))
            {
                List<Instruccion> variables = new List<Instruccion>();

                foreach (ParseTreeNode hijo in actual.ChildNodes)
                {
                    variables.Add((Instruccion)construirNodo(hijo));
                }

                return variables;

            }
            else if (nodeNameIsEqual(actual, "CONSTANTE"))
            {
                return new Constante(actual.ChildNodes[1].Token.Location.Line + 1, actual.ChildNodes[1].Token.Location.Column + 1,
                    getLexemeNode(actual.ChildNodes[0]), (Expresion)construirNodo(actual.ChildNodes[2]));
            }
            else if (nodeNameIsEqual(actual, "VARIABLE"))
            {
                if (actual.ChildNodes.Count == 3)
                {
                    if (nodeNameIsEqual(actual.ChildNodes[0], "id"))
                    {
                        return new Variable(actual.ChildNodes[1].Token.Location.Line + 1, actual.ChildNodes[1].Token.Location.Column + 1,
                        new List<string>() { getLexemeNode(actual.ChildNodes[0]) }, (string)construirNodo(actual.ChildNodes[2]));
                    }
                    else
                    {
                        return new Variable(actual.ChildNodes[1].Token.Location.Line + 1, actual.ChildNodes[1].Token.Location.Column + 1,
                        (List<string>)construirNodo(actual.ChildNodes[0]), (string)construirNodo(actual.ChildNodes[2]));
                    }
                }
                else if (actual.ChildNodes.Count == 5)
                {
                    return new Variable(actual.ChildNodes[1].Token.Location.Line + 1, actual.ChildNodes[1].Token.Location.Column + 1,
                        new List<string>() {getLexemeNode(actual.ChildNodes[0])}, (string)construirNodo(actual.ChildNodes[2]), (Expresion)construirNodo(actual.ChildNodes[4]));
                }
            }
            else if (nodeNameIsEqual(actual, "LISTAIDS"))
            {
                List<string> ids = new List<string>();

                foreach (ParseTreeNode hijo in actual.ChildNodes)
                {
                    ids.Add(getLexemeNode(hijo).Replace(",", ""));
                }

                return ids;

            }
            else if (nodeNameIsEqual(actual, "TIPO"))
            {
                return getLexemeNode(actual.ChildNodes[0]);
            }
            else if (nodeNameIsEqual(actual, "BLOQUE"))
            {
                return construirNodo(actual.ChildNodes[1]);
            }

            /*CONSTRUCCION DE INSTRUCCIONES LOCALES*/
            else if (nodeNameIsEqual(actual, "INSTSLOCAL"))
            {
                List<Instruccion> Instrucciones = new List<Instruccion>();

                foreach (ParseTreeNode hijo in actual.ChildNodes)
                {
                    Instrucciones.Add((Instruccion)construirNodo(hijo));
                }

                return Instrucciones; //return new AST(Instrucciones); 
            }
            else if (nodeNameIsEqual(actual, "INSTLOCAL"))
            {
                return construirNodo(actual.ChildNodes[0]);
            }
            else if (nodeNameIsEqual(actual, "TS"))
            {
                return new GraficarTS(actual.ChildNodes[0].Token.Location.Line + 1, actual.ChildNodes[0].Token.Location.Column + 1);
            }
            else if (nodeNameIsEqual(actual, "ASIG"))
            {
                return new AsignacionVar(actual.ChildNodes[1].Token.Location.Line + 1, actual.ChildNodes[1].Token.Location.Column + 1,
                    getLexemeNode(actual.ChildNodes[0]), (Expresion)construirNodo(actual.ChildNodes[2]));
            }
            else if (nodeNameIsEqual(actual, "WRITE"))
            {
                bool Writeln = false;
                if (getLexemeNode(actual.ChildNodes[0]) == "writeln")
                    Writeln = true;

                if (actual.ChildNodes.Count == 2)
                    return new Write(actual.ChildNodes[0].Token.Location.Line + 1, actual.ChildNodes[0].Token.Location.Column + 1,
                         (Expresion)construirNodo(actual.ChildNodes[1]), Writeln);
                else if (actual.ChildNodes.Count == 1)
                    return new Write(actual.ChildNodes[0].Token.Location.Line + 1, actual.ChildNodes[0].Token.Location.Column + 1,
                         new Primitivo(actual.ChildNodes[0].Token.Location.Line, 1, "", tiposPrimitivos.STRING), Writeln);
            }

            /*CONSTRUCCION DE EXPRESIONES*/
            else if (nodeNameIsEqual(actual, "EXP")|| nodeNameIsEqual(actual, "E"))
            {
                return construirNodo(actual.ChildNodes[0]);
            }
            else if (nodeNameIsEqual(actual, "EXPARIT") || nodeNameIsEqual(actual, "E_ARIT"))
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
            else if (nodeNameIsEqual(actual, "EXPREL") || nodeNameIsEqual(actual, "E_REL"))
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
            else if (nodeNameIsEqual(actual, "EXPLOG") || nodeNameIsEqual(actual, "E_LOG"))
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
            else if (nodeNameIsEqual(actual, "DATO")|| nodeNameIsEqual(actual, "LITERAL"))
            {
                if (nodeNameIsEqual(actual.ChildNodes[0], "EXP")|| nodeNameIsEqual(actual.ChildNodes[0], "E"))
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
                    else if (nodeNameIsEqual(actual.ChildNodes[0], "true") || nodeNameIsEqual(actual.ChildNodes[0], "false"))
                        return new Primitivo(actual.ChildNodes[0].Token.Location.Line + 1, actual.ChildNodes[0].Token.Location.Column + 1,
                           bool.Parse(valor), tiposPrimitivos.BOOLEAN);
                    else if (nodeNameIsEqual(actual.ChildNodes[0], "id"))
                        return new Acceso(actual.ChildNodes[0].Token.Location.Line + 1, actual.ChildNodes[0].Token.Location.Column + 1,
                            valor);

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
            if (this.arbolIrony.Root != null)
            {
                //GENERAMS EL CODIGO DEL GRAPHO
                this.astDot = "digraph G{";
                this.astDot += "nodo0[label = \"" + escapar(this.arbolIrony.Root.ToString()) + "\"];\n";
                this.contador = 1;
                construirASTdot("nodo0", this.arbolIrony.Root);
                this.astDot += "}";

                //CREAMOS EL ARCHIVO DOT Y ESCRIBIMOS EN EL EL CODIGO GENERADO
                File.WriteAllText("C:\\compiladores2\\AST.dot", this.astDot);

                //EJECUTAMOS EL COMANDO PARA COMPILAR Y GENERAR EL .dot a .jpg
                string comandoDot = "dot.exe -Tpng C:\\compiladores2\\AST.dot -o C:\\compiladores2\\AST.jpg";
                var comando = string.Format(comandoDot);
                var procStart = new System.Diagnostics.ProcessStartInfo("cmd", "/C" + comando);
                var proc = new System.Diagnostics.Process();
                proc.StartInfo = procStart;
                proc.Start();
                proc.WaitForExit();

                //EJECUTAMOS IMAGEN .jpg QUE CREAMOS
                if (File.Exists("C:\\compiladores2\\AST.jpg"))
                {
                    comandoDot = "C:\\compiladores2\\AST.jpg";
                    comando = string.Format(comandoDot);
                    procStart = new System.Diagnostics.ProcessStartInfo("cmd", "/C" + comando);
                    proc = new System.Diagnostics.Process();
                    proc.StartInfo = procStart;
                    proc.Start();
                    proc.WaitForExit();
                }
                else
                    MessageBox.Show("ERROR: el archivo C:\\compiladores2\\AST.jpg no existe :(");

            }

        }

        private void construirASTdot(string padre, ParseTreeNode actual)
        {
            foreach (ParseTreeNode hijo in actual.ChildNodes)
            {
                string nombreHijo = "nodo" + contador.ToString();
                this.astDot += nombreHijo + "[label=\"" + escapar(hijo.ToString()).Replace("t_","") + "\"];\n";
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

        public void generarErrores()
        {
            string erroresHTML = "<title>Reporte de Errores</title>\n\n" +

            "<style type = \"text / css\">\n\n" +

            "\ttable, th, td {\n" +
            "\t\tborder: 1px solid black;\n" +
            "\t\tborder-collapse: collapse;\n" +
            "\t}\n\n" +
            "\tth, td{\n" +
            "\t\tpadding: 10px;\n" +
            "\t}\n\n" +

            "</style>\n\n" +

            "<h1  style=\"text-align: center;\">Listado de Errores</h1>\n\n" +

            "<table border = 1.5 width = 100%>\n" +
            "<head>\n" +
            "\t<tr bgcolor = red >\n" +
            "\t\t<th>Tipo</th>\n" +
            "\t\t<th>Descripcion</th>\n" +
            "\t\t<th>Linea</th>\n" +
            "\t\t<th>Columna</th>\n" +
            "\t</tr>\n" +
            "</head>\n\n";

            foreach (string error in this.Errores)
            {
                erroresHTML += error + "\n\n";
            }

            //CREAMOS EL ARCHIVO DOT Y ESCRIBIMOS EN EL EL CODIGO GENERADO
            File.WriteAllText("C:\\compiladores2\\ListaErrores.HTML", erroresHTML);

            //EJECUTAMOS IMAGEN .HTML QUE CREAMOS
            if (File.Exists("C:\\compiladores2\\ListaErrores.HTML"))
            {
                string comandoDot = "C:\\compiladores2\\ListaErrores.HTML";
                var comando = string.Format(comandoDot);
                var procStart = new System.Diagnostics.ProcessStartInfo("cmd", "/C" + comando);
                var proc = new System.Diagnostics.Process();
                proc.StartInfo = procStart;
                proc.Start();
                proc.WaitForExit();
            }
            else
                MessageBox.Show("ERROR: el archivo C:\\compiladores2\\ListaErrores.HTM no existe :(");

        }

    }
}
