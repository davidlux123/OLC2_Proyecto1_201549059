using Irony.Parsing;
using System;
using System.Collections.Generic;
using System.Text;

namespace _OLC2_Proyecto1.src.Gramatica
{
    class Gramatica : Grammar
    {
        public Gramatica() : base(true)
        {
            #region ER'S
            //terminales
            RegexBasedTerminal numeroEntero = new RegexBasedTerminal("numeroEntero", "[0-9]+");
            RegexBasedTerminal numeroDecimal = new RegexBasedTerminal("numeroDecimal", "[0-9]+[.][0-9]+");
            IdentifierTerminal id = new IdentifierTerminal("id");
            StringLiteral cadena = new StringLiteral("cadena", "\'", StringOptions.IsTemplate);

            //comentarios
            CommentTerminal comentarioUnilinea = new CommentTerminal("comentarioUnilinea", "//", "\n", "\r\n");
            base.NonGrammarTerminals.Add(comentarioUnilinea);
            CommentTerminal comentarioMultilinea = new CommentTerminal("comentarioMultilinea", "(*", "*)");
            base.NonGrammarTerminals.Add(comentarioMultilinea);
            CommentTerminal comentarioMultilinea2 = new CommentTerminal("comentarioMultilinea", "{", "}");
            base.NonGrammarTerminals.Add(comentarioMultilinea2);
            #endregion

            #region DEFICION DE TERMINALES

            //simbolos 
            Terminal parIzq = ToTerm("("),
            parDer = ToTerm(")"),
            pntComa = ToTerm(";"),
            pnt = ToTerm("."),
            dosPnts = ToTerm(":"),
            asignacion = ToTerm(":="),
            coma = ToTerm(","),
            corchIzq = ToTerm("["),
            corchDer = ToTerm("]"),

            //simbolos aritmrticos
            por = ToTerm("*"),
            div = ToTerm("/"),
            modulo = ToTerm("%"),
            menos = ToTerm("-"),
            mas = ToTerm("+"),

            //simbolos relacionales
            igual = ToTerm("="),
            diferente = ToTerm("<>"),
            menorIgual = ToTerm("<="),
            menor = ToTerm("<"),
            mayorIgual = ToTerm(">="),
            mayor = ToTerm(">"),

            //palabras reservadas
            t_write = ToTerm("write"),
            t_writeln = ToTerm("writeln"),
            t_TS = ToTerm("graficar_ts"),
            t_not = ToTerm("not"),
            t_and = ToTerm("and"),
            t_or = ToTerm("or"),
            t_true = ToTerm("true"),
            t_false = ToTerm("false"),
            t_program = ToTerm("Program"),
            t_begin = ToTerm("begin"),
            t_end = ToTerm("end"),
            t_integer = ToTerm("integer"),
            t_real = ToTerm("real"),
            t_boolean = ToTerm("boolean"),
            t_string = ToTerm("string"),
            t_var = ToTerm("var"),
            t_const = ToTerm("const"),
            t_type = ToTerm("type"),
            t_object = ToTerm("object"),
            t_array = ToTerm("array"),
            t_of = ToTerm("of"),
            t_porc = ToTerm("procedure"),
            t_func = ToTerm("function"),
            t_if = ToTerm("if"),
            t_else = ToTerm("else"),
            t_then = ToTerm("then"),
            t_fot = ToTerm("for"),
            t_while = ToTerm("while"),
            t_to = ToTerm("to"),
            t_do = ToTerm("do"),
            t_repeat = ToTerm("repeat"),
            t_until = ToTerm("until");


            #endregion

            #region DECLARAR PALABRAS RESERVADAS
            /*PALABRAS RESERVADAS*/
            MarkReservedWords("write", "writeln", "graficar_ts", "not", "and", "or", "true", "flase", "Program", "begin", "end", "integer",
                "real", "boolean", "string", "var", "const", "type", "object", "array", "of", "procedure", "function", "if", "else", "then",
                "for","while", "to", "do", "repeat", "until");
            #endregion

            #region PRECEDENCIA DE OPERADORES
            RegisterOperators(1, Associativity.Left, t_or);
            RegisterOperators(2, Associativity.Left, t_and);
            RegisterOperators(3, Associativity.Left, igual, diferente);
            RegisterOperators(4, Associativity.Left, menor, menorIgual, mayor, mayorIgual);
            RegisterOperators(5, Associativity.Left, mas, menos);
            RegisterOperators(6, Associativity.Left, por, div, modulo);
            #endregion

            #region DEFICION DE NO TERMINALES

            NonTerminal INIT = new NonTerminal("INIT");

            NonTerminal INSTSGLOBAL = new NonTerminal("INSTSGLOBAL");
            NonTerminal INSTGLOBAL = new NonTerminal("INSTGLOBAL");
            NonTerminal DECLARACIONTYPE = new NonTerminal("DECLARACIONTYPE");
            NonTerminal DEFTYPES = new NonTerminal("DEFTYPES");
            NonTerminal DEFTYPE = new NonTerminal("DEFTYPE");
            NonTerminal TYPE = new NonTerminal("TYPE");

            NonTerminal ARRAY = new NonTerminal("ARRAY");
            NonTerminal DIMENSIONES = new NonTerminal("DIMENSIONES");
            NonTerminal DIMENSION = new NonTerminal("DIMENSION");

            NonTerminal STRUCT = new NonTerminal("STRUCT");
            NonTerminal VARS_STRUCT = new NonTerminal("VARS_STRUCT");
            NonTerminal VAR = new NonTerminal("VAR");

            NonTerminal DECLARACION = new NonTerminal("DECLARACION");
            NonTerminal CONSTANTES = new NonTerminal("CONSTANTES");
            NonTerminal CONSTANTE = new NonTerminal("CONSTANTE");
            NonTerminal VARIABLES = new NonTerminal("VARIABLES");
            NonTerminal VARIABLE = new NonTerminal("VARIABLE");
            NonTerminal LISTAIDS = new NonTerminal("LISTAIDS");
            NonTerminal TIPO = new NonTerminal("TIPO");
            NonTerminal BLOQUE = new NonTerminal("BLOQUE");

            NonTerminal PROCFUNC = new NonTerminal("PROCFUNC");
            NonTerminal PARAMS = new NonTerminal("PARAMS");
            NonTerminal PARAM = new NonTerminal("PARAM");
            NonTerminal DECLARACIONES = new NonTerminal("DECLARACIONES");

            NonTerminal INSTSLOCAL = new NonTerminal("INSTSLOCAL");
            NonTerminal INSTLOCAL = new NonTerminal("INSTLOCAL");
            NonTerminal TS = new NonTerminal("TS");
            NonTerminal WRITE = new NonTerminal("WRITE");
            NonTerminal ASIG = new NonTerminal("ASIG");
            NonTerminal ACCESOS = new NonTerminal("ACCESOS");
            NonTerminal ACCESO = new NonTerminal("ACCESO");
            NonTerminal POSICIONES = new NonTerminal("POSICIONES");
            NonTerminal POSICION = new NonTerminal("POSICION");

            NonTerminal IF = new NonTerminal("IF");
            NonTerminal ELSIF = new NonTerminal("ELSIF");
            NonTerminal LIST_ELSIF = new NonTerminal("LIST_ELSIF");
            NonTerminal ELSE = new NonTerminal("ELSE");

            NonTerminal FOR = new NonTerminal("FOR");
            NonTerminal WHILE = new NonTerminal("WHILE");
            NonTerminal REPEAT = new NonTerminal("REPEAT");

            NonTerminal LLAMADA = new NonTerminal("LLAMADA");
            NonTerminal VALSPARAMS = new NonTerminal("VALSPARAMS");

            NonTerminal E = new NonTerminal("E");
            NonTerminal E_ARIT = new NonTerminal("E_ARIT");
            NonTerminal E_LOG = new NonTerminal("E_LOG");
            NonTerminal E_REL = new NonTerminal("E_REL");
            NonTerminal LITERAL = new NonTerminal("LITERAL");


            NonTerminal EXP = new NonTerminal("EXP");
            NonTerminal EXPARIT = new NonTerminal("EXPARIT");
            NonTerminal EXPLOG = new NonTerminal("EXPLOG");
            NonTerminal EXPREL = new NonTerminal("EXPREL");
            NonTerminal DATO = new NonTerminal("DATO");

            #endregion

            #region DEFINICION DE GRAMATICA
            //CUERPO DE PROGRAM
            INIT.Rule = t_program + id + pntComa + INSTSGLOBAL + BLOQUE + pnt;

            //                                  INSTRUCCIONES GLOBALES
            INSTSGLOBAL.Rule = MakeStarRule(INSTSGLOBAL, INSTGLOBAL);
            INSTGLOBAL.Rule = DECLARACIONTYPE
                            | DECLARACION
                            | PROCFUNC;

            /*declaracion de Types*/
            DECLARACIONTYPE.Rule = t_type + DEFTYPES;

            /*definicion de Types*/
            DEFTYPES.Rule = MakePlusRule(DEFTYPES, DEFTYPE);
            DEFTYPE.Rule = id + igual + TYPE + pntComa;
            DEFTYPE.ErrorRule = SyntaxError + pntComa;
           
            TYPE.Rule = ARRAY
                      | STRUCT
                      | TIPO;

            /*declaracion de ARREGLOS*/
            ARRAY.Rule = t_array + corchIzq + DIMENSIONES + corchDer + t_of + TIPO;
            DIMENSIONES.Rule = MakePlusRule(DIMENSIONES, coma, DIMENSION);
            DIMENSION.Rule = EXP + pnt + pnt + EXP;

            /*declaracion de STRUCTS*/
            STRUCT.Rule = t_object + t_var + VARS_STRUCT + t_end;

            /*declaracion de varables del struct*/
            VARS_STRUCT.Rule = MakePlusRule(VARS_STRUCT, VAR);
            VAR.Rule = LISTAIDS + dosPnts + TIPO + pntComa
                     | LISTAIDS + dosPnts + ARRAY + pntComa;


            /*lista de declaraciones de CONSTANTES y ARREGLOS*/
            DECLARACION.Rule = t_const + CONSTANTES
                             | t_var + VARIABLES;

            /*declaracion de constantes con su asignacion*/
            CONSTANTES.Rule = MakePlusRule(CONSTANTES, CONSTANTE);
            CONSTANTE.Rule = id + igual + EXP + pntComa;
            CONSTANTE.ErrorRule = SyntaxError + pntComa;

            /*declaracion de varables CON Y SIN ASIGNACION*/
            VARIABLES.Rule = MakePlusRule(VARIABLES, VARIABLE);
            VARIABLE.Rule = id + dosPnts + TIPO + igual + EXP + pntComa
                          | id + dosPnts + TIPO + pntComa
                          | id + dosPnts + ARRAY + pntComa
                          | LISTAIDS + dosPnts + TIPO + pntComa
                          | LISTAIDS + dosPnts + ARRAY + pntComa; 
            VARIABLE.ErrorRule = SyntaxError + pntComa;

            /*LISTADO IDs*/
            LISTAIDS.Rule = MakePlusRule(LISTAIDS, coma, id);

            /*deficion de tipos*/
            TIPO.Rule = t_integer
                      | t_real
                      | t_boolean
                      | t_string
                      | id;

            //                                   METODOS Y FUNCIONES
            PROCFUNC.Rule = t_porc + id + parIzq + PARAMS + parDer + pntComa + DECLARACIONES + BLOQUE + pntComa 
                          | t_func + id + parIzq + PARAMS + parDer + dosPnts + TIPO + pntComa + DECLARACIONES + BLOQUE + pntComa;

            /*parametros*/
            PARAMS.Rule = MakeStarRule(PARAMS, coma ,PARAM);
            PARAM.Rule = LISTAIDS + dosPnts + TIPO
                       | t_var + id + dosPnts + TIPO;

            /*declaraciones de variables de metodos*/
            DECLARACIONES.Rule = MakeStarRule(DECLARACIONES, DECLARACION);

            /*'statament bloque de sentencias'*/
            BLOQUE.Rule = t_begin + INSTSLOCAL + t_end;

            //                                   INSTRUCCIONES LOCALES
            INSTSLOCAL.Rule = MakePlusRule(INSTSLOCAL, INSTLOCAL);
            INSTLOCAL.Rule = WRITE + pntComa
                           | ASIG + pntComa
                           | TS + pntComa
                           | IF
                           | FOR + pntComa
                           | WHILE + pntComa
                           | REPEAT + pntComa
                           | LLAMADA;

            INSTLOCAL.ErrorRule = SyntaxError + pntComa;

            /*graficas TS*/
            TS.Rule = t_TS + parIzq + parDer;

            /*write o write*/
            WRITE.Rule = t_write + parIzq + E + parDer
                       | t_writeln + parIzq + E + parDer
                       | t_write + parIzq + parDer
                       | t_writeln + parIzq + parDer;

            /*if*/
            IF.Rule = t_if + parIzq + E + parDer + t_then + BLOQUE + LIST_ELSIF + ELSE;
            LIST_ELSIF.Rule = MakeStarRule(LIST_ELSIF, ELSIF);
            ELSIF.Rule = t_else + t_if + parIzq + E + parDer + t_then + BLOQUE;
            ELSE.Rule = t_else + BLOQUE
                      | pntComa;
            /*for*/
            FOR.Rule = t_fot + id + asignacion + E + t_to + E + t_do + BLOQUE;

            /*while*/
            WHILE.Rule = t_while + parIzq + E + parDer + t_do + BLOQUE;

            REPEAT.Rule = t_repeat + BLOQUE + t_until + E;

            /*llamada de metodo*/
            LLAMADA.Rule = id + parIzq + VALSPARAMS + parDer;
            VALSPARAMS.Rule = MakeStarRule(VALSPARAMS, coma , E);


            /*asignaciones*/
            ASIG.Rule = ACCESOS + asignacion + E;
            ACCESOS.Rule = MakeStarRule(ACCESOS, pnt, ACCESO);
            ACCESO.Rule = id
                        | id + POSICIONES;

            /*Definicion de posicion de un arreglo*/
            POSICIONES.Rule = MakePlusRule(POSICIONES, POSICION);
            POSICION.Rule = corchIzq + E + corchDer;


            //                                      EXPRESIONES LITERALES
            E.Rule = E_ARIT
                   | E_REL
                   | E_LOG
                   | LITERAL;

            /*Expresiones aritmeticas*/
            E_ARIT.Rule = E + mas + E
                         | E + menos + E
                         | E + por + E
                         | E + div + E
                         | E + modulo + E
                         | menos + E;//unaria

            /*Expresiones relacionales*/
            E_REL.Rule = E + menor + E
                        | E + menorIgual + E
                        | E + mayor + E
                        | E + mayorIgual + E
                        | E + igual + E
                        | E + diferente + E;

            /*Expresiones logicas*/
            E_LOG.Rule = E + t_and + E
                       | E + t_or + E
                       | t_not + E;//unaria


            /*tipo de datos literales para todo tipo de intrucciones locales*/
            LITERAL.Rule = parIzq + E + parDer
                      | numeroEntero
                      | numeroDecimal
                      | cadena
                      | t_true
                      | t_false
                      | id
                      | id + POSICIONES
                      | id + parIzq + VALSPARAMS + parDer
                      | ACCESOS;

            #region EXPRESIONES PRIMITIVAS

            EXP.Rule = EXPARIT
                     | EXPREL
                     | EXPLOG
                     | DATO;

            /*Expresiones aritmeticas*/
            EXPARIT.Rule = EXP + mas + EXP
                         | EXP + menos + EXP
                         | EXP + por + EXP
                         | EXP + div + EXP
                         | EXP + modulo + EXP
                         | menos + EXP;//unaria

            /*Expresiones relacionales*/
            EXPREL.Rule = EXP + menor + EXP
                        | EXP + menorIgual + EXP
                        | EXP + mayor + EXP
                        | EXP + mayorIgual + EXP
                        | EXP + igual + EXP
                        | EXP + diferente + EXP;

            /*Expresiones logicas*/
            EXPLOG.Rule = EXP + t_and + EXP
                        | EXP + t_or + EXP
                        | t_not + EXP;//unaria

            /*tipo de datos primitivos, para declaraciones*/
            DATO.Rule = parIzq + EXP + parDer
                      | numeroEntero
                      | numeroDecimal
                      | cadena
                      | t_true
                      | t_false
                      | id;

            #endregion

            #endregion

            #region PREFERENCIAS
            MarkPunctuation(parIzq, parDer, coma, pntComa);
            this.Root = INIT;
            #endregion

        }


    }
}
