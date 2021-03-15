using _OLC2_Proyecto1.src.Expresiones;
using _OLC2_Proyecto1.src.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace _OLC2_Proyecto1.src.Ambientes
{
    public class Entorno
    {
        private Entorno anterior;
        public string nombreAmbito { get; set; }
        private Hashtable variables;

        public Entorno(Entorno anterior, string nombreAmbito)
        {
            this.anterior = anterior;
            this.nombreAmbito = nombreAmbito;
            variables = new Hashtable();
        }

        public Entorno(Entorno anterior)
        {
            this.anterior = anterior;
            this.variables = new Hashtable();
        }

        public void addVariable(object valor, string id, tiposPrimitivos type, int linea, int columna, bool constante)
        { 
            this.variables.Add(id, new Simbolo(id, valor, type,  linea, columna, constante));
        }
        public void addVariable(string id, Simbolo sim)
        {
            this.variables.Add(id, sim);
        }
        public bool existeID(string id)
        {
            return this.variables.Contains(id);
        }
        public bool existeIDGlobal(string id)
        {
            Entorno Entaux = this;
            while (Entaux.anterior != null)
                Entaux = Entaux.anterior;

            return this.variables.Contains(id);
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
        public string getAllVariables()
        {
            string variables = "";

            Entorno entActual = this;
            while (entActual != null)
            {
                foreach (DictionaryEntry var in this.variables)
                {
                    Simbolo variable = (Simbolo)var.Value;

                    variables += "<tr>\n" +
                    "\t<td>" + var.Key + "</td>\n" +
                    "\t<td>" + variable.type.ToString() + "</td>\n" +
                    "\t<td>" + this.nombreAmbito + "</td>\n" +
                    "\t<td>" + variable.linea + "</td>\n" +
                    "\t<td>" + variable.columna + "</td>\n" +
                    "</tr>\n\n";
                }
                entActual = entActual.anterior;
            }

            return variables;

        }
       
    }
}
