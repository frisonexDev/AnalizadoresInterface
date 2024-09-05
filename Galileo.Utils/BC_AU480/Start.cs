using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Galileo.Utils.BC_AU480
{
    public class Start
    {
        public Start(string content)
        {
            this.Content = content;
            this.Type = content.Substring(0, 1);
            this.SubType = content.Substring(1, 1);
        }

        public string Content { get; set; }
        public string Type { get; set; }
        public string SubType { get; set; }

    }
}
