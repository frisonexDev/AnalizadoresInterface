using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Galileo.Utils.ASTMModel
{

    public class TestObject
    {
        public string TestIdentifier;
        public string TestName;
        public string TestIdentifierType;
        public string Manufacturer;
        public string Content;
   


    }

    public class DataMeasurementObject
    {
        public string Value;
        public string ValuePart1;
        public string ValuePart2;
        public string ValuePart3;
        public string ValuePart4;

        

    }


    public class ResultRecord
    {
        public ResultRecord(string content, MessageHeader header)
        {

            if (header == null)
            {
                header = new MessageHeader();
                header.SegmentDelimeter = "|";
            }

            Content = content;
            Content = content + "|";
            var parms = Content.Split("|", StringSplitOptions.TrimEntries);

            if (parms.Length > 0)
                RecordTypeId = "R";

            if (parms.Length > 1)
                SecuenceNumber = parms[1];


            if (parms.Length > 2)
            {
                TestID = parms[2];

                var segments = TestID.Split(header.SegmentDelimeter, System.StringSplitOptions.TrimEntries);

                this.TestIdentifier = new TestObject();

                if (segments.Length > 0)
                {

                    this.TestIdentifier.Content = TestID;

                    if (segments.Length > 0)
                    {
                        this.TestIdentifier.TestIdentifier = segments[0];
                    }

                    if (segments.Length > 1)
                    {
                        this.TestIdentifier.TestName = segments[1];
                    }

                    if (segments.Length > 2)
                    {
                        this.TestIdentifier.TestIdentifierType = segments[2];
                    }

                    if (segments.Length > 3)
                    {
                        this.TestIdentifier.Manufacturer = segments[3];
                    }

                }
            }


            if (parms.Length > 3)
            {
                DataMeasurementValue = parms[3];

                var segments = DataMeasurementValue.Split(header.SegmentDelimeter, System.StringSplitOptions.TrimEntries);

                this.DataMeasurement = new DataMeasurementObject();

                if (segments.Length > 0)
                {

                    //this.TestIdentifier.Content = TestID;

                    if (segments.Length > 0)
                    {
                        this.DataMeasurement.Value = segments[0];
                    }

                    if (segments.Length > 1)
                    {
                        this.DataMeasurement.ValuePart1 = segments[1];
                    }

                    if (segments.Length > 2)
                    {
                        this.DataMeasurement.ValuePart2 = segments[2];
                    }

                    if (segments.Length > 3)
                    {
                        this.DataMeasurement.ValuePart3 = segments[3];
                    }

                }
                //DataMeasurementValue = parms[3];
            }

            if (parms.Length > 4)
                Units = parms[4];

            if (parms.Length > 5)
                ReferenceRanges = parms[5];

            if (parms.Length > 6)
                ResultAbnormalFlags = parms[6];

            if (parms.Length > 7)
                NatureOfAbnormalityTesting = parms[7];

            if (parms.Length > 8)
                ResultStatus = parms[8];

            if (parms.Length > 9)
                DateOfChangeInInstrumentyNormativeValueOrUnits = parms[9];

            if (parms.Length > 10)
                OperatorIdentification = parms[10];

            if (parms.Length > 11)
                DateTimeTestStarted = parms[11];

            if (parms.Length > 12)
                DateTimeTestCompleted = parms[12];

            if (parms.Length > 13)
                InstrumentId = parms[13];

        }
        

        public string Content;
        public string RecordTypeId;
        public string SecuenceNumber;

        public string TestID;
        public string DataMeasurementValue;
        public string Units;
        public string ReferenceRanges;
        public string ResultAbnormalFlags;
        public string NatureOfAbnormalityTesting;
        public string ResultStatus;
        public string DateOfChangeInInstrumentyNormativeValueOrUnits;
        public string OperatorIdentification;
        public string DateTimeTestStarted;
        public string DateTimeTestCompleted;
        public string InstrumentId;

        public TestObject TestIdentifier;
        public DataMeasurementObject DataMeasurement;



    }
}
