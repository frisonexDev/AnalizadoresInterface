using Galileo.Utils.Protocol3_1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Galileo.Protocol3_1
{
    public class Protocol3_1Message
    {
        public ResultRecord resultRecord;
        public Protocol3_1Message(string content)
        {

            string[] parameterSeparator = { "\n", "\r" };
            string[] partSeparator = { "\t", "\u0002", "\u0003", "\u0001", "\u0004" };
            //DescomponerTrama
            var lines = content.Split(parameterSeparator, System.StringSplitOptions.RemoveEmptyEntries);

            int length = lines.Length;

            ResultRecord r = new ResultRecord(content);
            r.details = new List<ResultRecordDetail>();

            for (int i = 0; i < length; i++)
            {

                string line = lines[i];
                var parts = line.Split(partSeparator, System.StringSplitOptions.TrimEntries);

                string nextLine = "";
                var nextParts = nextLine.Split(partSeparator, System.StringSplitOptions.TrimEntries);

                if (i + 1 < length)
                {
                    nextLine = lines[i + 1];
                    nextParts = nextLine.Split(partSeparator, System.StringSplitOptions.TrimEntries);
                }



                if (line.Contains(":"))
                {
                    switch (parts[0])
                    {
                        case "Serial No.:":
                            if (parts.Length > 1)
                                r.SerialNo = parts[1];
                            break;


                        case "RecNo:":
                            if (parts.Length > 1)
                                r.RecordNo = parts[1];
                            break;

                        case "Sample ID:":
                            if (parts.Length > 1)
                                r.SampleId = parts[1];
                            break;

                        case "Patient ID:":
                            if (parts.Length > 1)
                                r.PatientId = parts[1];
                            break;

                        case "Patient Name:":
                            if (parts.Length > 1)
                                r.PatientName = parts[1];
                            break;

                        case "Mode:":
                            if (parts.Length > 1)
                                r.Mode = parts[1];
                            break;


                        case "Doctor:":
                            if (parts.Length > 1)
                                r.Doctor = parts[1];
                            break;

                        case "Age:":
                            if (parts.Length > 1)
                                r.Age = parts[1];
                            break;

                        case "Birth(ymd):":
                            if (parts.Length > 1)
                                r.BirthDate = parts[1];
                            break;


                        case "Sex:":
                            if (parts.Length > 1)
                                r.Sex = parts[1];
                            break;


                        case "Test date(ymd):":
                            if (parts.Length > 1)
                            {
                                var TimeParts = lines.Where(l => l.Contains("Test time(hm):")).FirstOrDefault().Split(partSeparator, System.StringSplitOptions.TrimEntries);

                                int year = Convert.ToInt32(parts[1].Substring(0, 4));
                                int month = Convert.ToInt32(parts[1].Substring(4, 2));
                                int day = Convert.ToInt32(parts[1].Substring(6, 2));

                                int hour = Convert.ToInt32(TimeParts[1].Substring(0, 2));
                                int min = Convert.ToInt32(TimeParts[1].Substring(2, 2));
                                int secs = Convert.ToInt32(TimeParts[1].Substring(4, 2));

                                r.DateTimeTest = new DateTime(year, month, day, hour, min, secs);
                            }

                            break;

                        case "Flags:":
                            if (parts.Length > 1)
                                r.Sex = parts[1];
                            break;



                        default:
                            break;
                    }
                }
                else if (line.Contains("Param"))
                {
                    i++;
                    

                    
                    string subline = lines[i];

                    while (!subline.Contains(":"))
                    {
                        
                        var subparts = subline.Split(partSeparator, System.StringSplitOptions.TrimEntries);
                        ResultRecordDetail detail = new ResultRecordDetail(line);
                        detail.Parameter = subparts[0];
                        detail.Flag = subparts[1];
                        detail.Value = subparts[2];
                        detail.Unit = subparts[3];
                        detail.Reference = subparts[4];

                        r.details.Add(detail);
                        i++;
                        subline = lines[i];
                    }
                }
            }

            resultRecord = r;

        }
    }
}
