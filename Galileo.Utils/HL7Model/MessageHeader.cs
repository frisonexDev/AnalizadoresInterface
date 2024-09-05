using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Galileo.Utils.HL7Model
{
    public class MessageHeader
    {

        public MessageHeader()
        {
        }
            public MessageHeader(string content)
        {
            _obsResultCount = 0;
            Content = content;


            /*
            var aux = content.Split("MSH");
            FieldSeparator = aux[1].Substring(0, 1);
            */

            FieldSeparator = "|";

            var fields = Content.Split(FieldSeparator, System.StringSplitOptions.None);
            if (fields.Length > 1)
            {
                EncodigCharacters = fields[1];
                EncodingCharacterList = EncodigCharacters.ToCharArray();

            }

            if (fields.Length > 2)
                SendingApplication = fields[2];

            if (fields.Length > 3)
                SendingFacility = fields[3];

            if (fields.Length > 4)
                ReceivingApplication = fields[4];

            if (fields.Length > 5)
                ReceivingFacility = fields[5];

            if (fields.Length > 6)
                DateTimeOfMessage = fields[6];

            if (fields.Length > 7)
                Security = fields[7];

            if (fields.Length > 8)
                MessageType = fields[8];

            if (fields.Length > 9)
                MessageControlId = fields[9];

            if (fields.Length > 10)
                ProcessingId = fields[10];

            if (fields.Length > 11)
                VersionId = fields[11];

            if (fields.Length > 12)
                SequenceNumber = fields[12];

            if (fields.Length > 13)
                ContinuationPointer = fields[13];

            if (fields.Length > 14)
                AcceptanceACK = fields[14];

            if (fields.Length > 15)
                AppACK = fields[15];

            if (fields.Length > 16)
                CountryCode = fields[16];

            if (fields.Length > 17)
                CharacterSet = fields[17];

            if (fields.Length > 18)
                PrincipalLanguage = fields[18];

            if (fields.Length > 19)
                AlternateCharacterSet = fields[19];



        }
        public string Content;
        public int _obsResultCount;
        
        public string FieldSeparator;
        public string EncodigCharacters;
        public char[] EncodingCharacterList;


        public string SendingApplication;
        public string SendingFacility;


        public string ReceivingApplication;
        public string ReceivingFacility;
        public string DateTimeOfMessage;
        public string Security;
        public string MessageType;
        public string MessageControlId;
        public string ProcessingId;
        public string VersionId;
        public string SequenceNumber;
        public string ContinuationPointer;
        public string AcceptanceACK;
        public string AppACK;
        public string CountryCode;
        public string CharacterSet;
        public string PrincipalLanguage;
        public string AlternateCharacterSet;


        public string Serializar()
        {
            return $"MSH|{FieldSeparator}|" +
                   $"{SendingApplication}|{SendingFacility}|" +
                   $"{ReceivingApplication}|{ReceivingFacility}|{DateTimeOfMessage}|{Security}|{MessageType}|" +
                   $"{MessageControlId}|{ProcessingId}|{VersionId}|{SequenceNumber}|{ContinuationPointer}|" +
                   $"{AcceptanceACK}|{AppACK}|{CountryCode}|{CharacterSet}|{PrincipalLanguage}|{AlternateCharacterSet}|" + char.ConvertFromUtf32(13);
        }




    }
}
