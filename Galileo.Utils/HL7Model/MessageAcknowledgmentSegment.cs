using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Galileo.Utils.HL7Model
{
    public class MessageAcknowledgmentSegment
    {

        // Propiedad para Acknowledgment Code
        public string AcknowledgmentCode { get; set; }

        // Propiedad para Confirmation Code
        public string ConfirmationCode { get; set; }

        // Propiedad para Message Control ID
        public string MessageControlID { get; set; }

        // Propiedad para Text Message
        public string TextMessage { get; set; }

        // Propiedad para Expected Sequence Number
        public string ExpectedSequenceNumber { get; set; }

        // Propiedad para Delayed Acknowledgment Type
        public string DelayedAcknowledgmentType { get; set; }

        // Propiedad para Error Condition
        public string ErrorCondition { get; set; }

        public string Serializar()
        {
            return $"MSA|{AcknowledgmentCode}|{MessageControlID}|{TextMessage}|{ExpectedSequenceNumber}|{DelayedAcknowledgmentType}|{ErrorCondition}" + "|" + char.ConvertFromUtf32(13);
        }
    }
}
