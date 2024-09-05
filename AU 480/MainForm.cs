
using Galileo.Utils;
using Galileo.Utils.ASTMModel;
using Galileo.Utils.HL7Model;
using Galileo.Connect.Manager;
using Galileo.Connect.Model;
using System;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.IO.Ports;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Newtonsoft.Json;
using Microsoft.VisualBasic;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using System.Linq;
using Galileo.Protocol3_1;
using Galileo.Utils.ConversionModel;
using System.Text.RegularExpressions;
using Galileo.Utils.BC_AU480;

namespace Galileo.Online
{
    public partial class MainForm : Form
    {

        public string ServerLIS { get { return ConfigurationManager.AppSettings["ServerLIS"]; } }

        public string ServerSeguridad { get { return ConfigurationManager.AppSettings["ServerSeguridad"]; } }


        public string Instrumento { get { return ConfigurationManager.AppSettings["Instrumento"]; } }
        public string Sender { get { return ConfigurationManager.AppSettings["Sender"]; } }
        public string InstrumentoImg { get { return ConfigurationManager.AppSettings["Imagen"]; } }

        public string ConnectionMode { get { return ConfigurationManager.AppSettings["ConnectionMode"]; } }

        public string ConnectionSetup { get { return ConfigurationManager.AppSettings["ConnectionSetup"]; } }

        public string InstrumentIP { get { return ConfigurationManager.AppSettings["InstrumentIP"]; } }
        public string LocalPort { get { return ConfigurationManager.AppSettings["LocalPort"]; } }

        public string COM { get { return ConfigurationManager.AppSettings["COM"]; } }
        public string Bauds { get { return ConfigurationManager.AppSettings["Bauds"]; } }
        public string DataBits { get { return ConfigurationManager.AppSettings["DataBits"]; } }

        public string Protocol { get { return ConfigurationManager.AppSettings["Protocol"]; } }

        public string ProcessFolder { get { return ConfigurationManager.AppSettings["ProcessFolder"]; } }
        public string WorkListFolder { get { return ConfigurationManager.AppSettings["WorkListFolder"]; } }

        public string ProcessedFolder { get { return ConfigurationManager.AppSettings["ProcessedFolder"]; } }

        public string License { get { return ConfigurationManager.AppSettings["License"]; } }

        public string SeparadorMuestra { get { return ConfigurationManager.AppSettings["SeparadorMuestra"]; } }

        public string OrderField { get { return ConfigurationManager.AppSettings["OrderField"]; } }

        public string TestName { get { return ConfigurationManager.AppSettings["TestName"]; } }

        public string TestValue { get { return ConfigurationManager.AppSettings["TestValue"]; } }

        public int IdLaboratorio;
        public string IdInstrumento;
        public string ApiToken;
        public string SerialMessage;

        private static string mensaje;


        SerialPort mySerialPort;


        public enum LogEvent
        {
            Recibiendo,
            Enviando,
            Error,
            Esperando,
            Conectado,
            Desconectado
        }


        public LogHelper log;
        public CaptureHelper cap;
        public EncryptHelper enc;
        public FileHelper fil;
        public InstrumentManager insMngr;




        public MainForm()
        {
            InitializeComponent();
        }


        private void EventWriterRxTx(string emisor, string textLog, bool includeDateLog = true)
        {

            string imprimir = textLog.Replace("<CR><LF>", "<CR>\n");
            imprimir = imprimir.Replace("<LF><CR>", "<CR>\n");
            imprimir = imprimir.Replace("<CR>", "<CR>\n");
            imprimir = imprimir.Replace("<LF>", "<CR>\n");

            //string screenLog = DateTime.Now + "\t" + textLog;
            this.Invoke((MethodInvoker)delegate
            {

                var msgs = imprimir.Split("\n", StringSplitOptions.RemoveEmptyEntries);

                foreach (var item in msgs)
                {
                    Color foreColor = Color.White;


                    logScreen.SelectionStart = logScreen.TextLength;
                    logScreen.SelectionLength = 0;

                    logScreen.SelectionColor = foreColor;
                    logScreen.AppendText(DateTime.Now + "\t" + emisor + ":\t" + item);
                    logScreen.AppendText(Environment.NewLine);
                    logScreen.SelectionColor = logScreen.ForeColor;


                    log.Add(DateTime.Now + "\t" + emisor + ":\t" + item, includeDateLog);

                }





            });

        }

        private void EventWriterRxTxDebug(string emisor, string textLog, bool includeDateLog = true)
        {

            //string imprimir = textLog.Replace("<CR><LF>", "<CR>\n");
            //imprimir = imprimir.Replace("<LF><CR>", "<CR>\n");
            //imprimir = imprimir.Replace("<CR>", "<CR>\n");
            //imprimir = imprimir.Replace("<LF>", "<CR>\n");

            //string screenLog = DateTime.Now + "\t" + textLog;
            this.Invoke((MethodInvoker)delegate
            {


                Color foreColor = Color.White;


                logScreen.SelectionStart = logScreen.TextLength;
                logScreen.SelectionLength = 0;

                logScreen.SelectionColor = foreColor;
                logScreen.AppendText(DateTime.Now + "\t" + emisor + ":\t" + textLog);
                logScreen.AppendText(Environment.NewLine);
                logScreen.SelectionColor = logScreen.ForeColor;


                log.Add(DateTime.Now + "\t" + emisor + ":\t" + textLog, includeDateLog);







            });

        }


        private void MainForm_Load(object sender, EventArgs e)
        {

            InicializarApp();

        }

