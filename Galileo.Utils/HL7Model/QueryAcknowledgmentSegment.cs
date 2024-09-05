using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Galileo.Utils.HL7Model
{
    public class QueryAcknowledgmentSegment
    {
        public string QueryTag { get; set; }
        public string  QueryResponse { get; set; }

        public string Serializar()
        {
            return $"QAK|{QueryTag}|{QueryResponse}|" + char.ConvertFromUtf32(13);
        }
    }
}
