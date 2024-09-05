using Galileo.Utils.HL7Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Galileo.Utils
{
    public class HL7Helper
    {

        public string Content { get; set; }


        public string ToJson()
        {

            HL7Record oData = new()
            {
                messages = new()
            };


            string[] messageSeparator = { "\vMSH" };
            string[] parameterSeparator = { "\n", "\r" };
            string[] partSeparator = { "|" };


            //DescomponerTramas
            var messages = Content.Split(messageSeparator, System.StringSplitOptions.RemoveEmptyEntries);
            foreach (var message in messages)
            {
                var segments = message.Split(parameterSeparator, System.StringSplitOptions.RemoveEmptyEntries);
                int line = 0;

                HL7Message oMessage = new()
                {
                    ObservationResultList = new()
                };

                foreach (var segment in segments)
                {

                    if (line == 0)
                    {

                        oMessage.Header = new MessageHeader(segment);

                    }
                    else
                    {

                        var fields = segment.Split(oMessage.Header.FieldSeparator, System.StringSplitOptions.RemoveEmptyEntries);

                        switch (fields[0])
                        {
                            case "PID"://PatientId

                                oMessage.PatientId = new(segment, oMessage);
                                break;

                            case "PV1"://PatientVisit

                                oMessage.PatientVisit = new();
                                oMessage.PatientVisit.Content = segment;
                                break;

                            case "OBR"://ObservationRequest

                                oMessage.ObservationRequest = new(segment, oMessage);

                                break;

                            case "OBX"://ObservationResult

                                oMessage.Header._obsResultCount++;

                                ObservationResult oObservationResult = new(segment, oMessage);
                                oMessage.ObservationResultList.Add(oObservationResult);

                                break;

                        }




                    }

                    line++;
                }

                oData.messages.Add(oMessage);
            }


            return JsonConvert.SerializeObject(oData);
        }

        public HL7Record ToObject()
        {

            HL7Record oData = new()
            {
                messages = new()
            };


            string[] messageSeparator = { "\vMSH" };
            string[] parameterSeparator = { "\n", "\r" };
            string[] partSeparator = { "|" };


            //DescomponerTramas
            var messages = Content.Split(messageSeparator, System.StringSplitOptions.RemoveEmptyEntries);
            foreach (var message in messages)
            {
                var segments = message.Split(parameterSeparator, System.StringSplitOptions.RemoveEmptyEntries);
                int line = 0;

                HL7Message oMessage = new()
                {
                    ObservationResultList = new()
                };

                foreach (var segment in segments)
                {

                    if (line == 0)
                    {

                        oMessage.Header = new MessageHeader(segment);
                       
                    }
                    else
                    {

                        var fields = segment.Split(oMessage.Header.FieldSeparator, System.StringSplitOptions.RemoveEmptyEntries);

                        switch (fields[0])
                        {

                            case "QRD"://PatientId

                                oMessage.QueryDef = new QueryDefinitionSegment(segment);
                                break;

                            case "PID"://PatientId

                                oMessage.PatientId = new(segment,oMessage);
                                break;

                            case "PV1"://PatientVisit

                                oMessage.PatientVisit = new();
                                oMessage.PatientVisit.Content = segment;
                                break;

                            case "OBR"://ObservationRequest

                                oMessage.ObservationRequest = new(segment, oMessage);

                                break;

                            case "OBX"://ObservationResult

                                oMessage.Header._obsResultCount++;

                                ObservationResult oObservationResult = new(segment, oMessage);
                                oMessage.ObservationResultList.Add(oObservationResult);

                                break;

                        }




                    }

                    line++;
                }

                oData.messages.Add(oMessage);
            }


            return oData;
        }
    }
}
