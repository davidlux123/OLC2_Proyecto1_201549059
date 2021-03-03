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
            CommentTerminal comentarioMultilinea = new CommentTerminal("comentarioMultilinea", "/*", "*/");
            base.NonGrammarTerminals.Add(comentarioMultilinea);
            #endregion

            #region DEFICION DE TERMINALES

            //simbolos 
            Terminal parDer = ToTerm(")"),
            parIzq = ToTerm("("),
            pntComa = ToTerm(";"),
            pnt = ToTerm("."),
            dosPnts = ToTerm(":"),
            asignacion = ToTerm(":="),

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
            t_not = ToTerm("NOT"),
            t_and = ToTerm("AND"),
            t_or = ToTerm("OR"),
            t_true = ToTerm("true"),
            t_false = ToTerm("false"),
            t_program = ToTerm("program"),
            t_begin = ToTerm("begin"),
            t_end = ToTerm("end"),
            t_var = ToTerm("var"),
            t_const = ToTerm("const"),
            t_integer = ToTerm("integer"),
            t_real = ToTerm("real"),
            t_boolean = ToTerm("boolean"),
            t_string = ToTerm("string");

            #endregion

            #region DECLARAR PALABRAS RESERVADAS
            /*PALABRAS RESERVADAS LOGICAS*/
            MarkReservedWords("true", "flase", "NOT", "AND", "OR");
            /*PALABRAS RESERVADAS*/
            MarkReservedWords("write", "writeln", "program", "begin", "end", "integer", "real", "boolean", "string");
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

            //NonTerminal STRUCT = new NonTerminal("STRUCT");
            //NonTerminal PROCFUNC = new NonTerminal("PROCFUNC");
            NonTerminal DECLARACION = new NonTerminal("DECLARACION");
            NonTerminal CONSTANTES = new NonTerminal("CONSTANTES");
            NonTerminal CONSTANTE = new NonTerminal("CONSTANTE");
            NonTerminal VARIABLES = new NonTerminal("VARIABLES");
            NonTerminal VARIABLE = new NonTerminal("VARIABLE");
            NonTerminal LISTAIDS = new NonTerminal("LISTAIDS");
            NonTerminal TIPO = new NonTerminal("TIPO");
            NonTerminal BLOQUE = new NonTerminal("BLOQUE");

            NonTerminal INSTSLOCAL = new NonTerminal("INSTSLOCAL");
            NonTerminal INSTLOCAL = new NonTerminal("INSTLOCAL");
            NonTerminal WRITE = new NonTerminal("WRITE");
            NonTerminal ASIG = new NonTerminal("ASIG");

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

            //INSTRUCCIONES GLOBALES
            INSTSGLOBAL.Rule = MakeStarRule(INSTSGLOBAL, INSTGLOBAL);
            INSTGLOBAL.Rule = DECLARACION;
                          //| STRUCT
                          //| PROCFUNC;

            /*declaracion de constantes y variables*/
            DECLARACION.Rule = t_const + CONSTANTES
                             | t_var + VARIABLES;

            /*declaracion de constantes*/
            CONSTANTES.Rule = MakePlusRule(CONSTANTES, CONSTANTES);
            CONSTANTE.Rule = id + igual + EXP + pntComa;
            CONSTANTE.ErrorRule = SyntaxError + pntComa;

            /*declaracion de varables*/
            VARIABLES.Rule = MakePlusRule(VARIABLES, VARIABLE);
            VARIABLE.Rule = LISTAIDS + dosPnts + TIPO + pntComa
                        | id + dosPnts + TIPO + igual + EXP + pntComa;
            CONSTANTE.ErrorRule = SyntaxError + pntComa;

            /*listado de ids*/
            LISTAIDS.Rule = MakePlusRule(LISTAIDS, id);

            /*deficion de tipos*/
            TIPO.Rule = t_integer
                      | t_real
                      | t_boolean
                      | t_string;

            //BLOQUE QUE CONTIENE LAS INSTRUCCIONES LOCALES
            BLOQUE.Rule = t_begin + INSTSLOCAL + t_end;

            /*Instrucciones locales */
            INSTSLOCAL.Rule = MakePlusRule(INSTSLOCAL, INSTLOCAL);
            INSTLOCAL.Rule = WRITE + pntComa
                           | ASIG + pntComa;
            INSTLOCAL.ErrorRule = SyntaxError + pntComa;

            /*asignaciones*/
            ASIG.Rule = id + asignacion + E;

            /*write o write*/
            WRITE.Rule = t_write + parIzq + E + parDer
                       | t_writeln + parIzq + E + parDer
                       | t_write + parIzq + parDer
                       | t_writeln + parIzq + parDer;

            //EXPRESIONES LITERALES
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


            /*tipo de datos literales para todo tipo de intruccion 'locales'*/
            LITERAL.Rule = parIzq + E + parDer
                      | numeroEntero
                      | numeroDecimal
                      | cadena
                      | t_true
                      | t_false
                      | id;

            #region EXP PRIMITIVOS

            //EXPRESIONES PRIMITIVOS
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

            /*tipo de datos primitivos, para declaraciones y demas*/
            DATO.Rule = parIzq + EXP + parDer
                      | numeroEntero
                      | numeroDecimal
                      | cadena
                      | t_true
                      | t_false;

            #endregion


            #endregion

            #region PREFERENCIAS
            MarkPunctuation(parIzq, parDer, pntComa, pnt, dosPnts);
            this.Root = INIT;
            #endregion

        }


    }
}
