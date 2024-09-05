using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Galileo.Utils.ASTMModel
{
    public static class ASTM
    {
        public enum Sex
        {
            M,
            F,
            U

        }

        public enum Priority
        {
            R,
            A,
            S

        }

        public class Character
        {
            public const string ENQ = "\u0005";
            public const string NAK = "\u0015";
            public const string ACK = "\u0006";
            public const string EOT = "\u0004";
            public const string STX = "\u0002";
            public const string ETX = "\u0003";
            public const string SOH = "\u0001";
            public const string EOTB = "\u0017";
            public const string CR = "\u0013";
            public const string EB = "\x1C";
            public const string SB = "\x0B";

           


        }

        public static string CharacterACK()
        {
            return AsciiToHexaDecimal("06");


        }

        public static string CharacterENQ()
        {
            return AsciiToHexaDecimal("05");


        }

        public static string CharacterNAK()
        {
            return AsciiToHexaDecimal("15");


        }

        public static string CharacterEOT()
        {
            return AsciiToHexaDecimal("04");


        }

        public static string CharacterETX()
        {
            return AsciiToHexaDecimal("03");


        }

        public static string CharacterSTX()
        {
            return AsciiToHexaDecimal("02");


        }


        public static string HexadecimalToAscii(string strHexa)
        {
            try
            {
                Encoding ascii = Encoding.ASCII;
                string strAscci = "";
                byte[] encodedBytes = ascii.GetBytes(strHexa);
                for (int a = 0; a < encodedBytes.Length; a++)
                    strAscci = strAscci + String.Format("{0:x2}", Convert.ToInt32(encodedBytes[a]));
                return strAscci;
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }

        public static string AsciiToHexaDecimal(string strNumAscci)
        {
            int a;
            string strValorHexa;
            strValorHexa = "";
            try
            {
                for (a = 0; a < strNumAscci.Length; a = a + 2)
                    strValorHexa = strValorHexa + Convert.ToChar(Convert.ToInt64("0x" + strNumAscci.Trim().Substring(a, 2), 16));
                return strValorHexa;
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }
    



        public static string CleanMessage(string texto)
        {
            int inicio = texto.IndexOf("<STX>");

            while (inicio != -1)
            {
                // Buscar el índice de fin del subtexto "<STX>"
                int fin = texto.IndexOf(">", inicio) + 1;

                if (fin != -1)
                {
                    // Eliminar el subtexto
                    texto = texto.Remove(inicio, fin - inicio+1);
                }

                // Buscar el siguiente subtexto "<STX>"
                inicio = texto.IndexOf("<STX>");
            }


            inicio = texto.IndexOf("<EOTB>");
            while (inicio != -1)
            {
                // Buscar el índice de fin del subtexto "<STX>"
                int fin = texto.IndexOf(">", inicio) + 1;

                if (fin != -1)
                {
                    // Eliminar el subtexto
                    texto = texto.Remove(inicio, fin - inicio + 4);
                }

                // Buscar el siguiente subtexto "<STX>"
                inicio = texto.IndexOf("<EOTB>");
            }

            inicio = texto.IndexOf("<ETX>");
            while (inicio != -1)
            {
                // Buscar el índice de fin del subtexto "<STX>"
                int fin = texto.IndexOf(">", inicio) + 1;

                if (fin != -1)
                {
                    // Eliminar el subtexto
                    texto = texto.Remove(inicio, fin - inicio + 4);
                }

                // Buscar el siguiente subtexto "<STX>"
                inicio = texto.IndexOf("<ETX>");
            }

            return texto;
        }


        public static string WriteTxRx(string input)
        {
            StringBuilder result = new StringBuilder();

            foreach (char character in input)
            {
                string replacement = character switch
                {
                    '\u0005' => "<ENQ>",
                    '\u0006' => "<ACK>",
                    '\u0004' => "<EOT>",
                    '\u0002' => "<STX>",
                    '\u0003' => "<ETX>",
                    '\u0001' => "<SOH>",
                    '\u0017' => "<EOTB>",
                    '\u0015' => "<NAK>",
                    '\r'=>"<LF>",
                    '\n' => "<CR>",
                '\x1C' => "<EB>",
                '\x0B' => "<SB>",

                   
        _ => character.ToString()
                };

                result.Append(replacement);

            }

            return result.ToString();
        }

        public static string WriteSpecialChars(string input)
        {

            if (input != null)
            {
                StringBuilder result = new StringBuilder();

                foreach (char character in input)
                {
                    string replacement = character switch
                    {
                        '\u0005' => "<ENQ>",
                        '\u0006' => "<ACK>",
                        '\u0004' => "<EOT>",
                        '\u0002' => "<STX>",
                        '\u0003' => "<ETX>",
                        '\u0001' => "<SOH>",
                        '\u0017' => "<EOTB>",
                        '\u0015' => "<NAK>",
                        '\r' => "<LF>",
                        '\n' => "<CR>",
                        '\x1C' => "<EB>",
                        '\x0B' => "<SB>",


                        _ => character.ToString()
                    };

                    result.Append(replacement);

                }

                return result.ToString();
            }
            else return "NULL";
        }

        public static string RemoveSpecialChars(string input)
        {
            StringBuilder result = new StringBuilder();

            foreach (char character in input)
            {
                string replacement = character switch
                {
                    '\u0005' => "",
                    '\u0006' => "",
                    '\u0004' => "",
                    '\u0002' => "",
                    '\u0003' => "",
                    '\u0001' => "",
                    '\u0017' => "",
                    _ => character.ToString()
                };

                result.Append(replacement);

            }

            return result.ToString();
        }
        public static string TransalateEspecialCharacters(string character)
        {
            string result = character;
            switch (character)
            {
                case "\u0005":
                    return "<ENQ>";
                case "\u0006":
                    return "<ACK>";
                case "\u0004":
                    return "<EOT>";
                case "\u0002":
                    return "<STX>";
                case "\u0003":
                    return "<ETX>";

                case "\u0001":
                    return "<SOH>";

                case "\u0017":
                    return "<EOTB>";




                default:
                    return result;
            }
            
        }

        public static string TranslateMessage(string str)
        {
            StringBuilder sb = new StringBuilder();
            if (str.Contains("|"))
            {
                for (int i = 0; i < str.Length; i++)
                {
                    if (str[i] != (char)2 && str[i] != (char)5 && str[i] != (char)4 && str[i] != (char)3)
                    {
                        sb.Append(str[i]);

                        if (str[i] == (char)13 || str[i] == (char)10)
                            break;
                        
                    }
                }
            }

            return sb.ToString();
        }


        public static DateTime DeserializeDateTime(string value)
        {

            try
            {
                if (value.Length <= 8)
                {
                    var year = Convert.ToInt32(value.Substring(0, 4));
                    var month = Convert.ToInt32(value.Substring(4, 2));
                    var day = Convert.ToInt32(value.Substring(6, 2));

                    if (month == 0)
                        month = 1;

                    if (day == 0)
                        day = 1;

                    return new DateTime(year, month, day);
                }
                else
                {
                    var year = Convert.ToInt32(value.Substring(0, 4));
                    var month = Convert.ToInt32(value.Substring(4, 2));
                    var day = Convert.ToInt32(value.Substring(6, 2));
                    var hours = Convert.ToInt32(value.Substring(8, 2));
                    var mins = Convert.ToInt32(value.Substring(10, 2));
                    var secs = Convert.ToInt32(value.Substring(12, 2));
                    return new DateTime(year, month, day, hours, mins, secs);
                }
            }
            catch (Exception ex)
            {
                return new DateTime(1900, 1, 1);
            }



            

        }

        public static string SerializeDateTime(DateTime value, bool includeTime)
        {


            if (includeTime)
                return value.ToString("yyyyMMddhhmmss");
            else
                return value.ToString("yyyyMMdd");





        }

    }
}
