using Galileo.Connect.Model;
using Galileo.Connect.Model.Resultados;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Galileo.Connect.Manager
{
    public class HISManager
    {


        private string _url;
        private string _token;

        public HISManager(string token)
        {
            _token = token;
        }


        public ImportOrderResponse PostOrden(OrdenGalileo orden, ref string message )
        {

            try
            {
                var url = ConfigurationManager.AppSettings["ServerLIS"];
                var client = new RestClient(url + "ordenesorden/orden/v1/ImportacionOrden");
                client.Timeout = -1;
                var request = new RestRequest(Method.POST);
                request.AddHeader("token", _token);

                request.AddHeader("Content-Type", "application/json");

                request.AddParameter("application/json", JsonConvert.SerializeObject(orden), ParameterType.RequestBody);
                IRestResponse response = client.Execute(request);

                if (response.StatusCode == System.Net.HttpStatusCode.Created)
                {
                    var result = JsonConvert.DeserializeObject<ImportOrderResponse>(response.Content);
                    message = "OK";
                    return result;
                }
                else
                {
                    message = response.Content; ;
                    return null;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
          

            

        }

        public object ConfirmarResultado(List<ResultadosACK> results)
        {


            var url = ConfigurationManager.AppSettings["ServerLIS"];
            var client = new RestClient(url + "resultadosanalitica/analitica/v1/ActualizaEnvioResultado");
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/json");

            request.AddParameter("application/json", JsonConvert.SerializeObject(results), ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            Console.WriteLine(response.Content);
            return response.Content;

        }


        public ListarOrdenesResponse ListarOrdenes(DateTime desde, DateTime hasta, int idLaboratorio)
        {


            var url = ConfigurationManager.AppSettings["ServerLIS"];
            string strDesde = desde.ToString("yyyy-MM-dd");
            string strHasta = hasta.ToString("yyyy-MM-dd");

            var client = new RestClient(url + "ordenesorden/orden/v1/listar?IdLaboratorio=" + idLaboratorio + " &RangoFechas.Inicio=" + strDesde + "&RangoFechas.Fin=" + strHasta);



            client.Timeout = -1;
            var request = new RestRequest(Method.GET);
            request.AddHeader("token", _token);

            IRestResponse response = client.Execute(request);

            var result = JsonConvert.DeserializeObject<ListarOrdenesResponse>(response.Content);



            return result;
        }

        public ListarResultadosResponse ListarResultados(string orden, int idLaboratorio)
        {


            var url = ConfigurationManager.AppSettings["ServerLIS"];
       

            var client = new RestClient(url + "resultadosanalitica/analitica/v1/listarResultados?CodigoOrden=" + orden + "&IdLaboratorio=" + idLaboratorio.ToString());



            client.Timeout = -1;
            var request = new RestRequest(Method.GET);
            request.AddHeader("token", _token);

            IRestResponse response = client.Execute(request);

            string json = "{\"result\":" + response.Content + "}";

            var result = JsonConvert.DeserializeObject<ListarResultadosResponse>(json);



            return result;
        }


        public ConsultarOrden ListarDatosAdicionales(string orden, int idLaboratorio)
        {
            var url = ConfigurationManager.AppSettings["ServerLIS"];
            var client = new RestClient(url + "ordenesorden/orden/v1/consultar?CodigoOrden=" + orden + "&IdLaboratorio=" + idLaboratorio.ToString());

            client.Timeout = -1;
            var request = new RestRequest(Method.GET);
            request.AddHeader("token", _token);

            IRestResponse response = client.Execute(request);

            string json = "{\"result\":" + response.Content + "}";
            var result = JsonConvert.DeserializeObject<ConsultarOrden>(response.Content);
            return result;
        }

        public CosultaMedicoResult datosMedico(int IdMedico, int IdLaboratorio)
        {
            var url = ConfigurationManager.AppSettings["ServerLIS"];
            var client = new RestClient(url+ "configuracionesmedico/medico/v1/consultar?IdLaboratorio=" + IdLaboratorio+"&IdMedico="+IdMedico);
            client.Timeout = -1;
            var request = new RestRequest(Method.GET);
            request.AddHeader("token", _token);
            IRestResponse response = client.Execute(request);
            string json = "{\"result\":" + response.Content+ "}";
            var result = JsonConvert.DeserializeObject<CosultaMedicoResult>(response.Content);
            return result;
        }


        public Resultados GetResultados(DateTime desde, DateTime hasta, int idLaboratorio, string idSedes = "1")
        {


            var url = ConfigurationManager.AppSettings["ServerLIS"];
            string strDesde = desde.ToString("yyyy-MM-dd");
            string strHasta = hasta.ToString("yyyy-MM-dd");

            //var client = new RestClient(url + "analitica/v1/envioResultado?IdLaboratorio=" + idLaboratorio.ToString() + "&IdSedes=" + idSedes + "&RangoFechas.Inicio=" + strDesde + "&RangoFechas.Fin=" + strHasta);
            //https://apigateway.grupodifare.com:8443/gdifare/api/laboratorio/analitica/v1/consultar?IdLaboratorio=9&CodigoOrden=202301310003
            //var client = new RestClient("https://apigateway.grupodifare.com:8443/gdifare/api/laboratorio/orden/v1/consultar?IdLaboratorio=9&CodigoOrden=202301310003");

            var client = new RestClient("https://apigateway.grupodifare.com:8443/gdifare/api/laboratorio/orden/v1/listar");

            client.Timeout = -1;
            var request = new RestRequest(Method.GET); 
            request.AddHeader("token", _token);

            IRestResponse response = client.Execute(request);

            var result = JsonConvert.DeserializeObject<Resultados>(response.Content);



            return result;
        }

    }
}
