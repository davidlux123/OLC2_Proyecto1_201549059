using _OLC2_Proyecto1.src.Ambientes;
using _OLC2_Proyecto1.src.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace _OLC2_Proyecto1.src.Instrucciones
{
    class Bloque : Instruccion
    {
        public int line { get ; set; }
        public int column { get ; set ; }

        private List<Instruccion> codigo;
        private string nomAmbito;

        public Bloque(int line, int column, List<Instruccion> codigo, string nomAmbito)
        {
            this.line = line;
            this.column = column;
            this.codigo = codigo;
            this.nomAmbito = nomAmbito;
        }

        public void execute(Entorno ent, ProgramClass ast)
        {
            Entorno nvoEntorno = new Entorno (ent, this.nomAmbito);

            string errores = "";
            foreach (Instruccion inst in this.codigo)
            {
                try
                {
                    inst.execute(nvoEntorno, ast);
                }
                catch (Exception error)
                {
                    errores += error.Message;
                }
            }
            if (errores != "")
                throw new Exception(errores);

        }

    }
}
