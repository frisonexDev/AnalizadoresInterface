using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Galileo.Utils.ASTMModel
{
    public class ASTMMessage
    {
        public MessageHeader header;
        public List<PatientInformation> PatienInformationList;
        public QueryRecord Query;
        public MessageTerminator Terminator;

        public string Content;

        public ASTMMessage()
        {

        }

        public ASTMMessage(string content)
        {



            PatienInformationList = new List<PatientInformation>();

            string[] parameterSeparator = { "\n", "\r" };
            string  partSeparator = "|";




            Content = content;





            //DescomponerTrama
            var lines = Content.Split(parameterSeparator, System.StringSplitOptions.RemoveEmptyEntries);

            PatientInformation pat = null;
            OrderRecord ord = null;
            ResultRecord res = null;
            ManufacturerRecord man = null;
           


            int length = lines.Length;

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



                if (parts[0].Contains("H"))
                {
                    header = new MessageHeader(line);
                }
                else if (parts[0].Contains("Q"))
                {

                    Query = new QueryRecord(line, header);
                    
                }
                else if (parts[0].Contains("P"))
                {

                    pat = new PatientInformation(line, header);
                    pat.OrderRecordList = new List<OrderRecord>();
                    ord = null;
                }

                else if (parts[0].Contains("O"))
                {

                    ord = new OrderRecord(line,header);
                    ord.ResultRecordList = new List<ResultRecord>();
                    ord.ManufacturRecordList = new List<ManufacturerRecord>();
                    pat.OrderRecordList.Add(ord);
                    if (!((nextParts[0].Contains("O") || nextParts[0].Contains("R") || nextParts[0].Contains("M"))) && pat != null)
                    {
                        PatienInformationList.Add(pat);
                            pat = null;
                    }
                }

                else if (parts[0].Contains("R") || parts[0].Contains("M"))
                {

                    if (parts[0].Contains("R"))
                    {
                        res = new ResultRecord(line, header);
                        ord.ResultRecordList.Add(res);
                    }
                    else if (parts[0].Contains("M"))
                    {
                        man = new ManufacturerRecord(line, header);
                        ord.ManufacturRecordList.Add(man);
                    }

                    if (!((nextParts[0].Contains("R") || nextParts[0].Contains("M"))) && pat != null)
                    {
                        
                        
                        if (nextParts[0].Contains("P")|| nextParts[0].Contains("L") || nextParts[0].Contains("C"))
                        {

                            PatienInformationList.Add(pat);
                            pat = null;

                        }
                       
                    }
                }

              


                else if (parts[0].Contains("L"))
                {

                    Terminator = new MessageTerminator(line);
                }


                

            }






        }

        public string Serialize()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(header.Serialize());

            foreach (var item in PatienInformationList)
            {
                sb.Append(item.Serialize(header.SegmentDelimeter));
            }

            sb.Append(Terminator.Serialize());


            return sb.ToString();

        }

        public string SerializeASTM1394_97()
        {
            int sequence = 1;
            StringBuilder sb = new StringBuilder();
            sb.Append(header.Serialize1394_97(ref sequence));

            foreach (var item in PatienInformationList)
            {
                sb.Append(item.Serialize1394_97(header.SegmentDelimeter,ref sequence));
            }

            sb.Append(Terminator.SerializeASTM1394_97(ref  sequence));

            string result = sb.ToString();

            return result;

        }
    }


    }

