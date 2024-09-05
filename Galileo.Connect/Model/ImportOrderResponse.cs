using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Galileo.Connect.Model
{
    public class MuestraOrden
    {

        [JsonProperty("codigoMuestra")]
        public string CodigoMuestra { get; set; }

        [JsonProperty("numeroEtiquetas")]
        public string NumeroEtiquetas { get; set; }

        [JsonProperty("nombreRecipiente")]
        public string NombreRecipiente { get; set; }
    }

    public class ImportOrderResponse
    {

        [JsonProperty("idOrden")]
        public int IdOrden { get; set; }

        [JsonProperty("idLaboratorio")]
        public int IdLaboratorio { get; set; }

        [JsonProperty("nombreLaboratorio")]
        public string NombreLaboratorio { get; set; }

        [JsonProperty("nombrePaciente")]
        public string NombrePaciente { get; set; }

        [JsonProperty("fechaOrden")]
        public DateTime FechaOrden { get; set; }

        [JsonProperty("identificaforPaciente")]
        public string IdentificaforPaciente { get; set; }

        [JsonProperty("procesoExitoso")]
        public bool ProcesoExitoso { get; set; }

        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("codigoOrden")]
        public string CodigoOrden { get; set; }

        [JsonProperty("muestraOrden")]
        public IList<MuestraOrden> MuestraOrden { get; set; }

        [JsonProperty("mensaje")]
        public string Mensaje { get; set; }
    }

}
