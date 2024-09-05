using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Galileo.Utils.BC_AU480
{
    public class AnalisysData
    {
        public string Content { get; set; }
        public string TestName { get; set; }

        public string TestValue { get; set; }

        public AnalisysData(string content)
        {
            Content = content;
            
        }


    }

    public class AUTestResult
    {
        public string TestName { get; set; }
        public string TestValue { get; set; }
        public string MarcaResultado { get; set; }
        public string Contenido { get; set; }
    }

    public class PatientInfo
    {
        public string Sex { get; set; }
        public string YearAge { get; set; }
        public string MonthAge { get; set; }

        public string PatientInformation { get; set; }

        public string Content { get; set; }

        public string SampleId { get; set; }

        public List<AnalisysData> results { get; set; }

        public PatientInfo(string content)
        {
            Content = content;
            Sex = Content.Substring(9, 1);
            YearAge = Content.Substring(10, 1);
            MonthAge = Content.Substring(11, 1);
            PatientInformation = Content.Substring(15);
            SampleId = Content.Substring(14, 15).Trim();
            results = new List<AnalisysData>();

            string resultText = Content.Substring(159);


            List<AUTestResult> resultados = new List<AUTestResult>();

            for (int i = 0; i < resultText.Length; i += 11)
            {
                string subcadena = resultText.Substring(i, Math.Min(11, resultText.Length - i));

                if (subcadena.Length >= 11)
                {
                    string testName = subcadena.Substring(0, 3);
                    string testValue = subcadena.Substring(3, 6);
                    string marcaResultado = subcadena.Substring(9, 2);
                    string testContent = subcadena;

                    resultados.Add(new AUTestResult
                    {
                        TestName = testName,
                        TestValue = testValue,
                        MarcaResultado = marcaResultado,
                        Contenido = testContent
                    });
                }
                else
                {
                    // Manejar el caso en el que la subcadena no tenga suficientes caracteres.
                    // Puedes lanzar una excepción, agregar un valor predeterminado o tomar alguna otra acción según tus requisitos.
                }
            }


            //string[] resultTextList; 

            //    if (resultText.Contains("nr"))
            //    {
            //        if (resultText.Contains("/"))
            //        {
            //            resultText = resultText.Replace("/", "nr");
            //        }
            //        if (resultText.Contains("F"))
            //        {
            //            resultText = resultText.Replace("F", "r");
            //        }

            //        resultTextList = resultText.Split("nr", StringSplitOptions.RemoveEmptyEntries);
            //    }
            //    else
            //    {
            //        if (resultText.Contains("/"))
            //        {
            //            resultText = resultText.Replace("/", "n");
            //        }
            //        if (resultText.Contains("F"))
            //        {
            //            resultText = resultText.Replace("/", "");
            //        }
            //        resultTextList = resultText.Split("n", StringSplitOptions.RemoveEmptyEntries);
            //    }


            foreach (var item in resultados)
            {
                //if (item.TrimEnd() != "")
                //{
                //    AnalisysData result = new AnalisysData(item.TrimEnd().TrimStart());
                //    //var parts = result.Content.Split(" ");

                //    if (item.Length >= 3)
                //    {
                //        result.TestName = item.Trim().Substring(0, 3).Trim();
                //        result.TestValue = item.Trim().Substring(3).Trim();
                //        //result.TestName = parts[0];
                //        //result.TestValue = parts[1];
                //        results.Add(result);
                //    }
                //}

                AnalisysData result = new AnalisysData(item.Contenido);
                result.TestName = item.TestName;
                result.TestValue = item.TestValue;
                results.Add(result);


            }

        }

    }
    public class FixedPart
    {
        public FixedPart(string content)
        {
            Content = content;
        }

        public string Content { get; set; }
        public string RackNumber { get; set; }
        public string CupPosition { get; set; }

        public string SampleType { get; set; }
        public string SampleId { get; set; }


    }
}
