using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Galileo.Utils.BC_AU480
{
    
    public class AUMessage
    {
        public AUMessage(string content)
        {
            Content = content.Replace(ASTMModel.ASTM.Character.STX,"");
            StartMessage = new Start(Content);
            if (StartMessage.Type == "D" && StartMessage.SubType != "E")
            {
                Patient = new PatientInfo(Content);
                SampleQuery = null;
            }
            else if (StartMessage.Type == "R")
            {
                char[] caracteres = Content.ToCharArray();
                caracteres[0] = 'S';
                ResponseHeader = new string(caracteres);

                SampleQuery = Content.Substring(Content.Length - 15).TrimStart();


            }

        }

        private string ExtraerCampo(string texto, int startIndex, int length)
        {
            if (texto.Length >= startIndex + length)
            {
                return texto.Substring(startIndex - 1, length);
            }
            else
            {
                return string.Empty; // Campo no válido si está fuera de límites
            }
        }
        public string Content { get; set; }
        public Start StartMessage { get; set; }
        public PatientInfo Patient { get; set; }

        public string SampleQuery { get; set; }
        public string RackNumber { get; set; }
        public string CupPostion { get; set; }
        public string  TipoMuestra { get; set; }

        public string NumeroMuestra { get; set; }

        public string ResponseHeader { get; set; }


    }
}
