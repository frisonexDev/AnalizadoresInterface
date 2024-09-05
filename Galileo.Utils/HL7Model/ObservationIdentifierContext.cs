using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Galileo.Utils.HL7Model
{
    public class ObservationIdentifierContext
    {
        public ObservationIdentifierContext(string content, HL7Message msg)
        {
           // string[] segmentSeparator = { "^", "~" };
            Content = content;
            var components = Content.Split("^", System.StringSplitOptions.RemoveEmptyEntries);
            if (components.Length > 0)
                Identifier = components[0];

            if (components.Length > 1)
                Text = components[1];

            if (components.Length > 2)
                NameOfCodingSystem = components[2];
            

        }
        public string Content;
        public string Identifier;//0
        public string Text;//1
        public string NameOfCodingSystem;//2
    }
}
