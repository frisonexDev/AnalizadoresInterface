using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Galileo.Utils.HL7Model
{
    public class PatienName
    {
        public string LastName;
        public string FirstName;
        public string MiddleName;
        public string Suffix;
        public string Prefix;
        public string Degree;


    }
    public class PatientIdentification
    {
        public PatientIdentification(string content, HL7Message msg)
        {
            Content = content;

            var header = msg.Header;

            //string[] partSeparator = { "|" };
            var parts = Content.Split(header.FieldSeparator, System.StringSplitOptions.None);

            //string[] segmentSeparator = { "^", "~" };

            if (parts.Length > 3)
            {
                var segments = parts[3].Split("^", System.StringSplitOptions.RemoveEmptyEntries);
                if (segments.Length > 0)
                {
                    PatientIdentifier = segments[0];
                }
                else
                {
                    if (parts[2] != "")
                    {
                        PatientIdentifier = parts[2];
                    }
                }
            }

            if (parts.Length > 5)
            {
                PatientNameContent = parts[5];

                var segments = PatientNameContent.Split("^", System.StringSplitOptions.RemoveEmptyEntries);

                this.Patient = new PatienName();
                
                if (segments.Length > 0)
                {
                     
                    this.Patient.LastName = segments[0];

                }

                if (segments.Length > 1)
                {

                    this.Patient.FirstName = segments[1];

                }

                if (segments.Length > 2)
                {

                    this.Patient.MiddleName = segments[2];

                }

                if (segments.Length > 3)
                {

                    this.Patient.Suffix = segments[3];

                }

                if (segments.Length > 4)
                {

                    this.Patient.Prefix = segments[4];

                }

                if (segments.Length > 5)
                {

                    this.Patient.Degree = segments[5];

                }



            }

            if (parts.Length > 7)
            {
                try
                {
                    if (parts[7] != "")
                        DateOfBirth = new DateTime(Convert.ToInt32(parts[7].Substring(0, 4)), Convert.ToInt32(parts[7].Substring(4, 2)), Convert.ToInt32(parts[7].Substring(6, 2)));
                }
                catch (Exception)
                {

                    DateOfBirth = DateTime.Now;
                }
                
            }

            if (parts.Length > 8)
            {
                Sex = parts[8];
            }


        }
        public string Content;
        public string PatientIdentifier;//3
        public string PatientNameContent;//5

        public PatienName Patient;

        public DateTime DateOfBirth;//7
        
        public string Sex;//8


    }
}
