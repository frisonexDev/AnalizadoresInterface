using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Galileo.Connect.Model
{
    public class InstrumentoConversion
    {

        [JsonProperty("IdInstrumentoConversion")]
        public int IdInstrumentoConversion { get; set; }

        [JsonProperty("IdLaboratorio")]
        public int IdLaboratorio { get; set; }

        [JsonProperty("IdDetallesInstrumento")]
        public int IdDetallesInstrumento { get; set; }

        [JsonProperty("DatoEntrada")]
        public string DatoEntrada { get; set; }

        [JsonProperty("Formula")]
        public string Formula { get; set; }

        [JsonProperty("Activo")]
        public bool Activo { get; set; }
    }

    public class InstrumentoConversionMessage
    {

        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("details")]
        public IList<InstrumentoConversion> Details { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }
    }

    public class InstrumentoConversionResponse
    {

        [JsonProperty("instrumentoConversion")]
        public IList<InstrumentoConversionMessage> InstrumentoConversion { get; set; }
    }
}
