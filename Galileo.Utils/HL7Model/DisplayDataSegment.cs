using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Galileo.Utils.HL7Model
{
    public class DisplayDataSegment
    {
        public string SetIDDSP { get; set; } // Set ID - DSP
        public string DisplayLevel { get; set; } // Display Level
        public string DataLine { get; set; } // Data Line
        public string LogicalBreakPoint { get; set; } // Logical Break Point
        public string ResultID { get; set; } // Result ID

        public string Serializar()
        {
            return $"DSP|{SetIDDSP}|{DisplayLevel}|{DataLine}|{LogicalBreakPoint}|{ResultID}|"+ char.ConvertFromUtf32(13);
        }
    }



}
