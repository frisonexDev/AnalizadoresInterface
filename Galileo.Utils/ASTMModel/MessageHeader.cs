using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Galileo.Utils.ASTMModel
{
    public class MessageHeader
    {
        
        public string Content;
        public string RecordTypeId;
        public string DelimiterDefinition;
        public string MessageControlId;
        public string AccessPassword;
        public string SenderNameId;
        public string SenderStreetAddress;
        public string ReservedField;
        public string TelephoneNo;
        public string CharacteristicOfSender;
        public string ReceiverId;
        public string Comment;
        public string ProcessingId;
        public string VersionNo;
        public DateTime TimeOfMessage;

        public string SegmentDelimeter;

        public MessageHeader(string content)
        {

           
            Content = content;
            Content = content + "|";
            var parms = Content.Split("|", StringSplitOptions.TrimEntries);

            RecordTypeId = "H";
            DelimiterDefinition = parms[1];
            SegmentDelimeter = parms[1].Substring(1, 1);
            


            MessageControlId = parms[2];
            AccessPassword = parms[3];
            SenderNameId = parms[4];
            SenderStreetAddress = parms[5]; 
            
            if (parms.Length > 7)
                TelephoneNo = parms[7];

            if (parms.Length > 8)
                CharacteristicOfSender = parms[8];

            if (parms.Length > 9)
                ReceiverId = parms[9];

            if (parms.Length > 10)
                Comment = parms[10];

            if (parms.Length>11)
            ProcessingId = parms[11];
            if (parms.Length > 12)
                VersionNo = parms[12];

            if (parms.Length > 13)
                TimeOfMessage = ASTM.DeserializeDateTime(parms[13]);



        }

        public MessageHeader(string fieldDelimeter, string repeatDeleimeter, string segmentDelimeter, string escapeDelimeter)
        {
            RecordTypeId = "H";
            DelimiterDefinition = fieldDelimeter + repeatDeleimeter + segmentDelimeter + escapeDelimeter;
            SegmentDelimeter = segmentDelimeter;
            //"\\^&"
        }
        public MessageHeader()
        {
            RecordTypeId = "H";
        }


        public string Serialize()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(RecordTypeId );
            sb.Append(DelimiterDefinition + "|");
            sb.Append(MessageControlId + "|");
            sb.Append(AccessPassword + "|");
            sb.Append(SenderNameId + "|");
            sb.Append(SenderStreetAddress + "|");
            sb.Append(ReservedField + "|");
            sb.Append(TelephoneNo + "|");
            sb.Append(CharacteristicOfSender + "|");
            sb.Append(ReceiverId + "|");
            sb.Append(Comment + "|");
            sb.Append(ProcessingId + "|");
            sb.Append(VersionNo + "|");
            sb.Append(ASTM.SerializeDateTime(TimeOfMessage,true) + "|");
            sb.Append("\r");
            sb.Append("\n");

            return sb.ToString();

        }

        public string SerializeASTM1394_97()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(RecordTypeId);
            sb.Append(DelimiterDefinition + "|");
            sb.Append(MessageControlId + "|");
            sb.Append(AccessPassword + "|");
            sb.Append(SenderNameId + "|");
            sb.Append(SenderStreetAddress + "|");
            sb.Append(ReservedField + "|");
            sb.Append(TelephoneNo + "|");
            sb.Append(CharacteristicOfSender + "|");
            sb.Append(ReceiverId + "|");
            sb.Append(Comment + "|");
            sb.Append(ProcessingId + "|");
            sb.Append(VersionNo + "|");
            sb.Append(ASTM.SerializeDateTime(TimeOfMessage, true) + "|");
            sb.Append("\r");
           // sb.Append("\n");

            return sb.ToString();

        }

        public string Serialize1394_97()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(RecordTypeId + "|");
            sb.Append(DelimiterDefinition + "|");
            sb.Append(MessageControlId + "|");
            sb.Append(AccessPassword + "|");
            sb.Append(SenderNameId + "|");
            sb.Append(SenderStreetAddress + "|");
            sb.Append(ReservedField + "|");
            sb.Append(TelephoneNo + "|");
            sb.Append(CharacteristicOfSender + "|");
            sb.Append(ReceiverId + "|");
            sb.Append(Comment + "|");
            sb.Append(ProcessingId + "|");
            sb.Append(VersionNo + "|");
            sb.Append(ASTM.SerializeDateTime(TimeOfMessage, true) );
            sb.Append(char.ConvertFromUtf32(13));
            //sb.Append("\n");

            return sb.ToString();

        }
        public string Serialize1394_97(ref int sequence)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(sequence.ToString() + RecordTypeId + "|");
            sb.Append(DelimiterDefinition + "|");
            sb.Append(MessageControlId + "|");
            sb.Append(AccessPassword + "|");
            sb.Append(SenderNameId + "|");
            sb.Append(SenderStreetAddress + "|");
            sb.Append(ReservedField + "|");
            sb.Append(TelephoneNo + "|");
            sb.Append(CharacteristicOfSender + "|");
            sb.Append(ReceiverId + "|");
            sb.Append(Comment + "|");
            sb.Append(ProcessingId + "|");
            sb.Append(VersionNo + "|");
            sb.Append(ASTM.SerializeDateTime(TimeOfMessage, true));
            sb.Append(char.ConvertFromUtf32(13));
            
            //sb.Append("\n");

           
            sequence = sequence + 1;
            return sb.ToString();

        }




    }
}