        private void InicializarApp()
        {
            string LogPath = Environment.CurrentDirectory + "\\logs";
            string CapPath = Environment.CurrentDirectory + "\\data";








            #region ManagersAndHelpers


            log = new LogHelper(LogPath);
            cap = new CaptureHelper(CapPath);
            enc = new EncryptHelper();
            fil = new FileHelper();


            #endregion





            var key = "b14ca5898a4e4133bbce2ea2315a1916";
            string[] licenceDecrypt = enc.DecryptString(key, License).Split(";");

            IdLaboratorio = Convert.ToInt32(licenceDecrypt[1]);
            IdInstrumento = licenceDecrypt[0];
            ApiToken = licenceDecrypt[2];

            EventWriter(true, "Conectando con GALILEO ONLINE", LogEvent.Esperando, "Conectando");

            insMngr = new InstrumentManager(ServerLIS, IdInstrumento, IdLaboratorio, SeparadorMuestra, ApiToken);

            bool conectado = true;

            if (ConnectionSetup == "SERVER")
            {
                if (insMngr.GetInstrumento())
                {
                    //Configuraciones Locales
                    EventWriter(true, "Se encuentra el analizador configurado.", LogEvent.Conectado, "Conectado");
                }
                else
                {
                    EventWriter(true, "No implementada la sincronización con el servidor. Seleccione LOCAL.", LogEvent.Error, "OFFLINE");
                    conectado = false;
                }
            }
            else
            {

                EventWriter(true, "Se encuentra el analizador configurado de manera local.", LogEvent.Conectado, "Conectado");

            }








            if (conectado)
            {
                string config = JsonConvert.SerializeObject(insMngr.Instrumento);
                fil.WriteToFile(config, "Config.Json", Environment.CurrentDirectory + "\\DB\\");
                log.Add("Inicia la aplicación");
                this.Text = this.Text + this.Instrumento;
                this.lblNombreInstrumento.Text = this.Text;

                picInstrumento.Image = Image.FromFile(Environment.CurrentDirectory + "\\Images\\" + this.InstrumentoImg);
                log.Add("Esperando Conexión del Instrumento " + this.Instrumento);


                if (ConnectionMode == "TCP")
                {
                    Thread threadInput = new Thread(ListenTCP);
                    threadInput.Start();
                }
                else if (ConnectionMode == "FILES")
                {
                    Thread threadInput = new Thread(ListenFiles);
                    threadInput.Start();
                }
                else if (ConnectionMode == "SERIAL")
                {
                    Thread threadInput = new Thread(ListenSerial);
                    threadInput.Start();
                }
            }
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
            //Ocultamos el icono de la bandeja de sistema


        }

        private void frmPrincipal_Resize(object sender, EventArgs e)
        {

        }

        private void SetState(bool displayLoader, string action)
        {
            if (displayLoader)
            {
                this.Invoke((MethodInvoker)delegate
                {
                    stateProgressBar.Value = 100;

                    statusLabel.Text = action;
                    //this.Cursor = System.Windows.Forms.Cursors.WaitCursor;
                });
            }
            else
            {
                this.Invoke((MethodInvoker)delegate
                {
                    stateProgressBar.Value = 0;
                    stateProgressBar.Text = action;
                    //this.Cursor = System.Windows.Forms.Cursors.Default;
                });
            }
        }

        private void EventWriter(bool displayUser, string textLog, LogEvent logEvent, string statusText, bool includeDateLog = true)
        {
            string screenLog = DateTime.Now + "\t" + textLog;
            this.Invoke((MethodInvoker)delegate
            {
                if (displayUser)
                {

                    Color foreColor = Color.White;

                    switch (logEvent)
                    {
                        case LogEvent.Recibiendo:
                            foreColor = Color.LightGreen;
                            break;
                        case LogEvent.Enviando:
                            foreColor = Color.Blue;
                            break;
                        case LogEvent.Error:
                            foreColor = Color.Red;
                            break;
                        default:
                            break;
                    }

                    logScreen.SelectionStart = logScreen.TextLength;
                    logScreen.SelectionLength = 0;

                    logScreen.SelectionColor = foreColor;
                    logScreen.AppendText(screenLog);
                    logScreen.AppendText(Environment.NewLine);
                    logScreen.SelectionColor = logScreen.ForeColor;
                }

                log.Add(textLog, includeDateLog);

                stateProgressBar.Value = 100;

                statusLabel.Text = statusText;

            });

        }



        private void ListenTCP()
        {


            var appSettings = ConfigurationManager.AppSettings;




            FileHelper fHlp = new();
            ImageHelper iHlp = new();


            TcpListener server = new TcpListener(IPAddress.Any, Convert.ToInt32(LocalPort));
            //TcpListener server = new TcpListener(IPAddress.Parse(InstrumentIP), Convert.ToInt32(LocalPort));
            server.Start();
            SetState(false, "Conectado");

            byte[] bytes = new Byte[40000];
            String data = null;

            EventWriter(true, "Escuchando el puerto: " + LocalPort + " por TCP/IP", LogEvent.Conectado, "Conectado");

            while (true)
            {






                TcpClient clt;
                if (server.Pending())
                {

                    Thread tmp_thread = new Thread(new ThreadStart(() =>
                    {
                        //EventWriter(false, "Recibiendo: " + LocalPort, LogEvent.Conectado, "Recibiendo");
                        int i = 0;
                        clt = server.AcceptTcpClient();

                        NetworkStream stream = clt.GetStream();



                        while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                        {

                            data = System.Text.Encoding.ASCII.GetString(bytes, 0, i);

                            ReceiveTCPData(data);


                            SendTCPData(stream, ASTM.Character.ACK);



                        }



                    }));
                    tmp_thread.Start();

                }
                else
                {
                    Thread.Sleep(1000); //<--- timeout
                }


            }

        }

        private void SendTCPData(NetworkStream stream, string data)
        {

        }

        private void ReceiveTCPData(string data)
        {

        }

