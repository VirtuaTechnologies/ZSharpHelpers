using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ZSharpGeneralHelper
{
    public static class MailUtility
    {
        //Extension method for MailMessage to save to a file on disk
        public static void Save(this MailMessage message, string filename, bool addUnsentHeader = true)
        {
            try
            {
                using (var filestream = File.Open(filename, FileMode.Create))
                {
                    GenUtilities.LibNotes();
                    if (addUnsentHeader)
                    {
                        var binaryWriter = new BinaryWriter(filestream);
                        //Write the Unsent header to the file so the mail client knows this mail must be presented in "New message" mode
                        binaryWriter.Write(System.Text.Encoding.UTF8.GetBytes("X-Unsent: 1" + Environment.NewLine));
                    }

                    var assembly = typeof(SmtpClient).Assembly;
                    var mailWriterType = assembly.GetType("System.Net.Mail.MailWriter");

                    // Get reflection info for MailWriter contructor
                    var mailWriterContructor = mailWriterType.GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, null, new[] { typeof(Stream) }, null);

                    // Construct MailWriter object with our FileStream
                    var mailWriter = mailWriterContructor.Invoke(new object[] { filestream });

                    // Get reflection info for Send() method on MailMessage
                    var sendMethod = typeof(MailMessage).GetMethod("Send", BindingFlags.Instance | BindingFlags.NonPublic);

                    sendMethod.Invoke(message, BindingFlags.Instance | BindingFlags.NonPublic, null, new object[] { mailWriter, true, true }, null);

                    // Finally get reflection info for Close() method on our MailWriter
                    var closeMethod = mailWriter.GetType().GetMethod("Close", BindingFlags.Instance | BindingFlags.NonPublic);

                    // Call close method
                    closeMethod.Invoke(mailWriter, BindingFlags.Instance | BindingFlags.NonPublic, null, new object[] { }, null);
                }
            }
            catch (SystemException ex)
            {
                //MessageBox.Show(ex.ToString());
            }
        }

        public static bool emailIsValid(string email)
        {
            string expresion;
            expresion = "\\w+([-+.']\\w+)*@\\w+([-.]\\w+)*\\.\\w+([-.]\\w+)*";
            if (Regex.IsMatch(email, expresion))
            {
                if (Regex.Replace(email, expresion, string.Empty).Length == 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }


    }
}
