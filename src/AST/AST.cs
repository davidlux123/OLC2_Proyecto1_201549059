using System;
using System.Collections.Generic;
using System.Text;
using _OLC2_Proyecto1.src.Interfaces;

namespace _OLC2_Proyecto1.src
{
    class AST
    {
        public LinkedList<Instruccion> Instrucciones { set; get; }

        public AST(LinkedList<Instruccion> instrucciones)
        {
            this.Instrucciones = instrucciones;
        }
    }
}
