using RestSharp;
using System;
using Galileo.Connect.Model;

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Configuration;

namespace Galileo.Connect.Manager
{

    
    public class InstrumentManager
    {

        private string _url;
        private string _identificador;
        private string _idLaboratorio;
        private Instrumento _instrumento;
        private string _separadorMuestra;
        private string _token;


        public Instrumento Instrumento { get => _instrumento; set => _instrumento = value; }

        public InstrumentManager(string url, string identificador, int idLaboratorio, string separadorMuestra, string token)
        {
            _url = url;
            _identificador = identificador;
            _idLaboratorio = idLaboratorio.ToString();
            _separadorMuestra = separadorMuestra;
            _token = token;
        }




        public  bool GetInstrumento()
        {
            try
            {
                var client = new RestClient(_url + "instrumentosinstrumento/instrumento/v1/consultar?IdLaboratorio=" + _idLaboratorio + "&Identificador=" + _identificador + "&Activo=true");

          
                client.Timeout = -1;
                var request = new RestRequest(Method.GET);
                request.AddHeader("token",_token );


                IRestResponse response = client.Execute(request);

                var result = JsonConvert.DeserializeObject<InstrumentoResponse>(response.Content);
                Instrumento i = new Instrumento();
                if (result.Instrumento == null)
                {
                    
                    return false;
                }

                if (result.Instrumento.Count > 0)
                    if (result.Instrumento[0].Details.Count > 0)
                    {
                        i = result.Instrumento[0].Details[0];
                        i.DetallesInstrumento = GetDetalleInstrumento(i.IdInstrumento);

                    }

                i.SpecimenSeparator = _separadorMuestra;

                Instrumento = i;

                return true;
            }
            catch (Exception)
            {

                return false;
            }
           
        }

        public List<DetalleInstrumento> GetDetalleInstrumento(Int64 idInstrumento)
        {
            var client = new RestClient(_url + "instrumentosdetalleinstrumento/detalleinstrumento/v1/consultar?IdInstrumento=" + idInstrumento.ToString() +"&IdLaboratorio=" + _idLaboratorio);

            client.Timeout = -1;
            var request = new RestRequest(Method.GET);
            request.AddHeader("token", _token);


            IRestResponse response = client.Execute(request);
            List<DetalleInstrumento> details = new List<DetalleInstrumento>();
            var result = JsonConvert.DeserializeObject<DetalleInstrumentoResponse>(response.Content);

            foreach (var item in result.DetalleInstrumento[0].Details)
            {
                if (item.Activo == true)
                {
                    details.Add(item);
                }
            }

           
            //if (result.DetalleInstrumento.Count > 0)
            //    details.AddRange(result.DetalleInstrumento[0].Details);
            

            foreach (var item in details)
            {
                item.Conversiones = new List<InstrumentoConversion>();
                var convs = GetInstrumentoConversion(item.IdDetallesInstrumento);
                

                if (convs.Count()>0)
                    item.Conversiones.AddRange(convs);
            }


                return details; ;

        }

        public List<InstrumentoConversion> GetInstrumentoConversion(Int64 idDetalleInstrumento)
        {
            var client = new RestClient(_url + "instrumentosinstrumentoconversion/instrumentoconversion/v1/consultar?IdDetallesInstrumento=" + idDetalleInstrumento.ToString() + "&IdLaboratorio=" + _idLaboratorio);

            client.Timeout = -1;
            var request = new RestRequest(Method.GET);
            request.AddHeader("token", _token);


            IRestResponse response = client.Execute(request);
            List<InstrumentoConversion> details = new List<InstrumentoConversion>();
            var result = JsonConvert.DeserializeObject<InstrumentoConversionResponse>(response.Content);

            if (result.InstrumentoConversion.Count > 0)
                details.AddRange(result.InstrumentoConversion[0].Details);


            return details; ;

        }


        public Galileo.Connect.Model.OrderInfo.OrderInfo GetOrdenInfo(string etiqueta)
        {



            var client = new RestClient(_url + "ordenesorden/orden/v1/consultar?CodigoOrden="  + etiqueta +"&IdLaboratorio=" + _idLaboratorio);

            client.Timeout = -1;
            var request = new RestRequest(Method.GET);
            request.AddHeader("token", _token);


            IRestResponse response = client.Execute(request);

            var result = JsonConvert.DeserializeObject<Galileo.Connect.Model.OrderInfo.OrderInfo>(response.Content);




            return result;

        }



        public List<OrdenResponse> GetOrden(string  etiqueta)
        {

            string[] partesOrden = etiqueta.Split(Instrumento.SpecimenSeparator, StringSplitOptions.TrimEntries);

            string orden = partesOrden[0];
            string muestra = partesOrden[1];

            var client = new RestClient(_url + "instrumentosanalitica/analitica/v1/ConsultarAnalizadoresMuestra?IdLaboratorio=" + Instrumento.IdLaboratorio.ToString()+ "&Identificador=" + Instrumento.Identificador.ToString() + "&CodigoMuestra=" + muestra +"&CodigoOrden=" + orden);

            client.Timeout = -1;
            var request = new RestRequest(Method.GET);
            request.AddHeader("token", _token);


            IRestResponse response = client.Execute(request);
            
            var result = JsonConvert.DeserializeObject<List<OrdenResponse>>(response.Content);

            


            return result; 

        }