        private void ReadMessage(string data, ref string result)
        {
            //try
            //{



            if (data != "")
            {



                //EventWriter(true,ASTM.WriteSpecialChars(data), LogEvent.Conectado, "Recibiendo", false);


                if (Protocol == "HL7")
                {
                    HL7Helper helper = new HL7Helper();
                    helper.Content = data;
                    HL7Record obj = helper.ToObject();

                    foreach (var msg in obj.messages)
                    {
                        try
                        {
                            if (msg.ObservationRequest != null)
                            {
                                //EventWriter(true, "Procesando el mensaje HL7 para la muestra " + msg.ObservationRequest.FillerOrderNumber + " desde " + msg.Header.SendingApplication, LogEvent.Conectado, "Recibiendo");
                                ProcessOrder(msg);


                            }
                            else
                            {
                                EventWriter(true, "Mensaje HL7 Omitido", LogEvent.Conectado, "Conectado");

                            }


                        }
                        catch (Exception ex)
                        {
                            //throw ex;
                            // EventWriter(true, "Mensaje HL7 no válido, no se reconoce ", LogEvent.Error, "Error");

                        }
                    }
                }
                else if (Protocol == "ASTM 1394")
                {



                    ASTMMessage msg = new ASTMMessage(data);
                    if (msg.header != null && msg.header.RecordTypeId == "H")
                    {

                        if (msg.PatienInformationList.Count > 0)
                        {
                            foreach (var mf in msg.PatienInformationList[0].OrderRecordList[0].ManufacturRecordList)
                            {
                                ImageHelper imgHelper = new ImageHelper();
                                imgHelper.DrawImage(mf.DataMeasurement.Value, mf.TestIdentifier.Manufacturer);

                            }

                            ProcessOrder(msg);

                        }
                        else
                        {

                            result = ProcessQuery(msg);

                        }

                    }
                    else
                    {
                        EventWriter(true, "Mensaje ASTM Omitido", LogEvent.Conectado, "Conectado");
                    }

                }
                else if (Protocol == "3.1PROTOCOL")
                {
                    var msgs = data.Split(ASTM.Character.ETX);
                    foreach (var item in msgs)
                    {
                        Protocol3_1Message msg = new Protocol3_1Message(item);
                        if (msg.resultRecord.details.Count > 0)
                        {
                            ProcessOrder(msg);
                            var str = item;
                        }
                    }


                }
                else if (Protocol == "AU4080")
                {
                    //var chars = data.ToCharArray();

                    //List<string> messages = new List<string>();

                    var messages = data.Split(ASTM.Character.ETX, StringSplitOptions.RemoveEmptyEntries);



                    List<AUMessage> auMsgs = new List<AUMessage>();

                    foreach (var item in messages)
                    {
                        try
                        {

                            string clean_item = ASTM.RemoveSpecialChars(item);

                            if (clean_item == "DE" || clean_item == "DB")
                                break;


                            AUMessage auMsg = new AUMessage(clean_item);
                            if (auMsg.Patient != null)
                            {
                                auMsgs.Add(auMsg);
                                EventWriter(true, "Orden " + auMsg.Patient.SampleId, LogEvent.Recibiendo, "Interpretando", true);
                            }
                            else if (auMsg.SampleQuery != null)
                            {
                                EventWriter(true, "Consultando pruebas para la orden: " + auMsg.SampleQuery, LogEvent.Esperando, "Procesando", false);

                                string orden = auMsg.SampleQuery;
                                string muestra = "0";
                                if (auMsg.SampleQuery.Contains("-"))
                                {
                                    orden = auMsg.SampleQuery.Split("-", StringSplitOptions.TrimEntries)[0];
                                    muestra = auMsg.SampleQuery.Split("-", StringSplitOptions.TrimEntries)[1];

                                }
                                else
                                {
                                    orden = "202310260003";
                                    muestra = "3";
                                }


                                var orders = insMngr.GetOrdenMuestra(orden, muestra);

                                if (orders.Count > 0)
                                {

                                    var order =
                                        orders[0];


                                    // Obtener la fecha actual
                                    DateTime fechaActual = DateTime.Now;

                                    // Supongamos que la fecha de nacimiento es el 1 de enero de 1990
                                    DateTime fechaNacimiento = order.FechaNacimiento;

                                    // Calcular la diferencia en años
                                    int diferenciaAnios = fechaActual.Year - fechaNacimiento.Year;

                                    // Verificar si el día de nacimiento ya ocurrió este año
                                    if (fechaActual.Month < fechaNacimiento.Month || (fechaActual.Month == fechaNacimiento.Month && fechaActual.Day < fechaNacimiento.Day))
                                    {
                                        diferenciaAnios--;
                                    }

                                    // Asignar valores a las variables

                                    /*
                                    string tipoRespuesta = "S";
                                    string numeroRack = auMsg.RackNumber;
                                    string posicionCopa = auMsg.CupPostion;
                                    string tipoMuestra = "";
                                    string numeroMuestra1 = auMsg.NumeroMuestra;
                                    string codigoBarras = auMsg.SampleQuery;
                                    string numeroMuestraOriginal = auMsg.SampleQuery;
                                    */
                                    string responseHeader = auMsg.ResponseHeader;
                                    string dummy = "";
                                    string clasificacionData = "E";
                                    string sexo = order.Genero;
                                    string edadAnios = diferenciaAnios.ToString();
                                    string edadMeses = "00";
                                    string informacionPaciente1 = "Paciente1";
                                    string informacionPaciente2 = "Paciente2";
                                    string informacionPaciente3 = order.ApellidoPaciente;
                                    string informacionPaciente4 = "Paciente4";
                                    string informacionPaciente5 = "Paciente5";
                                    string informacionPaciente6 = "Paciente6";

                                    // Definir los strings de OnlineTest
                                    List<string> onlineTest = new List<string>();

                                    foreach (var r in order.Resultados)
                                    {
                                        onlineTest.Add(r.CodigoExamenHomologado);   
                                    }
                                    


                                    // Verificar y ajustar la longitud de las cadenas según la especificación
                                    
                                    dummy = dummy.PadRight(4).Substring(0, 4);
                                    clasificacionData = clasificacionData.PadRight(1).Substring(0, 1);
                                    sexo = sexo.PadRight(1).Substring(0, 1);
                                    edadAnios = edadAnios.PadLeft(3, '0').Substring(0, 3);
                                    edadMeses = edadMeses.PadLeft(2, '0').Substring(0, 2);

                                    // Construir la cadena
                                    StringBuilder resultado = new StringBuilder();
                               
                                    resultado.Append(responseHeader);
                                    resultado.Append(dummy);
                                    resultado.Append(clasificacionData);
                                    resultado.Append(sexo);
                                    resultado.Append(edadAnios);
                                    resultado.Append(edadMeses);
                                    resultado.Append(informacionPaciente1.PadRight(20).Substring(0, 20));
                                    resultado.Append(informacionPaciente2.PadRight(20).Substring(0, 20));
                                    resultado.Append(informacionPaciente3.PadRight(20).Substring(0, 20));
                                    resultado.Append(informacionPaciente4.PadRight(20).Substring(0, 20));
                                    resultado.Append(informacionPaciente5.PadRight(20).Substring(0, 20));
                                    resultado.Append(informacionPaciente6.PadRight(20).Substring(0, 20));

                                    // Concatenar strings adicionales desde onlineTest
                                    foreach (string onlineTestString in onlineTest)
                                    {
                                        resultado.Append(onlineTestString.PadRight(3).Substring(0, 3));
                                    }


                                    string aux = resultado.ToString();

                                    char[] caracteres = aux.ToCharArray();
                                    caracteres[8] = ' ';
                                    
                                    
                                    result = new string(caracteres);

                                    //result = resultado.ToString();


                                    //StringBuilder pruebas = new StringBuilder();
                                    //foreach (var p in order[0].Resultados)
                                    //{
                                    //    pruebas.Append(p.CodigoExamenHomologado);
                                    //}

                                    //StringBuilder sb = new StringBuilder();
                                    //sb.Append("S ");
                                    //sb.Append(auMsg.RackNumber);
                                    //sb.Append(auMsg.CupPostion);
                                    //sb.Append(" ");
                                    //sb.Append(auMsg.NumeroMuestra);
                                    //sb.Append(auMsg.SampleQuery);
                                    //sb.Append("    ");//Numero de muestra por 4 car
                                    //sb.Append("    ");//Dummy por 4 car
                                    //sb.Append("E");
                                    //sb.Append("M");//Sexo
                                    //sb.Append("03011"); // EDAD YYYMM
                                    //sb.Append("".PadRight(120, Convert.ToChar(" ")));
                                    //sb.Append(pruebas);

                                    ////int espaciosPorAgregar =  84- clean_item.Length;
                                    ////string espacios = new string(' ', espaciosPorAgregar);

                                    ////clean_item = clean_item.Remove(0,1).Insert(0, "S");

                                    ////string response = clean_item + espacios;

                                    //result = sb.ToString();
                                    return;

                                }

                                return;
                            }
                        }
                        catch (Exception ex)
                        {
                            var error = ex.Message;
                            result = "Error";
                        }
                    }
                    ProcessOrder(auMsgs);
                    result = "OK";


                    /*
                    StringBuilder msg = new StringBuilder();
                    foreach (var item in chars)
                    {
                        if (item.ToString() != ASTM.Character.ETX)
                        {
                            if (item.ToString() != ASTM.Character.STX)
                            {
                                msg.Append(item);
                            }
                        }
                        else
                        {
                            //msg.Append(item);
                            messages.Add(msg.ToString());

                            AUMessage auMsg = new AUMessage(msg.ToString());
                            auMsgs.Add(auMsg);

                            msg = new StringBuilder();
                        }
                    }
                    */


                }



            }
            Thread.Sleep(3000);
        }

