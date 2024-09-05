
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
                EventWriter(true, data, LogEvent.Conectado, "Recibiendo", false);

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

            }
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

        private void ProcessOrder(ASTMMessage msg)
        {
            string jsonResult = JsonConvert.SerializeObject(msg);






            EventWriter(false, JsonConvert.SerializeObject(msg, Newtonsoft.Json.Formatting.Indented), LogEvent.Esperando, "Procesando Mensaje", false);


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

                    var BarCode = order.SpecimenId;
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
                        var testName = result.TestIdentifier.Manufacturer;
                        var testValue = result.DataMeasurementValue;

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
                                    rq.ValorNumero = Convert.ToDouble(testValue.Replace(",", "."));
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






        private void DataReceivedHandler(
                           object sender,
                           SerialDataReceivedEventArgs e)
        {



            ASCIIHelper hlpAs = new ASCIIHelper();

            SerialPort sp = (SerialPort)sender;
            string data = sp.ReadExisting();

            if (lblStatus.Text != "En Espera")
            {
                if (lblStatus.Text != "Recibiendo")
                {
                    if (lblStatus.Text == "Enviando")
                    {
                        if (data == ASTM.Character.ACK)
                        {

                            StringBuilder sb1 = new StringBuilder();

                            sb1.Append(ASTM.Character.STX);
                            sb1.Append(txtOutBox.Text);
                            sb1.Append(ASTM.Character.ETX);
                            sb1.Append(ASTM.Character.EOT);
                            var chars1 = sb1.ToString().ToCharArray();
                            sp.Write(sb1.ToString());
                            return;

                        }
                        else if (data.Contains(ASTM.Character.NAK))
                        {
                            EventWriter(true, "Elisys no recibió la orden", LogEvent.Error, "Enviando");
                            ActualizarLabelStatus("En Espera");

                        }

                        return;


                    }
                    else
                    {
                        sp.Write(ASTM.Character.NAK);
                        return;
                    }
                }
            }

            StringBuilder sb = new StringBuilder();
            sb.Append(SerialMessage);

            /*if (data.Contains(ASTM.Character.ENQ))
            {
                ActualizarLabelStatus("Recibiendo");
                sp.Write(ASTM.Character.ACK);
                return;
            }*/

            var chars = data.ToCharArray();




            //foreach (var item in chars)
            for (int i = 0; i < chars.Length; i++)

            {
                var item = chars[i];

                if (item.ToString() == ASTM.Character.EOT)
                {

                    string msg = sb.ToString();
                    msg = ASTM.CleanMessage(msg);
                    ActualizarLabelStatus("En Espera");
                    string result = "";
                    ReadMessage(msg, ref result);

                    if (result != "") //Cuando es HOST QUERY devuelve el ASTM respuesta
                    {
                        ActualizarLabelStatus("Enviando");
                        ActualizarOutputMessage(result);
                        //sp.Write(ASTM.Character.ENQ);
                        //sp.Write(ASTM.Character.STX);
                        //sp.Write(result);
                        //sp.Write(ASTM.Character.ETX);
                        //sp.Write(ASTM.Character.EOT);
                        //ActualizarLabelStatus("En Espera");

                    }

                    sb.Clear();


                }
                else
                {

                    sb.Append(ASTM.TransalateEspecialCharacters(item.ToString()));





                }
            }

            string clean_data = sb.ToString();

            SerialMessage = clean_data;

            sp.Write(ASTM.Character.ACK);
            sb.Append("<ACK>");

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
                    if (muestra != "")
                    {
                        insMngr.GetOrden(muestra);

                    }

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
