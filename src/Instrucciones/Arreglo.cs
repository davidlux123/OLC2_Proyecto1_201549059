using _OLC2_Proyecto1.src.Ambientes;
using _OLC2_Proyecto1.src.Expresiones;
using _OLC2_Proyecto1.src.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace _OLC2_Proyecto1.src.Instrucciones
{
    class Arreglo : Instruccion, ICloneable
    {
        public int line { get; set; }
        public int column { get; set; }
        public string idArreglo { get; set;}
        public object[] contenido { get; set; }
        public object typeArrglo { get; set; }
        
        private List<Expresion[]> expsDimensiones;
        public List<int[]> dimensiones { get; set; }


        public Arreglo(int line, int column, object typeArrglo, List<Expresion[]> expsDimensiones)
        {
            this.line = line;
            this.column = column;
            this.idArreglo = "";
            this.typeArrglo = typeArrglo;
            this.expsDimensiones = expsDimensiones;
            this.dimensiones = new List<int[]>();
            
        }

        private Arreglo(int line, int column, string idArreglo, object[] contenido, object typeArrglo, List<int[]> dimensiones)
        {
            this.line = line;
            this.column = column;
            this.idArreglo = idArreglo;
            this.contenido = contenido;
            this.typeArrglo = typeArrglo;
            this.dimensiones = dimensiones;
        }

        public retorno execute(Entorno ent, ProgramClass programClass)
        {
            if (this.typeArrglo is string)
            {
                tiposPrimitivos type = programClass.getType((string)this.typeArrglo);
                if (type != tiposPrimitivos.error)
                {
                    if (type != tiposPrimitivos.STRUCT && type != tiposPrimitivos.ARRAEGLO)
                        this.typeArrglo = type;
                }
                else
                    throw new Exception("<tr>\n" +
                    "\t<td>Error Semantico</td>\n" +
                    "\t<td>No se ah declarado ningun Type con el id " + "'" + this.typeArrglo + "'" + "</td>\n" +
                    "\t<td>" + this.line + "</td>\n" +
                    "\t<td>" + this.column + "</td>\n</tr>\n\n");
            }

            this.dimensiones = validarDimensiones(ent, programClass);
            this.contenido = inicializarRecursivo(0, programClass);

            if (this.idArreglo != "")
            {
                if (!programClass.existeIDtypes(this.idArreglo))
                    programClass.addType(this.idArreglo, this);
                else
                    throw new Exception("<tr>\n" +
                    "\t<td>Error Semantico</td>\n" +
                    "\t<td>El id: '" + this.idArreglo + "' ya esta en uso</td>\n" +
                    "\t<td>" + this.line + "</td>\n" +
                    "\t<td>" + this.column + "</td>\n</tr>\n\n");
            }

            retorno ret;
            ret.value = null;
            ret.type = tiposPrimitivos.VOID;
            return ret;
        }

        private List<int[]> validarDimensiones(Entorno ent, ProgramClass programClass)
        {
            List<int[]> Dimensiones = new List<int[]>();

            foreach (Expresion[] exp in this.expsDimensiones)
            {
                retorno datoIzq = exp[0].getValorSintetizado(ent, programClass);
                retorno datoDer = exp[1].getValorSintetizado(ent, programClass);

                if (datoIzq.type == tiposPrimitivos.INT && datoDer.type == tiposPrimitivos.INT)
                {
                    if ((int)datoIzq.value < (int)datoDer.value)
                    {
                        int dimension = ((int)datoDer.value - (int)datoIzq.value) + 1;
                        int resto = (int)datoIzq.value;

                        int[] dim = { dimension, resto };
                        Dimensiones.Add(dim);
                    }
                    else
                    {
                        throw new Exception("<tr>\n" +
                        "\t<td>Error Semantico</td>\n" +
                        "\t<td>El Indice inferior " + datoIzq + " NO puede ser mayor o igual que el indice superior " + datoDer + "</td>\n" +
                        "\t<td>" + this.line + "</td>\n" +
                        "\t<td>" + this.column + "</td>\n</tr>\n\n");
                    }
                }
                else
                {
                    throw new Exception("<tr>\n" +
                    "\t<td>Error Semantico</td>\n" +
                    "\t<td>Los dos Inidices deben ser de tipo numerico</td>\n" +
                    "\t<td>" + this.line + "</td>\n" +
                    "\t<td>" + this.column + "</td>\n</tr>\n\n");
                }

            }
            return Dimensiones;
        }
        private object[] inicializarRecursivo(int i, ProgramClass programClass)
        {
            object[] arreglo = new object[this.dimensiones[i][0]];

            if (i + 1 < this.dimensiones.Count)
            {
                for (int j = 0; j < this.dimensiones[i][0]; j++)
                {
                    arreglo[j] = inicializarRecursivo(i + 1, programClass);
                }
                return arreglo;
            }
            else
            {
                for (int k = 0; k < arreglo.Length; k++)
                {
                    arreglo[k] = getValorxDefecto(programClass);
                }
                return arreglo;
            }

        }
        private object getValorxDefecto(ProgramClass programClass)
        {
            if (this.typeArrglo is string)
            {
                object clonValor = programClass.getValorType((string)this.typeArrglo);
                if (clonValor != null)
                    if (clonValor is Struct || clonValor is Arreglo)
                        return clonValor;
                    else
                        return getValorPrimitivo((tiposPrimitivos)clonValor);
                else
                    throw new Exception("<tr>\n" +
                    "\t<td>Error Semantico</td>\n" +
                    "\t<td>No se ah declarado ningun Type con el id " + "'" + this.typeArrglo + "'" + "</td>\n" +
                    "\t<td>" + this.line + "</td>\n" +
                    "\t<td>" + this.column + "</td>\n</tr>\n\n");
            }
            else
                return getValorPrimitivo((tiposPrimitivos)this.typeArrglo);


        }
        private object getValorPrimitivo(tiposPrimitivos type)
        {
            if (type == tiposPrimitivos.INT)
                return int.Parse("0");
            else if (type == tiposPrimitivos.REAL)
                return float.Parse("0.0", CultureInfo.GetCultureInfo("en-US"));
            else if (type == tiposPrimitivos.BOOLEAN)
                return bool.Parse("false");
            else //(type == tiposPrimitivos.STRING)
                return "";
        }

        public object Clone()
        {
            Arreglo clone = new Arreglo(this.line, this.column, this.idArreglo,this.contenido , this.typeArrglo, this.dimensiones);
            return clone;
        }

       
    }
}



/*if (this.typeArrglo is string)
{
    tiposPrimitivos type = programClass.getType((string)this.typeArrglo);
    if (type != tiposPrimitivos.error)
    {
        this.typeArrglo = type;
    }
    else
        throw new Exception("<tr>\n" +
        "\t<td>Error Semantico</td>\n" +
        "\t<td>No se ah declarado ningun Type con el id " + "'" + this.typeArrglo + "'" + "</td>\n" +
        "\t<td>" + this.line + "</td>\n" +
        "\t<td>" + this.column + "</td>\n</tr>\n\n");

}*/