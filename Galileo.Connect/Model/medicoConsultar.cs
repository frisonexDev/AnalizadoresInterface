using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Galileo.Connect.Model
{
    public class MedicoQueryDetail
    {

        [JsonProperty("IdMedico")]
        public int IdMedico { get; set; }

        [JsonProperty("IdLaboratorio")]
        public int IdLaboratorio { get; set; }

        [JsonProperty("Nombre")]
        public string Nombre { get; set; }

        [JsonProperty("Apellido")]
        public string Apellido { get; set; }

        [JsonProperty("Matricula")]
        public string Matricula { get; set; }

        [JsonProperty("Telefono")]
        public string Telefono { get; set; }

        [JsonProperty("Email")]
        public string Email { get; set; }

        [JsonProperty("Usuario")]
        public object Usuario { get; set; }

        [JsonProperty("Password")]
        public string Password { get; set; }

        [JsonProperty("Activo")]
        public bool Activo { get; set; }

        [JsonProperty("PublicKey")]
        public string PublicKey { get; set; }
    }

    public class MedicoQ
    {

        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("details")]
        public IList<MedicoQueryDetail> Details { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }
    }

    public class CosultaMedicoResult
    {

        [JsonProperty("medico")]
        public IList<MedicoQ> Medico { get; set; }
    }


}
