using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Galileo.Connect.Model
{
    public class ConsultarOrden
    {

        [JsonProperty("IdOrden")]
        public int IdOrden { get; set; }

        [JsonProperty("IdLaboratorio")]
        public int IdLaboratorio { get; set; }

        [JsonProperty("NombreLaboratorio")]
        public string NombreLaboratorio { get; set; }

        [JsonProperty("IdSede")]
        public int IdSede { get; set; }

        [JsonProperty("NombreSede")]
        public string NombreSede { get; set; }

        [JsonProperty("IdDiagnostico")]
        public int IdDiagnostico { get; set; }

        [JsonProperty("DescripcionDiagnostico")]
        public string DescripcionDiagnostico { get; set; }

        [JsonProperty("IdPaciente")]
        public int IdPaciente { get; set; }

        [JsonProperty("Nombre Paciente")]
        public string NombrePaciente { get; set; }

        [JsonProperty("Apellido Paciente")]
        public string ApellidoPaciente { get; set; }

        [JsonProperty("Genero del Paciente")]
        public string GenerodelPaciente { get; set; }

        [JsonProperty("Fecha Nacimiento")]
        public DateTime FechaNacimiento { get; set; }

        [JsonProperty("Edad")]
        public int Edad { get; set; }

        [JsonProperty("IdMedico")]
        public int IdMedico { get; set; }

        [JsonProperty("NombreMedico")]
        public object NombreMedico { get; set; }

        [JsonProperty("PrefijoOrden")]
        public string PrefijoOrden { get; set; }

        [JsonProperty("CodigoOrden")]
        public string CodigoOrden { get; set; }

        [JsonProperty("NumeroOrden")]
        public int NumeroOrden { get; set; }

        [JsonProperty("PrioridadOrden")]
        public string PrioridadOrden { get; set; }

        [JsonProperty("FechaIngreso")]
        public DateTime FechaIngreso { get; set; }

        [JsonProperty("Comentario")]
        public object Comentario { get; set; }

        [JsonProperty("CodigoExterno")]
        public object CodigoExterno { get; set; }

        [JsonProperty("IdUsuario")]
        public int IdUsuario { get; set; }

        [JsonProperty("IdEstadoOrden")]
        public int IdEstadoOrden { get; set; }

        [JsonProperty("Activo")]
        public bool Activo { get; set; }

        [JsonProperty("DetalleOrden")]
        public object DetalleOrden { get; set; }

        [JsonProperty("InformacionOrden")]
        public object InformacionOrden { get; set; }

        [JsonProperty("InformacionPaciente")]
        public object InformacionPaciente { get; set; }

        [JsonProperty("TomaMuestra")]
        public object TomaMuestra { get; set; }

        [JsonProperty("EdadCompleta")]
        public string EdadCompleta { get; set; }

        [JsonProperty("code")]
        public string code { get; set; }

        [JsonProperty("message")]
        public string message { get; set; }

        [JsonProperty("status")]
        public string status { get; set; }
    }

}
