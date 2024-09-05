
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
using Galileo.Utils.ConversionModel;
using System.Diagnostics;

namespace Galileo.Online
{
    public partial class MainForm : Form
    {

        public string ServerLIS { get { return ConfigurationManager.AppSettings["ServerLIS"]; } }

        public string ServerSeguridad { get { return ConfigurationManager.AppSettings["ServerSeguridad"]; } }


        public string Instrumento { get { return ConfigurationManager.AppSettings["Instrumento"]; } }
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
            string LogPath = Environment.CurrentDirectory + "\\logs";
            string CapPath = Environment.CurrentDirectory + "\\data";


            string actualProcess = Process.GetCurrentProcess().ProcessName;

            Process[] InstanciasCorriendo = Process.GetProcessesByName(actualProcess);

            if (InstanciasCorriendo.Length > 1)
            {
                MessageBox.Show("No puede ejecutarse varias instancias de este programa varias veces", "GALILEO LIS", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Environment.Exit(0);

            }





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
                    EventWriter(true, "No implementada la sincronización con el servidor o no se encuentra el analizador seleccionado.", LogEvent.Error, "OFFLINE");
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

                log.Add(textLog,includeDateLog);

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

                           
                            SendTCPData(stream,ASTM.Character.ACK);

                           

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

        private  void SendTCPData(NetworkStream stream, string data)
        {
            stream.Write(ASCIIEncoding.ASCII.GetBytes(data));
            AppendTXTextBox(data);
            //EventWriter(true, data, LogEvent.Enviando, "Enviando");
        }

        private void ReceiveTCPData(string data)
        {
            AppendRXTextBox(data);
            AppendMessageTextBox(data);
            //EventWriter(true, data, LogEvent.Recibiendo, "Recibiendo");
        }

        public void SendMessage(string data, string filename = null)
        {
            switch (ConnectionMode)
            {
                case "FILES":
                    WriteFile(data, filename);

                    break;
                default:
                    break;
            }
        }

        private void ReadMessage(string data)
        {
            //try
            //{

            EventWriter(true, data, LogEvent.Conectado, "Recibiendo",false);

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
                else
                if (Protocol == "ASTM 1394")
                {

                   
                       
                        ASTMMessage msg = new ASTMMessage(data);
                        if (msg.header != null && msg.header.RecordTypeId == "H")
                        {

                            //foreach (var mf in msg.PatienInformationList[0].OrderRecordList[0].ManufacturRecordList)
                            //{
                            //        ImageHelper imgHelper = new ImageHelper();
                            //        imgHelper.DrawImage(mf.DataMeasurement.Value, mf.TestIdentifier.Manufacturer);

                            //}
                            
                            ProcessOrder(msg);

                        }
                        else
                        {
                            EventWriter(true, "Mensaje ASTM Omitido", LogEvent.Conectado, "Conectado");
                        }
                    
                }

            //}
            //catch (Exception ex)
            //{
            //    log.Add("Error" + ex.Message);
            //    EventWriter(true, "Error al procesar el mensaje " + ex.Message, LogEvent.Error, "Error");
            //}
        }

        private void ProcessOrder(HL7Message msg)
        {
            string jsonResult = JsonConvert.SerializeObject(msg);

            EventWriter(false, JsonConvert.SerializeObject(msg, Newtonsoft.Json.Formatting.Indented), LogEvent.Esperando, "Procesando Mensaje", false);

            JsonHelper message = new JsonHelper(jsonResult);

            string sender = message.GetValue("Header/SendingApplication");

            string BarCode;
            string OrderNumber;
            string CodigoMuestra;

            try
            {


                BarCode = message.GetValue(OrderField);
                OrderNumber = BarCode.Split(SeparadorMuestra)[0];
                CodigoMuestra = BarCode.Split(SeparadorMuestra)[1];
            }
            catch
            {
                EventWriter(true, "No se encuentra el path: " + OrderField + " en el mensaje", LogEvent.Error, "Enviando a Galileo");
                return;
            }



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
                string testName;
                string testValue;

                try
                {
                    testName = line.GetValue(TestName);
                    testValue = line.GetValue(TestValue);
                }
                catch
                {
                    EventWriter(true, "No se encuentra el path: " + TestName + "-" + TestValue + " en el mensaje", LogEvent.Error, "Enviando a Galileo");
                    return;
                }

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
                string BarCode;
                string OrderNumber;
                string CodigoMuestra;

                try
                {
                    //JsonHelper orderJson = new JsonHelper(JsonConvert.SerializeObject(order));

                    BarCode = patient.LaboratoryAssignedPatientId;
                    OrderNumber = BarCode.Split(SeparadorMuestra)[0];
                    CodigoMuestra = BarCode.Split(SeparadorMuestra)[1];
                }
                catch
                {
                    EventWriter(true, "No se encuentra el identificador de la muestra", LogEvent.Error, "Enviando a Galileo");
                    return;
                }


                EventWriter(true, "EVUALUANDO RESULTADOS", LogEvent.Conectado, "Procesando", false);
                foreach (var order in patient.OrderRecordList)
                {




                    string ImagePath = Environment.CurrentDirectory + "\\result_images\\" + sender;
                    if (Directory.Exists(ImagePath))
                    {
                        Directory.Delete(ImagePath, true);
                        Directory.CreateDirectory(ImagePath);
                    }



                    foreach (var result in order.ResultRecordList)
                    {
                        JsonHelper lineJson = new JsonHelper(JsonConvert.SerializeObject(result));
                        string testName;
                        string testValue;

                        try
                        {
                            testName = lineJson.GetValue(TestName);
                            testValue = lineJson.GetValue(TestValue);
                        }
                        catch
                        {
                            EventWriter(true, "No se encuentra el path: " + TestName + "-" + TestValue + " en el mensaje", LogEvent.Error, "Enviando a Galileo");
                            return;
                        }
                        foreach (var item in insMngr.Instrumento.DetallesInstrumento)
                        {
                            if (item.Homologacion.ToUpper() == testName.ToUpper())
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

                                EventWriter(true, testName + "||" + testValue, LogEvent.Conectado, "Procesando", false);

                                rq.Identificador = insMngr.Instrumento.Identificador;
                                rq.Orden = item.Orden;

                                if (rq.ValorTexto != null)
                                {
                                    EventWriter(true, JsonConvert.SerializeObject(rq), LogEvent.Enviando, "Enviando a Galileo");
                                    results.Add(rq);
                                }
                                else
                                {
                                    if (rq.ValorNumero > -99999)
                                    {
                                        EventWriter(true, JsonConvert.SerializeObject(rq), LogEvent.Enviando, "Enviando a Galileo");
                                        results.Add(rq);
                                    }
                                   
                                }


                            }


                        }


                    }
                }


                //Calculo del LDL//

                {


                    string hChol = ConfigurationManager.AppSettings["Colesterol"];
                    string hHDL = ConfigurationManager.AppSettings["HDL"];
                    string hTrig = ConfigurationManager.AppSettings["Trigliceridos"];


                    var rCHOL = results.Where(x => x.CodigoExamenHomologado == hChol && x.CodigoOrden == OrderNumber).FirstOrDefault();
                    var rCHOLLDH = results.Where(x => x.CodigoExamenHomologado == hHDL && x.CodigoOrden == OrderNumber).FirstOrDefault();
                    var rTRG = results.Where(x => x.CodigoExamenHomologado == hTrig && x.CodigoOrden == OrderNumber ).FirstOrDefault();

                  

                    if (!(rCHOL == null || rCHOLLDH == null || rTRG == null))
                    {
                        GalileoPostResultRequest rq = new GalileoPostResultRequest();

                        rq.IdLaboratorio = IdLaboratorio;
                        rq.CodigoOrden = OrderNumber;
                        rq.CodigoMuestra = CodigoMuestra;
                        rq.CodigoExamenHomologado = "LDL*";
                        rq.Validado = false;

                        double calc = rCHOL.ValorNumero - rCHOLLDH.ValorNumero - (rTRG.ValorNumero / 5);

                        try
                        {
                            rq.ValorNumero = Convert.ToDouble(calc);
                            rq.ValorTexto = null;


                        }
                        catch
                        {
                            rq.ValorTexto = calc.ToString();
                        }

                        EventWriter(true, "LDL" + "||" + calc.ToString(), LogEvent.Conectado, "Procesando", false);

                        rq.Identificador = insMngr.Instrumento.Identificador;
                        rq.Orden = 999; //poner el ultimo

                        EventWriter(true, JsonConvert.SerializeObject(rq), LogEvent.Enviando, "Enviando a Galileo");
                        results.Add(rq);
                    }

                    string hBilta = ConfigurationManager.AppSettings["BilirubinaTotal"];
                    string hBilda = ConfigurationManager.AppSettings["BilirubinaDirecta"];


                    var rbilTotal = results.Where(x => x.CodigoExamenHomologado == hBilta && x.CodigoOrden == OrderNumber).FirstOrDefault();
                    var rbilDirect = results.Where(x => x.CodigoExamenHomologado == hBilda && x.CodigoOrden == OrderNumber).FirstOrDefault();



                    if (!(rbilTotal == null || rbilDirect == null))
                    {
                        GalileoPostResultRequest rq = new GalileoPostResultRequest();

                        rq.IdLaboratorio = IdLaboratorio;
                        rq.CodigoOrden = OrderNumber;
                        rq.CodigoMuestra = CodigoMuestra;
                        rq.CodigoExamenHomologado = "BilIn*";
                        rq.Validado = false;

                        double calc = rbilTotal.ValorNumero - rbilDirect.ValorNumero;

                        try
                        {
                            rq.ValorNumero = Convert.ToDouble(calc);
                            rq.ValorTexto = null;


                        }
                        catch
                        {
                            rq.ValorTexto = calc.ToString();
                        }

                        EventWriter(true, "BILIN" + "||" + calc.ToString(), LogEvent.Conectado, "Procesando", false);

                        rq.Identificador = insMngr.Instrumento.Identificador;
                        rq.Orden = 999; //poner el ultimo

                        EventWriter(true, JsonConvert.SerializeObject(rq), LogEvent.Enviando, "Enviando a Galileo");
                        results.Add(rq);
                    }

                    //Envio a GALILEO

                    EventWriter(true, "CONECTANDO A GALILEO", LogEvent.Conectado, "Procesando", false);
                    
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


        public void AppendRXTextBox(string value)
        {
            string text = value;
            switch (text)
            {
                case ASTM.Character.ENQ:
                    text = "<ENQ>";
                        break;

                case ASTM.Character.ACK:
                    text = "<ACK>";
                    break;

                case ASTM.Character.EOT:
                    text = "<EOT>";
                    break;

                case ASTM.Character.STX:
                    text = "<STX>";
                    break;

                case ASTM.Character.ETX:
                    text = "<ETX>";
                    break;

                default:
                    break;
            }

            if (InvokeRequired)
            {
                this.Invoke(new Action<string>(AppendRXTextBox), new object[] { text });
                return;
            }
            receiveTxt.Text = text;
            Thread.Sleep(20);
        }

        public void AppendMessageTextBox(string value)
        {
            if (value != ASTM.Character.EOT)
            {
                string text = ASTM.TranslateMessage(value);

                if (InvokeRequired)
                {
                    this.Invoke(new Action<string>(AppendMessageTextBox), new object[] { text });
                    return;
                }
                msgText.Text += text;
            }
            else
            {
                ReadMessage(msgText.Text);
                ClearMessageTextBox("");
            }
        }

        public void  ClearMessageTextBox(string value)
        {
           
              

                if (InvokeRequired)
                {
                    this.Invoke(new Action<string>(ClearMessageTextBox), new object[] { value });
                    return;
                }
                msgText.Text = "";
           
        }


        public delegate object GetTextBoxValueDelegate();

        public object GetRXTextBoxValue()
        {
            if (InvokeRequired)
                return Invoke(new GetTextBoxValueDelegate(GetRXTextBoxValue));
            return receiveTxt.Text;
        }

        public void AppendTXTextBox(string value)
        {

            string text = value;
            switch (text)
            {
                case ASTM.Character.ENQ:
                    text = "<ENQ>";
                    break;

                case ASTM.Character.ACK:
                    text = "<ACK>";
                    break;

                case ASTM.Character.EOT:
                    text = "<EOT>";
                    break;

                default:
                    break;
            }

            if (InvokeRequired)
            {
                this.Invoke(new Action<string>(AppendTXTextBox), new object[] { text });
                return;
            }
            sendTxt.Text = text;
        }


        private  void DataReceivedHandler(
                        object sender,
                        SerialDataReceivedEventArgs e)
        {

            ASCIIHelper hlpAs = new ASCIIHelper();

            SerialPort sp = (SerialPort)sender;
            string data = sp.ReadExisting();
            Thread.Sleep(1000);
            AppendRXTextBox(data);

           

        }

        private void SendSerialSession(string text)
        {

            Thread threadInput = new Thread(WriteSerial);
            threadInput.Start(text);

        }

        private void WriteFile(string data, string name)
        {
            FileHelper h = new FileHelper();
            h.WriteToFile(data, name, WorkListFolder);
        }

        private void WriteSerial(object obj)
        {
            bool sessionOpened = false;
            bool waitingForAck = true;

            //Enviar ENQ
            AppendTXTextBox("\x05");
           
            while (waitingForAck)
            {
                Thread.Sleep(1000);

                string rx = GetRXTextBoxValue().ToString();

                if (rx == "\\x06")
                {
                    sessionOpened = true;
                    waitingForAck = false;
                    AppendRXTextBox("");

                }
                else
                {
                    sessionOpened = false;
                }
            }




            AppendTXTextBox(obj.ToString());
        }

        private void ListenFiles()
        {


            var appSettings = ConfigurationManager.AppSettings;




            FileHelper fHlp = new();
            ImageHelper iHlp = new();

            if (insMngr.Instrumento == null)
            {
                EventWriter(true, "No existen exámenes asignados a este instrumento, por favor asigne y reinicie.", LogEvent.Error, "Error");
                return;
            }


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

                            ReadMessage(data);



                            //EventWriter(true, "Fin del Mensaje", LogEvent.Recibiendo, "Conectado");



                        }
                        try
                        {
                            File.Move(file, newFile);
                        }
                        catch (Exception ex)
                        {
                            EventWriter(true, ex.Message, LogEvent.Error, "Conectado");
                        }
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
    }
}
