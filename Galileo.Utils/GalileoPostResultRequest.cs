using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Galileo.Utils
{
    public class GalileoPostResultRequest
    {

        [JsonProperty("idLaboratorio")]
        public int IdLaboratorio { get; set; }

        [JsonProperty("codigoOrden")]
        public string CodigoOrden { get; set; }

        [JsonProperty("CodigoMuestra")]
        public string CodigoMuestra { get; set; }

        [JsonProperty("CodigoExamenHomologado")]
        public string CodigoExamenHomologado { get; set; }

        [JsonProperty("valorNumero")]
        public double ValorNumero { get; set; }
        
        [JsonProperty("valorTexto")]
        public string ValorTexto { get; set; }

        [JsonProperty("Validado")]
        public bool Validado { get; set; }

        [JsonProperty("Identificador")]
        public string Identificador { get; set; }

        [JsonProperty("Orden")]
        public int Orden { get; set; }
    }


}
