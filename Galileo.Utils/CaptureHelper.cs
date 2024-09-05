using System;
using System.IO;

namespace Galileo.Utils
{
    public class CaptureHelper
    {
        public string Path { get; set; }
        public string Salto { get; set; }
        public CaptureHelper(string path)
        {
            this.Path = path;
            Salto = "\n";
        }

        public void Add(string sLog)
        {
            CreateDirectory();
            string nombre = GetFileName();
            string cadena = "";
            cadena += sLog;
            StreamWriter sw = new StreamWriter(Path + "/" + nombre, true);
            sw.Write(cadena);
            sw.Close();

        }

        #region Helper

        private string GetFileName()
        {
            string nombre = "";
            nombre = "log_" + DateTime.Now.Year + "_" + DateTime.Now.Month.ToString().PadLeft(2, Convert.ToChar("0")) + "_" + DateTime.Now.Day.ToString().PadLeft(2, Convert.ToChar("0")) + ".txt";
            return nombre;
        }

        private bool CreateDirectory()
        {
            try
            {
                if (!Directory.Exists(Path))
                    Directory.CreateDirectory(Path);
                return true;
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
                return false;
            }
        }


        #endregion
    }
}