        public List<Label> GetMuestrasPorOrden(string ordenFrom, string ordenTo)
        {

           

            var client = new RestClient(_url + "ordenesorden/orden/v1/listarEtiquetas?IdLaboratorio=" + _idLaboratorio +  "&RangoOrdenes.Inicio=" + ordenFrom + "&RangoOrdenes.Fin=" + ordenTo);

            client.Timeout = -1;
            var request = new RestRequest(Method.GET);
            request.AddHeader("token", _token);


            IRestResponse response = client.Execute(request);

            var result = JsonConvert.DeserializeObject<Galileo.Connect.Model.SampleQuery.Result>(response.Content);


            List<Label> labels = new List<Label>();

            foreach (var item in result.Ordenes.Details)
            {
                var numOrden = item.CodigoOrden;
                
                var detalleOrden = GetOrdenInfo(numOrden);

                var muestras = item.Muestra.Distinct().ToList();
                

                foreach (var sample in muestras)
                {

                    if (labels.Where(x => x.SampleId == item.CodigoOrden + "-" + sample.CodigoMuestra).Count() == 0)
                    {
                        Label l = new Label();
                        l.LabName = detalleOrden.NombreLaboratorio;
                        l.PatientName = detalleOrden.ApellidoPaciente + " " + detalleOrden.NombrePaciente;
                        l.PatientId = detalleOrden.IdPaciente.ToString();
                        l.SampleId = item.CodigoOrden + "-" + sample.CodigoMuestra;
                        l.Container = sample.Recipiente.NombreRecipiente;
                        l.SampleName = sample.NombreMuestra;
                        l.Source = detalleOrden.NombreSede;
                        l.Count = sample.NumeroEtiquetas;
                        l.PatientAge = detalleOrden.EdadCompleta;
                        l.Date = detalleOrden.FechaIngreso.ToString("dd-MM-yyyy");

                        for (int i = 0; i < l.Count; i++)
                        {
                            labels.Add(l);
                        }
                    }
                }


                


            }

            return labels;



        }


        public List<OrdenResponse> GetOrden(DateTime desde, DateTime hasta)
        {

            string strDesde = desde.ToString("yyyy-MM-dd");
            string strHasta = hasta.ToString("yyyy-MM-dd");

            var client = new RestClient(_url + "instrumentosanalitica/analitica/v1/ConsultarAnalizadores?IdLaboratorio=" + Instrumento.IdLaboratorio.ToString() + "&Identificador=" + Instrumento.Identificador + "&RangoFechas.Inicio=" + strDesde + "&RangoFechas.Fin=" + strHasta);
            
            client.Timeout = -1;
            var request = new RestRequest(Method.GET);
            request.AddHeader("token", _token);

            IRestResponse response = client.Execute(request);

            var result = JsonConvert.DeserializeObject<List<OrdenResponse>>(response.Content);




            return result;

        }

        public OrdenResponse GetOrdenMuestra2(string orden)
        {

            DateTime fecha = new DateTime(Convert.ToInt32(orden.Substring(0, 4)), Convert.ToInt32(orden.Substring(4, 2)), Convert.ToInt32(orden.Substring(6, 2)));
            
            string strDesde = fecha.ToString("yyyy-MM-dd");
            string strHasta = fecha.ToString("yyyy-MM-dd");

            var client = new RestClient(_url + "instrumentosanalitica/analitica/v1/ConsultarAnalizadores?IdLaboratorio=" + Instrumento.IdLaboratorio.ToString() + "&Identificador=" + Instrumento.Identificador + "&RangoFechas.Inicio=" + strDesde + "&RangoFechas.Fin=" + strHasta);

            client.Timeout = -1;
            var request = new RestRequest(Method.GET);
            request.AddHeader("token", _token);

            IRestResponse response = client.Execute(request);

            var result = JsonConvert.DeserializeObject<List<OrdenResponse>>(response.Content);

            OrdenResponse r = null;

            foreach (var item in result)
            {
                if (item.CodigoOrden == orden)
                    return item;
            }


            return r;

        }

        public List<OrdenMuestraResponse> GetOrdenMuestra(string orden, string codMuestra)
        {

          

            var client = new RestClient(_url + "instrumentosanalitica/analitica/v1/ConsultarAnalizadoresMuestra?IdLaboratorio=" + Instrumento.IdLaboratorio.ToString() + "&CodigoOrden=" + orden + "&Identificador=" +Instrumento.Identificador + "&CodigoMuestra=" + codMuestra);

            client.Timeout = -1;
            var request = new RestRequest(Method.GET);
            request.AddHeader("token", _token);

            IRestResponse response = client.Execute(request);

            var result = JsonConvert.DeserializeObject<List<OrdenMuestraResponse>>(response.Content);




            return result;

        }



        public string PostResult(string body)
        {
            var url = ConfigurationManager.AppSettings["ServerLIS"];
            var client = new RestClient(url + "instrumentosanalitica/analitica/v1/registrarResultadoAnalizadores");
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("token", _token);

            request.AddHeader("Content-Type", "application/json");

            
            request.AddParameter("application/json", body, ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            Console.WriteLine(response.Content);
            return response.Content;
        }
        
    }
}
