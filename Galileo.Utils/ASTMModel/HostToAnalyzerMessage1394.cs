using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Galileo.Utils.ASTMModel
{
    public class HostToAnalyzerMessage1394
    {
        public MessageHeader Header;
        public List<PatientInformation> PatienInformationList;
        public MessageTerminator Terminator;

        public string SerializeMessage()
        {
            try
            {
                string stringMsg = "";

                StringBuilder sb = new StringBuilder();
                sb.AppendLine(Header.Serialize());
                int patientIndex = 1;
                foreach (var p in PatienInformationList)
                {
                    p.SequenceNumber = patientIndex.ToString();
                    sb.AppendLine(p.Serialize(Header.SegmentDelimeter));

                    int orderIndex = 1;
                    foreach (var o in p.OrderRecordList)
                    {
                        o.SecuenceNumber = orderIndex.ToString();
                        sb.AppendLine(o.Serialize(Header.SegmentDelimeter));
                        orderIndex++;
                    }
                    patientIndex++;
                }
                sb.AppendLine(Terminator.Serialize());



                return stringMsg;
            }
            catch (Exception)
            {
                return "Error";
            }

        }

     


    }
}
