using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Galileo.Connect.Model.SampleQuery
{
    public class Recipiente
    {

        [JsonProperty("NombreRecipiente")]
        public string NombreRecipiente { get; set; }

        [JsonProperty("ImagenRecipienteUrl")]
        public string ImagenRecipienteUrl { get; set; }
    }

    public class Muestra
    {

        [JsonProperty("codigoMuestra")]
        public string CodigoMuestra { get; set; }

        [JsonProperty("NombreMuestra")]
        public string NombreMuestra { get; set; }

        [JsonProperty("NumeroEtiquetas")]
        public int NumeroEtiquetas { get; set; }

        [JsonProperty("ListIdExamen")]
        public object ListIdExamen { get; set; }

        [JsonProperty("Recipiente")]
        public Recipiente Recipiente { get; set; }
    }

    public class Detail
    {

        [JsonProperty("CodigoOrden")]
        public string CodigoOrden { get; set; }

        [JsonProperty("FechaCreacion")]
        public DateTime FechaCreacion { get; set; }

        [JsonProperty("Muestra")]
        public IList<Muestra> Muestra { get; set; }
    }

    public class Ordenes
    {

        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("details")]
        public IList<Detail> Details { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }
    }

    public class Result
    {

        [JsonProperty("Ordenes")]
        public Ordenes Ordenes { get; set; }
    }
}
