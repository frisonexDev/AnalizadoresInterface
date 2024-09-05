
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
        public string MuestraPorDefecto { get { return ConfigurationManager.AppSettings["MuestraPorDefecto"]; } }

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

            //            string tramaASTM = "1H|\\^&|||U-WAM^00-22_Build003^A7608^^^^AU501736||||||||LIS2-A2|20240830081106\u0003" +
            //"82\u0002" +
            //"2P|1|||||||U\u0003F8\u0002" +
            //"3O|1|202408260001||^^^C-URO\\^^^C-BLD\\^^^C-BIL\\^^^C-KET\\^^^C-GLU\\^^^C-PRO\\^^^C-PH\\^^^C-NIT\\^^^C-LEU\\^^^C-CRE\\^^^C-ALB\\^^^C-P/C\\^^^C-A/C\\^^^C-S.G.(Ref)\\^^^C-COLOR\\^^^C-ColorRANK\\^^^C-CLOUD\\^^^C-Error Code\\^^^RBC\\^^^NL RBC\\^^^EC\\^^^Squa.EC\\^^^N\u0017CC\u0002" +
            //"4on SEC\\^^^Tran.EC\\^^^RTEC\\^^^CAST\\^^^Hy.CAST\\^^^Path.CAST\\^^^BACT\\^^^X'TAL\\^^^YLC\\^^^SPERM\\^^^MUCUS\\^^^WBC\\^^^WBC Clumps\\^^^RBC-Info.\\^^^UTI-Info.\\^^^BACT-Info.\\^^^RC_UNIVERSAL\\^^^SF_DSS_PxSF_FSC_P\\^^^HIST_SF_FSC_P\\^^^CW_SSH_AxCW_FSC_W\\^^^C\u001779\u0002" +
            //"5W_FLL_AxCW_FSC_W\\^^^CB_FLH_PxCB_FSC_P\\^^^SF_FLL_WxSF_FLL_A\\^^^CW_FLH_PxCW_FSC_P\\^^^CW_SSH_PxCW_FLL_P\\^^^HIST_CW_SSH_P\\^^^M-RBCs\\^^^M-NL RBC\\^^^M-Iso RBCs\\^^^M-WBCs\\^^^M-WBC Clumps\\^^^M-Squa.EC\\^^^M-Tran. EC\\^^^M-RTEC\\^^^M-OFB\\^^^M-Atyp. C\\^u001716\u0002" +
            //"6^^M-Hy. Casts\\^^^M-Epith. casts\\^^^M-Gra. Casts\\^^^M-RBC Casts\\^^^M-Mucus\\^^^M-Bacteria\\^^^M-Yeast\\^^^M-Trichomonas\\^^^M-Urate\\^^^M-Phosphate\\^^^M-CaOxm X'TAL\\^^^M-UA X'TAL\\^^^M-Bilirubin\\^^^M-Sperma\\^^^M-Dys RBC\\|R||20240829111133||||N|||2\u00173B\u0002" +
            //"70240829111203|*||||||||||F\u00039C\u0002" +
            //"0R|1|^^^C-URO^A^1^S^  E004^02|normal^RAW|||N||||^^sysmex^sysmex||20240829155919|UC-3500\u000396\u0002" +
            //"1R|2|^^^C-URO^A^1^S^  E004^02|normal^MAINFORMAT|||N||||^^sysmex^sysmex||20240829155919|UC-3500\u00039C\u0002" +
            //"2R|3|^^^C-BLD^A^1^S^  E004^02|0.75^RAW|mg/dL||H||||^^sysmex^sysmex||20240829155919|UC-3500\u000364\u0002" +
            //"3R|4|^^^C-BLD^A^1^S^  E004^02|0.75^MAINFORMAT|mg/dL||H||||^^sysmex^sysmex||20240829155919|UC-3500\u00036A\u0002" +
            //"4R|5|^^^C-BIL^A^1^S^  E004^02|-^RAW|||N||||^^sysmex^sysmex||20240829155919|UC-3500\u000323\u0002" +
            //"5R|6|^^^C-BIL^A^1^S^  E004^02|-^MAINFORMAT|||N||||^^sysmex^sysmex||20240829155919|UC-3500\u000329";

            //string nuevosDatos = "\u00026R|7|^^^C-KET^A^1^S^  E004^02|-^RAW|||N||||^^sysmex^sysmex||20240829155919|UC-3500\u000334\u0002" +
            //        "7R|8|^^^C-KET^A^1^S^  E004^02|-^MAINFORMAT|||N||||^^sysmex^sysmex||20240829155919|UC-3500\u00033A\u0002" +
            //        "0R|9|^^^C-GLU^A^1^S^  E004^02|-^RAW|||N||||^^sysmex^sysmex||20240829155919|UC-3500\u000334\u0002" +
            //        "1R|10|^^^C-GLU^A^1^S^  E004^02|-^MAINFORMAT|||N||||^^sysmex^sysmex||20240829155919|UC-3500\u000361\u0002" +
            //        "2R|11|^^^C-PRO^A^1^S^  E004^02|30^RAW|mg/dL||H||||^^sysmex^sysmex||20240829155919|UC-3500\u00034B\u0002" +
            //        "3R|12|^^^C-PRO^A^1^S^  E004^02|30^MAINFORMAT|mg/dL||H||||^^sysmex^sysmex||20240829155919|UC-3500\u000351\u0002" +
            //        "4R|13|^^^C-PH^A^1^S^  E004^02|8.0^RAW|||N||||^^sysmex^sysmex||20240829155919|UC-3500\u00037C\u0002" +
            //        "5R|14|^^^C-PH^A^1^S^  E004^02|8.0^MAINFORMAT|||N||||^^sysmex^sysmex||20240829155919|UC-3500\u000382\u0002" +
            //        "6R|15|^^^C-NIT^A^1^S^  E004^02|-^RAW|||N||||^^sysmex^sysmex||20240829155919|UC-3500\u00036A\u0002" +
            //        "7R|16|^^^C-NIT^A^1^S^  E004^02|-^MAINFORMAT|||N||||^^sysmex^sysmex||20240829155919|UC-3500\u000370" +
            //        "0R|17|^^^C-LEU^A^1^S^  E004^02|25^RAW|c/?L||H||||^^sysmex^sysmex||20240829155919|UC-3500281R|18|^^^C-LEU^A^1^S^  E004^02|25^MAINFORMAT|c/?L||H||||^^sysmex^sysmex||20240829155919|UC-3500" +
            //        "2E2R|19|^^^C-CRE^A^1^S^  E004^02|10^RAW|mg/dL||N||||^^sysmex^sysmex||20240829155919|UC-3500403R|20|^^^C-CRE^A^1^S^  E004^02|10^MAINFORMAT|mg/dL||N||||^^sysmex^sysmex||20240829155919|UC-35003D4R|21|^^^C-ALB^A^1^S^  E004^02|150^RAW|mg/L||N||||^^sysmex^sysmex||20240829155919|UC-350001" +
            //        "5R|22|^^^C-ALB^A^1^S^  E004^02|150^MAINFORMAT|mg/L||N||||^^sysmex^sysmex||20240829155919|UC-3500076R|23|^^^C-P/C^A^1^S^  E004^02|>=0.50^RAW|g/gCr||H||||^^sysmex^sysmex||20240829155919|UC-3500FD7R|24|^^^C-P/C^A^1^S^  E004^02|>=0.50^MAINFORMAT|g/gCr||H||||^^sysmex^sysmex||20240829155919|UC-3500030R|25|^^^C-A/C^A^1^S^  E004^02|>=300^RAW|mg/gCr||H||||^^sysmex^sysmex||20240829155919|UC-3500271R|26|^^^C-A/C^A^1^S^  E004^02|>=300^MAINFORMAT|mg/gCr||H||||^^sysmex^sysmex||20240829155919|UC-35002D" +
            //        "2R|27|^^^C-S.G.(Ref)^A^1^S^  E004^02|1.017^RAW|||N||||^^sysmex^sysmex||20240829155919|UC-3500AC3R|28|^^^C-S.G.(Ref)^A^1^S^  E004^02|1.017^MAINFORMAT|||N||||^^sysmex^sysmex||20240829155919|UC-3500B24R|29|^^^C-COLOR^A^1^S^  E004^02|STRAW     02^RAW|||N||||^^sysmex^sysmex||20240829155919|UC-3500675R|30|^^^C-COLOR^A^1^S^  E004^02|STRAW     02^MAINFORMAT|||N||||^^sysmex^sysmex||20240829155919|UC-3500646" +
            //        "R|31|^^^C-ColorRANK^A^1^S^  E004^02|2221^RAW|||N||||^^sysmex^sysmex||20240829155919|UC-3500427R|32|^^^C-ColorRANK^A^1^S^  E004^02|2221^MAINFORMAT|||N||||^^sysmex^sysmex||20240829155919|UC-3500480R|33|^^^C-CLOUD^A^1^S^  E004^02|1+^RAW|||N||||^^sysmex^sysmex||20240829155919|UC-35001F1R|34|^^^C-CLOUD^A^1^S^  E004^02|1+^MAINFORMAT|||N||||^^sysmex^sysmex||20240829155919|UC-3500252R|35|^^^C-Error Code^A^1^S^  E004^02|0000^RAW|||N||||^^sysmex^sysmex||20240829155919|UC-3500B53R|36|^^^C-Error Code^A^1^S^  E004^02|0000^MAINFORMAT|||N||||^^sysmex^sysmex||20240829155919|UC-3500BB";


            string tramaASTM = "H|\\^&|||U-WAM^00-03^11001^^^^BE198113||||||||LIS2-A2|20151209170319<CR>" +
                                "P | 1 || 1 || Test ^ Ur\\Test ^ Ur\\|| 20130116 | M |||||| APOS < CR >" +
                                "O | 1 | 0000000369 || ^^^M - RBC\\^^^M - WBC\\^^^M - WBC Clumps\\^^^M - EC\\^^^M - Squa.EC\\^^^M - OFB\\^^^M - At" +
                                "yp.C\\^^^M - Hy.CAST\\^^^M - MUCUS\\^^^M - BACT\\^^^M - YLC\\^^^M - Trichomonas\\^^^M - SPERM\\^^^M - Non" +
                                "SEC\\^^^M - CAST\\^^^M - Path.CAST\\^^^M - X'TAL\\|R||20151209095323||||N|||20151209095520|*|||123|||||||" +
                                "F<CR>" +
                                "R | 1 | ^^^M - RBC ^ M ^ 1 ^ S |< 1 / HPF ^ RAW |/ HPF || N |||| admin ^ administrator ^ admin ^" +
                                "administrator || 20151209165858 | ENTERED < CR >" +
                                "R | 2 | ^^^M - RBC ^ M ^ 1 ^ S |< 1 / HPF ^ MAINFORMAT |/ HPF || N |||| admin ^ administrator ^ admin ^" +
                                "administrator || 20151209165858 | ENTERED < CR >" +
                                "R | 3 | ^^^M - WBC ^ M ^ 1 ^ S | 30 - 49 / HPF ^ RAW |/ HPF || N |||| admin ^ administrator ^ admin ^" +
                                "administrator || 20151209165858 | ENTERED < CR >" +
                                "R | 4 | ^^^M - WBC ^ M ^ 1 ^ S | 30 - 49 / HPF ^ MAINFORMAT |/ HPF || N |||| admin ^ administrator ^ admin ^ administrator ||" +
                                "20151209165858 | ENTERED < CR >" +
                                "R | 33 | ^^^M - X'TAL^M^1^S|---^RAW|||N||||admin^administrator^admin^" +
                                "administrator || 20151209165858 | ENTERED < CR >" +
                                "R | 34 | ^^^M - X'TAL^M^1^S|---^MAINFORMAT|||N||||admin^administrator^admin^" +
                                "administrator || 20151209165858 | ENTERED < CR >" +
                                "L | 1 | N < CR >";

            //tramaASTM += nuevosDatos;


            //ReceiveTCPData(data);
            ReadMessage(tramaASTM);

            while (true)
            {
                //if (server.Pending())
                //{

                //    Thread tmp_thread = new Thread(new ThreadStart(() =>
                //    {
                //        //EventWriter(false, "Recibiendo: " + LocalPort, LogEvent.Conectado, "Recibiendo");
                //        int i = 0;
                //        clt = server.AcceptTcpClient();

                //        NetworkStream stream = clt.GetStream();
                 


                //        while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                //        {

                //            data = System.Text.Encoding.ASCII.GetString(bytes, 0, i);

                //            //ReceiveTCPData(data);
                //            ReadMessage(data);

                //            SendTCPData(stream,ASTM.Character.ACK);

                           

                //        }



                //    }));
                //    tmp_thread.Start();

                //}
                //else
                //{
                //    Thread.Sleep(1000); //<--- timeout
                //}


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
                }
                else
                if (Protocol == "ASTM 1394")
                {                                          
                        ASTMMessage msg = new ASTMMessage(data);
                        if (msg.header != null && msg.header.RecordTypeId == "H")
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
                            EventWriter(true, "Mensaje ASTM Omitido", LogEvent.Conectado, "Conectado");
                        }
                    
                }
            else if(Protocol == "ASTM 1381")
            {
                ASTMMessage msg = new ASTMMessage(data);
                if (msg.header != null && msg.header.RecordTypeId == "H")
                {
                    //foreach (var mf in msg.PatienInformationList[0].OrderRecordList[0].ManufacturRecordList)
                    //{
                    //    ImageHelper imgHelper = new ImageHelper();
                    //    imgHelper.DrawImage(mf.DataMeasurement.Value, mf.TestIdentifier.Manufacturer);

                    //}

                    ProcessOrder(msg);
                }
                else
                {
                    EventWriter(true, "Mensaje ASTM Omitido", LogEvent.Conectado, "Conectado");
                }
            }
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

                if (BarCode.Contains(SeparadorMuestra))
                {
                    OrderNumber = BarCode.Split(SeparadorMuestra)[0];
                    CodigoMuestra = BarCode.Split(SeparadorMuestra)[1];
                }
                else
                {
                    OrderNumber = BarCode;
                    CodigoMuestra = MuestraPorDefecto;
                }
            }
            catch
            {
                EventWriter(true, "No se encuentra el path: " + OrderField + " en el mensaje", LogEvent.Error, "Enviando a Galileo");
                return;
            }

           // var muestras = insMngr.GetOrdenMuestra(OrderNumber, CodigoMuestra);

            //if (muestras.Count == 0)
            //{
            //    EventWriter(true, "No existen pruebas pendientes para: " + OrderNumber + "-" + CodigoMuestra, LogEvent.Error, "Recibiendo", false);

            //}


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
                    item.Tipo = "";
                    if (item.Homologacion == testName)
                    {
                        foreach (var convertion in item.Conversiones)
                        {
                            ConvertionHelper h = new ConvertionHelper();
                            try
                            {
                                string tipo = "";
                                h.Evaluate(JsonConvert.DeserializeObject<Formula>(convertion.Formula),ref testValue,ref tipo);
                                item.Tipo = tipo;
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

                        //rq.CodigoOrden = "202207200015";
                        //rq.CodigoMuestra = "5";

                        rq.CodigoExamenHomologado = item.Homologacion;
                        rq.Validado = false;

                        if (item.Tipo == "")
                        {
                            try
                            {

                                rq.ValorNumero = Convert.ToDouble(testValue);
                                rq.ValorTexto = null;



                            }
                            catch
                            {
                                rq.ValorTexto = testValue;
                            }
                        }
                        else {

                            if (item.Tipo == "Texto")
                            {
                                rq.ValorTexto = testValue;
                               
                               
                            }
                        }

                        rq.Identificador = insMngr.Instrumento.Identificador;
                        rq.Orden = item.Orden;


                        results.Add(rq);


                    }


                }


            }

            foreach (var item in results)
            {
                List<GalileoPostResultRequest> results1 = new List<GalileoPostResultRequest>();
                results1.Add(item);
                EventWriter(true, JsonConvert.SerializeObject(JsonConvert.SerializeObject(results1.OrderBy(x => x.Orden).ToList()), Formatting.Indented), LogEvent.Enviando, "Enviando a Galileo");
                var resultMsg = insMngr.PostResult(JsonConvert.SerializeObject(results1.OrderBy(x => x.Orden).ToList()));
                EventWriter(true, resultMsg, LogEvent.Enviando, "Respuesta de Galileo");
            }

            //EventWriter(true, JsonConvert.SerializeObject(JsonConvert.SerializeObject(results.OrderBy(x => x.Orden).ToList()), Formatting.Indented), LogEvent.Enviando, "Enviando a Galileo");
            //var resultMsg = insMngr.PostResult(JsonConvert.SerializeObject(results.OrderBy(x => x.Orden).ToList()));
            //EventWriter(true, resultMsg, LogEvent.Enviando, "Respuesta de Galileo");





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
                    string BarCode;
                    string OrderNumber;
                    string CodigoMuestra;

                    try
                    {
                        JsonHelper orderJson = new JsonHelper(JsonConvert.SerializeObject(order));

                        BarCode = orderJson.GetValue(OrderField);
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
                //ReadMessage(msgText.Text);
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

        public void SendSerialSession(string text)
        {

            Thread threadInput = new Thread(WriteSerial);
            threadInput.Start(text);

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
    }
}
