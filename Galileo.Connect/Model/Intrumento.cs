using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Galileo.Connect.Model
{
   public class Instrumento
    {

        [JsonProperty("IdInstrumento")]
        public int IdInstrumento { get; set; }

        [JsonProperty("IdLaboratorio")]
        public int IdLaboratorio { get; set; }

        [JsonProperty("IdModeloInstrumento")]
        public int IdModeloInstrumento { get; set; }

        [JsonProperty("ModeloInstrumento")]
        public string ModeloInstrumento { get; set; }

        [JsonProperty("NombreAnalizador")]
        public string NombreAnalizador { get; set; }

        [JsonProperty("Identificador")]
        public string Identificador { get; set; }

        [JsonProperty("Speed")]
        public int Speed { get; set; }

        [JsonProperty("DataBits")]
        public int DataBits { get; set; }

        [JsonProperty("Parity")]
        public int Parity { get; set; }

        [JsonProperty("StopBits")]
        public int StopBits { get; set; }

        [JsonProperty("FlowControl")]
        public int FlowControl { get; set; }

        [JsonProperty("HostIP")]
        public object HostIP { get; set; }

        [JsonProperty("ClientIP")]
        public object ClientIP { get; set; }

        [JsonProperty("URL")]
        public object URL { get; set; }

        [JsonProperty("Activo")]
        public bool Activo { get; set; }

        [JsonProperty("DetallesInstrumento")]
        public List<DetalleInstrumento> DetallesInstrumento { get; set; }


        [JsonProperty("SeparadorMuestra")]
        public string SpecimenSeparator { get; set; }

    }

    public class InstrumentoMessage
    {

        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("details")]
        public IList<Instrumento> Details { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }
    }

    public class InstrumentoResponse
    {

        [JsonProperty("instrumento")]
        public IList<InstrumentoMessage> Instrumento { get; set; }
    }
}
