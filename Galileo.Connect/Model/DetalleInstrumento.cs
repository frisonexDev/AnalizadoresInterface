using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Galileo.Connect.Model
{
    public class DetalleInstrumento
    {

        [JsonProperty("IdDetallesInstrumento")]
        public int IdDetallesInstrumento { get; set; }

        [JsonProperty("IdInstrumento")]
        public int IdInstrumento { get; set; }

        [JsonProperty("Instrumento")]
        public string Instrumento { get; set; }

        [JsonProperty("IdLaboratorio")]
        public int IdLaboratorio { get; set; }

        [JsonProperty("IdExamen")]
        public int IdExamen { get; set; }

        [JsonProperty("Examen")]
        public string Examen { get; set; }

        [JsonProperty("IdMuestra")]
        public int IdMuestra { get; set; }

        [JsonProperty("Muestra")]
        public string Muestra { get; set; }

        [JsonProperty("Homologacion")]
        public string Homologacion { get; set; }

        [JsonProperty("Activo")]
        public bool Activo { get; set; }

        [JsonProperty("Orden")]
        public int Orden { get; set; }

        [JsonProperty("Tipo")]
        public string Tipo { get; set; }

        [JsonProperty("Conversiones")]
        public List<InstrumentoConversion> Conversiones { get; set; }
    }

    public class DetalleInstrumentoMessage
    {

        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("details")]
        public List<DetalleInstrumento> Details { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }
    }

    public class DetalleInstrumentoResponse
    {

        [JsonProperty("detalleInstrumento")]
        public IList<DetalleInstrumentoMessage> DetalleInstrumento { get; set; }
    }
}
