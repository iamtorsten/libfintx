using libfintx;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace asp.net_matrixcode_example
{
    public partial class WebForm1 : System.Web.UI.Page
    {
        public string PhotoTANString = string.Empty;

        protected void Page_Load(object sender, EventArgs e)
        {
            var PhotoCode = File.ReadAllText(Path.Combine(Server.MapPath("~"), "matrixcode.txt"));

            var mCode = new MatrixCode(PhotoCode);

            byte[] imgBytes = ImageToByteArray(mCode.CodeImage);
            string imgString = Convert.ToBase64String(imgBytes);
            PhotoTANString = String.Format("<img src=\"data:image/Png;base64,{0}\">", imgString);
        }

        public static byte[] ImageToByteArray(System.Drawing.Image img)
        {
            MemoryStream ms = new MemoryStream();
            img.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            return ms.ToArray();
        }
    }
}