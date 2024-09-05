using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Galileo.Utils.Protocol3_1
{

    public class ResultRecordDetail
    {
        public string Parameter;
        public string Flag;
        public string Value;
        public string Unit;
        public string Reference;

        public ResultRecordDetail(string content)
        {
            
        }
    }
    public class ResultRecord
    {

        public string SerialNo;
        public string RecordNo;
        public string SampleId;
        public string PatientId;
        public string PatientName;
        public string Mode;
        public string Doctor;
        public string Age;
        public string BirthDate;
        public string Sex;
        public DateTime DateTimeTest;
        public List<ResultRecordDetail> details;
        public string Flags;



        public ResultRecord(string content)
        {

        }
    }
}
