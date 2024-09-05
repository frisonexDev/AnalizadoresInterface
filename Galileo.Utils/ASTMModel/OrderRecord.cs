using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Galileo.Utils.ASTMModel
{

    public class SpecimenIdObject
    {
        public string SpecimenId;
        public string SpecimenPart1;
        public string SpecimenPart2;
        public string SpecimenPart3;
        public string SpecimenPart4;



    }
    public class OrderRecord
    {
        public OrderRecord()
        {
            Content = "";
            RecordTypeId = "O";
            SecuenceNumber = "";
            SpecimenId = "";
            InstrumentSpecimenId = "";
            UniversalTestIdPartOne = "";
            UniversalTestIdPartTwo = "";
            UniversalTestIdPartThree = "";
            Priority = ASTM.Priority.R;
            RequestedOrderderedDateTime = new DateTime(1900,1,1);
            SpecimenCollectionDateTime = new DateTime(1900, 1, 1);
            CollectionEndTime = "";
            CollectionVolume = "";
            CollectorId = "";
            ActionCode = "";
            DangerCode = "";
            RelevantClinicalInformation = "";
            DateTimeSpecimenReceived = new DateTime(1900, 1, 1);
            SpecimenDescriptor = "";
            OrderingPhysician = "";
            PhysicianTelephoneNo = "";
            UserField1 = "";
            UserField2 = "";
            LaboratoryField1 = "";
            LaboratoryField2 = "";
            ResultsReportedModifiedDateTime = "";
            InstreumentChargeToComputerSystem = "";
            InstrumentSectionId = "";
            ReportTypes = "";
            ReservedField = "";
            LocationOfWardOfSpecimenCollection = "";
            NosocomialIngectionFlag = "";
            SpecimenService = "";
            SpecimenInstitution = "";

        }
        public OrderRecord(string content, MessageHeader header)
        {

            if (header == null)
            {
                header = new MessageHeader();
                header.SegmentDelimeter = "|";
            }

            Content = content + "|";
            var parms = Content.Split("|", StringSplitOptions.TrimEntries);

            if (parms.Length > 0)
                RecordTypeId = "O";

            if (parms.Length > 1)
                SecuenceNumber = parms[1];

            if (parms.Length > 2)
            {
                SpecimenId = parms[2];

                var segments = SpecimenId.Split(header.SegmentDelimeter, System.StringSplitOptions.TrimEntries);

                this.SpecimenIdRecord = new SpecimenIdObject();

                if (segments.Length > 0)
                {

                    //this.TestIdentifier.Content = TestID;

                    if (segments.Length > 0)
                    {
                        this.SpecimenIdRecord.SpecimenId = segments[0];
                    }

                    if (segments.Length > 1)
                    {
                        this.SpecimenIdRecord.SpecimenPart1 = segments[1];
                    }

                    if (segments.Length > 2)
                    {
                        this.SpecimenIdRecord.SpecimenPart2 = segments[2];
                    }

                    if (segments.Length > 3)
                    {
                        this.SpecimenIdRecord.SpecimenPart3 = segments[3];
                    }

                    if (segments.Length > 4)
                    {
                        this.SpecimenIdRecord.SpecimenPart4 = segments[4];
                    }

                }

            }




            if (parms.Length > 3)
                InstrumentSpecimenId = parms[3];

            if (parms.Length > 4)
            {

                var fields = parms[4];
                var segments = fields.Split(header.SegmentDelimeter, System.StringSplitOptions.None);

                if (segments.Length <= 3)
                    UniversalTestIdPartOne = parms[4];


                if (segments.Length > 3)
                    UniversalTestIdPartOne = segments[3];

                if (segments.Length > 4)
                    UniversalTestIdPartTwo = segments[4];

                if (segments.Length > 5)
                    UniversalTestIdPartTwo = segments[5];
            }




            if (parms.Length > 5)

                if (parms[5] == "R")
                    Priority = ASTM.Priority.R;
                else
                    Priority = ASTM.Priority.A;

            if (parms.Length > 6)
                RequestedOrderderedDateTime = ASTM.DeserializeDateTime(parms[6]);

            if (parms.Length > 7)
                SpecimenCollectionDateTime = ASTM.DeserializeDateTime(parms[7]);

            if (parms.Length > 8)
                CollectionEndTime = parms[8];

            if (parms.Length > 9)
                CollectionVolume = parms[9];

            if (parms.Length > 10)
                CollectorId = parms[10];

            if (parms.Length > 11)
                ActionCode = parms[11];

            if (parms.Length > 12)
                DangerCode = parms[12];

            if (parms.Length > 13)
                RelevantClinicalInformation = parms[13];

            if (parms.Length > 14)
                DateTimeSpecimenReceived = ASTM.DeserializeDateTime(parms[14]);

            if (parms.Length > 15)
                SpecimenDescriptor = parms[15];

            if (parms.Length > 16)
                OrderingPhysician = parms[16];

            if (parms.Length > 17)
                PhysicianTelephoneNo = parms[17];

            if (parms.Length > 18)
                UserField1 = parms[18];

            if (parms.Length > 19)
                UserField2 = parms[19];

            if (parms.Length > 20)
                LaboratoryField1 = parms[20];

            if (parms.Length > 21)
                LaboratoryField2 = parms[21];

            if (parms.Length > 22)
                ResultsReportedModifiedDateTime = parms[22];

            if (parms.Length > 23)
                InstreumentChargeToComputerSystem = parms[23];

            if (parms.Length > 24)
                InstrumentSectionId = parms[24];

            if (parms.Length > 25)
                ReportTypes = parms[25];

            if (parms.Length > 26)
                ReservedField = parms[26];

            if (parms.Length > 27)
                LocationOfWardOfSpecimenCollection = parms[27];

            if (parms.Length > 28)
                NosocomialIngectionFlag = parms[28];

            if (parms.Length > 29)
                SpecimenService = parms[29];

            if (parms.Length > 30)
                SpecimenInstitution = parms[30];



        }
        public string Serialize(string segmentDelimeter)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("O" + "|");//1
            sb.Append(SecuenceNumber + "|");//2
            sb.Append(SpecimenId + "|");//3
            sb.Append(InstrumentSpecimenId + "|");//4
            if (UniversalTestIdPartTwo == null || UniversalTestIdPartTwo == "")
                if (UniversalTestIdPartThree == null || UniversalTestIdPartThree == "")
                    sb.Append(UniversalTestIdPartOne + "|");//5
                else
                    sb.Append(UniversalTestIdPartOne + segmentDelimeter + UniversalTestIdPartTwo + segmentDelimeter + UniversalTestIdPartThree + "|");//5
            else
                sb.Append(UniversalTestIdPartOne + segmentDelimeter + UniversalTestIdPartTwo + segmentDelimeter + UniversalTestIdPartThree + "|");//5

            sb.Append(Priority + "|");//6
            sb.Append(ASTM.SerializeDateTime(RequestedOrderderedDateTime, true) + "|");//7
            sb.Append(ASTM.SerializeDateTime(SpecimenCollectionDateTime, true) + "|");//8
            sb.Append(CollectionEndTime + "|");//9
            sb.Append(CollectionVolume + "|");//10
            sb.Append(CollectorId + "|");//11
            sb.Append(ActionCode + "|");//12
            sb.Append(DangerCode + "|");//13
            sb.Append(RelevantClinicalInformation + "|");//14
            sb.Append("" + "|");//15 sb.Append(DateTimeSpecimenReceived + "|");//15
            sb.Append(SpecimenDescriptor + "|");//16
            sb.Append(OrderingPhysician + "|");//17
            sb.Append(PhysicianTelephoneNo + "|");//18
            sb.Append(UserField1 + "|");//19
            sb.Append(UserField2 + "|");//20
            sb.Append(LaboratoryField1 + "|");//21
            sb.Append(LaboratoryField2 + "|");//22
            sb.Append(ResultsReportedModifiedDateTime + "|");//23
            sb.Append(InstreumentChargeToComputerSystem + "|");//24
            sb.Append(InstrumentSectionId + "|");//25
            sb.Append(ReportTypes + "|");//26
            sb.Append(ReservedField + "|");//27
            sb.Append(LocationOfWardOfSpecimenCollection + "|");//28
            sb.Append(NosocomialIngectionFlag + "|");//29
            sb.Append(SpecimenService + "|");//30
            sb.Append(SpecimenInstitution);//31
            sb.Append("\r");
            sb.Append("\n");
            return sb.ToString();
        }

        public string SerializeASTM1394_97(string segmentDelimeter)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("O" + "|");//1
            sb.Append(SecuenceNumber + "|");//2
            sb.Append(SpecimenId + "|");//3
            sb.Append(InstrumentSpecimenId + "|");//4
            if (UniversalTestIdPartTwo == null || UniversalTestIdPartTwo == "")
                if (UniversalTestIdPartThree == null || UniversalTestIdPartThree == "")
                sb.Append(UniversalTestIdPartOne + "|");//5
                else
                    sb.Append(UniversalTestIdPartOne + segmentDelimeter + UniversalTestIdPartTwo + segmentDelimeter + UniversalTestIdPartThree + "|");//5
            else
            sb.Append(UniversalTestIdPartOne + segmentDelimeter + UniversalTestIdPartTwo + segmentDelimeter + UniversalTestIdPartThree + "|");//5

            sb.Append(Priority + "|");//6
            sb.Append(ASTM.SerializeDateTime(RequestedOrderderedDateTime,true) + "|");//7
            sb.Append(ASTM.SerializeDateTime(SpecimenCollectionDateTime,true) + "|");//8
            sb.Append(CollectionEndTime + "|");//9
            sb.Append(CollectionVolume + "|");//10
            sb.Append(CollectorId + "|");//11
            sb.Append(ActionCode + "|");//12
            sb.Append(DangerCode + "|");//13
            sb.Append(RelevantClinicalInformation + "|");//14
            sb.Append(ASTM.SerializeDateTime(DateTimeSpecimenReceived,true) + "|");//15
            sb.Append(SpecimenDescriptor + "|");//16
            sb.Append(OrderingPhysician + "|");//17
            sb.Append(PhysicianTelephoneNo + "|");//18
            sb.Append(UserField1 + "|");//19
            sb.Append(UserField2 + "|");//20
            sb.Append(LaboratoryField1 + "|");//21
            sb.Append(LaboratoryField2 + "|");//22
            sb.Append(ResultsReportedModifiedDateTime + "|");//23
            sb.Append(InstreumentChargeToComputerSystem + "|");//24
            sb.Append(InstrumentSectionId + "|");//25
            sb.Append(ReportTypes + "|");//26
            sb.Append(ReservedField + "|");//27
            sb.Append(LocationOfWardOfSpecimenCollection + "|");//28
            sb.Append(NosocomialIngectionFlag + "|");//29
            sb.Append(SpecimenService + "|");//30
            sb.Append(SpecimenInstitution);//31
            sb.Append(char.ConvertFromUtf32(13));
            return sb.ToString();
        }

        public string SerializeASTM1394_97(string segmentDelimeter, ref int sequence)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(sequence.ToString()+"O" + "|");//1
            sb.Append(SecuenceNumber + "|");//2
            sb.Append(SpecimenId + "|");//3
            sb.Append(InstrumentSpecimenId + "|");//4
            if (UniversalTestIdPartTwo == null || UniversalTestIdPartTwo == "")
                if (UniversalTestIdPartThree == null || UniversalTestIdPartThree == "")
                    sb.Append(UniversalTestIdPartOne + "|");//5
                else
                    sb.Append(UniversalTestIdPartOne + segmentDelimeter + UniversalTestIdPartTwo + segmentDelimeter + UniversalTestIdPartThree + "|");//5
            else
                sb.Append(UniversalTestIdPartOne + segmentDelimeter + UniversalTestIdPartTwo + segmentDelimeter + UniversalTestIdPartThree + "|");//5

            sb.Append(Priority + "|");//6
            sb.Append(ASTM.SerializeDateTime(RequestedOrderderedDateTime, true) + "|");//7
            sb.Append(ASTM.SerializeDateTime(SpecimenCollectionDateTime, true) + "|");//8
            sb.Append(CollectionEndTime + "|");//9
            sb.Append(CollectionVolume + "|");//10
            sb.Append(CollectorId + "|");//11
            sb.Append(ActionCode + "|");//12
            sb.Append(DangerCode + "|");//13
            sb.Append(RelevantClinicalInformation + "|");//14
            sb.Append(ASTM.SerializeDateTime(DateTimeSpecimenReceived, true) + "|");//15
            sb.Append(SpecimenDescriptor + "|");//16
            sb.Append(OrderingPhysician + "|");//17
            sb.Append(PhysicianTelephoneNo + "|");//18
            sb.Append(UserField1 + "|");//19
            sb.Append(UserField2 + "|");//20
            sb.Append(LaboratoryField1 + "|");//21
            sb.Append(LaboratoryField2 + "|");//22
            sb.Append(ResultsReportedModifiedDateTime + "|");//23
            sb.Append(InstreumentChargeToComputerSystem + "|");//24
            sb.Append(InstrumentSectionId + "|");//25
            sb.Append(ReportTypes + "|");//26
            sb.Append(ReservedField + "|");//27
            sb.Append(LocationOfWardOfSpecimenCollection + "|");//28
            sb.Append(NosocomialIngectionFlag + "|");//29
            sb.Append(SpecimenService + "|");//30
            sb.Append(SpecimenInstitution);//31
            sb.Append(char.ConvertFromUtf32(13));
            sequence++;
            return sb.ToString();
        }


        public string Content;
        public string RecordTypeId;
        public string SecuenceNumber;
        public string SpecimenId;
        public string InstrumentSpecimenId;


        public string UniversalTestIdPartOne;
        public string UniversalTestIdPartTwo;
        public string UniversalTestIdPartThree;
        public ASTM.Priority Priority;
        public DateTime RequestedOrderderedDateTime;
        public DateTime SpecimenCollectionDateTime;
        public string CollectionEndTime;
        public string CollectionVolume;
        public string CollectorId;
        public string ActionCode;
        public string DangerCode;
        public string RelevantClinicalInformation;
        public DateTime DateTimeSpecimenReceived;
        public string SpecimenDescriptor;
        public string OrderingPhysician;
        public string PhysicianTelephoneNo;
        public string UserField1;
        public string UserField2;
        public string LaboratoryField1;
        public string LaboratoryField2;
        public string ResultsReportedModifiedDateTime;
        public string InstreumentChargeToComputerSystem;
        public string InstrumentSectionId;
        public string ReportTypes;
        public string ReservedField;
        public string LocationOfWardOfSpecimenCollection;
        public string NosocomialIngectionFlag;
        public string SpecimenService;
        public string SpecimenInstitution;

        public PatientInformation PatientInfo;
        public List<ResultRecord> ResultRecordList;
        public List<ManufacturerRecord> ManufacturRecordList;

        public SpecimenIdObject SpecimenIdRecord;





    }
}
