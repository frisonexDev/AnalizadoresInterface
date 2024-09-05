using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Galileo.Utils.ASTMModel
{
    public class HostToAnalyzerMessage1381
    {
        public MessageHeader Header;
        public List<OrderRecord> TestList;
        public MessageTerminator Terminator;

        public string SerializeMessage(int TestOrderIndex)
        {
            try
            {
                string stringMsg = "";

                StringBuilder sb = new StringBuilder();
                sb.AppendLine(Header.Serialize());
           
                OrderRecord o = TestList.ToArray()[TestOrderIndex];
                sb.AppendLine(o.PatientInfo.Serialize(Header.SegmentDelimeter));
                sb.AppendLine(o.Serialize(Header.SegmentDelimeter));

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
