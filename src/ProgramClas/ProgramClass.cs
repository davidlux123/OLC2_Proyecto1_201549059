using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using _OLC2_Proyecto1.src.Expresiones;
using _OLC2_Proyecto1.src.Instrucciones.InstGlobales;
using _OLC2_Proyecto1.src.Interfaces;

namespace _OLC2_Proyecto1.src
{
    public class ProgramClass
    {
        private Hashtable Types;
        private Hashtable ProcFunc;

        public ProgramClass()
        {
            this.Types = new Hashtable();
            this.ProcFunc = new Hashtable();
        }

        
        public void addType(string idType, object nvoType)
        {
            Types.Add(idType, nvoType);
        }
        public bool existeIDProcFunc(string id)
        {
            if (ProcFunc.Contains(id))
            {
                return true;
            }
            return false;
        }
        public bool existeIDtypes(string id)
        {
            if (Types.Contains(id))
            {
                return true;
            }
            return false;
        }
        public object getValorType(string id)
        { 
            if (this.Types.Contains(id))
            {
                object typeFind = this.Types[id];

                if (typeFind is Struct)
                {
                    Struct structFind = (Struct)typeFind;
                    return structFind.Clone();
                } 
                else if (typeFind is tiposPrimitivos.INT)
                {
                    return int.Parse("0");
                }
                else if (typeFind is tiposPrimitivos.REAL)
                {
                    return float.Parse("0.0", CultureInfo.GetCultureInfo("en-US"));
                }
                else if (typeFind is tiposPrimitivos.STRING)
                {
                    return "";
                }
                else if (typeFind is tiposPrimitivos.BOOLEAN)
                {
                    return bool.Parse("false");
                } 
            }

            return null;
        } 
        public tiposPrimitivos getType(string id)
        {
            if (this.Types.Contains(id))
            {
                object typeFind = this.Types[id];

                if (typeFind is Struct)
                {
                    return tiposPrimitivos.STRUCT;
                }
                else if (typeFind is tiposPrimitivos.INT)
                {
                    return tiposPrimitivos.INT;
                }
                else if (typeFind is tiposPrimitivos.REAL)
                {
                    return tiposPrimitivos.REAL;
                }
                else if (typeFind is tiposPrimitivos.STRING)
                {
                    return tiposPrimitivos.STRING;
                }
                else //if (typeFind is tiposPrimitivos.BOOLEAN)
                {
                    return tiposPrimitivos.BOOLEAN;
                }
            }
            else
            {
                return tiposPrimitivos.error;
            }
        }
    }
}
