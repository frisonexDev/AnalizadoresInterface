using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Galileo.Utils.HL7Model
{
    public class ErrorSegment
    {

        public string ErrorCode { get; set; }

        public string Serializar()
        {
            return $"ERR|{ErrorCode}|" + char.ConvertFromUtf32(13);
        }

    }
}
