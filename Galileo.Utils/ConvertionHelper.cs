using Galileo.Utils.ConversionModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Galileo.Utils
{
    public class ConvertionHelper
    {
        public void Evaluate (Formula f, ref string value)
        {

//            {
//                "Operador": "=",
//                  "Valor1": "-",
//                  "Valor2": "",
//                  "FormulaText": "",
//                  "Resultado": "0",
//                    "ConvertirTexto" = true
//            }
            

            switch (f.Operador)
            {
                case "=":

                    if (f.Valor1 == value)
                        value = f.Resultado;

                    break;
                default:
                   
                    break;
            }

           
        }

        public void Evaluate(Formula f, ref string value, ref string tipo)
        {

            //            {
            //                "Operador": "=",
            //                  "Valor1": "-",
            //                  "Valor2": "",
            //                  "FormulaText": "",
            //                  "Resultado": "0",
            //                    "TipoResultado" = "Texto"
            //            }


            if (f.TipoResultado != null)
                tipo = f.TipoResultado;

            switch (f.Operador)
            {
                case "=":

                    if (f.Valor1 == value)
                        value = f.Resultado;

                    break;
                default:

                    break;
            }

            


        }
    }
}
