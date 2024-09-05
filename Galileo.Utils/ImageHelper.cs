using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Galileo.Utils
{
    public class ImageHelper
    {
        private object resizeImage;


        public Bitmap DrawImage(string imagen, string tipo)
        {
            string text = "";
            int pos = 0, pixeles = 0;
            string[] datos = new string[6000];
            for (int a = 0; a < imagen.Length - 1; a += 2)
            {
                text = imagen[a].ToString() + imagen[a + 1].ToString();
                int value = Convert.ToInt32(text, 16);
                datos[pos] = value.ToString();
                pos++;
            }

            Bitmap Image = new Bitmap(256, 256);
            for (int i = 0; i < pos; i++)
                if (Int32.Parse(datos[i]) > 0)
                {
                    pixeles = 255 - Int32.Parse(datos[i]);
                    for (int j = 255; j > pixeles; j--)
                    {
                        Image.SetPixel(i, j, Color.Black);
                        Image.SetPixel(i + 1, j, Color.Black);
                    }
                }
            return Image;
        }


        public Image GetImage(string base64ImageString)
        {
            byte[] data = Convert.FromBase64String(base64ImageString);
            using (var stream = new MemoryStream(data, 0, data.Length))
            {
                System.Drawing.Image image = System.Drawing.Image.FromStream(stream);
                return image;
            }
        }

        public void SaveImage(string base64ImageString, string path, string fileName)
        {

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
           

            string base64BinaryStr = base64ImageString;
            byte[] sPDFDecoded = Convert.FromBase64String(base64BinaryStr);

            

            File.WriteAllBytes(path + "\\" + fileName, sPDFDecoded);
        }



    }
}
