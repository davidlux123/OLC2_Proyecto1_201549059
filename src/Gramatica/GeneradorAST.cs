using _OLC2_Proyecto1.src.Interfaces;
using _OLC2_Proyecto1.src.Expresiones;
using Irony.Parsing;
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Globalization;
using System.Windows.Forms;
using _OLC2_Proyecto1.src.Instrucciones.InstLocales;
using _OLC2_Proyecto1.src.Instrucciones;
using _OLC2_Proyecto1.src.Ambientes;

namespace _OLC2_Proyecto1.src.Gramatica
{
    public struct expPosArr
    {
        public string id;
        public List<Expresion> posicion;
    }

    public class GeneradorAST
    {
        public List<string> Errores { get; set; }
        public List<Instruccion> Instrucciones { get; set; }
        public ParseTree arbolIrony { get; }

        private string astDot;
        private int contador;

        public GeneradorAST(ParseTree ArbolIrony, List<string> LexSyntErrores)
        {
            this.Errores = LexSyntErrores;

            this.arbolIrony = ArbolIrony;
            if (this.arbolIrony.Root != null)

                this.Instrucciones = (List<Instruccion>)construirNodo(this.arbolIrony.Root);
        }
        public GeneradorAST(ParseTree ArbolIrony)
        {
            this.Errores = new List<string>();

            this.arbolIrony = ArbolIrony;
            if (this.arbolIrony.Root != null)
                this.Instrucciones = (List<Instruccion>)construirNodo(this.arbolIrony.Root);
        }
        private object construirNodo(ParseTreeNode actual)
        {
            if (nodeNameIsEqual(actual, "INIT"))
            {
                List<Instruccion> Instrucciones = (List<Instruccion>)construirNodo(actual.ChildNodes[2]);// aca devuelve las variables Globales

                Instrucciones.AddRange((List<Instruccion>)construirNodo(actual.ChildNodes[3].ChildNodes[1]));// aca agrega las instruccones del bloque 

                return Instrucciones;
            }