        private string ProcessQuery(ASTMMessage msg)
        {
            ASTMMessage r = new ASTMMessage();
            r.header = new Utils.ASTMModel.MessageHeader();
            r.header.DelimiterDefinition = "|\\^&";
            r.header.SenderNameId = "GalileoLIS";
            r.header.TimeOfMessage = DateTime.Now;

            PatientInformation p = new PatientInformation();
            p.RecordTypeId = "P";
            p.SequenceNumber = "1";
            p.PatientName = "VILLA^CAMILA";
            p.BirthDate = new DateTime(2021, 2, 6);


            p.OrderRecordList = new List<OrderRecord>();
            OrderRecord o = new OrderRecord();

            o.RecordTypeId = "O";
            o.SecuenceNumber = "1";
            o.SpecimenId = "SpecimenId1";
            //^^^AFP
            o.UniversalTestIdPartOne = "";
            o.UniversalTestIdPartTwo = "";
            o.UniversalTestIdPartThree = "AFP";
            o.SpecimenDescriptor = "S";
            o.RequestedOrderderedDateTime = DateTime.Now;
            o.SpecimenCollectionDateTime = DateTime.Now;
            o.ReportTypes = "X";

            p.OrderRecordList.Add(o);


            r.PatienInformationList = new List<PatientInformation>();
            r.PatienInformationList.Add(p);

            r.Terminator = new MessageTerminator();
            r.Terminator.RecordTypeId = "L";
            r.Terminator.SecuenceNumber = "1";
            r.Terminator.TerminationCode = "N";

            var result_astm = r.Serialize();








            return result_astm;
        }

        private void ProcessOrder(HL7Message msg)
        {
            string jsonResult = JsonConvert.SerializeObject(msg);

            EventWriter(false, JsonConvert.SerializeObject(msg, Newtonsoft.Json.Formatting.Indented), LogEvent.Esperando, "Procesando Mensaje", false);

            JsonHelper message = new JsonHelper(jsonResult);

            string sender = message.GetValue("ObservationRequest/FillerOrderNumber");

            var BarCode = message.GetValue(OrderField);
            var OrderNumber = BarCode.Split(SeparadorMuestra)[0];
            var CodigoMuestra = BarCode.Split(SeparadorMuestra)[1];



            string ImagePath = Environment.CurrentDirectory + "\\result_images\\" + sender;
            if (Directory.Exists(ImagePath))
            {
                Directory.Delete(ImagePath, true);
                Directory.CreateDirectory(ImagePath);
            }


            EventWriter(true, jsonResult, LogEvent.Esperando, "Procesando Mensaje");







            List<GalileoPostResultRequest> results = new List<GalileoPostResultRequest>();

            foreach (var result in msg.ObservationResultList)
            {
                JsonHelper line = new JsonHelper(JsonConvert.SerializeObject(result));
                var testName = line.GetValue(TestName);
                var testValue = line.GetValue(TestValue);

                foreach (var item in insMngr.Instrumento.DetallesInstrumento)
                {
                    if (item.Homologacion == testName)
                    {
                        foreach (var convertion in item.Conversiones)
                        {
                            ConvertionHelper h = new ConvertionHelper();
                            try
                            {

                                h.Evaluate(JsonConvert.DeserializeObject<Formula>(convertion.Formula), ref testValue);
                            }
                            catch (Exception ex)
                            {
                                string g = ex.Message;
                            }


                        }

                        GalileoPostResultRequest rq = new GalileoPostResultRequest();

                        rq.IdLaboratorio = IdLaboratorio;
                        rq.CodigoOrden = OrderNumber;
                        rq.CodigoMuestra = CodigoMuestra;
                        rq.CodigoExamenHomologado = item.Homologacion;
                        rq.Validado = false;
                        try
                        {
                            rq.ValorNumero = Convert.ToDouble(testValue);
                            rq.ValorTexto = null;


                        }
                        catch
                        {
                            rq.ValorTexto = testValue;
                        }

                        rq.Identificador = insMngr.Instrumento.Identificador;
                        rq.Orden = item.Orden;


                        results.Add(rq);


                    }


                }


            }


            EventWriter(false, JsonConvert.SerializeObject(JsonConvert.SerializeObject(results.OrderBy(x => x.Orden).ToList()), Formatting.Indented), LogEvent.Enviando, "Enviando a Galileo");
            var resultMsg = insMngr.PostResult(JsonConvert.SerializeObject(results.OrderBy(x => x.Orden).ToList()));
            EventWriter(true, resultMsg, LogEvent.Enviando, "Respuesta de Galileo");



        }


