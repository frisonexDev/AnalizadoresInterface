using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Galileo.Utils.HL7Model
{

    public class HL7Message
    {
        public MessageHeader Header;
        public PatientIdentification PatientId;
        public PatientVisit PatientVisit;
        public ObservationRequest ObservationRequest;
        public List<ObservationResult> ObservationResultList;
        public MessageAcknowledgmentSegment MessageAcknowledgment;
        public ErrorSegment ErrorACK;
        public QueryAcknowledgmentSegment QueryACK;

        public QueryDefinitionSegment QueryDef;
        public QueryFilterSegment QueryFilter;

        public List<DisplayDataSegment> DisplayDataList;

        public ContinuationPointerSegment ContinuesPointer;


        public string Serializar()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(Header.Serializar());
            if (MessageAcknowledgment != null)
                sb.Append(MessageAcknowledgment.Serializar());

            if (ErrorACK != null)
                sb.Append(ErrorACK.Serializar());

            if (QueryACK != null)
                sb.Append(QueryACK.Serializar());

            if (QueryDef != null)
                sb.Append(QueryDef.Serializar());

            if (QueryFilter != null)
                sb.Append(QueryFilter.Serializar());

            if (DisplayDataList != null)
            {
                foreach (var item in DisplayDataList)
                {
                    sb.Append(item.Serializar());
                }
                
            }

            if (ContinuesPointer != null)
                sb.Append(ContinuesPointer.Serializar());

            return sb.ToString();
        }

    }
    
}
