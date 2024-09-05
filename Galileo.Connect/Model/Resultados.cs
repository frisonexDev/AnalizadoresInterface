using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Galileo.Connect.Model.Resultados
{
    public class CamposAdicionales
    {

        [JsonProperty("nombreCampo")]
        public string nombreCampo { get; set; }

        [JsonProperty("valor")]
        public string valor { get; set; }
    }

    public class EnvioResultado
    {

        [JsonProperty("codigoExterno")]
        public object codigoExterno { get; set; }

        [JsonProperty("nombreExamen")]
        public string nombreExamen { get; set; }

        [JsonProperty("idResultado")]
        public int idResultado { get; set; }

        [JsonProperty("AbreviaturaExamen")]
        public string AbreviaturaExamen { get; set; }

        [JsonProperty("NombreExamenHomologado")]
        public string NombreExamenHomologado { get; set; }

        [JsonProperty("resultadoActual")]
        public string resultadoActual { get; set; }

        [JsonProperty("fechaUltimoResultado")]
        public DateTime fechaUltimoResultado { get; set; }

        [JsonProperty("fechaValidacion")]
        public DateTime fechaValidacion { get; set; }

        [JsonProperty("NombreusuarioValidacion")]
        public object NombreusuarioValidacion { get; set; }

        [JsonProperty("NombreusuarioResultado")]
        public object NombreusuarioResultado { get; set; }

        [JsonProperty("refMinima")]
        public double refMinima { get; set; }

        [JsonProperty("refMaxima")]
        public double refMaxima { get; set; }

        [JsonProperty("panMinima")]
        public double panMinima { get; set; }

        [JsonProperty("panMaxima")]
        public double panMaxima { get; set; }

        [JsonProperty("fechaImpresion")]
        public DateTime fechaImpresion { get; set; }

        [JsonProperty("fechaEnvio")]
        public DateTime fechaEnvio { get; set; }

        [JsonProperty("fechaToma")]
        public DateTime fechaToma { get; set; }

        [JsonProperty("fechaRecepcion")]
        public DateTime fechaRecepcion { get; set; }

        [JsonProperty("codigoOrden")]
        public string codigoOrden { get; set; }

        [JsonProperty("resultadoMicrobiologia")]
        public IList<object> resultadoMicrobiologia { get; set; }
    }

    public class Detail
    {

        [JsonProperty("nombrePaciente")]
        public string nombrePaciente { get; set; }

        [JsonProperty("apellidoPaciente")]
        public string apellidoPaciente { get; set; }

        [JsonProperty("fechaNacimiento")]
        public DateTime fechaNacimiento { get; set; }

        [JsonProperty("Edad")]
        public string Edad { get; set; }

        [JsonProperty("generoPaciente")]
        public string generoPaciente { get; set; }

        [JsonProperty("camposAdicionales")]
        public IList<CamposAdicionales> camposAdicionales { get; set; }

        [JsonProperty("correo")]
        public string correo { get; set; }

        [JsonProperty("envioResultado")]
        public IList<EnvioResultado> envioResultado { get; set; }
    }

    public class Ordenes
    {

        [JsonProperty("code")]
        public string code { get; set; }

        [JsonProperty("details")]
        public IList<Detail> details { get; set; }

        [JsonProperty("message")]
        public string message { get; set; }

        [JsonProperty("status")]
        public string status { get; set; }
    }

    public class Resultados
    {

        [JsonProperty("ordenes")]
        public IList<Ordenes> ordenes { get; set; }
    }


}
