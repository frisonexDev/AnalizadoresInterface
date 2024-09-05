using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Galileo.Connect.Model
{
    public class ResultRequest
    {

        [JsonProperty("idLaboratorio")]
        public int idLaboratorio { get; set; }

        [JsonProperty("codigoOrden")]
        public string codigoOrden { get; set; }

        [JsonProperty("CodigoMuestra")]
        public string CodigoMuestra { get; set; }

        [JsonProperty("CodigoExamenHomologado")]
        public string CodigoExamenHomologado { get; set; }

        [JsonProperty("valorNumero")]
        public int valorNumero { get; set; }

        [JsonProperty("valorTexto")]
        public string valorTexto { get; set; }

        [JsonProperty("Validado")]
        public bool Validado { get; set; }

        [JsonProperty("Identificador")]
        public string Identificador { get; set; }
    }
}
