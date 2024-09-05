using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Galileo.Connect.Model
{
    public class ResultadoLiteral
    {

        [JsonProperty("IdResultado")]
        public int IdResultado { get; set; }

        [JsonProperty("codigo")]
        public string codigo { get; set; }

        [JsonProperty("texto")]
        public string texto { get; set; }
    }

    public class ResultadoOrden
    {

        [JsonProperty("idResultado")]
        public int idResultado { get; set; }

        [JsonProperty("idLaboratorio")]
        public int idLaboratorio { get; set; }

        [JsonProperty("idExamen")]
        public int idExamen { get; set; }

        [JsonProperty("idMuestra")]
        public int idMuestra { get; set; }

        [JsonProperty("codigoOrden")]
        public string codigoOrden { get; set; }

        [JsonProperty("codigoExterno")]
        public object codigoExterno { get; set; }

        [JsonProperty("nombrePaciente")]
        public string nombrePaciente { get; set; }

        [JsonProperty("apellidoPaciente")]
        public string apellidoPaciente { get; set; }

        [JsonProperty("generoPaciente")]
        public string generoPaciente { get; set; }

        [JsonProperty("fechaNacimiento")]
        public DateTime fechaNacimiento { get; set; }

        [JsonProperty("edad")]
        public int edad { get; set; }

        [JsonProperty("estado")]
        public string estado { get; set; }

        [JsonProperty("esMicrobiologia")]
        public bool esMicrobiologia { get; set; }

        [JsonProperty("nombreExamen")]
        public string nombreExamen { get; set; }

        [JsonProperty("siglaExamen")]
        public string siglaExamen { get; set; }

        [JsonProperty("codigoMuestra")]
        public string codigoMuestra { get; set; }

        [JsonProperty("descripcionMuestra")]
        public string descripcionMuestra { get; set; }

        [JsonProperty("tipoResultado")]
        public string tipoResultado { get; set; }

        [JsonProperty("siglasUnidad")]
        public string siglasUnidad { get; set; }

        [JsonProperty("refMinima")]
        public double refMinima { get; set; }

        [JsonProperty("refMaxima")]
        public double refMaxima { get; set; }

        [JsonProperty("panMinima")]
        public double panMinima { get; set; }

        [JsonProperty("panMaxima")]
        public double panMaxima { get; set; }

        [JsonProperty("resultadoActual")]
        public string resultadoActual { get; set; }

        [JsonProperty("ResultadoAnterior")]
        public object ResultadoAnterior { get; set; }

        [JsonProperty("fechaCreación")]
        public DateTime fechaCreación { get; set; }

        [JsonProperty("fechaUltimoResultado")]
        public DateTime fechaUltimoResultado { get; set; }

        [JsonProperty("fechaValidacion")]
        public DateTime fechaValidacion { get; set; }

        [JsonProperty("fechaImpresion")]
        public DateTime fechaImpresion { get; set; }

        [JsonProperty("fechaEnvio")]
        public DateTime fechaEnvio { get; set; }

        [JsonProperty("fechaToma")]
        public DateTime fechaToma { get; set; }

        [JsonProperty("fechaRecepcion")]
        public DateTime fechaRecepcion { get; set; }

        [JsonProperty("usuarioResultado")]
        public int usuarioResultado { get; set; }

        [JsonProperty("usuarioValidacion")]
        public int usuarioValidacion { get; set; }

        [JsonProperty("usuarioToma")]
        public int usuarioToma { get; set; }

        [JsonProperty("usuarioTransporte")]
        public int usuarioTransporte { get; set; }

        [JsonProperty("usuarioRecepcion")]
        public int usuarioRecepcion { get; set; }

        [JsonProperty("transportado")]
        public int transportado { get; set; }

        [JsonProperty("tomado")]
        public bool tomado { get; set; }

        [JsonProperty("recibido")]
        public bool recibido { get; set; }

        [JsonProperty("resultado")]
        public bool resultado { get; set; }

        [JsonProperty("repeticion")]
        public bool repeticion { get; set; }

        [JsonProperty("validado")]
        public bool validado { get; set; }

        [JsonProperty("enviado")]
        public bool enviado { get; set; }

        [JsonProperty("impreso")]
        public bool impreso { get; set; }

        [JsonProperty("Comentario")]
        public object Comentario { get; set; }

        [JsonProperty("MuestraInforme")]
        public bool MuestraInforme { get; set; }

        [JsonProperty("AplicaFormula")]
        public bool AplicaFormula { get; set; }

        [JsonProperty("IdArea")]
        public int IdArea { get; set; }

        [JsonProperty("nombreArea")]
        public string nombreArea { get; set; }

        [JsonProperty("EdadCompleta")]
        public string EdadCompleta { get; set; }

        [JsonProperty("CodigoExamen")]
        public string CodigoExamen { get; set; }

        [JsonProperty("NumeroDecimales")]
        public int NumeroDecimales { get; set; }

        [JsonProperty("activo")]
        public bool activo { get; set; }

        [JsonProperty("resultadoLiteral")]
        public IList<ResultadoLiteral> resultadoLiteral { get; set; }

        [JsonProperty("code")]
        public object code { get; set; }

        [JsonProperty("menasje")]
        public object menasje { get; set; }

        [JsonProperty("IdCorrelacion")]
        public string IdCorrelacion { get; set; }

        [JsonProperty("OrdenArea")]
        public int OrdenArea { get; set; }

        [JsonProperty("OrdenExamen")]
        public int OrdenExamen { get; set; }
    }

    public class ListarResultadosResponse
    {

        [JsonProperty("result")]
        public List<ResultadoOrden> Resultados { get; set; }
    }

}
