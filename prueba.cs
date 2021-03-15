using System;
using System.Collections.Generic;
using System.Text;

namespace _OLC2_Proyecto1 
{
    public class prueba : ICloneable
    {


        public string nom;

        public prueba()
        {
        }

        public prueba(string nom)
        {
            this.nom = nom;
        }

        public object Clone()
        {
            return new prueba(this.nom);
        }
    }


}