            /*CONSTRUCCION DE INSTRUCCIONES GLOBALES*/
            else if (nodeNameIsEqual(actual, "INSTSGLOBAL"))
            {
                List<Instruccion> Instrucciones = new List<Instruccion>();
                 
                foreach (ParseTreeNode hijo in actual.ChildNodes)
                {
                    Instrucciones.AddRange((List<Instruccion>)construirNodo(hijo.ChildNodes[0]));
                }

                return Instrucciones;
            }
            else if (nodeNameIsEqual(actual, "DECLARACIONTYPE"))
            {
                List<Instruccion> definiciones = new List<Instruccion>();
                foreach (ParseTreeNode hijoVars in actual.ChildNodes[1].ChildNodes)
                {
                    definiciones.Add((Instruccion)construirNodo(hijoVars));
                }
                return definiciones;
            }
            else if (nodeNameIsEqual(actual, "DEFTYPE"))    
            {
                object TYPE = construirNodo(actual.ChildNodes[2].ChildNodes[0]);

                if (TYPE is Struct)
                {
                    Struct type = (Struct)TYPE; 
                    type.idStruct = getLexemeNode(actual.ChildNodes[0]);
                    type.entStruct.nombreAmbito = getLexemeNode(actual.ChildNodes[0]);
                    return type;
                }
                else if (TYPE is Arreglo)
                {
                    Arreglo type = (Arreglo)TYPE;
                    type.idArreglo = getLexemeNode(actual.ChildNodes[0]);
                    return type;
                }
                else
                {
                    return new Literal(actual.ChildNodes[0].Token.Location.Line + 1, actual.ChildNodes[0].Token.Location.Column + 1,
                        getLexemeNode(actual.ChildNodes[0]), TYPE);
                }
            }
            else if (nodeNameIsEqual(actual, "ARRAY"))
            {
                List<Expresion[]> dimensiones = new List<Expresion[]>();
                foreach (ParseTreeNode hijoVars in actual.ChildNodes[2].ChildNodes)
                {
                    dimensiones.Add((Expresion[])construirNodo(hijoVars));
                }

                return new Arreglo(actual.ChildNodes[0].Token.Location.Line + 1, actual.ChildNodes[0].Token.Location.Column + 1,
                    construirNodo(actual.ChildNodes[5]), dimensiones);

            }
            else if (nodeNameIsEqual(actual, "DIMENSION"))
            {
                Expresion[] dimension = { (Expresion)construirNodo(actual.ChildNodes[0].ChildNodes[0]),
                                          (Expresion)construirNodo(actual.ChildNodes[3].ChildNodes[0]) };
                return dimension;
            }
            else if (nodeNameIsEqual(actual, "STRUCT"))
            {
                List<Declaracion> variables = new List<Declaracion>();

                foreach (ParseTreeNode hijoVars in actual.ChildNodes[2].ChildNodes)
                {
                    variables.AddRange((List<Declaracion>)construirNodo(hijoVars));
                }

                return new Struct(actual.ChildNodes[0].Token.Location.Line + 1, actual.ChildNodes[0].Token.Location.Column + 1, variables);
            }
            else if (nodeNameIsEqual(actual, "VAR"))
            {
                List<Declaracion> decs = new List<Declaracion>();

                foreach (ParseTreeNode idDecVar in actual.ChildNodes[0].ChildNodes)
                {
                    decs.Add(new Declaracion(actual.ChildNodes[1].Token.Location.Line + 1, actual.ChildNodes[1].Token.Location.Column + 1,
                        getLexemeNode(idDecVar).Replace(",", ""), construirNodo(actual.ChildNodes[2])));
                }
                return  decs;
            }
            else if (nodeNameIsEqual(actual, "DECLARACION"))
            {
                List<Instruccion> Variables = new List<Instruccion>();
                foreach (ParseTreeNode hijoVars in actual.ChildNodes[1].ChildNodes)
                {
                    object dec = construirNodo(hijoVars);
                    if (dec is List<Declaracion>)
                        Variables.AddRange((List<Declaracion>)dec);
                    else
                        Variables.Add((Instruccion)dec);

                }
                return Variables;
            }
            else if (nodeNameIsEqual(actual, "CONSTANTE"))
            {
                return new Declaracion(actual.ChildNodes[1].Token.Location.Line + 1, actual.ChildNodes[1].Token.Location.Column + 1,
                    getLexemeNode(actual.ChildNodes[0]), "", (Expresion)construirNodo(actual.ChildNodes[2].ChildNodes[0]), true);
            }
            else if (nodeNameIsEqual(actual, "VARIABLE"))
            {
                List<Declaracion> decs = new List<Declaracion>();

                if (nodeNameIsEqual(actual.ChildNodes[0], "id"))
                    if (actual.ChildNodes.Count == 5)
                        decs.Add(new Declaracion(actual.ChildNodes[1].Token.Location.Line + 1, actual.ChildNodes[1].Token.Location.Column + 1,
                        getLexemeNode(actual.ChildNodes[0]), getLexemeNode(actual.ChildNodes[2].ChildNodes[0]),
                        (Expresion)construirNodo(actual.ChildNodes[4].ChildNodes[0]), false));
                    else //sino se le asigna una expresion
                        decs.Add(new Declaracion(actual.ChildNodes[1].Token.Location.Line + 1, actual.ChildNodes[1].Token.Location.Column + 1,
                            getLexemeNode(actual.ChildNodes[0]), construirNodo(actual.ChildNodes[2])));
                else//si viene un listado de ids
                    foreach (ParseTreeNode idDecVar in actual.ChildNodes[0].ChildNodes)
                        decs.Add(new Declaracion(actual.ChildNodes[1].Token.Location.Line + 1, actual.ChildNodes[1].Token.Location.Column + 1,
                            getLexemeNode(idDecVar).Replace(",", ""), construirNodo(actual.ChildNodes[2])));

                return decs;
            }
            else if (nodeNameIsEqual(actual, "TIPO"))
            {
                string tipo = getLexemeNode(actual.ChildNodes[0]);
                if (nodeNameIsEqual(actual.ChildNodes[0], "id"))
                    return tipo;
                else if (tipo == "integer")
                    return tiposPrimitivos.INT;
                else if (tipo == "real")
                    return tiposPrimitivos.REAL;
                else if (tipo == "string")
                    return tiposPrimitivos.STRING;
                else //if (tipo == "boolean")
                    return tiposPrimitivos.BOOLEAN;
            }
            else if (nodeNameIsEqual(actual, "PROCFUNC"))
            {
                List<Parametro> parametros = new List<Parametro>();
                foreach (ParseTreeNode hijoVars in actual.ChildNodes[2].ChildNodes)
                {
                    parametros.AddRange((List<Parametro>)construirNodo(hijoVars));
                }

                if (actual.ChildNodes.Count == 5)
                {
                    List<Instruccion> instrucciones = new List<Instruccion>();
                    foreach (ParseTreeNode hijo in actual.ChildNodes[3].ChildNodes)
                    {
                        Instrucciones.AddRange((List<Instruccion>)construirNodo(hijo));
                    }
                    Instrucciones.AddRange((List<Instruccion>)construirNodo(actual.ChildNodes[4].ChildNodes[1]));

                    return new Funcion(actual.ChildNodes[0].Token.Location.Line + 1, actual.ChildNodes[0].Token.Location.Column + 1,
                        getLexemeNode(actual.ChildNodes[1]), tiposPrimitivos.VOID, instrucciones, parametros);
                }
                else if (actual.ChildNodes.Count == 7)
                {
                    List<Instruccion> instrucciones = new List<Instruccion>();
                    foreach (ParseTreeNode hijo in actual.ChildNodes[5].ChildNodes)
                    {
                        Instrucciones.AddRange((List<Instruccion>)construirNodo(hijo));
                    }
                    Instrucciones.AddRange((List<Instruccion>)construirNodo(actual.ChildNodes[6].ChildNodes[1]));

                    return new Funcion(actual.ChildNodes[0].Token.Location.Line + 1, actual.ChildNodes[0].Token.Location.Column + 1,
                        getLexemeNode(actual.ChildNodes[1]), construirNodo(actual.ChildNodes[4]), instrucciones, parametros);
                }

            }
            else if (nodeNameIsEqual(actual, "PARAM"))
            {
                List<Parametro> decs = new List<Parametro>();

                if (getLexemeNode(actual.ChildNodes[0]) == "var")
                {
                    decs.Add(new Parametro(actual.ChildNodes[0].Token.Location.Line + 1, actual.ChildNodes[0].Token.Location.Column + 1,
                            getLexemeNode(actual.ChildNodes[1]).Replace(",", ""), construirNodo(actual.ChildNodes[2]), true));
                }
                else
                {
                    foreach (ParseTreeNode idDecVar in actual.ChildNodes[0].ChildNodes)
                    {
                        decs.Add(new Parametro(actual.ChildNodes[0].Token.Location.Line + 1, actual.ChildNodes[0].Token.Location.Column + 1,
                            getLexemeNode(idDecVar).Replace(",", ""), construirNodo(actual.ChildNodes[2]), false));
                    }
                }
                return decs;
            }

