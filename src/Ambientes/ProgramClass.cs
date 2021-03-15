using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using _OLC2_Proyecto1.src.Expresiones;
using _OLC2_Proyecto1.src.Instrucciones;
using _OLC2_Proyecto1.src.Interfaces;

namespace _OLC2_Proyecto1.src.Ambientes
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
            this.Types.Add(idType, nvoType);
        }
        public bool existeIDtypes(string id)
        {
            return this.Types.Contains(id);
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
                else if (typeFind is Arreglo)
                {
                    Arreglo arrFind = (Arreglo)typeFind;
                    return arrFind.Clone();
                }
                else
                {
                    return typeFind;
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
                else if (typeFind is Arreglo)
                {
                    return tiposPrimitivos.ARRAEGLO;
                }
                else
                {
                    return (tiposPrimitivos)typeFind;
                }
            }
            return tiposPrimitivos.error;
        }
        //---------------------------------------------------------------------------
        public void addProcFunc(string idTFunc, Funcion nvoFunc)
        {
            this.addProcFunc(idTFunc, nvoFunc);
        }
        public bool existeIDProcFunc(string id)
        {
            return ProcFunc.Contains(id);
        }
        public  Funcion getProcFunc(string id)
        {
            if (this.ProcFunc.Contains(id))
            {
                return (Funcion)this.ProcFunc[id];
            }
            return null;
        }

    }
}
