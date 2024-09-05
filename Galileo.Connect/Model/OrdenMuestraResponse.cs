using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Galileo.Connect.Model
{
    public class ResultadosEsperados
    {

        [JsonProperty("IdExamen")]
        public int IdExamen { get; set; }

        [JsonProperty("CodigoMuestra")]
        public string CodigoMuestra { get; set; }

        [JsonProperty("nombreExamen")]
        public string NombreExamen { get; set; }

        [JsonProperty("AbreviaturaExamen")]
        public string AbreviaturaExamen { get; set; }

        [JsonProperty("CodigoExamenHomologado")]
        public string CodigoExamenHomologado { get; set; }

        [JsonProperty("Repeticion")]
        public bool Repeticion { get; set; }

        [JsonProperty("Estado")]
        public string Estado { get; set; }

        [JsonProperty("ResultadoAnterior")]
        public object ResultadoAnterior { get; set; }
    }

    public class OrdenMuestraResponse
    {

        [JsonProperty("CodigoOrden")]
        public string CodigoOrden { get; set; }

        [JsonProperty("NombrePaciente")]
        public string NombrePaciente { get; set; }

        [JsonProperty("ApellidoPaciente")]
        public string ApellidoPaciente { get; set; }

        [JsonProperty("FechaNacimiento")]
        public DateTime FechaNacimiento { get; set; }

        [JsonProperty("Edad")]
        public string Edad { get; set; }

        [JsonProperty("Identificador")]
        public string Identificador { get; set; }

        [JsonProperty("Genero")]
        public string Genero { get; set; }

        [JsonProperty("Diagnostico")]
        public string Diagnostico { get; set; }

        [JsonProperty("PrioridadOrden")]
        public string PrioridadOrden { get; set; }

        [JsonProperty("FechaIngreso")]
        public DateTime FechaIngreso { get; set; }

        [JsonProperty("IdOrden")]
        public int IdOrden { get; set; }

        [JsonProperty("Resultados")]
        public IList<ResultadosEsperados> Resultados { get; set; }
    }


}
