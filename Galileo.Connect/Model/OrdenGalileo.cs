using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Galileo.Connect.Model
{

    public class ResultadosACK
    {

        [JsonProperty("opcion")]
        public int opcion { get; set; }

        [JsonProperty("idLaboratorio")]
        public int idLaboratorio { get; set; }

        [JsonProperty("idResultados")]
        public int idResultados { get; set; }
    }

    public class CamposAdicional
    {

        [JsonProperty("nombreCampo")]
        public object NombreCampo { get; set; }

        [JsonProperty("valor")]
        public object Valor { get; set; }
    }

    public class Paciente
    {

        [JsonProperty("Identificador")]
        public string Identificador { get; set; }

        [JsonProperty("NombrePaciente")]
        public string NombrePaciente { get; set; }

        [JsonProperty("ApellidoPaciente")]
        public string ApellidoPaciente { get; set; }

        [JsonProperty("FechaNacimiento")]
        public string FechaNacimiento { get; set; }

        [JsonProperty("Genero")]
        public string Genero { get; set; }

        [JsonProperty("CorreoElectronico")]
        public object CorreoElectronico { get; set; }

        [JsonProperty("CamposAdicional")]
        public IList<CamposAdicional> CamposAdicional { get; set; }
    }

    public class Medico
    {

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
    }

    public class DetalleOrden
    {

        [JsonProperty("NombreExamenPerfil")]
        public string NombreExamenPerfil { get; set; }

        [JsonProperty("CodigoExamen")]
        public string CodigoExamen { get; set; }
    }

    public class InformacionOrden
    {

        [JsonProperty("nombreCampo")]
        public string NombreCampo { get; set; }

        [JsonProperty("valor")]
        public string Valor { get; set; }
    }

    public class TomaMuestra
    {

        [JsonProperty("tomaRemota")]
        public bool TomaRemota { get; set; }

        [JsonProperty("direccionToma")]
        public string DireccionToma { get; set; }

        [JsonProperty("activo")]
        public bool Activo { get; set; }
    }

    public class OrdenGalileo
    {

        [JsonProperty("idLaboratorio")]
        public int IdLaboratorio { get; set; }

        [JsonProperty("idSede")]
        public int IdSede { get; set; }
        
        [JsonProperty("Laboratorio")]
        public string Laboratorio { get; set; }

        [JsonProperty("Cliente")]
        public string Cliente { get; set; }



        [JsonProperty("Servicios")]
        public string Servicios { get; set; }


        [JsonProperty("CieDiagnostico")]
        public string CieDiagnostico { get; set; }

        [JsonProperty("IdSistemaExterno")]
        public string IdSistemaExterno { get; set; }

        [JsonProperty("Paciente")]
        public Paciente Paciente { get; set; }

        [JsonProperty("Medico")]
        public Medico Medico { get; set; }

        [JsonProperty("prioridadOrden")]
        public string PrioridadOrden { get; set; }

        [JsonProperty("fechaIngreso")]
        public string FechaIngreso { get; set; }

        [JsonProperty("comentario")]
        public object Comentario { get; set; }

        [JsonProperty("codigoExterno")]
        public string CodigoExterno { get; set; }

        [JsonProperty("usuario")]
        public string Usuario { get; set; }

        [JsonProperty("detalleOrden")]
        public IList<DetalleOrden> DetalleOrden { get; set; }

        [JsonProperty("informacionOrden")]
        public IList<InformacionOrden> InformacionOrden { get; set; }

        [JsonProperty("tomaMuestra")]
        public TomaMuestra TomaMuestra { get; set; }

        [JsonProperty("activo")]
        public bool Activo { get; set; }
    }


}
    /*
     { 
  "idLaboratorio": 1,
  "idSede": 1,
  "CieDiagnostico": "",
  "IdSistemaExterno": 1,
  "Paciente": {
    "Identificador": "1713937947",
    "NombrePaciente": "Xavier",
    "ApellidoPaciente": "Villafuerte",
    "FechaNacimiento": "2015-11-25T05:36:49.491Z",
    "Genero": "F",
    "CorreoElectronico": "laurita_i2004@hotmail.com",
    "CamposAdicional": [
      {
        "nombreCampo": "EMBARAZO",
        "valor": "SI"
      }
    ]
  },
  "Medico": {
    "Nombre": "Oneciomo",
    "Apellido": "Villegas",
    "Matricula": "1256325",
    "Telefono": "098456325",
    "Email": "ovillegas@hotmail.com"
  },
  "prioridadOrden": "R",
  "fechaIngreso": "2021-11-25T05:36:49.491Z",
  "comentario": "prueba de Integracion",
  "codigoExterno": "EXTERNO0005",
  "usuario": "admin@difare.com",
  "detalleOrden": [
    {
      "CodigoExamen": "CT01",
      "NombreExamenPerfil": "string"
    },
    {
         "CodigoExamen": "BT01",
      "NombreExamenPerfil": "string"
    }

    
  ],
  "informacionOrden": [
    {
      "nombreCampo": "SERVICIO",
      "valor": "UCI"
    }
  ],
  "tomaMuestra": {
    "tomaRemota": true,
    "direccionToma": "string",
    "activo": true
  },
  "activo": true
}
     */




