using iTextSharp.text.pdf.draw;
using iTextSharp.text.pdf;
using iTextSharp.text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rectangle = iTextSharp.text.Rectangle;
using System.IO;
using System.Windows.Forms;
using System.Configuration;
using System.Text.RegularExpressions;

namespace Galileo.Connect.Model
{
    public class PDF_MSP
    {
        public string? filename;
        public string? usuarioValidador;
        public int uValidador;
        public int uaValidador;
        public DateTime fValidacion;
        public DateTime faValidacion;
        public string RutaDestino { get { return ConfigurationManager.AppSettings["RutaReportes"]; } }

        private string ObtenerRuta(string raiz, string origen)
        {

            string path = raiz + origen;
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }


            return path + "\\";
        }

        public void crearPDFFile(
            string NumOrden,
            string NombrePaciente,
            string GeneroPaciente,
            string nombreMedico,
            string telefonoMedico,
            string correoMedico,
            string NombreLaboratorio,
            string NombreSede,
            DateTime FechaIngreso,
            string Origen,
            string EdadPaciente,
            string EstadoPaciente,
            List<ResultadoOrden> Resultados,
            bool ExValidado = false,
            bool ExTodos = false
            )
        {
            string root = ObtenerRuta(RutaDestino, Origen + "\\" + FechaIngreso.ToString("ddMMyyyy"));
            var uncPath = root + filename + "_" + NumOrden + ".pdf";
            var uncTitle = NumOrden + "_" + NombrePaciente;
            FileStream fs = new FileStream(uncPath, FileMode.Create);
            Document doc = new Document(PageSize.A4, 1, 1, 20, 20);
            PdfWriter writer = PdfWriter.GetInstance(doc, fs);
            doc.Open();
            doc.AddAuthor("GALILEO | LIMS");
            doc.AddTitle(uncTitle);
            //iTextSharp.text.Image logo = iTextSharp.text.Image.GetInstance("C:/reportes/img/logo.png");
            iTextSharp.text.Image logo = iTextSharp.text.Image.GetInstance(Environment.CurrentDirectory + "\\img\\logo.png");

            logo.SetAbsolutePosition(0, 0);
            var escalaLogo = ((50 * logo.Width) / 100);
            logo.ScalePercent(escalaLogo);
            iTextSharp.text.Font font0 = FontFactory.GetFont("Arial", 6, iTextSharp.text.Font.NORMAL, BaseColor.BLUE);
            //iTextSharp.text.Font font1 = FontFactory.GetFont(FontFactory.TIMES_BOLD, 9);
            iTextSharp.text.Font font1 = FontFactory.GetFont("Arial", 9, iTextSharp.text.Font.BOLD, BaseColor.BLACK);
            iTextSharp.text.Font font2 = FontFactory.GetFont(FontFactory.COURIER, 8);
            iTextSharp.text.Font font3 = FontFactory.GetFont(FontFactory.COURIER_BOLD, 9);
            iTextSharp.text.Font fontArea = FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.BOLD, BaseColor.BLACK);
            iTextSharp.text.Font fontExaCab = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, BaseColor.WHITE);
            iTextSharp.text.Font fontCmt = FontFactory.GetFont(FontFactory.COURIER_OBLIQUE, 7);
            //DEFINIR FUENTE PARA RESULTADOS FUERA DE RANGO
            iTextSharp.text.Font fontFR = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD, BaseColor.BLUE);


            PdfPTable tbfooter = new PdfPTable(3);
            //tbfooter.TotalWidth = document.PageSize.Width - document.LeftMargin - document.RightMargin;
            tbfooter.TotalWidth = 550f;
            tbfooter.LockedWidth = true;
            tbfooter.DefaultCell.Border = 0;

            var _cellCodigo = new PdfPCell(new Paragraph(new Chunk(NumOrden, FontFactory.GetFont("Arial", 6, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLUE))));
            _cellCodigo.Border = Rectangle.TOP_BORDER;
            tbfooter.AddCell(_cellCodigo);

            var _cellPaciente = new PdfPCell(new Paragraph(new Chunk(NombrePaciente, FontFactory.GetFont("Arial", 6, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLUE))));
            _cellPaciente.HorizontalAlignment = Element.ALIGN_CENTER;
            //_cellPaciente.BorderWidthTop = 1;
            _cellPaciente.Border = Rectangle.TOP_BORDER;
            _cellPaciente.BorderColorTop = BaseColor.BLUE;
            tbfooter.AddCell(_cellPaciente);

            var _cellPage = new PdfPCell(new Paragraph(writer.PageNumber.ToString(), FontFactory.GetFont("Arial", 6, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLUE)));//For page no.
            _cellPage.HorizontalAlignment = Element.ALIGN_RIGHT;
            _cellPage.Border = Rectangle.TOP_BORDER;
            tbfooter.AddCell(_cellPage);

            float[] widths1 = new float[] { 20f, 60f, 20f };
            tbfooter.SetWidths(widths1);
            tbfooter.WriteSelectedRows(0, -1, 25f, writer.PageSize.GetBottom(doc.BottomMargin), writer.DirectContent);

            float[] anchoHDR = new float[] { 0.5f, 2.5f, 0.5f };
            PdfPTable tableHDR = new PdfPTable(3);
            //tableHDR.HeaderRows=1;
            tableHDR.SetWidths(anchoHDR);
            tableHDR.TotalWidth = 550f;
            tableHDR.LockedWidth = true;
            PdfPCell cell = new PdfPCell(logo);
            cell.Rowspan = 4;
            cell.Border = 0;
            cell.HorizontalAlignment = 0;
            tableHDR.AddCell(cell);
            cell = new PdfPCell(new Phrase("MINISTERIO DE SALUD PÚBLICA", font1));
            cell.HorizontalAlignment = 1;
            cell.Border = 0;
            tableHDR.AddCell(cell);
            PdfContentByte cb = writer.DirectContent;
            iTextSharp.text.pdf.Barcode128 bc = new Barcode128();
            bc.TextAlignment = Element.ALIGN_CENTER;
            bc.Code = NumOrden;
            bc.StartStopText = false;
            bc.CodeType = iTextSharp.text.pdf.Barcode128.CODE128;
            bc.X = 1f;
            bc.Font = null;
            bc.Extended = true;

            iTextSharp.text.Image PatImage1 = bc.CreateImageWithBarcode(cb, iTextSharp.text.BaseColor.BLACK, iTextSharp.text.BaseColor.BLACK);
            PatImage1.ScaleAbsolute(90f, 40f);
            PdfPCell palletBarcodeCell = new PdfPCell(PatImage1);
            palletBarcodeCell.Border = 0;
            palletBarcodeCell.FixedHeight = 40f;
            palletBarcodeCell.HorizontalAlignment = 2;
            palletBarcodeCell.VerticalAlignment = 1;
            palletBarcodeCell.Rowspan = 4;
            tableHDR.AddCell(palletBarcodeCell);


            cell = new PdfPCell(new Phrase(NombreLaboratorio, font1));
            cell.HorizontalAlignment = 1;
            cell.Border = 0;
            tableHDR.AddCell(cell);

            cell = new PdfPCell(new Phrase("SERVICIO DE LABORATORIO CLÍNICO", font1));
            cell.HorizontalAlignment = 1;
            cell.Border = 0;
            tableHDR.AddCell(cell);

            cell = new PdfPCell(new Phrase("INFORME DE RESULTADOS ORDEN " + NumOrden.ToString(), font1));
            cell.HorizontalAlignment = 1;
            cell.Border = 0;
            tableHDR.AddCell(cell);

            doc.Add(tableHDR);
            //tableHDR.HeaderRows = 1;

            //INSERTAR UNA LINEA
            Chunk linebreak = new Chunk(new LineSeparator());
            doc.Add(linebreak);



            PdfPTable tabla = new PdfPTable(3);
            tabla.TotalWidth = 550f;
            tabla.LockedWidth = true;
            float[] widths = new float[] { 1f, 2f, 1f };
            PdfPCell celda = new PdfPCell(new Phrase(" ", font1));
            celda.Border = 0;
            tabla.SetWidths(widths);
            celda.Colspan = 3;
            celda.HorizontalAlignment = 1;
            tabla.AddCell(celda);

            iTextSharp.text.Font font4 = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 8f, BaseColor.GRAY);
            celda = new PdfPCell(new Phrase("N° ORDEN: " + NumOrden, font4));
            celda.HorizontalAlignment = 0;
            celda.Border = 0;
            tabla.AddCell(celda);

            iTextSharp.text.Font font5 = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 8f);
            celda = new PdfPCell(new Phrase("FECHA ORDEN: " + FechaIngreso.ToString("dd/MM/yyyy HH:mm"), font4));
            celda.HorizontalAlignment = 1;
            celda.Border = 0;
            tabla.AddCell(celda);

            celda = new PdfPCell(new Phrase("ESTADO: " + EstadoPaciente.ToUpper(), font4));
            celda.HorizontalAlignment = 2;
            celda.Border = 0;
            tabla.AddCell(celda);
            doc.Add(tabla);

            PdfPTable tblSS = new PdfPTable(2);
            tblSS.TotalWidth = 550f;
            tblSS.LockedWidth = true;
            float[] tblSSW = new float[] { 2f, 1f }; //DISTRIBUCIÓN DE LAS CELDAS
            tblSS.SetWidths(tblSSW);
            tblSS.DefaultCell.Border = Rectangle.NO_BORDER;
            tblSS.AddCell(new Phrase("PACIENTE", font1));
            tblSS.AddCell(new Phrase("SOLICITANTE", font1));
            tblSS.HeaderRows = 1;
            tblSS.AddCell(new Phrase("PACIENTE: " + NombrePaciente.ToUpper(), font2));
            tblSS.AddCell(new Phrase("MEDICO: "+ nombreMedico.ToUpper(), font2));
            tblSS.AddCell(new Phrase("EDAD: " + EdadPaciente, font2));
            tblSS.AddCell(new Phrase("TELEFONO: "+telefonoMedico, font2));
            switch (GeneroPaciente)
            {
                case "M":
                    tblSS.AddCell(new Phrase("SEXO: HOMBRE", font2));
                    break;
                case "F":
                    tblSS.AddCell(new Phrase("SEXO: MUJER", font2));
                    break;
                default:
                    break;
            }
            tblSS.AddCell(new Phrase("EMAIL: "+correoMedico, font2));
            tblSS.AddCell(new Phrase("TOMA DE MUESTRA: " + FechaIngreso.ToString("dd/MM/yyyy HH:mm"), font2));
            tblSS.AddCell(new Phrase("ORIGEN: " + NombreSede, font2));
            tblSS.AddCell(new Phrase("RECEPCION DE MUESTRA: ACEPTADA", font2));
            tblSS.AddCell(new Phrase("", font2));
            doc.Add(linebreak);

            //AGREGAR ESPACIO EN BLANCO ENTRE LAS AREAS
            celda = new PdfPCell(new Phrase("", font1))
            {
                BackgroundColor = new BaseColor(255, 255, 255)
            };
            celda.Border = Rectangle.NO_BORDER;
            celda.Colspan = 4;
            celda.HorizontalAlignment = 1;
            tblSS.AddCell(celda);
            doc.Add(tblSS);


            PdfPTable seccionNueva = new PdfPTable(1);
            seccionNueva.TotalWidth = 550f;
            seccionNueva.LockedWidth = true;
            PdfPCell celdaSeccion = new PdfPCell();
            celdaSeccion = new PdfPCell(new Phrase(" ", font1));
            celdaSeccion.Border = Rectangle.TOP_BORDER;
            celdaSeccion.Colspan = 4;
            celdaSeccion.HorizontalAlignment = 1;
            seccionNueva.AddCell(celdaSeccion);
            doc.Add(linebreak);
            doc.Add(seccionNueva);

            //AGRUPAR AREAS DE ESTUDIO
            string nArea = "";
            string aArea = "";
            string eNombreExamen = "";
            string eResultadoT = "";
            string eUnidades = "";
            string eReferencia = "";
            string eRangoMin = "";
            string eRangoMax = "";
            string enValidado = "";
            string eaValidado = "";
            string eunValidado = "";
            string euaValidado = "";
            DateTime fnValidacion;
            string eComentario = "";

            bool cabeceraExamen = false;
            float[] widthsArea = new float[] { 2f, 1f, 1f, 1f };

            PdfPTable tblAreas = new PdfPTable(4);
            tblAreas.TotalWidth = 550f;
            tblAreas.LockedWidth = true;

            //DETALLE DE LOS RESULTADOS
            foreach (var data in Resultados)
            {
                nArea = data.nombreArea.ToString();
                eNombreExamen = data.nombreExamen.ToString();
                //eResultadoN = data.resultadoActual;
                eResultadoT = data.resultadoActual ?? "-";
                //eNombreSede 
                eUnidades = (string)(data.siglasUnidad ?? "");
                eReferencia = "[" + data.refMinima.ToString() + " - " + data.refMaxima.ToString() + "]";
                eRangoMin = data.refMinima.ToString();
                eRangoMax = data.refMaxima.ToString();
                uValidador = data.usuarioValidacion;
                fValidacion = data.fechaValidacion;
                bool rValidado = data.validado;
                if (ExTodos == true)
                    rValidado = false;

            if (ExValidado == rValidado)
            {
                    switch (uValidador)
                {
                    //USUARIOS EL ANGEL
                    case 5244:
                        usuarioValidador = "ANDREA BRAVO";
                        break;

                    case 5243:
                        usuarioValidador = "PAOLA LUNA";
                        break;

                    case 5250:
                        usuarioValidador = "JOSE GUAYANLEMA";
                        break;

                    case 5252:
                        usuarioValidador = "JOHANNA MAYANQUER";
                        break;

                    case 5251:
                        usuarioValidador = "ERIK CORTEZ";
                        break;

                    //USUARIOS CS MANTA TIPO C
                    case 5333:
                        usuarioValidador = "CLAUDIA MENDOZA";
                        break;

                    case 5334:
                        usuarioValidador = "MARIBEL MENDOZA";
                        break;

                    case 5335:
                        usuarioValidador = "CAROLINA LOOR";
                        break;

                    case 5336:
                        usuarioValidador = "MARIA CORDERO";
                        break;

                    case 5337:
                        usuarioValidador = "GINA DELGADO";
                        break;

                    case 5338:
                        usuarioValidador = "JANINA BENITEZ";
                        break;

                    case 5339:
                        usuarioValidador = "VERONICA CEDEÑO";
                        break;

                    case 5340:
                        usuarioValidador = "JORGE CHIQUITO";
                        break;

                    case 5341:
                        usuarioValidador = "VICTOR FERRIN";
                        break;

                    case 5242:
                        usuarioValidador = "CARLOS ANCHUNDIA";
                        break;

                        //USUARIOS CS MANTA CUBA LIBRE
                        case 5348:
                            usuarioValidador = "PEDRO PALACIOS";
                            break;

                        case 5349:
                            usuarioValidador = "ADRIANA VELEZ";
                            break;

                        case 5350:
                            usuarioValidador = "GINGER SUAREZ";
                            break;

                        case 5351:
                            usuarioValidador = "ANGEL GARCIA";
                            break;

                        case 5352:
                            usuarioValidador = "ANTONIO ALCIVAR";
                            break;

                        case 5353:
                            usuarioValidador = "GENNY PAZMIÑO";
                            break;

                        default:
                        usuarioValidador = "??";
                        break;
                }

                iTextSharp.text.Image firma = iTextSharp.text.Image.GetInstance(Environment.CurrentDirectory + "\\img\\" + uaValidador + ".png");
                enValidado = data.validado.ToString();
                eunValidado = usuarioValidador;
                fnValidacion = fValidacion;
                eComentario = (string)(data.Comentario ?? "");



                if (aArea == nArea)
                {
                    //AGREGAR LOS EXAMENES
                    celda.Border = 0;
                    if (cabeceraExamen)
                    {
                        //AGREGAR LOS EXAMENES ASOCIADOS
                        //iTextSharp.text.Font font3 = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 8f, BaseColor.Blue);
                        celda = new PdfPCell(new Phrase(eNombreExamen, font2));
                        celda.Border = 0;
                        tblAreas.AddCell(celda);

                        // bool resultadoE = float.TryParse(eResultadoT, out eResultadoN);
                        if (!Regex.IsMatch(eResultadoT, "^[0-9]+([.][0-9]+)?$"))
                        {
                            celda = new PdfPCell(new Phrase(eResultadoT, font2));
                            celda.Border = 0;
                            tblAreas.AddCell(celda);
                        }
                        else
                        {
                            if (float.Parse(eResultadoT) <= float.Parse(eRangoMin) || float.Parse(eResultadoT) >= float.Parse(eRangoMax))
                            {
                                celda = new PdfPCell(new Phrase("* " + eResultadoT, fontFR));
                                celda.Border = 0;
                                tblAreas.AddCell(celda);
                            }
                            else
                            {
                                celda = new PdfPCell(new Phrase(eResultadoT, font2));
                                celda.Border = 0;
                                tblAreas.AddCell(celda);
                            }
                        }




                        celda = new PdfPCell(new Phrase(eUnidades, font2));
                        celda.Border = 0;
                        tblAreas.AddCell(celda);

                        celda = new PdfPCell(new Phrase(eReferencia, font2));
                        celda.Border = 0;
                        tblAreas.AddCell(celda);
                        Console.WriteLine(eNombreExamen);
                        //COMENTARIOS DE EXAMENES / REFERENCIAS TEXTUALES
                        if (eComentario != null && eComentario != "")
                        {
                            //PdfPTable tablaCMT = new PdfPTable(1);
                            celda = new PdfPCell(new Phrase(eComentario, fontCmt));
                            celda.Border = 0;
                            celda.Colspan = 4;
                            celda.HorizontalAlignment = 0;
                            tblAreas.AddCell(celda);
                            //document.Add(tablaCMT);
                            Console.WriteLine("COMENTARIO..." + eComentario);
                        }
                    }
                }
                else
                {
                    celda.Border = 0;

                    if (nArea != aArea && aArea != "" && eaValidado == "True")
                    {
                        celda = new PdfPCell(new Phrase("VALIDADO POR " + euaValidado + " " + faValidacion.ToString("dd/MM/yyyy HH:mm"), font2))
                        {
                            BackgroundColor = new BaseColor(255, 255, 255)
                        };
                        celda.Border = 0;
                        celda.Colspan = 3;
                        celda.HorizontalAlignment = 2;
                        celda.VerticalAlignment = 2;
                        tblAreas.AddCell(celda);
                        Console.WriteLine("VALIDADO");
                        firma.ScalePercent(10);
                        firma.SetAbsolutePosition(0, 0);
                        celda = new PdfPCell(firma);
                        celda.Border = 0;
                        celda.BackgroundColor = new BaseColor(255, 255, 255);
                        tblAreas.AddCell(celda);

                        //AGREGAR ESPACIO EN BLANCO ENTRE LAS AREAS
                        celda = new PdfPCell(new Phrase(" ", font1))
                        {
                            BackgroundColor = new BaseColor(255, 255, 255)
                        };
                        celda.Border = 0;
                        celda.Colspan = 4;
                        celda.HorizontalAlignment = 1;
                        tblAreas.AddCell(celda);
                    }
                    else if (nArea != aArea && aArea != "" && eaValidado == "False")
                    {
                        //AGREGAR LINEA DE NO VALIDACION
                        celda = new PdfPCell(new Phrase("NO VALIDADO ", font2))
                        {
                            BackgroundColor = new BaseColor(255, 255, 255)
                        };
                        celda.Border = 0;
                        celda.Colspan = 4;
                        celda.HorizontalAlignment = 2;
                        tblAreas.AddCell(celda);
                        Console.WriteLine("VALIDADO");

                        //AGREGAR ESPACIO EN BLANCO ENTRE LAS AREAS
                        celda = new PdfPCell(new Phrase(" ", font1))
                        {
                            BackgroundColor = new BaseColor(255, 255, 255)
                        };
                        celda.Border = 0;
                        celda.Colspan = 4;
                        celda.HorizontalAlignment = 1;
                        tblAreas.AddCell(celda);
                    }


                    //AGREGAR LAS AREAS
                    Console.WriteLine(nArea);
                    celda = new PdfPCell(new Phrase(nArea, fontArea))
                    {
                        BackgroundColor = new BaseColor(255, 255, 255)
                    };
                    celda.Border = 0;
                    tblAreas.SetWidths(widthsArea);
                    celda.Colspan = 4;
                    celda.HorizontalAlignment = 1;
                    celda.VerticalAlignment = 1;


                    tblAreas.AddCell(celda);


                    //AGREGAR LAS CABECERAS DEL DETALLE
                    celda.Border = 1;
                    celda.BackgroundColor = BaseColor.BLACK;
                    celda = new PdfPCell(new Phrase("EXAMEN", fontExaCab))
                    {
                        BackgroundColor = new BaseColor(0, 0, 0)
                    };
                    tblAreas.AddCell(celda);
                    celda = new PdfPCell(new Phrase("RESULTADO", fontExaCab))
                    {
                        BackgroundColor = new BaseColor(0, 0, 0)
                    };
                    tblAreas.AddCell(celda);
                    celda = new PdfPCell(new Phrase("UNIDADES", fontExaCab))
                    {
                        BackgroundColor = new BaseColor(0, 0, 0)
                    };
                    tblAreas.AddCell(celda);
                    celda = new PdfPCell(new Phrase("REFERENCIA", fontExaCab))
                    {
                        BackgroundColor = new BaseColor(0, 0, 0)
                    };
                    tblAreas.AddCell(celda);
                    //tblAreas.HeaderRows = 1;
                    cabeceraExamen = true;
                    aArea = nArea;
                    eaValidado = enValidado;
                    euaValidado = usuarioValidador;
                    uaValidador = uValidador;
                    faValidacion = fnValidacion;

                    if (cabeceraExamen)
                    {
                        Console.WriteLine(eNombreExamen + "|" + eResultadoT + "|" + eUnidades + "|" + eReferencia);
                        //AGREGAR LOS EXAMENES ASOCIADOS
                        //iTextSharp.text.Font font3 = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 8f, BaseColor.Blue);
                        celda = new PdfPCell(new Phrase(eNombreExamen, font2));
                        celda.Border = 0;
                        tblAreas.AddCell(celda);

                        if (!Regex.IsMatch(eResultadoT, "^[0-9]+([.][0-9]+)?$"))
                        {
                            celda = new PdfPCell(new Phrase(eResultadoT, font2));
                            celda.Border = 0;
                            tblAreas.AddCell(celda);
                        }
                        else
                        {
                            if (float.Parse(eResultadoT) <= float.Parse(eRangoMin) || float.Parse(eResultadoT) >= float.Parse(eRangoMax))
                            {
                                celda = new PdfPCell(new Phrase("* " + eResultadoT, fontFR));
                                celda.Border = 0;
                                tblAreas.AddCell(celda);
                            }
                            else
                            {
                                celda = new PdfPCell(new Phrase(eResultadoT, font2));
                                celda.Border = 0;
                                tblAreas.AddCell(celda);
                            }
                        }

                        celda = new PdfPCell(new Phrase(eUnidades, font2));
                        celda.Border = 0;
                        tblAreas.AddCell(celda);

                        celda = new PdfPCell(new Phrase(eReferencia, font2));
                        celda.Border = 0;
                        tblAreas.AddCell(celda);

                        //COMENTARIOS DE EXAMENES / REFERENCIAS TEXTUALES
                        if (eComentario != null && eComentario != "")
                        {
                            //PdfPTable tablaCMT = new PdfPTable(1);
                            celda = new PdfPCell(new Phrase(eComentario, fontCmt));
                            celda.Colspan = 4;
                            celda.Border = 0;
                            celda.HorizontalAlignment = 0;
                            tblAreas.AddCell(celda);
                            //document.Add(tablaCMT);
                            Console.WriteLine("COMENTARIO..." + eComentario);
                        }
                    }
                }

            }//End If rValidado

        }//End Foreach
            if (usuarioValidador == "NV")
                {
                    celda = new PdfPCell(new Phrase("NO VALIDADO", font2))
                    {
                        BackgroundColor = new BaseColor(255, 255, 255)
                    };
                }
                else
                {

                    celda = new PdfPCell(new Phrase("VALIDADO POR " + euaValidado + " " + faValidacion.ToString("dd/MM/yyyy HH:mm"), font2))

                    {
                        BackgroundColor = new BaseColor(255, 255, 255)
                    };
                }
                celda.Border = 0;
                celda.Colspan = 3;
                celda.HorizontalAlignment = 2;
                tblAreas.AddCell(celda);
                Console.WriteLine("VALIDADO");
                iTextSharp.text.Image firma1 = iTextSharp.text.Image.GetInstance(Environment.CurrentDirectory +"\\img\\" + uaValidador + ".png");
                firma1.ScalePercent(10);
                firma1.SetAbsolutePosition(0, 0);
                celda = new PdfPCell(firma1);
                celda.Border = 0;
                celda.BackgroundColor = new BaseColor(255, 255, 255);
                tblAreas.AddCell(celda);

                doc.Add(tblAreas);


            PdfPTable tbfooterA = new PdfPTable(3);
            //tbfooter.TotalWidth = document.PageSize.Width - document.LeftMargin - document.RightMargin;
            tbfooterA.TotalWidth = 550f;
            tbfooterA.LockedWidth = true;
            tbfooterA.DefaultCell.Border = 0;
            var _cellCodigoF = new PdfPCell(new Paragraph(new Chunk(NumOrden, FontFactory.GetFont("Arial", 6, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLUE))));
            _cellCodigoF.Border = Rectangle.TOP_BORDER;
            tbfooterA.AddCell(_cellCodigoF);

            var _cellA = new PdfPCell(new Paragraph(new Chunk(NombrePaciente, FontFactory.GetFont("Arial", 6, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLUE))));
            _cellA.HorizontalAlignment = Element.ALIGN_CENTER;
            _cellA.Border = Rectangle.TOP_BORDER;
            _cellA.BorderColorTop = BaseColor.BLUE;
            tbfooterA.AddCell(_cellA);

            var _cellB = new PdfPCell(new Paragraph(writer.PageNumber.ToString(), FontFactory.GetFont("Arial", 6, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLUE)));//For page no.
            _cellB.HorizontalAlignment = Element.ALIGN_RIGHT;
            _cellB.Border = Rectangle.TOP_BORDER;
            tbfooterA.AddCell(_cellB);

            float[] widthsA = new float[] { 20f, 60f, 20f };
            tbfooterA.SetWidths(widthsA);
            tbfooterA.WriteSelectedRows(0, -1, 25f, writer.PageSize.GetBottom(doc.BottomMargin), writer.DirectContent);


            doc.Close();
            writer.Close();
        }


        //IMPRIMIR PDF POR AREA DE ESTUDIO
        public void crearPDFFileArea(
        string NumOrden,
        string NombrePaciente,
        string GeneroPaciente,
        string nombreMedico,
        string telefonoMedico,
        string correoMedico,
        string NombreLaboratorio,
        string NombreSede,
        DateTime FechaIngreso,
        string Origen,
        string EdadPaciente,
        string EstadoPaciente,
        List<ResultadoOrden> Resultados,
        string Area,
        bool ExValidado = false,
        bool ExTodos = false
        )
        {
            bool areaProcesada=false;
            string root = ObtenerRuta(RutaDestino, Origen + "\\" + FechaIngreso.ToString("ddMMyyyy"));
            var uncPath = root + filename + "_" + NumOrden + ".pdf";
            var uncTitle = NumOrden + "_" + NombrePaciente;

            FileStream fs = new FileStream(uncPath, FileMode.Create);
            Document doc = new Document(PageSize.A4, 1, 1, 20, 20);
            PdfWriter writer = PdfWriter.GetInstance(doc, fs);
            doc.Open();
            doc.AddAuthor("GALILEO | LIMS");
            doc.AddTitle(uncTitle);
            iTextSharp.text.Image logo = iTextSharp.text.Image.GetInstance(Environment.CurrentDirectory + "\\img\\logo.png");
            logo.SetAbsolutePosition(0, 0);
            var escalaLogo = ((50 * logo.Width) / 100);
            logo.ScalePercent(escalaLogo);
            iTextSharp.text.Font font0 = FontFactory.GetFont("Arial", 6, iTextSharp.text.Font.NORMAL, BaseColor.BLUE);
            //iTextSharp.text.Font font1 = FontFactory.GetFont(FontFactory.TIMES_BOLD, 9);
            iTextSharp.text.Font font1 = FontFactory.GetFont("Arial", 9, iTextSharp.text.Font.BOLD, BaseColor.BLACK);
            iTextSharp.text.Font font2 = FontFactory.GetFont(FontFactory.COURIER, 8);
            iTextSharp.text.Font font3 = FontFactory.GetFont(FontFactory.COURIER_BOLD, 9);
            iTextSharp.text.Font fontArea = FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.BOLD, BaseColor.BLACK);
            iTextSharp.text.Font fontExaCab = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, BaseColor.WHITE);
            iTextSharp.text.Font fontCmt = FontFactory.GetFont(FontFactory.COURIER_OBLIQUE, 7);
            //DEFINIR FUENTE PARA RESULTADOS FUERA DE RANGO
            iTextSharp.text.Font fontFR = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD, BaseColor.BLUE);


            PdfPTable tbfooter = new PdfPTable(3);
            //tbfooter.TotalWidth = document.PageSize.Width - document.LeftMargin - document.RightMargin;
            tbfooter.TotalWidth = 550f;
            tbfooter.LockedWidth = true;
            tbfooter.DefaultCell.Border = 0;

            var _cellCodigo = new PdfPCell(new Paragraph(new Chunk(NumOrden, FontFactory.GetFont("Arial", 6, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLUE))));
            _cellCodigo.Border = Rectangle.TOP_BORDER;
            tbfooter.AddCell(_cellCodigo);

            var _cellPaciente = new PdfPCell(new Paragraph(new Chunk(NombrePaciente, FontFactory.GetFont("Arial", 6, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLUE))));
            _cellPaciente.HorizontalAlignment = Element.ALIGN_CENTER;
            //_cellPaciente.BorderWidthTop = 1;
            _cellPaciente.Border = Rectangle.TOP_BORDER;
            _cellPaciente.BorderColorTop = BaseColor.BLUE;
            tbfooter.AddCell(_cellPaciente);

            var _cellPage = new PdfPCell(new Paragraph(writer.PageNumber.ToString(), FontFactory.GetFont("Arial", 6, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLUE)));//For page no.
            _cellPage.HorizontalAlignment = Element.ALIGN_RIGHT;
            _cellPage.Border = Rectangle.TOP_BORDER;
            tbfooter.AddCell(_cellPage);

            float[] widths1 = new float[] { 20f, 60f, 20f };
            tbfooter.SetWidths(widths1);
            tbfooter.WriteSelectedRows(0, -1, 25f, writer.PageSize.GetBottom(doc.BottomMargin), writer.DirectContent);

            float[] anchoHDR = new float[] { 0.5f, 2.5f, 0.5f };
            PdfPTable tableHDR = new PdfPTable(3);
            //tableHDR.HeaderRows=1;
            tableHDR.SetWidths(anchoHDR);
            tableHDR.TotalWidth = 550f;
            tableHDR.LockedWidth = true;
            PdfPCell cell = new PdfPCell(logo);
            cell.Rowspan = 4;
            cell.Border = 0;
            cell.HorizontalAlignment = 0;
            tableHDR.AddCell(cell);
            cell = new PdfPCell(new Phrase("MINISTERIO DE SALUD PÚBLICA", font1));
            cell.HorizontalAlignment = 1;
            cell.Border = 0;
            tableHDR.AddCell(cell);
            PdfContentByte cb = writer.DirectContent;
            iTextSharp.text.pdf.Barcode128 bc = new Barcode128();
            bc.TextAlignment = Element.ALIGN_CENTER;
            bc.Code = NumOrden;
            bc.StartStopText = false;
            bc.CodeType = iTextSharp.text.pdf.Barcode128.CODE128;
            bc.X = 1f;
            bc.Font = null;
            bc.Extended = true;

            iTextSharp.text.Image PatImage1 = bc.CreateImageWithBarcode(cb, iTextSharp.text.BaseColor.BLACK, iTextSharp.text.BaseColor.BLACK);
            PatImage1.ScaleAbsolute(90f, 40f);
            PdfPCell palletBarcodeCell = new PdfPCell(PatImage1);
            palletBarcodeCell.Border = 0;
            palletBarcodeCell.FixedHeight = 40f;
            palletBarcodeCell.HorizontalAlignment = 2;
            palletBarcodeCell.VerticalAlignment = 1;
            palletBarcodeCell.Rowspan = 4;
            tableHDR.AddCell(palletBarcodeCell);


            cell = new PdfPCell(new Phrase(NombreLaboratorio, font1));
            cell.HorizontalAlignment = 1;
            cell.Border = 0;
            tableHDR.AddCell(cell);

            cell = new PdfPCell(new Phrase("SERVICIO DE LABORATORIO CLÍNICO", font1));
            cell.HorizontalAlignment = 1;
            cell.Border = 0;
            tableHDR.AddCell(cell);

            cell = new PdfPCell(new Phrase("INFORME DE RESULTADOS ORDEN " + NumOrden.ToString(), font1));
            cell.HorizontalAlignment = 1;
            cell.Border = 0;
            tableHDR.AddCell(cell);

            doc.Add(tableHDR);
            //tableHDR.HeaderRows = 1;

            //INSERTAR UNA LINEA
            Chunk linebreak = new Chunk(new LineSeparator());
            doc.Add(linebreak);



            PdfPTable tabla = new PdfPTable(3);
            tabla.TotalWidth = 550f;
            tabla.LockedWidth = true;
            float[] widths = new float[] { 1f, 2f, 1f };
            PdfPCell celda = new PdfPCell(new Phrase(" ", font1));
            celda.Border = 0;
            tabla.SetWidths(widths);
            celda.Colspan = 3;
            celda.HorizontalAlignment = 1;
            tabla.AddCell(celda);

            iTextSharp.text.Font font4 = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 8f, BaseColor.GRAY);
            celda = new PdfPCell(new Phrase("N° ORDEN: " + NumOrden, font4));
            celda.HorizontalAlignment = 0;
            celda.Border = 0;
            tabla.AddCell(celda);

            iTextSharp.text.Font font5 = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 8f);
            celda = new PdfPCell(new Phrase("FECHA ORDEN: " + FechaIngreso.ToString("dd/MM/yyyy HH:mm"), font4));
            celda.HorizontalAlignment = 1;
            celda.Border = 0;
            tabla.AddCell(celda);

            celda = new PdfPCell(new Phrase("ESTADO: " + EstadoPaciente.ToUpper(), font4));
            celda.HorizontalAlignment = 2;
            celda.Border = 0;
            tabla.AddCell(celda);
            doc.Add(tabla);

            PdfPTable tblSS = new PdfPTable(2);
            tblSS.TotalWidth = 550f;
            tblSS.LockedWidth = true;
            float[] tblSSW = new float[] { 2f, 1f };
            tblSS.SetWidths(tblSSW);
            tblSS.DefaultCell.Border = Rectangle.NO_BORDER;
            tblSS.AddCell(new Phrase("PACIENTE", font1));
            tblSS.AddCell(new Phrase("SOLICITANTE", font1));
            tblSS.HeaderRows = 1;
            tblSS.AddCell(new Phrase("PACIENTE: " + NombrePaciente.ToUpper(), font2));
            tblSS.AddCell(new Phrase("MEDICO: "+ nombreMedico.ToUpper(), font2));
            tblSS.AddCell(new Phrase("EDAD: " + EdadPaciente, font2));
            tblSS.AddCell(new Phrase("TELEFONO: "+telefonoMedico, font2));
            switch (GeneroPaciente)
            {
                case "M":
                    tblSS.AddCell(new Phrase("SEXO: HOMBRE", font2));
                    break;
                case "F":
                    tblSS.AddCell(new Phrase("SEXO: MUJER", font2));
                    break;
                default:
                    break;
            }
            tblSS.AddCell(new Phrase("EMAIL: "+correoMedico, font2));
            tblSS.AddCell(new Phrase("TOMA DE MUESTRA: " + FechaIngreso.ToString("dd/MM/yyyy HH:mm"), font2));
            tblSS.AddCell(new Phrase("ORIGEN: " + NombreSede, font2));
            tblSS.AddCell(new Phrase("RECEPCION DE MUESTRA: ACEPTADA", font2));
            tblSS.AddCell(new Phrase("", font2));
            doc.Add(linebreak);

            //AGREGAR ESPACIO EN BLANCO ENTRE LAS AREAS
            celda = new PdfPCell(new Phrase("", font1))
            {
                BackgroundColor = new BaseColor(255, 255, 255)
            };
            celda.Border = Rectangle.NO_BORDER;
            celda.Colspan = 4;
            celda.HorizontalAlignment = 1;
            tblSS.AddCell(celda);
            doc.Add(tblSS);


            PdfPTable seccionNueva = new PdfPTable(1);
            seccionNueva.TotalWidth = 550f;
            seccionNueva.LockedWidth = true;
            PdfPCell celdaSeccion = new PdfPCell();
            celdaSeccion = new PdfPCell(new Phrase(" ", font1));
            celdaSeccion.Border = Rectangle.TOP_BORDER;
            celdaSeccion.Colspan = 4;
            celdaSeccion.HorizontalAlignment = 1;
            seccionNueva.AddCell(celdaSeccion);
            doc.Add(linebreak);
            doc.Add(seccionNueva);

            //AGRUPAR AREAS DE ESTUDIO
            string nArea = "";
            string aArea = "";
            string eNombreExamen = "";
            string eResultadoT = "";
            string eUnidades = "";
            string eReferencia = "";
            string eRangoMin = "";
            string eRangoMax = "";
            string enValidado = "";
            string eaValidado = "";
            string eunValidado = "";
            string euaValidado = "";
            //DateTime faValidacion;
            DateTime fnValidacion;

            string eComentario = "";

            bool cabeceraExamen = false;
            float[] widthsArea = new float[] { 2f, 1f, 1f, 1f };

            PdfPTable tblAreas = new PdfPTable(4);
            tblAreas.TotalWidth = 550f;
            tblAreas.LockedWidth = true;

            //DETALLE DE LOS RESULTADOS
            foreach (var data in Resultados)
            {
                nArea = data.nombreArea.ToString();
                eNombreExamen = data.nombreExamen.ToString();
                //eResultadoN = data.resultadoActual;
                eResultadoT = data.resultadoActual ?? "-";
                //eNombreSede 
                eUnidades = (string)(data.siglasUnidad ?? "");
                eReferencia = "[" + data.refMinima.ToString() + " - " + data.refMaxima.ToString() + "]";
                eRangoMin = data.refMinima.ToString();
                eRangoMax = data.refMaxima.ToString();
                uValidador = data.usuarioValidacion;
                fValidacion = data.fechaValidacion;
                bool rValidado = data.validado;
                if (ExTodos == true)
                    rValidado = false;

                if (ExValidado == rValidado)
            {
                    switch (uValidador)
                    {
                        //USUARIOS EL ANGEL
                        case 5244:
                            usuarioValidador = "ANDREA BRAVO";
                            break;

                        case 5243:
                            usuarioValidador = "PAOLA LUNA";
                            break;

                        case 5250:
                            usuarioValidador = "JOSE GUAYANLEMA";
                            break;

                        case 5252:
                            usuarioValidador = "JOHANNA MAYANQUER";
                            break;

                        case 5251:
                            usuarioValidador = "ERIK CORTEZ";
                            break;

                        //USUARIOS CS MANTA TIPO C
                        case 5333:
                            usuarioValidador = "CLAUDIA MENDOZA";
                            break;

                        case 5334:
                            usuarioValidador = "MARIBEL MENDOZA";
                            break;

                        case 5335:
                            usuarioValidador = "CAROLINA LOOR";
                            break;

                        case 5336:
                            usuarioValidador = "MARIA CORDERO";
                            break;

                        case 5337:
                            usuarioValidador = "GINA DELGADO";
                            break;

                        case 5338:
                            usuarioValidador = "JANINA BENITEZ";
                            break;

                        case 5339:
                            usuarioValidador = "VERONICA CEDEÑO";
                            break;

                        case 5340:
                            usuarioValidador = "JORGE CHIQUITO";
                            break;

                        case 5341:
                            usuarioValidador = "VICTOR FERRIN";
                            break;

                        case 5242:
                            usuarioValidador = "CARLOS ANCHUNDIA";
                            break;

                        //USUARIOS CS MANTA CUBA LIBRE
                        case 5348:
                            usuarioValidador = "PEDRO PALACIOS";
                            break;

                        case 5349:
                            usuarioValidador = "ADRIANA VELEZ";
                            break;

                        case 5350:
                            usuarioValidador = "GINGER SUAREZ";
                            break;

                        case 5351:
                            usuarioValidador = "ANGEL GARCIA";
                            break;

                        case 5352:
                            usuarioValidador = "ANTONIO ALCIVAR";
                            break;

                        case 5353:
                            usuarioValidador = "GENNY PAZMIÑO";
                            break;

                        default:
                            usuarioValidador = "??";
                            break;
                    }

                    iTextSharp.text.Image firma = iTextSharp.text.Image.GetInstance(Environment.CurrentDirectory + "\\img\\" + uaValidador + ".png");
                enValidado = data.validado.ToString();
                eunValidado = usuarioValidador;
                fnValidacion = fValidacion;
                eComentario = (string)(data.Comentario ?? "");
                if (nArea == Area) {
                    areaProcesada = true;
                    if (aArea == nArea)
                    {
                        //AGREGAR LOS EXAMENES
                        celda.Border = 0;
                        if (cabeceraExamen)
                        {
                            //AGREGAR LOS EXAMENES ASOCIADOS
                            //iTextSharp.text.Font font3 = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 8f, BaseColor.Blue);
                            celda = new PdfPCell(new Phrase(eNombreExamen, font2));
                            celda.Border = 0;
                            tblAreas.AddCell(celda);

                            // bool resultadoE = float.TryParse(eResultadoT, out eResultadoN);
                            if (!Regex.IsMatch(eResultadoT, "^[0-9]+([.][0-9]+)?$"))
                            {
                                celda = new PdfPCell(new Phrase(eResultadoT, font2));
                                celda.Border = 0;
                                tblAreas.AddCell(celda);
                            }
                            else
                            {
                                if (float.Parse(eResultadoT) <= float.Parse(eRangoMin) || float.Parse(eResultadoT) >= float.Parse(eRangoMax))
                                {
                                    celda = new PdfPCell(new Phrase("* " + eResultadoT, fontFR));
                                    celda.Border = 0;
                                    tblAreas.AddCell(celda);
                                }
                                else
                                {
                                    celda = new PdfPCell(new Phrase(eResultadoT, font2));
                                    celda.Border = 0;
                                    tblAreas.AddCell(celda);
                                }
                            }




                            celda = new PdfPCell(new Phrase(eUnidades, font2));
                            celda.Border = 0;
                            tblAreas.AddCell(celda);

                            celda = new PdfPCell(new Phrase(eReferencia, font2));
                            celda.Border = 0;
                            tblAreas.AddCell(celda);
                            Console.WriteLine(eNombreExamen);
                            //COMENTARIOS DE EXAMENES / REFERENCIAS TEXTUALES
                            if (eComentario != null && eComentario != "")
                            {
                                //PdfPTable tablaCMT = new PdfPTable(1);
                                celda = new PdfPCell(new Phrase(eComentario, fontCmt));
                                celda.Border = 0;
                                celda.Colspan = 4;
                                celda.HorizontalAlignment = 0;
                                tblAreas.AddCell(celda);
                                //document.Add(tablaCMT);
                                Console.WriteLine("COMENTARIO..." + eComentario);
                            }
                        }
                    }
                    else
                    {
                        celda.Border = 0;

                        //if (nArea != aArea && aArea != "" && eaValidado == "True")
                        //{
                        //    //AGREGAR USUARIO VALIDADOR
                        //    celda = new PdfPCell(new Phrase("VALIDADO POR " + usuarioValidador + " " + fValidacion.ToString("dd/MM/yyyy HH:mm"), font2))
                        //    {
                        //        BackgroundColor = new BaseColor(255, 255, 255)
                        //    };
                        //    celda.Border = 0;
                        //    celda.Colspan = 4;
                        //    celda.HorizontalAlignment = 2;
                        //    tblAreas.AddCell(celda);
                        //    Console.WriteLine("VALIDADO");

                        //    //AGREGAR ESPACIO EN BLANCO ENTRE LAS AREAS
                        //    celda = new PdfPCell(new Phrase(" ", font1))
                        //    {
                        //        BackgroundColor = new BaseColor(255, 255, 255)
                        //    };
                        //    celda.Border = 0;
                        //    celda.Colspan = 4;
                        //    celda.HorizontalAlignment = 1;
                        //    tblAreas.AddCell(celda);
                        //}


                        //AGREGAR LAS AREAS
                        Console.WriteLine(nArea);
                        celda = new PdfPCell(new Phrase(nArea, fontArea))
                        {
                            BackgroundColor = new BaseColor(255, 255, 255)
                        };
                        celda.Border = 0;
                        tblAreas.SetWidths(widthsArea);
                        celda.Colspan = 4;
                        celda.HorizontalAlignment = 1;
                        celda.VerticalAlignment = 1;
                        tblAreas.AddCell(celda);
                        //AGREGAR LAS CABECERAS DEL DETALLE
                        celda.Border = 1;
                        celda.BackgroundColor = BaseColor.BLACK;
                        celda = new PdfPCell(new Phrase("EXAMEN", fontExaCab))
                        {
                            BackgroundColor = new BaseColor(0, 0, 0)
                        };
                        tblAreas.AddCell(celda);
                        celda = new PdfPCell(new Phrase("RESULTADO", fontExaCab))
                        {
                            BackgroundColor = new BaseColor(0, 0, 0)
                        };
                        tblAreas.AddCell(celda);
                        celda = new PdfPCell(new Phrase("UNIDADES", fontExaCab))
                        {
                            BackgroundColor = new BaseColor(0, 0, 0)
                        };
                        tblAreas.AddCell(celda);
                        celda = new PdfPCell(new Phrase("REFERENCIA", fontExaCab))
                        {
                            BackgroundColor = new BaseColor(0, 0, 0)
                        };
                        tblAreas.AddCell(celda);
                        //tblAreas.HeaderRows = 1;
                        cabeceraExamen = true;
                        aArea = nArea;
                        eaValidado = enValidado;
                        euaValidado = usuarioValidador;
                        uaValidador = uValidador;
                        faValidacion = fnValidacion;
                        if (cabeceraExamen)
                        {
                            Console.WriteLine(eNombreExamen + "|" + eResultadoT + "|" + eUnidades + "|" + eReferencia);
                            //AGREGAR LOS EXAMENES ASOCIADOS
                            //iTextSharp.text.Font font3 = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 8f, BaseColor.Blue);
                            celda = new PdfPCell(new Phrase(eNombreExamen, font2));
                            celda.Border = 0;
                            tblAreas.AddCell(celda);

                            if (!Regex.IsMatch(eResultadoT, "^[0-9]+([.][0-9]+)?$"))
                            {
                                celda = new PdfPCell(new Phrase(eResultadoT, font2));
                                celda.Border = 0;
                                tblAreas.AddCell(celda);
                            }
                            else
                            {
                                if (float.Parse(eResultadoT) <= float.Parse(eRangoMin) || float.Parse(eResultadoT) >= float.Parse(eRangoMax))
                                {
                                    celda = new PdfPCell(new Phrase("* " + eResultadoT, fontFR));
                                    celda.Border = 0;
                                    tblAreas.AddCell(celda);
                                }
                                else
                                {
                                    celda = new PdfPCell(new Phrase(eResultadoT, font2));
                                    celda.Border = 0;
                                    tblAreas.AddCell(celda);
                                }
                            }

                            celda = new PdfPCell(new Phrase(eUnidades, font2));
                            celda.Border = 0;
                            tblAreas.AddCell(celda);

                            celda = new PdfPCell(new Phrase(eReferencia, font2));
                            celda.Border = 0;
                            tblAreas.AddCell(celda);

                            //COMENTARIOS DE EXAMENES / REFERENCIAS TEXTUALES
                            if (eComentario != null && eComentario != "")
                            {
                                //PdfPTable tablaCMT = new PdfPTable(1);
                                celda = new PdfPCell(new Phrase(eComentario, fontCmt));
                                celda.Colspan = 4;
                                celda.Border = 0;
                                celda.HorizontalAlignment = 0;
                                tblAreas.AddCell(celda);
                                //document.Add(tablaCMT);
                                Console.WriteLine("COMENTARIO..." + eComentario);
                            }
                        }


                    }

                }
                else
                {
                    aArea = nArea;
                    //eaValidado = enValidado;
                    if (areaProcesada && (eaValidado == "False" || eaValidado == ""))
                    {
                        celda = new PdfPCell(new Phrase("NO VALIDADO", font2))
                        {
                            BackgroundColor = new BaseColor(255, 255, 255)
                        };
                        celda.Border = 0;
                        celda.Colspan = 4;
                        celda.HorizontalAlignment = 2;
                        tblAreas.AddCell(celda);
                        Console.WriteLine("VALIDADO");
                        doc.Add(tblAreas);

                        //AGREGAR PIE DE ARCHIVO
                        PdfPTable tbfooterA = new PdfPTable(3);
                        //tbfooter.TotalWidth = document.PageSize.Width - document.LeftMargin - document.RightMargin;
                        tbfooterA.TotalWidth = 550f;
                        tbfooterA.LockedWidth = true;
                        tbfooterA.DefaultCell.Border = 0;
                        var _cellCodigoF = new PdfPCell(new Paragraph(new Chunk(NumOrden, FontFactory.GetFont("Arial", 6, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLUE))));
                        _cellCodigoF.Border = Rectangle.TOP_BORDER;
                        tbfooterA.AddCell(_cellCodigoF);

                        var _cellA = new PdfPCell(new Paragraph(new Chunk(NombrePaciente, FontFactory.GetFont("Arial", 6, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLUE))));
                        _cellA.HorizontalAlignment = Element.ALIGN_CENTER;
                        _cellA.Border = Rectangle.TOP_BORDER;
                        _cellA.BorderColorTop = BaseColor.BLUE;
                        tbfooterA.AddCell(_cellA);

                        var _cellB = new PdfPCell(new Paragraph(writer.PageNumber.ToString(), FontFactory.GetFont("Arial", 6, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLUE)));//For page no.
                        _cellB.HorizontalAlignment = Element.ALIGN_RIGHT;
                        _cellB.Border = Rectangle.TOP_BORDER;
                        tbfooterA.AddCell(_cellB);

                        float[] widthsA = new float[] { 20f, 60f, 20f };
                        tbfooterA.SetWidths(widthsA);
                        tbfooterA.WriteSelectedRows(0, -1, 25f, writer.PageSize.GetBottom(doc.BottomMargin), writer.DirectContent);


                        doc.Close();
                        writer.Close();
                        return;
                    }
                    else if (areaProcesada && eaValidado == "True")
                    {
                        celda = new PdfPCell(new Phrase("VALIDADO POR " + euaValidado + " " + faValidacion.ToString("dd/MM/yyyy HH:mm"), font2))
                        {
                            BackgroundColor = new BaseColor(255, 255, 255)
                        };
                        celda.Border = 0;
                        celda.Colspan = 3;
                        celda.HorizontalAlignment = 2;
                        tblAreas.AddCell(celda);
                        Console.WriteLine("VALIDADO");
                        iTextSharp.text.Image firmaAesp = iTextSharp.text.Image.GetInstance("C:/reportes/img/" + uaValidador + ".png");
                        firmaAesp.ScalePercent(10);
                        firmaAesp.SetAbsolutePosition(0, 0);
                        celda = new PdfPCell(firmaAesp);
                        celda.Border = 0;
                        celda.BackgroundColor = new BaseColor(255, 255, 255);
                        tblAreas.AddCell(celda);

                        doc.Add(tblAreas);

                        firma.ScalePercent(10);
                        firma.SetAbsolutePosition(0, 0);
                        celda = new PdfPCell(firma);
                        celda.Border = 0;
                        celda.BackgroundColor = new BaseColor(255, 255, 255);
                        tblAreas.AddCell(celda);

                        //AGREGAR PIE DE ARCHIVO
                        PdfPTable tbfooterA = new PdfPTable(3);
                        //tbfooter.TotalWidth = document.PageSize.Width - document.LeftMargin - document.RightMargin;
                        tbfooterA.TotalWidth = 550f;
                        tbfooterA.LockedWidth = true;
                        tbfooterA.DefaultCell.Border = 0;
                        var _cellCodigoF = new PdfPCell(new Paragraph(new Chunk(NumOrden, FontFactory.GetFont("Arial", 6, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLUE))));
                        _cellCodigoF.Border = Rectangle.TOP_BORDER;
                        tbfooterA.AddCell(_cellCodigoF);

                        var _cellA = new PdfPCell(new Paragraph(new Chunk(NombrePaciente, FontFactory.GetFont("Arial", 6, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLUE))));
                        _cellA.HorizontalAlignment = Element.ALIGN_CENTER;
                        _cellA.Border = Rectangle.TOP_BORDER;
                        _cellA.BorderColorTop = BaseColor.BLUE;
                        tbfooterA.AddCell(_cellA);

                        var _cellB = new PdfPCell(new Paragraph(writer.PageNumber.ToString(), FontFactory.GetFont("Arial", 6, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLUE)));//For page no.
                        _cellB.HorizontalAlignment = Element.ALIGN_RIGHT;
                        _cellB.Border = Rectangle.TOP_BORDER;
                        tbfooterA.AddCell(_cellB);

                        float[] widthsA = new float[] { 20f, 60f, 20f };
                        tbfooterA.SetWidths(widthsA);
                        tbfooterA.WriteSelectedRows(0, -1, 25f, writer.PageSize.GetBottom(doc.BottomMargin), writer.DirectContent);


                        doc.Close();
                        writer.Close();
                        return;
                    }

                }

            }//End If Validados

        }//End Foreach

            if (areaProcesada && eaValidado == "False" || eaValidado == "")
            {
                celda = new PdfPCell(new Phrase("NO VALIDADO", font2))
                {
                    BackgroundColor = new BaseColor(255, 255, 255)
                };
                celda.Border = 0;
                celda.Colspan = 4;
                celda.HorizontalAlignment = 2;
                tblAreas.AddCell(celda);
                Console.WriteLine("VALIDADO");
                doc.Add(tblAreas);

                //AGREGAR PIE DE ARCHIVO
                PdfPTable tbfooterA = new PdfPTable(3);
                //tbfooter.TotalWidth = document.PageSize.Width - document.LeftMargin - document.RightMargin;
                tbfooterA.TotalWidth = 550f;
                tbfooterA.LockedWidth = true;
                tbfooterA.DefaultCell.Border = 0;
                var _cellCodigoF = new PdfPCell(new Paragraph(new Chunk(NumOrden, FontFactory.GetFont("Arial", 6, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLUE))));
                _cellCodigoF.Border = Rectangle.TOP_BORDER;
                tbfooterA.AddCell(_cellCodigoF);

                var _cellA = new PdfPCell(new Paragraph(new Chunk(NombrePaciente, FontFactory.GetFont("Arial", 6, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLUE))));
                _cellA.HorizontalAlignment = Element.ALIGN_CENTER;
                _cellA.Border = Rectangle.TOP_BORDER;
                _cellA.BorderColorTop = BaseColor.BLUE;
                tbfooterA.AddCell(_cellA);

                var _cellB = new PdfPCell(new Paragraph(writer.PageNumber.ToString(), FontFactory.GetFont("Arial", 6, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLUE)));//For page no.
                _cellB.HorizontalAlignment = Element.ALIGN_RIGHT;
                _cellB.Border = Rectangle.TOP_BORDER;
                tbfooterA.AddCell(_cellB);

                float[] widthsA = new float[] { 20f, 60f, 20f };
                tbfooterA.SetWidths(widthsA);
                tbfooterA.WriteSelectedRows(0, -1, 25f, writer.PageSize.GetBottom(doc.BottomMargin), writer.DirectContent);


                doc.Close();
                writer.Close();
                return;
            }
            else if (areaProcesada && eaValidado == "True")
            {
                celda = new PdfPCell(new Phrase("VALIDADO POR " + euaValidado + " " + faValidacion.ToString("dd/MM/yyyy HH:mm"), font2))
                {
                    BackgroundColor = new BaseColor(255, 255, 255)
                };
                celda.Border = 0;
                celda.Colspan = 3;
                celda.HorizontalAlignment = 2;
                tblAreas.AddCell(celda);
                Console.WriteLine("VALIDADO");

                iTextSharp.text.Image firma = iTextSharp.text.Image.GetInstance("C:/reportes/img/" + uaValidador + ".png");
                firma.ScalePercent(10);
                firma.SetAbsolutePosition(0, 0);
                celda = new PdfPCell(firma);
                celda.Border = 0;
                celda.BackgroundColor = new BaseColor(255, 255, 255);
                tblAreas.AddCell(celda);
                doc.Add(tblAreas);

                //AGREGAR PIE DE ARCHIVO
                PdfPTable tbfooterA = new PdfPTable(3);
                //tbfooter.TotalWidth = document.PageSize.Width - document.LeftMargin - document.RightMargin;
                tbfooterA.TotalWidth = 550f;
                tbfooterA.LockedWidth = true;
                tbfooterA.DefaultCell.Border = 0;
                var _cellCodigoF = new PdfPCell(new Paragraph(new Chunk(NumOrden, FontFactory.GetFont("Arial", 6, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLUE))));
                _cellCodigoF.Border = Rectangle.TOP_BORDER;
                tbfooterA.AddCell(_cellCodigoF);

                var _cellA = new PdfPCell(new Paragraph(new Chunk(NombrePaciente, FontFactory.GetFont("Arial", 6, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLUE))));
                _cellA.HorizontalAlignment = Element.ALIGN_CENTER;
                _cellA.Border = Rectangle.TOP_BORDER;
                _cellA.BorderColorTop = BaseColor.BLUE;
                tbfooterA.AddCell(_cellA);

                var _cellB = new PdfPCell(new Paragraph(writer.PageNumber.ToString(), FontFactory.GetFont("Arial", 6, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLUE)));//For page no.
                _cellB.HorizontalAlignment = Element.ALIGN_RIGHT;
                _cellB.Border = Rectangle.TOP_BORDER;
                tbfooterA.AddCell(_cellB);

                float[] widthsA = new float[] { 20f, 60f, 20f };
                tbfooterA.SetWidths(widthsA);
                tbfooterA.WriteSelectedRows(0, -1, 25f, writer.PageSize.GetBottom(doc.BottomMargin), writer.DirectContent);


                doc.Close();
                writer.Close();
                return;
            }






        }


    }
}
