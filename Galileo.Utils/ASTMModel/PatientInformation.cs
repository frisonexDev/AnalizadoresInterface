using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Galileo.Utils.ASTMModel
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

    public class PatienBirthDate
    {
        public string BirthDate;
        public string Age;
        public string AgeUnits;
        


    }




    public class PatientInformation
    {

        public PatientInformation()
        {
            Content = "";
            RecordTypeId = "";
            SequenceNumber = "";
            PracticeAssignedPatientId = "";
            LaboratoryAssignedPatientId = "";
            PatientId = "";
            PatientName = "";
            PatientMotherName = "";
            BirthDate = new DateTime(1900,1,1);
            PatientSex = "M";
            PatientRaceEthnicOrigin = "";
            PatientAddress = "";
            ReservedField = "";
            PatientTelephoneNo = "";
            AttendingPhysicianId = "";
            SpecialField1 = "";
            SpecialField2 = "";
            PatientHeight = "";
            PatientWeight = "";
            Diagnosis = "";
            ActiveMedications = "";
            Diet = "";
            PracticeField1 = "";
            PracticeField2 = "";
            AdmissionandDischargeDates = "";
            AdmissionStatus = "";
            Location = "";
            NatureOfAternativeDiagnosisCodeAndClassifiers = "";
            AlternativeDiagnosticCodeAndClasssification = "";
            Religion = "";
            MaritalStatus = "";
            IsolationsStatus = "";
            Language = "";
            HospitalService = "";
            HospitalInstitution = "";
            DosageCategory = "";
            

        }
        public PatientInformation(string content, MessageHeader header)
        {
          
            Content = content+ "|";
            var parms = Content.Split("|",StringSplitOptions.TrimEntries);

            if (parms.Length>0)
            RecordTypeId = "P";

            if (parms.Length > 1)
                SequenceNumber =  parms[1];

            if (parms.Length > 2)
                PracticeAssignedPatientId =  parms[2];

            if (parms.Length > 3)
                LaboratoryAssignedPatientId = parms[3];

            if (parms.Length > 4)
                PatientId = parms[4];

            if (parms.Length > 5)
            {
                PatientName = parms[5];
                var segments = PatientName.Split(header.SegmentDelimeter, System.StringSplitOptions.RemoveEmptyEntries);

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
            
            if (parms.Length > 6)
                PatientMotherName = parms[6];

            if (parms.Length > 7)
            {
                string birthDateField = parms[7];
                var segments = birthDateField.Split(header.SegmentDelimeter, System.StringSplitOptions.RemoveEmptyEntries);

                if (segments.Length > 1)
                {

                    BirthDate = ASTM.DeserializeDateTime(segments[1]);

                }

                if (segments.Length > 2)
                {

                    this.Age = segments[2];

                }

                if (segments.Length > 3)
                {

                    this.AgeUnits = segments[3];

                }


            }

            if (parms.Length > 8)
                PatientSex = parms[8];
               


            if (parms.Length > 9)
                PatientRaceEthnicOrigin = parms[9];

            if (parms.Length > 10)
                PatientAddress = parms[10];

            if (parms.Length > 11)
                ReservedField =  parms[11];

            if (parms.Length > 12)
                PatientTelephoneNo = parms[12];

            if (parms.Length > 13)
                AttendingPhysicianId =  parms[13];

            if (parms.Length > 14)
                SpecialField1 = parms[14];

            if (parms.Length > 15)
                SpecialField2 = parms[15];

            if (parms.Length > 16)
                PatientHeight = parms[16];

            if (parms.Length > 17)
                PatientWeight = parms[17];

            if (parms.Length > 18)
                Diagnosis = parms[18];

            if (parms.Length > 19)
                ActiveMedications =  parms[19];

            if (parms.Length > 20)
                Diet = parms[20];

            if (parms.Length > 21)
                PracticeField1 = parms[21];

            if (parms.Length > 22)
                PracticeField2 = parms[22];

            if (parms.Length > 23)
                AdmissionandDischargeDates = parms[23];

            if (parms.Length > 24)
                AdmissionStatus = parms[24];

            if (parms.Length > 25)
                Location = parms[25];

            if (parms.Length > 26)
                NatureOfAternativeDiagnosisCodeAndClassifiers = parms[26];

            if (parms.Length > 27)
                AlternativeDiagnosticCodeAndClasssification = parms[27];

            if (parms.Length > 28)
                Religion = parms[28];

            if (parms.Length > 29)
                MaritalStatus = parms[29];

            if (parms.Length > 30)
                IsolationsStatus = parms[30];

            if (parms.Length > 31)
                Language = parms[31];

            if (parms.Length > 32)
                HospitalService = parms[32];

            if (parms.Length > 33)
                HospitalInstitution = parms[33];

            if (parms.Length > 34)
                DosageCategory = parms[34];







        }

        public string Content;
        public string RecordTypeId;
        public string SequenceNumber;
        public string PracticeAssignedPatientId;
        public string LaboratoryAssignedPatientId;
        public string PatientId;
        public string PatientName;
        public string PatientMotherName;
        public DateTime BirthDate;
        public string Age;
        public string AgeUnits;
        public string PatientSex;
        public string PatientRaceEthnicOrigin;
        public string PatientAddress;
        public string ReservedField;
        public string PatientTelephoneNo;
        public string AttendingPhysicianId;
        public string SpecialField1;
        public string SpecialField2;
        public string PatientHeight;
        public string PatientWeight;
        public string Diagnosis;
        public string ActiveMedications;
        public string Diet;
        public string PracticeField1;
        public string PracticeField2;
        public string AdmissionandDischargeDates;
        public string AdmissionStatus;
        public string Location;
        public string NatureOfAternativeDiagnosisCodeAndClassifiers;
        public string AlternativeDiagnosticCodeAndClasssification;
        public string Religion;
        public string MaritalStatus;
        public string IsolationsStatus;
        public string Language;
        public string HospitalService;
        public string HospitalInstitution;
        public string DosageCategory;

        public List<OrderRecord> OrderRecordList;
       
        public PatienName Patient;


        public string Serialize(string segmentDelimeter)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("P" + "|");//1
            sb.Append(SequenceNumber + "|");//2
            sb.Append(PracticeAssignedPatientId + "|");//3
            sb.Append(LaboratoryAssignedPatientId + "|");//4
            sb.Append(PatientId + "|");//5
            sb.Append(PatientName.Replace(" ",segmentDelimeter) + "|");//6
            sb.Append(PatientMotherName + "|");//7

            if (Age == "")
            {
                sb.Append(ASTM.SerializeDateTime(BirthDate, false) + "|");//8
            }
            else
            {
                sb.Append(ASTM.SerializeDateTime(BirthDate, false) +   segmentDelimeter + Age + segmentDelimeter + AgeUnits +   segmentDelimeter + "|") ;//8
            }
            
            
            sb.Append(PatientSex + "|");//9
            sb.Append(PatientRaceEthnicOrigin + "|");//10
            sb.Append(PatientAddress + "|");//11
            sb.Append(ReservedField + "|");//12
            sb.Append(PatientTelephoneNo + "|");//13
            sb.Append(AttendingPhysicianId + "|");//14
            sb.Append(SpecialField1 + "|");//15
            sb.Append(SpecialField2 + "|");//16
            sb.Append(PatientHeight + "|");//17
            sb.Append(PatientWeight + "|");//18
            sb.Append(Diagnosis + "|");//19
            sb.Append(ActiveMedications + "|");//20
            sb.Append(Diet + "|");//21
            sb.Append(PracticeField1 + "|");//22
            sb.Append(PracticeField2 + "|");//23
            sb.Append(AdmissionandDischargeDates + "|");//24
            sb.Append(AdmissionStatus + "|");//25
            sb.Append(Location + "|");//26
            sb.Append(NatureOfAternativeDiagnosisCodeAndClassifiers + "|");//27
            sb.Append(AlternativeDiagnosticCodeAndClasssification + "|");//28
            sb.Append(Religion + "|");//29
            sb.Append(MaritalStatus + "|");//30
            sb.Append(IsolationsStatus + "|");//31
            sb.Append(Language + "|");//32
            sb.Append(HospitalService + "|");//33
            sb.Append(HospitalInstitution + "|");//34
            sb.Append(DosageCategory);//35
            sb.Append("\r");
            sb.Append("\n");

            foreach (var item in OrderRecordList)
            {
                sb.Append(item.Serialize(segmentDelimeter));
                
            }



            return sb.ToString();

        }

        public string Serialize1394_97(string segmentDelimeter)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("P" + "|");//1
            sb.Append(SequenceNumber + "|");//2
            sb.Append(PracticeAssignedPatientId + "|");//3
            
            sb.Append(PatientId + "|");//5
            sb.Append(LaboratoryAssignedPatientId + "|");//4
            sb.Append(PatientName.Replace(" ", segmentDelimeter) + "|");//6
            sb.Append(PatientMotherName + "|");//7

            if (Age == "")
            {
                sb.Append(ASTM.SerializeDateTime(BirthDate, false) + "|");//8
            }
            else
            {
                sb.Append(ASTM.SerializeDateTime(BirthDate, false) + "^" + Age + "^" + AgeUnits +  "|");//8
            }


            sb.Append(PatientSex + "|");//9
            sb.Append(PatientRaceEthnicOrigin + "|");//10
            sb.Append(PatientAddress + "|");//11
            sb.Append(ReservedField + "|");//12
            sb.Append(PatientTelephoneNo + "|");//13
            sb.Append(AttendingPhysicianId + "|");//14
            sb.Append(SpecialField1 + "|");//15
            sb.Append(SpecialField2 + "|");//16
            sb.Append(PatientHeight + "|");//17
            sb.Append(PatientWeight + "|");//18
            sb.Append(Diagnosis + "|");//19
            sb.Append(ActiveMedications + "|");//20
            sb.Append(Diet + "|");//21
            sb.Append(PracticeField1 + "|");//22
            sb.Append(PracticeField2 + "|");//23
            sb.Append(AdmissionandDischargeDates + "|");//24
            sb.Append(AdmissionStatus + "|");//25
            sb.Append(Location + "|");//26
            sb.Append(NatureOfAternativeDiagnosisCodeAndClassifiers + "|");//27
            sb.Append(AlternativeDiagnosticCodeAndClasssification + "|");//28
            sb.Append(Religion + "|");//29
            sb.Append(MaritalStatus + "|");//30
            sb.Append(IsolationsStatus + "|");//31
            sb.Append(Language + "|");//32
            sb.Append(HospitalService + "|");//33
            sb.Append(HospitalInstitution + "|");//34
            sb.Append(DosageCategory);//35
            sb.Append(char.ConvertFromUtf32(13));

            foreach (var item in OrderRecordList)
            {
                sb.Append(item.SerializeASTM1394_97(segmentDelimeter));

            }



            return sb.ToString();

        }

        public string Serialize1394_97(string segmentDelimeter, ref int sequence)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(sequence.ToString() + "P" + "|");//1
            sequence++;
            sb.Append(SequenceNumber + "|");//2
            sb.Append(PracticeAssignedPatientId + "|");//3

            sb.Append(PatientId + "|");//5
            sb.Append(LaboratoryAssignedPatientId + "|");//4
            sb.Append(PatientName.Replace(" ", segmentDelimeter) + "|");//6
            sb.Append(PatientMotherName + "|");//7

            if (Age == "")
            {
                sb.Append(ASTM.SerializeDateTime(BirthDate, false) + "|");//8
            }
            else
            {
                sb.Append(ASTM.SerializeDateTime(BirthDate, false) + "^" + Age + "^" + AgeUnits + "|");//8
            }


            sb.Append(PatientSex + "|");//9
            sb.Append(PatientRaceEthnicOrigin + "|");//10
            sb.Append(PatientAddress + "|");//11
            sb.Append(ReservedField + "|");//12
            sb.Append(PatientTelephoneNo + "|");//13
            sb.Append(AttendingPhysicianId + "|");//14
            sb.Append(SpecialField1 + "|");//15
            sb.Append(SpecialField2 + "|");//16
            sb.Append(PatientHeight + "|");//17
            sb.Append(PatientWeight + "|");//18
            sb.Append(Diagnosis + "|");//19
            sb.Append(ActiveMedications + "|");//20
            sb.Append(Diet + "|");//21
            sb.Append(PracticeField1 + "|");//22
            sb.Append(PracticeField2 + "|");//23
            sb.Append(AdmissionandDischargeDates + "|");//24
            sb.Append(AdmissionStatus + "|");//25
            sb.Append(Location + "|");//26
            sb.Append(NatureOfAternativeDiagnosisCodeAndClassifiers + "|");//27
            sb.Append(AlternativeDiagnosticCodeAndClasssification + "|");//28
            sb.Append(Religion + "|");//29
            sb.Append(MaritalStatus + "|");//30
            sb.Append(IsolationsStatus + "|");//31
            sb.Append(Language + "|");//32
            sb.Append(HospitalService + "|");//33
            sb.Append(HospitalInstitution + "|");//34
            sb.Append(DosageCategory);//35
            sb.Append(char.ConvertFromUtf32(13));

            foreach (var item in OrderRecordList)
            {
                sb.Append(item.SerializeASTM1394_97(segmentDelimeter, ref sequence));

            }



            return sb.ToString();

        }







    }
}