            /*CONSTRUCCION DE INSTRUCCIONES LOCALES*/
            else if (nodeNameIsEqual(actual, "INSTSLOCAL"))
            {
                List<Instruccion> Instrucciones = new List<Instruccion>();

                foreach (ParseTreeNode hijo in actual.ChildNodes)
                {
                    Instrucciones.Add((Instruccion)construirNodo(hijo.ChildNodes[0]));
                }

                return Instrucciones; //return new AST(Instrucciones); 
            }
            else if (nodeNameIsEqual(actual, "TS"))
            {
                return new GraficarTS(actual.ChildNodes[0].Token.Location.Line + 1, actual.ChildNodes[0].Token.Location.Column + 1);
            }
            else if (nodeNameIsEqual(actual, "ASIG"))
            {
                List<expPosArr> accesos = new List<expPosArr>();
                foreach (ParseTreeNode hijo in actual.ChildNodes[0].ChildNodes)
                {
                    accesos.Add((expPosArr)construirNodo(hijo));
                }
                return new Asignacion(actual.ChildNodes[1].Token.Location.Line + 1, actual.ChildNodes[1].Token.Location.Column + 1,
                    accesos, (Expresion)construirNodo(actual.ChildNodes[2].ChildNodes[0]));
            }
            else if (nodeNameIsEqual(actual, "ACCESO"))
            {
                if (actual.ChildNodes.Count == 1)
                {
                    expPosArr nvo;
                    nvo.id = getLexemeNode(actual.ChildNodes[0]);
                    nvo.posicion = new List<Expresion>();
                    return nvo; 
                }
                else if (actual.ChildNodes.Count == 2)
                {
                    List<Expresion> posicion = new List<Expresion>();
                    foreach (ParseTreeNode hijo in actual.ChildNodes[1].ChildNodes)
                    {
                        posicion.Add((Expresion)construirNodo(hijo.ChildNodes[1].ChildNodes[0]));
                    }
                    expPosArr nvo;
                    nvo.id = getLexemeNode(actual.ChildNodes[0]);
                    nvo.posicion = posicion;
                    return nvo;
                }
            }
            else if (nodeNameIsEqual(actual, "WRITE"))
            {
                bool Writeln = false;
                if (getLexemeNode(actual.ChildNodes[0]) == "writeln")
                    Writeln = true;

                if (actual.ChildNodes.Count == 2)
                    return new Write(actual.ChildNodes[0].Token.Location.Line + 1, actual.ChildNodes[0].Token.Location.Column + 1,
                         (Expresion)construirNodo(actual.ChildNodes[1].ChildNodes[0]), Writeln);
                else if (actual.ChildNodes.Count == 1)
                    return new Write(actual.ChildNodes[0].Token.Location.Line + 1, actual.ChildNodes[0].Token.Location.Column + 1,
                         new Primitivo(actual.ChildNodes[0].Token.Location.Line, 1, "", tiposPrimitivos.STRING), Writeln);
            }
            else if (nodeNameIsEqual(actual, "IF"))
            {
                List<IF> elsifs = new List<IF>();
                foreach (ParseTreeNode elsif in actual.ChildNodes[4].ChildNodes)
                {
                    elsifs.Add((IF)construirNodo(elsif));
                }

                return new IF(actual.ChildNodes[0].Token.Location.Line + 1, actual.ChildNodes[0].Token.Location.Column + 1,
                    (Expresion)construirNodo(actual.ChildNodes[1].ChildNodes[0]), (List<Instruccion>)construirNodo(actual.ChildNodes[3].ChildNodes[1]),
                    elsifs, (List<Instruccion>)construirNodo(actual.ChildNodes[5]));
            }
            else if (nodeNameIsEqual(actual, "ELSIF"))
            {
                return new IF(actual.ChildNodes[0].Token.Location.Line + 1, actual.ChildNodes[0].Token.Location.Column + 1,
                    (Expresion)construirNodo(actual.ChildNodes[1].ChildNodes[0]), (List<Instruccion>)construirNodo(actual.ChildNodes[3].ChildNodes[1]));
            }
            else if (nodeNameIsEqual(actual, "ELSE"))
            {
                List<Instruccion> instsElse = new List<Instruccion>();
                if (actual.ChildNodes.Count == 2)
                {
                    instsElse.AddRange((List<Instruccion>)construirNodo(actual.ChildNodes[1].ChildNodes[1]));
                }
                return instsElse;
            }
            else if (nodeNameIsEqual(actual, "FOR"))
            {
                return new For(actual.ChildNodes[0].Token.Location.Line + 1, actual.ChildNodes[0].Token.Location.Column + 1,
                    getLexemeNode(actual.ChildNodes[1]), (Expresion)construirNodo(actual.ChildNodes[3].ChildNodes[0]), (Expresion)construirNodo(actual.ChildNodes[5].ChildNodes[0]),
                    (List<Instruccion>)construirNodo(actual.ChildNodes[7].ChildNodes[1]));

            }
            else if (nodeNameIsEqual(actual, "WHILE"))
            {
                return new While(actual.ChildNodes[0].Token.Location.Line + 1, actual.ChildNodes[0].Token.Location.Column + 1,
                    (Expresion)construirNodo(actual.ChildNodes[1].ChildNodes[0]), (List<Instruccion>)construirNodo(actual.ChildNodes[3].ChildNodes[1]));
            }
            else if (nodeNameIsEqual(actual, "REPEAT"))
            {
                return new Repeat(actual.ChildNodes[0].Token.Location.Line + 1, actual.ChildNodes[0].Token.Location.Column + 1,
                    (Expresion)construirNodo(actual.ChildNodes[3].ChildNodes[0]), (List<Instruccion>)construirNodo(actual.ChildNodes[1].ChildNodes[1]));
            }
            else if (nodeNameIsEqual(actual, "LLAMADA"))
            {
                List<Expresion> vals = new List<Expresion>();

                foreach (ParseTreeNode valores in actual.ChildNodes[1].ChildNodes)
                {
                    vals.Add((Expresion)construirNodo(valores.ChildNodes[0]));
                }

                return new LlamadaFunc(actual.ChildNodes[0].Token.Location.Line + 1, actual.ChildNodes[0].Token.Location.Column + 1,
                    getLexemeNode(actual.ChildNodes[0]), vals);
            }


