using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Galileo.Utils.HL7Model
{
    public class QueryDefinitionSegment
    {

        public QueryDefinitionSegment()
        {

        }
        public QueryDefinitionSegment(string inputText)
        {
            // Dividir la cadena en campos usando el carácter "|"
            string[] fields = inputText.Split('|');

            // Verificar si hay suficientes campos antes de asignar valores
            if (fields.Length >= 12)
            {
                // Asignar valores a las propiedades
                QueryDateTime = fields[1];
                QueryFormatCode = fields[2]; // Valor fijo
                QueryPriority = fields[3]; // Valor fijo
                QueryID = fields[4];
                DeferredResponseType = "";
                DeferredResponseDateTime = "";
                QuantityLimitedRequest = fields[7];
                WhoSubjectFilter = fields[8];
                WhatSubjectFilter = fields[9];
                WhatDepartmentDataCode = fields[10];
                WhatDataCodeValueQualifier = "";
                QueryResultsLevel = ""; // Valor fijo
            }
            else
            {
                // Manejar el caso en que no haya suficientes campos
                throw new ArgumentException("La cadena de entrada no tiene suficientes campos.");
            }
        }
       






        public string QueryDateTime { get; set; } // Query Date/Time
        public string QueryFormatCode { get; set; } // Query Format Code
        public string QueryPriority { get; set; } // Query Priority
        public string QueryID { get; set; } // Query ID
        public string DeferredResponseType { get; set; } // Deferred Response Type
        public string DeferredResponseDateTime { get; set; } // Deferred Response Date/Time
        public string QuantityLimitedRequest { get; set; } // Quantity Limited Request
        public string WhoSubjectFilter { get; set; } // Who Subject Filter
        public string WhatSubjectFilter { get; set; } // What Subject Filter
        public string WhatDepartmentDataCode { get; set; } // What Department Data Code
        public string WhatDataCodeValueQualifier { get; set; } // What Data Code Value Qualifier
        public string QueryResultsLevel { get; set; } // Query Results Level

        public string Serializar()
        {
            return $"QRD|{QueryDateTime}|{QueryFormatCode}|{QueryPriority}|{QueryID}|" +
                   $"{DeferredResponseType}|{DeferredResponseDateTime}|{QuantityLimitedRequest}|" +
                   $"{WhoSubjectFilter}|{WhatSubjectFilter}|{WhatDepartmentDataCode}|" +
                   $"{WhatDataCodeValueQualifier}|{QueryResultsLevel}|" + char.ConvertFromUtf32(13);
        }

    }
}
