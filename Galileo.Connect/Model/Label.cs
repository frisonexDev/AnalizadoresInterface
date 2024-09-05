using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Galileo.Connect.Model
{
    public class Label
    {
        public string LabName { get; set; }
        public string PatientName { get; set; }
        public string PatientId { get; set; }

        public string SampleId { get; set; }

        public string SampleName { get; set; }

        public string Container { get; set; }

        public string Source { get; set; }

        public string  Date { get; set; }

        public string PatientAge { get; set; }

        public string Areas { get; set; }

        public int Count { get; set; }
    }
}
