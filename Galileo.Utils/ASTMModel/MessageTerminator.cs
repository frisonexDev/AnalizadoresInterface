using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Galileo.Utils.ASTMModel
{
    public class MessageTerminator
    {
        public MessageTerminator(string content)
        {
            
            Content = content;
            Content = content + "|";
            var parms = Content.Split("|", StringSplitOptions.TrimEntries);

            RecordTypeId = parms[0];
            SecuenceNumber = parms[1];
            TerminationCode = parms[2];

        }

        public MessageTerminator()
        {
            Content = "";
            RecordTypeId = "L";
            SecuenceNumber = "";
            TerminationCode = "";

        }
        public string Content;
        public string RecordTypeId;
        public string SecuenceNumber;
        public string TerminationCode;

        public string Serialize()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(RecordTypeId + "|");//1
            sb.Append(SecuenceNumber + "|");//2
            sb.Append(TerminationCode );//3
            sb.Append("\n");//3

            return sb.ToString();
        }

        
             public string SerializeASTM1394_97()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("L" + "|");//1
            sb.Append(SecuenceNumber + "|");//2
            sb.Append(TerminationCode);//3
            sb.Append(char.ConvertFromUtf32(13));

            return sb.ToString();
        }

        public string SerializeASTM1394_97(ref int sequence)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(sequence.ToString() + "L" + "|");//1
            sb.Append(SecuenceNumber + "|");//2
            sb.Append(TerminationCode);//3
            sb.Append(char.ConvertFromUtf32(13));

            return sb.ToString();
        }


    }
}