        private void ProcessOrder(List<AUMessage> msg)
        {
            string jsonResult = JsonConvert.SerializeObject(msg);
            //EventWriter(true, jsonResult, LogEvent.Esperando, "Enviando a Galileo");

            JsonHelper message = new JsonHelper(jsonResult);

            string sender = message.GetValue("header/SenderNameId");





            List<GalileoPostResultRequest> results = new List<GalileoPostResultRequest>();


            foreach (var order in msg)
            {
                JsonHelper orderJson = new JsonHelper(JsonConvert.SerializeObject(order));

                var OrderNumber = "";
                var CodigoMuestra = "";

                var BarCode = order.Patient.SampleId;
                if (BarCode != null)
                {
                    OrderNumber = BarCode.Split(SeparadorMuestra)[0];
                    CodigoMuestra = BarCode.Split(SeparadorMuestra)[1];
                }
                else
                {
                    MessageBox.Show("No se encuentra la orden en el campo: " + OrderField, "Error de Configuración", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                    BarCode = "202306280010-2";
                    OrderNumber = BarCode.Split(SeparadorMuestra)[0];
                    CodigoMuestra = BarCode.Split(SeparadorMuestra)[1];
                }

                var muestras = insMngr.GetOrdenMuestra(OrderNumber, CodigoMuestra);

                if (muestras.Count == 0)
                {
                    EventWriter(true, "No existen pruebas pendientes para: " + OrderNumber + "-" + CodigoMuestra, LogEvent.Error, "Recibiendo", false);

                }
                else
                {
                    string ImagePath = Environment.CurrentDirectory + "\\result_images\\" + sender;
                    if (Directory.Exists(ImagePath))
                    {
                        Directory.Delete(ImagePath, true);
                        Directory.CreateDirectory(ImagePath);
                    }



                    foreach (var result in order.Patient.results)
                    {
                        JsonHelper lineJson = new JsonHelper(JsonConvert.SerializeObject(result));
                        var testName = result.TestName;
                        var testValue = result.TestValue;



                        foreach (var item in insMngr.Instrumento.DetallesInstrumento)
                        {

                            if (item.Homologacion == testName)
                            {
                                var aux = muestras[0].Resultados.Where(r => r.CodigoExamenHomologado == testName && r.CodigoMuestra == CodigoMuestra).FirstOrDefault();
                                if (aux == null)
                                {
                                    break;
                                }
                                foreach (var convertion in item.Conversiones)
                                {

                                    ConvertionHelper h = new ConvertionHelper();
                                    try
                                    {

                                        h.Evaluate(JsonConvert.DeserializeObject<Formula>(convertion.Formula), ref testValue);
                                    }
                                    catch (Exception ex)
                                    {
                                        string g = ex.Message;
                                    }


                                }

                                GalileoPostResultRequest rq = new GalileoPostResultRequest();

                                rq.IdLaboratorio = IdLaboratorio;
                                rq.CodigoOrden = OrderNumber;
                                rq.CodigoMuestra = CodigoMuestra;
                                rq.CodigoExamenHomologado = aux.CodigoExamenHomologado;
                                rq.Validado = false;
                                try
                                {
                                    rq.ValorNumero = Convert.ToDouble(testValue);
                                    rq.ValorTexto = null;

                                    //if (rq.CodigoExamenHomologado == "028")
                                    //{
                                    //    rq.ValorNumero = rq.ValorNumero * 2.14;
                                    //}


                                }
                                catch
                                {
                                    rq.ValorTexto = testValue;
                                }





                                rq.Identificador = insMngr.Instrumento.Identificador;
                                rq.Orden = item.Orden;

                                EventWriter(true, JsonConvert.SerializeObject(rq), LogEvent.Enviando, "Enviando a Galileo");
                                results.Add(rq);
                                goto SalidaBucle;


                            }


                        }

                    SalidaBucle:
                        Console.WriteLine("Saliendo de los bucles");

                    }
                }
            }

            if (results.Count > 0)
            {

                EventWriter(false, JsonConvert.SerializeObject(JsonConvert.SerializeObject(results.OrderBy(x => x.Orden).ToList()), Formatting.Indented), LogEvent.Enviando, "Enviando a Galileo");
                var resultMsg = insMngr.PostResult(JsonConvert.SerializeObject(results.OrderBy(x => x.Orden).ToList()));
                EventWriter(true, resultMsg, LogEvent.Enviando, "Respuesta de Galileo");
            }
        }
    
            

        
        private void ProcessOrder(ASTMMessage msg)
        {
            string jsonResult = JsonConvert.SerializeObject(msg);

            EventWriter(false, JsonConvert.SerializeObject(msg, Newtonsoft.Json.Formatting.Indented), LogEvent.Esperando, "Procesando Mensaje",false);


            JsonHelper message = new JsonHelper(jsonResult);

            string sender = message.GetValue("header/SenderNameId");





            List<GalileoPostResultRequest> results = new List<GalileoPostResultRequest>();

            foreach (var patient in msg.PatienInformationList)
            {
                foreach (var order in patient.OrderRecordList)
                {
                    JsonHelper orderJson = new JsonHelper(JsonConvert.SerializeObject(order));

                    var OrderNumber = "";
                    var CodigoMuestra = "";

                    var BarCode = orderJson.GetValue(OrderField);
                    if (BarCode != null)
                    {
                         OrderNumber = BarCode.Split(SeparadorMuestra)[0];
                         CodigoMuestra = BarCode.Split(SeparadorMuestra)[1];
                    }
                    else
                    {
                        MessageBox.Show("No se encuentra la orden en el campo: " + OrderField, "Error de Configuración", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                        BarCode = "202306280010-2";
                        OrderNumber = BarCode.Split(SeparadorMuestra)[0];
                        CodigoMuestra = BarCode.Split(SeparadorMuestra)[1];
                    }



                    string ImagePath = Environment.CurrentDirectory + "\\result_images\\" + sender;
                    if (Directory.Exists(ImagePath))
                    {
                        Directory.Delete(ImagePath, true);
                        Directory.CreateDirectory(ImagePath);
                    }

                    foreach (var result in order.ResultRecordList)
                    {
                        JsonHelper lineJson = new JsonHelper(JsonConvert.SerializeObject(result));
                        var testName = lineJson.GetValue(TestName);
                        var testValue = lineJson.GetValue(TestValue);

                        foreach (var item in insMngr.Instrumento.DetallesInstrumento)
                        {
                            if (item.Homologacion == testName)
                            {
                                foreach (var convertion in item.Conversiones)
                                {
                                    ConvertionHelper h = new ConvertionHelper();
                                    try
                                    {

                                        h.Evaluate(JsonConvert.DeserializeObject<Formula>(convertion.Formula), ref testValue);
                                    }
                                    catch (Exception ex)
                                    {
                                        string g = ex.Message;
                                    }


                                }

                                GalileoPostResultRequest rq = new GalileoPostResultRequest();

                                rq.IdLaboratorio = IdLaboratorio;
                                rq.CodigoOrden = OrderNumber;
                                rq.CodigoMuestra = CodigoMuestra;
                                rq.CodigoExamenHomologado = item.Homologacion;
                                rq.Validado = false;
                                try
                                {
                                    rq.ValorNumero = Convert.ToDouble(testValue);
                                    rq.ValorTexto = null;


                                }
                                catch
                                {
                                    rq.ValorTexto = testValue;
                                }

                                
                              


                                rq.Identificador = insMngr.Instrumento.Identificador;
                                rq.Orden = item.Orden;

                                EventWriter(true, JsonConvert.SerializeObject(rq), LogEvent.Enviando, "Enviando a Galileo");
                                results.Add(rq);


                            }


                        }


                    }
                }


                EventWriter(false, JsonConvert.SerializeObject(JsonConvert.SerializeObject(results.OrderBy(x => x.Orden).ToList()), Formatting.Indented), LogEvent.Enviando, "Enviando a Galileo");
                var resultMsg = insMngr.PostResult(JsonConvert.SerializeObject(results.OrderBy(x => x.Orden).ToList()));
                EventWriter(true, resultMsg, LogEvent.Enviando, "Respuesta de Galileo");

            }




            //foreach (var result in msg.ObservationResultList)
            //{
            //    if (result.ValueType == "ED")//Imagen
            //    {
            //        ImageHelper iHelper = new ImageHelper();
            //        iHelper.SaveImage(result.ObservationValue.Value, ImagePath, result.ObservationIdentifier.Text + ".bmp");
            //    }

            //    JObject rsss = JObject.Parse(JsonConvert.SerializeObject(result));

            //    var TestNameValue = (string)rsss[TestName];

            //   /* GalileoPostResultRequest r = new GalileoPostResultRequest();
            //    r.CodigoExamenHomologado = result.ObservationIdentifier.Identifier;
            //    r.CodigoMuestra = msg.ObservationRequest.

            //    */

            //}
        }

        private void ProcessOrder(Protocol3_1Message msg)
        {
            string jsonResult = JsonConvert.SerializeObject(msg);

            EventWriter(false, JsonConvert.SerializeObject(msg, Newtonsoft.Json.Formatting.Indented), LogEvent.Esperando, "Procesando Mensaje", false);


            JsonHelper message = new JsonHelper(jsonResult);

            string sender = this.Sender;





                   List<GalileoPostResultRequest> results = new List<GalileoPostResultRequest>();


           
                    

                    var BarCode = message.GetValue(OrderField);

                    var barCodeParts = BarCode.Split(SeparadorMuestra);

                    var OrderNumber = barCodeParts[0];
                    var CodigoMuestra = "";

                        if (barCodeParts.Length >= 2)
                        
                             CodigoMuestra = barCodeParts[1];

                        

                    string ImagePath = Environment.CurrentDirectory + "\\result_images\\" + sender;
                    if (Directory.Exists(ImagePath))
                    {
                        Directory.Delete(ImagePath, true);
                        Directory.CreateDirectory(ImagePath);
                    }

                    foreach (var result in msg.resultRecord.details)
                    {
                        JsonHelper lineJson = new JsonHelper(JsonConvert.SerializeObject(result));
                        var testName = lineJson.GetValue(TestName);
                        var testValue = lineJson.GetValue(TestValue);

                        foreach (var item in insMngr.Instrumento.DetallesInstrumento)
                        {
                            if (item.Homologacion == testName)
                            {
                        foreach (var convertion in item.Conversiones)
                        {
                            ConvertionHelper h = new ConvertionHelper();
                            try
                            {

                                h.Evaluate(JsonConvert.DeserializeObject<Formula>(convertion.Formula), ref testValue);
                            }
                            catch (Exception ex)
                            {
                                string g = ex.Message;
                            }


                        }

                        GalileoPostResultRequest rq = new GalileoPostResultRequest();

                                rq.IdLaboratorio = IdLaboratorio;
                                rq.CodigoOrden = OrderNumber;
                                rq.CodigoMuestra = CodigoMuestra;
                                rq.CodigoExamenHomologado = item.Homologacion;
                                rq.Validado = false;
                                try
                                {
                                    rq.ValorNumero = Convert.ToDouble(testValue);
                                    rq.ValorTexto = null;


                                }
                                catch
                                {
                                    rq.ValorTexto = testValue;
                                }

                                rq.Identificador = insMngr.Instrumento.Identificador;
                                rq.Orden = item.Orden;

                                EventWriter(true, JsonConvert.SerializeObject(rq), LogEvent.Enviando, "Enviando a Galileo");
                                results.Add(rq);


                            }


                        


                    
                }


               

                    }

            EventWriter(false, JsonConvert.SerializeObject(JsonConvert.SerializeObject(results.OrderBy(x => x.Orden).ToList()), Formatting.Indented), LogEvent.Enviando, "Enviando a Galileo");
            var resultMsg = insMngr.PostResult(JsonConvert.SerializeObject(results.OrderBy(x => x.Orden).ToList()));
            EventWriter(true, resultMsg, LogEvent.Enviando, "Respuesta de Galileo");





        }



        private void ListenSerial()
        {


            SetState(false, "Conectado");

            EventWriter(true, "Escuchando el puerto: " + COM + " ", LogEvent.Conectado, "Conectado");

            mySerialPort = new SerialPort(COM);
            try
            {

                mySerialPort.BaudRate = Convert.ToInt32(Bauds);
                mySerialPort.Parity = Parity.None;
                mySerialPort.StopBits = StopBits.One;
                mySerialPort.DataBits = Convert.ToInt32(DataBits);
                mySerialPort.Handshake = Handshake.None;
                mySerialPort.RtsEnable = true;



                mySerialPort.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);

                mySerialPort.Open();

            }
            catch (Exception ex)
            {
                mySerialPort.Close();
                SetState(false, "Error al Conectar");

                EventWriter(true, "Error al conectar en el puerto: " + COM, LogEvent.Conectado, "Error");
                EventWriter(false, "Error al conectar en el puerto: " + COM + " " + ex.Message, LogEvent.Conectado, "Error");
            }
        }


       

    

       


        public delegate object GetTextBoxValueDelegate();

        

       


        private string CleanSerialData(string data)
        {

            //string newData = data;
            //int index = 0;
            //newData = data.
           
                
                
                return "";
        
        }

        private void ActualizarLabelStatus(string mensaje)
        {
            if (lblStatus.InvokeRequired)
            {
                // Si se está accediendo al control desde un subproceso distinto,
                // utilizar Invoke o BeginInvoke para realizar la actualización en el subproceso de la interfaz de usuario
                lblStatus.Invoke(new Action<string>(ActualizarLabelStatus), mensaje);
            }
            else
            {
                // Actualizar el control directamente en el subproceso de la interfaz de usuario
                lblStatus.Text = mensaje;
            }
        }

        private void ActualizarOutputMessage(string mensaje)
        {
            if (txtOutBox.InvokeRequired)
            {
                // Si se está accediendo al control desde un subproceso distinto,
                // utilizar Invoke o BeginInvoke para realizar la actualización en el subproceso de la interfaz de usuario
                txtOutBox.Invoke(new Action<string>(ActualizarOutputMessage), mensaje);
            }
            else
            {
                // Actualizar el control directamente en el subproceso de la interfaz de usuario
                txtOutBox.Text = mensaje;
            }
        }



        static (string Titulo, string Valor)[] DescomponerCadena(string cadena, List<(int Longitud, string Titulo)> tabla)
        {
            List<(string Titulo, string Valor)> resultado = new List<(string, string)>();
            int index = 0;

            foreach (var (longitud, titulo) in tabla)
            {
                if (index + longitud <= cadena.Length)
                {
                    string valor = cadena.Substring(index, longitud);
                    resultado.Add((titulo, valor));
                    index += longitud;
                }
                else
                {
                    // Manejar el caso en el que la cadena no tiene suficientes caracteres para la longitud especificada.
                    resultado.Add((titulo, "No hay suficientes caracteres"));
                }
            }

            return resultado.ToArray();
        }



        private void DataReceivedHandler(
                           object sender,
                           SerialDataReceivedEventArgs e)
        {

            

            ASCIIHelper hlpAs = new ASCIIHelper();

            SerialPort sp = (SerialPort)sender;
            string data = sp.ReadExisting();

            //EventWriter(true, "AU480: " + ASTM.WriteSpecialChars(data), LogEvent.Enviando, "Recibiendo", true);
           


            mensaje = mensaje + data;

            if (mensaje.Contains(ASTM.Character.ETX) && mensaje.Contains(ASTM.Character.STX))
            {
                EventWriterRxTx("AU480", ASTM.WriteSpecialChars(mensaje));
                //EventWriterRxTxDebug("AU480", mensaje);

                string paraEnviar = char.ConvertFromUtf32(6); //ACK

                EventWriterRxTx("AU480", ASTM.WriteSpecialChars(paraEnviar));
                //EventWriterRxTxDebug("AU480", paraEnviar);

                sp.WriteLine(paraEnviar);


                string result1 = "";

                ReadMessage(mensaje, ref result1);
                mensaje = "";

                if (result1 != "OK" && result1 != "Error")
                {
                    //string test = "S 002901 0001        000170020408        EF03010123123123           PRUEBA               PRUEBA1 PRUEBA2      URGENCIAS            fff              098097099";
                    //string test = "S 002901 0003        000170020408        EF098097099";
                    //string test = "S 002901 0003        000170020408        E";
                    string mensaje = char.ConvertFromUtf32(2) + result1 + char.ConvertFromUtf32(3);

                    EventWriterRxTx("GALILEO", ASTM.WriteSpecialChars(mensaje));
                    EventWriterRxTxDebug("GALILEO_DEBUG", mensaje);


                    sp.Write(mensaje);




                    List<(int Longitud, string Titulo)> tabla = new List<(int, string)>
        {
            (2, "Tipo de Mensaje"),
            (4, "Rack"),
            (2, "Posicion"),
            (1, "TipoMuestra"),
            (4, "NumeroMuestra"),
            (15, "IdMuestra"),
            (4, "Dummy"),
            (1, "Clasificacion"),
            (1, "Sexo"),
            (3, "EdadAnios"),
            (2, "EdadMeses"),
            (20, "Info1"),
            (20, "Info2"),
            (20, "Info3"),
            (20, "Info4"),
            (20, "Info5"),
            (20, "Info6"),
            (3, "Prueba1"),
            (3, "Prueba2"),
            (3, "Prueba3"),
            (3, "Prueba4"),
            (3, "Prueba5"),
            (3, "Prueba5"),
        };

                    var resultado = DescomponerCadena(ASTM.RemoveSpecialChars(mensaje), tabla);

                    foreach (var item in resultado)
                    {

                        EventWriter(true, $"{item.Titulo}: {item.Valor.Replace(" ", ".")}", LogEvent.Enviando, "Enviando", false);
                    }


                    Thread.Sleep(1000);
                    return;
                }


            }
            else if (mensaje.Contains(ASTM.Character.ACK))
            {
                EventWriterRxTx("AU480", ASTM.WriteSpecialChars(mensaje));

            }


                //Thread.Sleep(2000);

                //sp.Write(ASTM.Character.ACK);


            }

        public int CalcularTamanioEnBits(string cadena, Encoding encoding)
        {
            byte[] bytes = encoding.GetBytes(cadena);
            int bits = bytes.Length * 8; // Multiplicar por 8 para obtener el tamaño en bits
            return bits;
        }

        public void SendSerialSession(string text)
        {

           

        }

   

        private void ListenFiles()
        {


            var appSettings = ConfigurationManager.AppSettings;




            FileHelper fHlp = new();
            ImageHelper iHlp = new();




            EventWriter(true, "Sensando la ruta: " + ProcessFolder + " ", LogEvent.Conectado, "Conectado");

            while (true)
            {

                string[] filePaths = Directory.GetFiles(ProcessFolder);

                foreach (var file in filePaths)
                {
                    try
                    {
                        var fileStream = new FileStream(file, FileMode.Open, FileAccess.Read);
                        var newFile = ProcessedFolder + "\\" + DateTime.Now.Year.ToString().PadLeft(4, Convert.ToChar("0")) + DateTime.Now.Month.ToString().PadLeft(2, Convert.ToChar("0")) + DateTime.Now.Day.ToString().PadLeft(2, Convert.ToChar("0")) + DateTime.Now.Hour.ToString().PadLeft(2, Convert.ToChar("0")) + DateTime.Now.Minute.ToString().PadLeft(2, Convert.ToChar("0")) + DateTime.Now.Second.ToString().PadLeft(2, Convert.ToChar("0")) + fileStream.Name.Split("\\")[fileStream.Name.Split("\\").Length - 1];

                        using (var streamReader = new StreamReader(fileStream, Encoding.UTF8))
                        {
                            var data = streamReader.ReadToEnd();
                            EventWriter(true, "Se ha encontrado el archivo " + file, LogEvent.Recibiendo, "Recibiendo");

                            //EventWriter(true, data, LogEvent.Recibiendo, "Recibiendo");

                            Thread.Sleep(1000);
                            string result = "";
                            ReadMessage(data, ref result);



                            //EventWriter(true, "Fin del Mensaje", LogEvent.Recibiendo, "Conectado");



                        }
                        File.Move(file, newFile);
                        EventWriter(true, "Se mueve el archivo: " + newFile, LogEvent.Recibiendo, "Conectado");
                    }
                    catch
                    {
                        Thread.Sleep(1000);
                    }
                }


            }






            




        }

        private void button1_Click(object sender, EventArgs e)
        {
            SetState(true, "Conectado");
        }

        private void contextMenuStripeConfig_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            MessageBox.Show(e.ClickedItem.Text);
        }

