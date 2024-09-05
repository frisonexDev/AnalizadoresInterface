using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Galileo.Utils.HL7Model
{
    public class ObservationRequest
    {

        public ObservationRequest(string content, HL7Message msg)
        {
            Content = content;
            //string[] partSeparator = { "|" };
            var parts = Content.Split(msg.Header.FieldSeparator, System.StringSplitOptions.None);

            SetId = parts[1];
            if (parts.Length > 2)
            {
                PlaceOrderNumber = parts[2];
            }

            if (parts.Length > 3)
            {
                FillerOrderNumber = parts[3];
            }

            if (parts.Length > 4)
            {
                UniversalServiceId = parts[4];
            }

            if (parts.Length > 5)
            {
                UniversalServiceId = parts[5];
            }

            if (parts.Length > 6)
            {
                string strDate = parts[6];
                if (strDate != "")
                    RequestedTime = new DateTime(Convert.ToInt32(strDate.Substring(0, 4)), Convert.ToInt32(strDate.Substring(4, 2)), Convert.ToInt32(strDate.Substring(6, 2)), Convert.ToInt32(strDate.Substring(8, 2)), Convert.ToInt32(strDate.Substring(10, 2)), Convert.ToInt32(strDate.Substring(12, 2)));
            }

            if (parts.Length > 7)
            {
                try
                {
                    string strDate = parts[7];
                    if (strDate != "")
                        ObservationTime = new DateTime(Convert.ToInt32(strDate.Substring(0, 4)), Convert.ToInt32(strDate.Substring(4, 2)), Convert.ToInt32(strDate.Substring(6, 2)), Convert.ToInt32(strDate.Substring(8, 2)), Convert.ToInt32(strDate.Substring(10, 2)), Convert.ToInt32(strDate.Substring(12, 2)));

                }
                catch (Exception)
                {

                    
                }
                
            }

            if (parts.Length > 10)
            {
                CollectorId = parts[10];
            }

            if (parts.Length > 13)
            {
                RelevantClinicalInfo = parts[13];
            }

            if (parts.Length > 14)
            {
                try {
                    string strDate = parts[14];
                    if (strDate != "")
                        SpecimenReceivedTime = new DateTime(Convert.ToInt32(strDate.Substring(0, 4)), Convert.ToInt32(strDate.Substring(4, 2)), Convert.ToInt32(strDate.Substring(6, 2)), Convert.ToInt32(strDate.Substring(8, 2)), Convert.ToInt32(strDate.Substring(10, 2)), Convert.ToInt32(strDate.Substring(12, 2)));

                }
                catch 
                { }
                }
            if (parts.Length > 22)
            {
                try {
                    string strDate = parts[22];
                    if (strDate != "")
                        ApprovalTime = new DateTime(Convert.ToInt32(strDate.Substring(0, 4)), Convert.ToInt32(strDate.Substring(4, 2)), Convert.ToInt32(strDate.Substring(6, 2)), Convert.ToInt32(strDate.Substring(8, 2)), Convert.ToInt32(strDate.Substring(10, 2)), Convert.ToInt32(strDate.Substring(12, 2)));
                }
                catch 
                { }
            }

            if (parts.Length > 15)
            {
                SpecimenSource = parts[15];
            }

            if (parts.Length > 24)
            {
                DiagnosticServId = parts[24];
            }

            if (parts.Length > 28)
            {
                ResultCopiesTo = parts[28];
            }

            if (parts.Length > 32)
            {
                ResultCopiesTo = parts[32];
            }

        }
        public string Content;

        public string SetId; //1
        public string PlaceOrderNumber;//2
        public string FillerOrderNumber;//3
        public string UniversalServiceId;//4
        public string Priority;//5
        public DateTime RequestedTime;//6
        public DateTime ObservationTime;//7
        public string CollectorId;//10
        public string RelevantClinicalInfo;//13
        public DateTime SpecimenReceivedTime;//14
        public string SpecimenSource;//15
        public DateTime ApprovalTime;//22
        public string DiagnosticServId;//24
        public string ResultCopiesTo;//28
        public string ResultInterpreter;//32






    }
}
