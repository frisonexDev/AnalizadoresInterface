using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Galileo.Utils.HL7Model
{
    public class ObservationResult
    {
        public ObservationResult(string content, HL7Message msg)
        {

            Content = content;
            //string[] partSeparator = { "|" };
            var parts = Content.Split(msg.Header.FieldSeparator, System.StringSplitOptions.None);
            Index = msg.Header._obsResultCount;

            if (parts.Length > 1)
                SecuenceId = parts[1];

            if (parts.Length > 2)
                ValueType = parts[2];

            if (parts.Length > 3)
                ObservationIdentifier = new(parts[3],msg);

            if (parts.Length > 4)
                ObservationSubId = parts[4];

            if (parts.Length > 5)
                ObservationValue = new ObservationValueContext(parts[5]);
            
            
            if (parts.Length > 6)
            {
                Units = parts[6];
                ReferenceRange = parts[7];
                AbnormalFlags = parts[8];
                ObservationResultStatus = parts[11];
            }




            if (ValueType == "NM")
            {
                char a = Convert.ToChar(Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator);

                if (parts.Length > 5)
                {
                    string stringValue = parts[5];
                    if (a.ToString() == ",")
                    {
                        stringValue = stringValue.Replace(".", ",");
                        decimal nmvalue = 0;
                        if (decimal.TryParse(stringValue, out nmvalue))
                            ObservationNumericValue = nmvalue;
                    }
                }

               

               
            }



        }
        public string Content;
        public int Index;
        public string SecuenceId; //1
        public string ValueType;//2
        public ObservationIdentifierContext ObservationIdentifier;//3
        public string ObservationSubId;//4

        public decimal ObservationNumericValue;


        public ObservationValueContext ObservationValue;//5


        public string Units;//6
        public string ReferenceRange;//7
        public string AbnormalFlags;//8
        public string ObservationResultStatus;//11




    }
}
