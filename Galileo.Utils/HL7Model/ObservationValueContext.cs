using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Galileo.Utils.HL7Model
{
    public class ObservationValueContext
    {
        public ObservationValueContext(string content)
        {
            string[] segmentSeparator = { "^", "~" };
            Value = content;
            Arguments = new();
            var segments = Value.Split(segmentSeparator, System.StringSplitOptions.RemoveEmptyEntries);

            if (segments.Length == 1)
            {
                Value = segments[0];
            }
            else
            {

                foreach (var s in segments)
                {
                    Arguments.Add(s);
                }
            }

        }

        public string Value;
        public List<string> Arguments;

    }

}
