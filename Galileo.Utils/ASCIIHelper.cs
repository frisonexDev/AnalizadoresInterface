using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Galileo.Utils
{

    public class AscciChar
    {
        public string Character { get; set; }
        public string Definition { get; set; }
    }

    public class ASCIIHelper
    {

        public StringBuilder Data { get; set; }
        public List<AscciChar> AscciChars { get; set; }
        public ASCIIHelper()
        {
            Data = new StringBuilder();
            AscciChars = new List<AscciChar>();
            AscciChars.Add(new AscciChar { Character = "\u0005", Definition = "<ENQ>" });
            AscciChars.Add(new AscciChar { Character = "\u0002", Definition = "<STX>" });
            AscciChars.Add(new AscciChar { Character = "\u0003", Definition = "<ETX>" });
            AscciChars.Add(new AscciChar { Character = "\u0006", Definition = "<ACK>" });
            AscciChars.Add(new AscciChar { Character = "\u0021", Definition = "<NAK>" });
            AscciChars.Add(new AscciChar { Character = "\u0023", Definition = "<ETB>" });
            AscciChars.Add(new AscciChar { Character = "\u0004", Definition = "<EOT>" });
            AscciChars.Add(new AscciChar { Character = "\x1C", Definition = "<SB>" });
            AscciChars.Add(new AscciChar { Character = "\x0B", Definition = "<EB>" });
           



        }

        public string TranslateMesasage(string data)
        {
            string result = data;

            foreach (var ch in AscciChars)
            {
                result = result.Replace(ch.Character, ch.Definition);
            }




            return result;
        }

        public string CleanMessage(string data)
        {
            string result = data;

            foreach (var ch in AscciChars)
            {
                result = result.Replace(ch.Character, "");
            }




            return result;
        }

        public string TranslateASTM(string data)
        {
            string result = data;

            foreach (var ch in AscciChars)
            {
                result = result.Replace(ch.Character, "");
            }

            if (result.StartsWith("<ETX>") || result.StartsWith("<EOT>"))
            {
                return "";
            }

            return CleanMessage(result);
        }




    }

       






    }