        private void frmPrincipal_FormClosed(object sender, FormClosedEventArgs e)
        {
           
        }

        private void logScreen_TextChanged(object sender, EventArgs e)
        {

        }

        private void menuStrip_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void contextMenuStripLog_MouseClick(object sender, MouseEventArgs e)
        {

        }

        private void contextMenuStripLog_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            switch (e.ClickedItem.Text)
            {
                case "Limpiar Log":
                    logScreen.Text = "";
                    break;
                
                default:
                    break;
            }
        }

        private void logScreen_TextChanged_1(object sender, EventArgs e)
        {

        }

        private void contextMenuStripWorkList_Click(object sender, EventArgs e)
        {
            
        }

        private void contextMenuStripWorkList_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            switch (e.ClickedItem.Text)
            {
                case "Ingresar Muestra":
                    var muestra = Interaction.InputBox("Ingrese el código de muestra", "Buscar Muestra");

                    ProcessRequest(muestra);

                    break;

                case "Lista de Trabajo":
                    Forms.frmWorkList form = new Forms.frmWorkList();

                    form.ShowDialog(this);

                    break;


                case "Enviar Resultado":
                    Forms.frmEnviarResultado formResultado = new Forms.frmEnviarResultado();

                    formResultado.ShowDialog(this);

                    break;
                default:
                    break;
            }
            
        }

        private void ProcessRequest(string data)
        {
            if (data != "")
            {

                string orden = data.Substring(26, 14);

                var pedido = insMngr.GetOrden(orden);

                StringBuilder tests = new StringBuilder();

                if (pedido.Count > 0)
                {
                    foreach (var item in pedido[0].DetallesFinales)
                    {
                        tests.Append(item.CodigoExamenHomologado.PadLeft(30));
                    }
                }


            }
        }

        private void receiveTxt_TextChanged(object sender, EventArgs e)
        {
            
        }

        private void sendTxt_TextChanged(object sender, EventArgs e)
        {
           
        }

        private void msgText_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            string response = ProcessQuery(null);
        }

        private void lblStatus_TextChanged(object sender, EventArgs e)
        {
            if (lblStatus.Text == "Recibiendo")
            {
                lblStatus.BackColor = Color.GreenYellow;
            }
            else if (lblStatus.Text == "En Espera")
            {
                lblStatus.BackColor = Color.Transparent;
            }

            else if (lblStatus.Text == "Enviando")
            {
                lblStatus.BackColor = Color.IndianRed;
            }

        }

        private void btnActivarEnvio_Click(object sender, EventArgs e)
        {
            ActualizarLabelStatus("Enviando");
            mySerialPort.Write(ASTM.Character.ENQ);
            
        }
    }
}
