using _OLC2_Proyecto1.src.Expresiones;
using _OLC2_Proyecto1.src.Interfaces;
using System;
using System.Collections;
using System.Text;

namespace _OLC2_Proyecto1.src.Ambientes
{
    public class Entorno
    {
        private Entorno anterior;
        private string nombreAmbito;
        private Hashtable variables;

        public Entorno(Entorno anterior, string nombreAmbito)
        {
            this.anterior = anterior;
            this.nombreAmbito = nombreAmbito;
            variables = new Hashtable();
        }

        public void addVariable(object valor, string id, tiposPrimitivos type, int linea, int columna, bool constante)
        { 
            this.variables.Add(id, new Simbolo(valor, id, type, this.nombreAmbito, linea, columna, constante));
        }
        public bool existeID(string id)
        {
            Entorno Entaux = this;
            while (Entaux != null)
            {
                if (Entaux.variables.Contains(id))
                {
                    return true;
                }
                Entaux = Entaux.anterior;
            }
            return false;
        }
        public Simbolo getVariable(string id)
        {
            Entorno Entaux = this;
            while (Entaux != null)
            {
                if (Entaux.variables.Contains(id))
                {
                     return (Simbolo)this.variables[id];
                }
                Entaux = Entaux.anterior;
            }

            return null;
        }
        public void actualizaVariable(string id, Simbolo nvoSimb)
        {
            Entorno Entaux = this;
            while (Entaux != null)
            {
                if (this.variables.Contains(id))
                {
                    this.variables[id] = nvoSimb;
                    return;
                }
                Entaux = Entaux.anterior;
            }

        }

    }
}
