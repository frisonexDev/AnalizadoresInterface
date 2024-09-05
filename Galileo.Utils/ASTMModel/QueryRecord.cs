using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Galileo.Utils.ASTMModel
{

    
    public class QueryRecord
    {
        public QueryRecord()
        {
            Content = "";
            RecordTypeId = "O";
            SecuenceNumber = "";
            SpecimenId = "";
           

        }
        public QueryRecord(string content, MessageHeader header)
        {
          

            Content = content + "|";
            var parms = Content.Split("|", StringSplitOptions.TrimEntries);

            if (parms.Length > 0)
                RecordTypeId = "Q";

            if (parms.Length > 1)
                SecuenceNumber = parms[1];

            if (parms.Length > 2)
            {
                string auxField = parms[2];

                var segments = auxField.Split(header.SegmentDelimeter, System.StringSplitOptions.TrimEntries);

                if (segments.Length > 0)
                {

                    //this.TestIdentifier.Content = TestID;

                    if (segments.Length > 0)
                    {
                        this.PatientId = segments[0];
                    }

                    if (segments.Length > 1)
                    {
                        this.SpecimenId = segments[1];
                    }

                }

            }




            if (parms.Length > 4)
                UniversalTestId = parms[4];

            if (parms.Length > 12)
                NatureOfRequest = parms[12];





        }

        public string Serialize ()
        {
            StringBuilder sb = new StringBuilder();
            return sb.ToString();
        }


        public string Content;
        public string RecordTypeId;
        public string SecuenceNumber;
        public string PatientId;
        public string SpecimenId;
        public string UniversalTestId;
        public string NatureOfRequest;









    }
}