            /*CONSTRUCCION DE EXPRESIONES*/
            else if (nodeNameIsEqual(actual, "EXPARIT") || nodeNameIsEqual(actual, "E_ARIT"))
            {
                if (actual.ChildNodes.Count == 3)
                {
                    string opcionA = getLexemeNode(actual.ChildNodes[1]);

                    if (opcionA == "+")
                        return new Aritmetica(actual.ChildNodes[1].Token.Location.Line + 1, actual.ChildNodes[1].Token.Location.Column + 1,
                            (Expresion)construirNodo(actual.ChildNodes[0].ChildNodes[0]), (Expresion)construirNodo(actual.ChildNodes[2].ChildNodes[0]), opcionAritmetica.SUMAR, opcionA);
                    else if (opcionA == "-")
                        return new Aritmetica(actual.ChildNodes[1].Token.Location.Line + 1, actual.ChildNodes[1].Token.Location.Column + 1,
                            (Expresion)construirNodo(actual.ChildNodes[0].ChildNodes[0]), (Expresion)construirNodo(actual.ChildNodes[2].ChildNodes[0]), opcionAritmetica.RESTAR, opcionA);
                    else if (opcionA == "*")
                        return new Aritmetica(actual.ChildNodes[1].Token.Location.Line + 1, actual.ChildNodes[1].Token.Location.Column + 1,
                            (Expresion)construirNodo(actual.ChildNodes[0].ChildNodes[0]), (Expresion)construirNodo(actual.ChildNodes[2].ChildNodes[0]), opcionAritmetica.MULTIPLICAR, opcionA);
                    else if (opcionA == "-")
                        return new Aritmetica(actual.ChildNodes[1].Token.Location.Line + 1, actual.ChildNodes[1].Token.Location.Column + 1,
                            (Expresion)construirNodo(actual.ChildNodes[0].ChildNodes[0]), (Expresion)construirNodo(actual.ChildNodes[2].ChildNodes[0]), opcionAritmetica.DIVIDIR, opcionA);
                    else if (opcionA == "%")
                        return new Aritmetica(actual.ChildNodes[1].Token.Location.Line + 1, actual.ChildNodes[1].Token.Location.Column + 1,
                            (Expresion)construirNodo(actual.ChildNodes[0].ChildNodes[0]), (Expresion)construirNodo(actual.ChildNodes[2].ChildNodes[0]), opcionAritmetica.RESTO, opcionA);

                }
                else if (actual.ChildNodes.Count == 2)
                {
                    return new AritmeticaUnaria(actual.ChildNodes[0].Token.Location.Line + 1, actual.ChildNodes[0].Token.Location.Column + 1,
                        (Expresion)construirNodo(actual.ChildNodes[1].ChildNodes[0]));
                }

            }
            else if (nodeNameIsEqual(actual, "EXPREL") || nodeNameIsEqual(actual, "E_REL"))
            {
                string opcionR = getLexemeNode(actual.ChildNodes[1]);

                if (opcionR == "<")
                    return new Relacional(actual.ChildNodes[1].Token.Location.Line + 1, actual.ChildNodes[1].Token.Location.Column + 1,
                        (Expresion)construirNodo(actual.ChildNodes[0].ChildNodes[0]), (Expresion)construirNodo(actual.ChildNodes[2].ChildNodes[0]), opcionRelacional.MENOR, opcionR);
                else if (opcionR == "<=")
                    return new Relacional(actual.ChildNodes[1].Token.Location.Line + 1, actual.ChildNodes[1].Token.Location.Column + 1,
                        (Expresion)construirNodo(actual.ChildNodes[0].ChildNodes[0]), (Expresion)construirNodo(actual.ChildNodes[2].ChildNodes[0]), opcionRelacional.MENORIGUAL, opcionR);
                else if (opcionR == ">")
                    return new Relacional(actual.ChildNodes[1].Token.Location.Line + 1, actual.ChildNodes[1].Token.Location.Column + 1,
                        (Expresion)construirNodo(actual.ChildNodes[0].ChildNodes[0]), (Expresion)construirNodo(actual.ChildNodes[2].ChildNodes[0]), opcionRelacional.MAYOR, opcionR);
                else if (opcionR == ">=")
                    return new Relacional(actual.ChildNodes[1].Token.Location.Line + 1, actual.ChildNodes[1].Token.Location.Column + 1,
                        (Expresion)construirNodo(actual.ChildNodes[0].ChildNodes[0]), (Expresion)construirNodo(actual.ChildNodes[2].ChildNodes[0]), opcionRelacional.MAYORIGUAL, opcionR);
                else if (opcionR == "=")
                    return new Relacional(actual.ChildNodes[1].Token.Location.Line + 1, actual.ChildNodes[1].Token.Location.Column + 1,
                        (Expresion)construirNodo(actual.ChildNodes[0].ChildNodes[0]), (Expresion)construirNodo(actual.ChildNodes[2].ChildNodes[0]), opcionRelacional.IGUAL, opcionR);
                else if (opcionR == "<>")
                    return new Relacional(actual.ChildNodes[1].Token.Location.Line + 1, actual.ChildNodes[1].Token.Location.Column + 1,
                        (Expresion)construirNodo(actual.ChildNodes[0].ChildNodes[0]), (Expresion)construirNodo(actual.ChildNodes[2].ChildNodes[0]), opcionRelacional.DIFERENCIACION, opcionR);

            }
            else if (nodeNameIsEqual(actual, "EXPLOG") || nodeNameIsEqual(actual, "E_LOG"))
            {
                if (actual.ChildNodes.Count == 3)
                {
                    string opcionL = getLexemeNode(actual.ChildNodes[1]);

                    if (opcionL == "AND")
                        return new Logica(actual.ChildNodes[1].Token.Location.Line + 1, actual.ChildNodes[1].Token.Location.Column + 1,
                            (Expresion)construirNodo(actual.ChildNodes[0].ChildNodes[0]), (Expresion)construirNodo(actual.ChildNodes[2].ChildNodes[0]), opcionLogica.AND, opcionL);
                    else if (opcionL == "OR")
                        return new Logica(actual.ChildNodes[1].Token.Location.Line + 1, actual.ChildNodes[1].Token.Location.Column + 1,
                            (Expresion)construirNodo(actual.ChildNodes[0].ChildNodes[0]), (Expresion)construirNodo(actual.ChildNodes[2].ChildNodes[0]), opcionLogica.OR, opcionL);

                }
                else if (actual.ChildNodes.Count == 2)
                {
                    return new LogicaUnaria(actual.ChildNodes[0].Token.Location.Line + 1, actual.ChildNodes[0].Token.Location.Column + 1,
                        (Expresion)construirNodo(actual.ChildNodes[1].ChildNodes[0]));
                }

            }
            else if (nodeNameIsEqual(actual, "DATO")|| nodeNameIsEqual(actual, "LITERAL"))
            {
                if (actual.ChildNodes.Count == 1)
                {
                    if (nodeNameIsEqual(actual.ChildNodes[0], "EXP") || nodeNameIsEqual(actual.ChildNodes[0], "E"))
                        return construirNodo(actual.ChildNodes[0].ChildNodes[0]);
                    else if (nodeNameIsEqual(actual.ChildNodes[0], "numeroEntero"))
                        return new Primitivo(actual.ChildNodes[0].Token.Location.Line + 1, actual.ChildNodes[0].Token.Location.Column + 1,
                            int.Parse(getLexemeNode(actual.ChildNodes[0])), tiposPrimitivos.INT);
                    else if (nodeNameIsEqual(actual.ChildNodes[0], "numeroDecimal"))
                        return new Primitivo(actual.ChildNodes[0].Token.Location.Line + 1, actual.ChildNodes[0].Token.Location.Column + 1,
                            float.Parse(getLexemeNode(actual.ChildNodes[0]), CultureInfo.GetCultureInfo("en-US")), tiposPrimitivos.REAL);
                    else if (nodeNameIsEqual(actual.ChildNodes[0], "cadena"))
                        return new Primitivo(actual.ChildNodes[0].Token.Location.Line + 1, actual.ChildNodes[0].Token.Location.Column + 1,
                            getLexemeNode(actual.ChildNodes[0]), tiposPrimitivos.STRING);
                    else if (nodeNameIsEqual(actual.ChildNodes[0], "true") || nodeNameIsEqual(actual.ChildNodes[0], "false"))
                        return new Primitivo(actual.ChildNodes[0].Token.Location.Line + 1, actual.ChildNodes[0].Token.Location.Column + 1,
                            bool.Parse(getLexemeNode(actual.ChildNodes[0])), tiposPrimitivos.BOOLEAN);
                    else
                    {
                        List<expPosArr> accesos = new List<expPosArr>();
                        foreach (ParseTreeNode hijo in actual.ChildNodes[0].ChildNodes)
                        {
                            accesos.Add((expPosArr)construirNodo(hijo));
                        }
                        return new Acceso(actual.ChildNodes[0].ChildNodes[0].ChildNodes[0].Token.Location.Line + 1, 0,
                            accesos);
                    }
                }
                else if (actual.ChildNodes.Count == 2)
                {
                    if (nodeNameIsEqual(actual.ChildNodes[1], "POSICIONES"))
                    {
                        List<Expresion> posicion = new List<Expresion>();
                        foreach (ParseTreeNode hijo in actual.ChildNodes[1].ChildNodes)
                        {
                            posicion.Add((Expresion)construirNodo(hijo.ChildNodes[1].ChildNodes[0]));
                        }
                        expPosArr nvo;
                        nvo.id = getLexemeNode(actual.ChildNodes[0]);
                        nvo.posicion = posicion;
                        List<expPosArr> accesos = new List<expPosArr>();
                        accesos.Add(nvo);

                        return new Acceso(actual.ChildNodes[0].Token.Location.Line + 1, actual.ChildNodes[0].Token.Location.Column + 1,
                            accesos);

                    }
                    else if (nodeNameIsEqual(actual.ChildNodes[1], "VALSPARAMS"))
                    {
                        List<Expresion> vals = new List<Expresion>();

                        foreach (ParseTreeNode valores in actual.ChildNodes[1].ChildNodes)
                        {
                            vals.Add((Expresion)construirNodo(valores.ChildNodes[0]));
                        }

                        return new LlamadaFunc(actual.ChildNodes[0].Token.Location.Line + 1, actual.ChildNodes[0].Token.Location.Column + 1,
                            getLexemeNode(actual.ChildNodes[0]), vals);
                    }
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
