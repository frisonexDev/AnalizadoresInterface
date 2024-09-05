using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Galileo.Connect.Model
{
    class pacienteConsultar
    {
        [JsonProperty("Identificador")]
        public string Identificador { get; set; }

        [JsonProperty("NombrePaciente")]
        public string NombrePaciente { get; set; }

        [JsonProperty("ApellidoPaciente")]
        public string ApellidoPaciente { get; set; }

        [JsonProperty("FechaNacimiento")]
        public DateTime FechaNacimiento { get; set; }

        [JsonProperty("Genero")]
        public string Genero { get; set; }

        [JsonProperty("CorreoElectronico")]
        public object CorreoElectronico { get; set; }

        [JsonProperty("Contrasena")]
        public object Contrasena { get; set; }

        [JsonProperty("PublicKey")]
        public object PublicKey { get; set; }

        [JsonProperty("EnvioCorreo")]
        public bool EnvioCorreo { get; set; }

        [JsonProperty("AccesoPortal")]
        public bool AccesoPortal { get; set; }

        [JsonProperty("Activo")]
        public bool Activo { get; set; }

        [JsonProperty("CampoAdicional")]
        public IList<object> CampoAdicional { get; set; }

        [JsonProperty("IdPaciente")]
        public int IdPaciente { get; set; }

        [JsonProperty("IdLaboratorio")]
        public int IdLaboratorio { get; set; }

        [JsonProperty("code")]
        public string code { get; set; }

        [JsonProperty("mensaje")]
        public string mensaje { get; set; }

    }
}
