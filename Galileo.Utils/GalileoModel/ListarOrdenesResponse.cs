using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Galileo.Connect.Model
{
    public class DetalleOrdene
    {

        //[JsonProperty("IdOrden")]
        //public int IdOrden { get; set; }

        //[JsonProperty("IdLaboratorio")]
        //public int IdLaboratorio { get; set; }

        //[JsonProperty("IdSede")]
        //public int IdSede { get; set; }

        //[JsonProperty("IdDiagnostico")]
        //public int IdDiagnostico { get; set; }

        //[JsonProperty("IdPaciente")]
        //public int IdPaciente { get; set; }

        [JsonProperty("NombrePaciente")]
        [DisplayName("Nombre Paciente")]
        public string NombrePaciente { get; set; }

        [JsonProperty("ApellidoPaciente")]
        public string ApellidoPaciente { get; set; }

        //[JsonProperty("IdMedico")]
        //public int IdMedico { get; set; }

        //[JsonProperty("PrefijoOrden")]
        //public string PrefijoOrden { get; set; }

        [JsonProperty("CodigoOrden")]
        public string CodigoOrden { get; set; }

        //[JsonProperty("NumeroOrden")]
        //public int NumeroOrden { get; set; }

        [JsonProperty("PrioridadOrden")]
        public string PrioridadOrden { get; set; }

        [JsonProperty("FechaIngreso")]
        public DateTime FechaIngreso { get; set; }

        //[JsonProperty("Comentario")]
        //public object Comentario { get; set; }

        //[JsonProperty("CodigoExterno")]
        //public object CodigoExterno { get; set; }

        //[JsonProperty("IdUsuario")]
        //public int IdUsuario { get; set; }

        //[JsonProperty("IdEstadoOrden")]
        //public int IdEstadoOrden { get; set; }

        [JsonProperty("DescripcionEstado")]
        public string DescripcionEstado { get; set; }

        [JsonProperty("EdadCompleta")]
        public string EdadCompleta { get; set; }

        //[JsonProperty("Activo")]
        //public bool Activo { get; set; }
    }

    public class ListarOrdenesResponse
    {

        [JsonProperty("code")]
        public string code { get; set; }

        [JsonProperty("DetalleOrdenes")]
        public IList<DetalleOrdene> DetalleOrdenes { get; set; }

        [JsonProperty("message")]
        public string message { get; set; }

        [JsonProperty("status")]
        public string status { get; set; }

        [JsonProperty("idOrden")]
        public int idOrden { get; set; }
    }

}
