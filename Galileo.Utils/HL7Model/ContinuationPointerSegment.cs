using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Galileo.Utils.HL7Model
{
    public class ContinuationPointerSegment
    {
        public string ContinuationPointer { get; set; } // Continuation pointer

        public string Serializar()
        {
            return $"DSC|{ContinuationPointer}|" + char.ConvertFromUtf32(13);
        }
    }
}
