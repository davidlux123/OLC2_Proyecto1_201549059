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
            RegexBasedTerminal numeroDecimal = new RegexBasedTerminal("numeroDecimal","[0-9]+[.][0-9]+");
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
            write = ToTerm("write"),
            writeln = ToTerm("writeln"),
            not = ToTerm("NOT"),
            and = ToTerm("AND"),
            or = ToTerm("OR"),
            True = ToTerm("true"),
            False = ToTerm("false"),
            program = ToTerm("program"),
            begin = ToTerm ("begin"),
            end = ToTerm("end");

            #endregion

            #region DECLARAR PALABRAS RESERVADAS
            /*PALABRAS RESERVADAS LOGICAS*/
            MarkReservedWords("true", "flase", "NOT", "AND", "OR");
            /*PALABRAS RESERVADAS*/
            MarkReservedWords("write", "writeln", "program", "begin", "end");
            #endregion

            #region PRECEDENCIA DE OPERADORES
            RegisterOperators(1, Associativity.Left, or);
            RegisterOperators(2, Associativity.Left, and);
            RegisterOperators(3, Associativity.Left, igual, diferente);
            RegisterOperators(4, Associativity.Left, menor, menorIgual, mayor, mayorIgual);
            RegisterOperators(5, Associativity.Left, mas, menos);
            RegisterOperators(6, Associativity.Left, por, div, modulo);
            #endregion


            #region DEFICION DE NO TERMINALES

            NonTerminal INIT = new NonTerminal("INIT");
            NonTerminal INSTRUCCION = new NonTerminal("INSTRUCCION");
            NonTerminal INSTRUCCIONES = new NonTerminal("INSTRUCCIONES");
            NonTerminal WRITE = new NonTerminal("WRITE");
            NonTerminal EXP = new NonTerminal("EXP");
            NonTerminal EXPARIT = new NonTerminal("EXPARIT");
            NonTerminal EXPLOG = new NonTerminal("EXPLOG");
            NonTerminal EXPREL = new NonTerminal("EXPREL");
            NonTerminal DATO = new NonTerminal("DATO");
            #endregion

            #region DEFINICION DE GRAMATICA



            INIT.Rule = program + id + pntComa + begin + INSTRUCCIONES + end + pnt;
            INSTRUCCIONES.Rule = MakePlusRule(INSTRUCCIONES, INSTRUCCION);

            INSTRUCCION.Rule = /*instrucciones*/
                        WRITE + pntComa;
            INSTRUCCION.ErrorRule = SyntaxError + ";";

            WRITE.Rule = write + parIzq + EXP + parDer
                       | writeln + parIzq + EXP + parDer
                       | write + parIzq + parDer
                       | writeln + parIzq + parDer;

            EXP.Rule = /*Expresiones en general*/
                       EXPARIT
                     | EXPREL
                     | EXPLOG

                     | DATO;

            EXPARIT.Rule = /*Expresiones aritmeticas*/
                       EXP + mas + EXP
                     | EXP + menos + EXP
                     | EXP + por + EXP
                     | EXP + div + EXP
                     | EXP + modulo + EXP
                     | menos + EXP;//unaria

            EXPREL.Rule = /*Expresiones relacionales*/
                       EXP + menor + EXP
                     | EXP + menorIgual + EXP
                     | EXP + mayor + EXP
                     | EXP + mayorIgual + EXP
                     | EXP + igual + EXP
                     | EXP + diferente + EXP;

            EXPLOG.Rule = /*Expresiones logicas*/
                      EXP + and + EXP
                     | EXP + or + EXP
                     | not + EXP;//unaria

            DATO.Rule = /*tipo de datos primitivos*/
                        parIzq + EXP + parDer
                      | numeroEntero
                      | numeroDecimal
                      | cadena
                      | True
                      | False;

            #endregion

            #region PREFERENCIAS
            MarkPunctuation(parIzq, parDer, pntComa, pnt);
            this.Root = INIT;
            #endregion

        }


    }
}
