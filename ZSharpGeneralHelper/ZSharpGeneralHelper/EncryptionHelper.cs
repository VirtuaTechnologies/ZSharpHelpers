using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ZSharpGeneralHelper
{
    public class EncryptionHelper
    {
        private static string txt_doc_string;
        private static byte[] _salt = Encoding.ASCII.GetBytes("o6806642kbM7c5");

        public static void EncryptFile(string inputFile, string outputFile)
        {
            try
            {
                txt_doc_string = "";
                using (StreamReader reader = new StreamReader(inputFile))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        //MessageBox.Show(line);
                        txt_doc_string += EncodeTo64(line) + Environment.NewLine;
                    }
                }

                FFManager.Txt_writer(outputFile, txt_doc_string);
            }
            catch (Exception ex)
            {
               Debug.Write(ex.ToString());
            }
        }

        public static void DecryptFile(string inputFile, string outputFile)
        {
            try
            {
                txt_doc_string = "";
                using (StreamReader reader = new StreamReader(inputFile))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        //MessageBox.Show(line);
                        txt_doc_string += DecodeFrom64(line) + Environment.NewLine; ;
                    }
                }

                FFManager.Txt_writer(outputFile, txt_doc_string);
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
            }
        }

        public static string EncodeTo64(string toEncode)
        {
            string returnValue = null;
            try
            {
                byte[] toEncodeAsBytes = System.Text.ASCIIEncoding.ASCII.GetBytes(toEncode);
                returnValue = System.Convert.ToBase64String(toEncodeAsBytes);

            }
            catch (SystemException e)
            {
            }
            return returnValue;
        }

        public static string DecodeFrom64(string encodedData)
        {
            string returnValue = null;
            try
            {
                byte[] encodedDataAsBytes = System.Convert.FromBase64String(encodedData);
                returnValue = System.Text.ASCIIEncoding.ASCII.GetString(encodedDataAsBytes);
            }
            catch (SystemException e)
            {
            }
            return returnValue;
        }

        public static XmlDocument EncryptXmlData(string path)
        {
            XmlDocument doc;
            RijndaelManaged key = null;
            try
            {
                key = new RijndaelManaged();
                doc = new XmlDocument();
                doc.Load(path);
                EncryptXML(doc);
                doc.Save(path);
            }
            catch (Exception ex)
            {
                throw;
            }
            return doc;
        }

        public static XmlDocument DecryptXmlData(string path)
        {
            XmlDocument doc;
            RijndaelManaged key = null;
            try
            {
                key = new RijndaelManaged();
                doc = new XmlDocument();
                doc.Load(path);
                DecryptXML(doc);
                //Convert.ToDateTime(path, CultureInfo.CurrentCulture);
                //doc.Save(path);
            }
            catch (Exception ex)
            {
                throw;
            }
            return doc;
        }

        private static void DecryptXML(XmlDocument doc)
        {
            try
            {
                if (doc == null)
                    throw new ArgumentNullException("Doc");
                SymmetricAlgorithm symAlgo = new RijndaelManaged();
                byte[] salt = Encoding.ASCII.GetBytes("This is my salt");
                Rfc2898DeriveBytes theKey = new Rfc2898DeriveBytes("myclass", salt);
                symAlgo.Key = theKey.GetBytes(symAlgo.KeySize / 8);
                symAlgo.IV = theKey.GetBytes(symAlgo.BlockSize / 8);
                XmlElement encryptedElement = doc.DocumentElement;
                if (encryptedElement == null)
                {
                    throw new XmlException("The EncryptedData element was not found.");
                }
                EncryptedData edElement = new EncryptedData();
                edElement.LoadXml(encryptedElement);
                EncryptedXml exml = new EncryptedXml();
                byte[] rgbOutput = exml.DecryptData(edElement, symAlgo);
                exml.ReplaceData(encryptedElement, rgbOutput);
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public static void EncryptXML(XmlDocument doc)
        {
            SymmetricAlgorithm symAlgo = new RijndaelManaged();
            byte[] salt = Encoding.ASCII.GetBytes("This is my salt");
            Rfc2898DeriveBytes theKey = new Rfc2898DeriveBytes("myclass", salt);
            symAlgo.Key = theKey.GetBytes(symAlgo.KeySize / 8);
            symAlgo.IV = theKey.GetBytes(symAlgo.BlockSize / 8);
            if (doc == null)
                throw new ArgumentNullException("Doc");
            if (symAlgo == null)
                throw new ArgumentNullException("Alg");
            XmlElement elementToEncrypt = doc.DocumentElement;
            if (elementToEncrypt == null)
            {
                throw new XmlException("The specified element was not found");

            }
            EncryptedXml eXml = new EncryptedXml();
            byte[] encryptedElement = eXml.EncryptData(elementToEncrypt, symAlgo, false);
            EncryptedData edElement = new EncryptedData();
            edElement.Type = EncryptedXml.XmlEncElementUrl;
            string encryptionMethod = null;
            if (symAlgo is TripleDES)
            {
                encryptionMethod = EncryptedXml.XmlEncTripleDESUrl;
            }
            else if (symAlgo is DES)
            {
                encryptionMethod = EncryptedXml.XmlEncDESUrl;
            }
            if (symAlgo is Rijndael)
            {
                switch (symAlgo.KeySize)
                {
                    case 128:
                        encryptionMethod = EncryptedXml.XmlEncAES128Url;
                        break;
                    case 192:
                        encryptionMethod = EncryptedXml.XmlEncAES192Url;
                        break;
                    case 256:
                        encryptionMethod = EncryptedXml.XmlEncAES256Url;
                        break;
                    default:
                        // do the defalut action
                        break;
                }
            }
            else
            {
                throw new CryptographicException("The specified algorithm is not supported for XML Encryption.");
            }
            edElement.EncryptionMethod = new EncryptionMethod(encryptionMethod);
            edElement.CipherData.CipherValue = encryptedElement;
            EncryptedXml.ReplaceElement(elementToEncrypt, edElement, false);
        }

        public static string EncryptStringAES(string plainText, string sharedSecret, string salt)
        {
            if(salt != null)
                _salt = Encoding.ASCII.GetBytes(salt);
            if (string.IsNullOrEmpty(plainText))
                throw new ArgumentNullException("plainText");
            if (string.IsNullOrEmpty(sharedSecret))
                throw new ArgumentNullException("sharedSecret");

            string outStr = null;                       // Encrypted string to return
            RijndaelManaged aesAlg = null;              // RijndaelManaged object used to encrypt the data.

            try
            {
                // generate the key from the shared secret and the salt
                Rfc2898DeriveBytes key = new Rfc2898DeriveBytes(sharedSecret, _salt);

                // Create a RijndaelManaged object
                aesAlg = new RijndaelManaged();
                aesAlg.Key = key.GetBytes(aesAlg.KeySize / 8);

                // Create a decryptor to perform the stream transform.
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for encryption.
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    // prepend the IV
                    msEncrypt.Write(BitConverter.GetBytes(aesAlg.IV.Length), 0, sizeof(int));
                    msEncrypt.Write(aesAlg.IV, 0, aesAlg.IV.Length);
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            //Write all data to the stream.
                            swEncrypt.Write(plainText);
                        }
                    }
                    outStr = Convert.ToBase64String(msEncrypt.ToArray());
                }
            }
            finally
            {
                // Clear the RijndaelManaged object.
                if (aesAlg != null)
                    aesAlg.Clear();
            }

            // Return the encrypted bytes from the memory stream.
            return outStr;
        }

        /// <summary>
        /// Decrypt the given string.  Assumes the string was encrypted using 
        /// EncryptStringAES(), using an identical sharedSecret.
        /// </summary>
        /// <param name="cipherText">The text to decrypt.</param>
        /// <param name="sharedSecret">A password used to generate a key for decryption.</param>
        public static string DecryptStringAES(string cipherText, string sharedSecret, string salt)
        {
            if (salt != null)
                _salt = Encoding.ASCII.GetBytes(salt);
            if (string.IsNullOrEmpty(cipherText))
                throw new ArgumentNullException("cipherText");
            if (string.IsNullOrEmpty(sharedSecret))
                throw new ArgumentNullException("sharedSecret");

            // Declare the RijndaelManaged object
            // used to decrypt the data.
            RijndaelManaged aesAlg = null;

            // Declare the string used to hold
            // the decrypted text.
            string plaintext = null;

            try
            {
                // generate the key from the shared secret and the salt
                Rfc2898DeriveBytes key = new Rfc2898DeriveBytes(sharedSecret, _salt);

                // Create the streams used for decryption.                
                byte[] bytes = Convert.FromBase64String(cipherText);
                using (MemoryStream msDecrypt = new MemoryStream(bytes))
                {
                    // Create a RijndaelManaged object
                    // with the specified key and IV.
                    aesAlg = new RijndaelManaged();
                    aesAlg.Key = key.GetBytes(aesAlg.KeySize / 8);
                    // Get the initialization vector from the encrypted stream
                    aesAlg.IV = ReadByteArray(msDecrypt);
                    // Create a decrytor to perform the stream transform.
                    ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))

                            // Read the decrypted bytes from the decrypting stream
                            // and place them in a string.
                            plaintext = srDecrypt.ReadToEnd();
                    }
                }
            }
            finally
            {
                // Clear the RijndaelManaged object.
                if (aesAlg != null)
                    aesAlg.Clear();
            }

            return plaintext;
        }

        private static byte[] ReadByteArray(Stream s)
        {
            byte[] rawLength = new byte[sizeof(int)];
            if (s.Read(rawLength, 0, rawLength.Length) != rawLength.Length)
            {
                throw new SystemException("Stream did not contain properly formatted byte array");
            }

            byte[] buffer = new byte[BitConverter.ToInt32(rawLength, 0)];
            if (s.Read(buffer, 0, buffer.Length) != buffer.Length)
            {
                throw new SystemException("Did not read byte array properly");
            }

            return buffer;
        }
    }
}
