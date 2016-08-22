using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDFHelper
{
    public class PDFHelper
    {
        public void pdfConverterClass(string sourceFile, string destinationFile, string convertParam)
        {
            try
            {
                //POWERSHELL
                //[string]$PDFHelper = "H:\Copy\Projects\MS.NET\PDFConverter\PDFConverter\bin\Debug\PDFHelper.dll"
                //[void][System.Reflection.Assembly]::LoadFrom($PDFHelper)
                PDFConverter.IPDFConverterX obj = null;
                Type type = Type.GetTypeFromProgID("PDFConverter.PDFConverterX");
                obj = (PDFConverter.IPDFConverterX)Activator.CreateInstance(type);
                obj.LogFile = "C:\\Log.txt";
                obj.Convert(sourceFile, destinationFile, string.Format("-c {0}", convertParam));

            }
            catch (Exception ex)
            {

            }

        }
    }
}
