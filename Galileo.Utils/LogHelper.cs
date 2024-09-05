using System;
using System.IO;

namespace Galileo.Utils
{
    public class LogHelper
    {
        public string Path { get; set; }
        public string Salto { get; set; }
        public LogHelper(string path)
        {
            this.Path = path;
            Salto = "\n";
        }

        public void Add(string sLog, bool includeDate = true)
        {
            CreateDirectory();
            string nombre = GetFileName();
            string cadena = "";

            if (includeDate)
            {
                cadena += DateTime.Now + "-" + sLog + Environment.NewLine + Environment.NewLine;
            }
            else
            {
                cadena +=  sLog + Environment.NewLine + Environment.NewLine;
            }
                
            StreamWriter sw = new StreamWriter(Path + "/" + nombre, true);
            sw.Write(cadena);
            sw.Close();

        }

        public void Add(string sLog, string path, bool includeDate = true)
        {
            CreateDirectory();
            string nombre = path;
            string cadena = "";

            if (includeDate)
            {
                cadena += DateTime.Now + "-" + sLog + Environment.NewLine + Environment.NewLine;
            }
            else
            {
                cadena += sLog + Environment.NewLine + Environment.NewLine;
            }

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
