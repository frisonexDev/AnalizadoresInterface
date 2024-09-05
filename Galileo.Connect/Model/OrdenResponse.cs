using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Galileo.Connect.Model
{
    public class DetallesOrden
    {

        [JsonProperty("IdExamen")]
        public int IdExamen { get; set; }

        [JsonProperty("CodigoMuestra")]
        public string CodigoMuestra { get; set; }

        [JsonProperty("nombreExamen")]
        public string nombreExamen { get; set; }

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

    public class OrdenResponse
    {
        [JsonProperty("Programar")]
        public bool Programar { get; set; }

        [JsonProperty("CodigoOrden")]
        public string CodigoOrden { get; set; }

        [JsonProperty("NombrePaciente")]
        public string Nombre { get; set; }

        [JsonProperty("ApellidoPaciente")]
        public string Apellido{ get; set; }

        [JsonProperty("Identificador")]
        public string  Identificacion { get; set; }


        [JsonProperty("FechaNacimiento")]
        public DateTime FechaNacimiento { get; set; }

        [JsonProperty("Edad")]
        public string Edad { get; set; }

        [JsonProperty("Genero")]
        public string Sexo { get; set; }

        [JsonProperty("Diagnostico")]
        public string Diag { get; set; }

        [JsonProperty("IdOrden")]
        public int Id { get; set; }


        [JsonProperty("Estado")]
        public string Estado { get {

                if (DetallesFinales.Count > 0)
                    return "Activo";
                else
                    return "Anulado";
            
            } }



        public string Tests {
            get {

                string tests = "";

                foreach (var item in DetallesFinales)
                {
                    if (item.Estado != "Anulado")
                    {
                        tests = tests + item.CodigoExamenHomologado + "-";
                    }
                }

                return tests; }
        }

        [JsonProperty("PrioridadOrden")]
        public string Prioridad { get; set; }

        [JsonProperty("FechaIngreso")]
        public DateTime Fecha { get; set; }

        [JsonProperty("Resultados")]
        public IList<DetallesOrden> Detalles { get; set; }

        public IList<DetallesOrden> DetallesFinales { get
            {
                return Detalles.Where(X => X.Estado != "Anulado").ToList();
            }
                }




        public OrdenResponse()
        {
            Programar = true;
        }
    }
}
