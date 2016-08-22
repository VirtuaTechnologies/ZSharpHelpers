using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Reflection;
using System.IO;
using System.Windows.Media;
using System.Windows.Data;
using MColor = System.Windows.Media.Color;
using DColor = System.Drawing.Color;
using Microsoft.Win32;
using System.Security.Principal;

namespace ZSharpGeneralHelper
{
    class GenUtilities
    {
        public class AutoClosingMessageBox
        {
            System.Threading.Timer _timeoutTimer;
            string _caption;
            AutoClosingMessageBox(string text, string caption, int timeout)
            {
                _caption = caption;
                _timeoutTimer = new System.Threading.Timer(OnTimerElapsed,
                    null, timeout, System.Threading.Timeout.Infinite);
                System.Windows.MessageBox.Show(text, caption);
            }
            public static void Show(string text, string caption, int timeout)
            {
                new AutoClosingMessageBox(text, caption, timeout);
            }
            void OnTimerElapsed(object state)
            {
                IntPtr mbWnd = FindWindow(null, _caption);
                if (mbWnd != IntPtr.Zero)
                    SendMessage(mbWnd, WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
                _timeoutTimer.Dispose();
            }
            const int WM_CLOSE = 0x0010;
            [System.Runtime.InteropServices.DllImport("user32.dll", SetLastError = true)]
            static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
            [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto)]
            static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam, IntPtr lParam);
        }

        static public string AssemblyDirectory
        {
            get
            {
                string codeBase = Assembly.GetExecutingAssembly().CodeBase;
                UriBuilder uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path);
            }
        }

        public static void DevNotes(string Filepath, string message)
        {
            try
            {
                //string message = "Developer: Raghulan Gowthaman \n www.raghulangowthaman.com";
                if (System.IO.File.Exists(Filepath))
                    System.IO.File.Delete(Filepath);
                System.IO.File.Create(Filepath).Close();

                using (System.IO.StreamWriter StreamWriter1 = new System.IO.StreamWriter(Filepath, true))
                {
                    StreamWriter1.WriteLine(message);
                }
            }
            catch (System.Exception ex)
            {
                //MessageBox.Show(ex.ToString());
            }
        }

        public static void LibNotes() 
        {
            try
            {
                string libFile = AssemblyDirectory + @"\" + Assembly.GetExecutingAssembly().GetName().Name + ".txt";
                System.Windows.Forms.MessageBox.Show(libFile);
                string message = "This Library was developed as an accessory for .NET C# Developers. Please keep this document for copyright purposes.";
                if (System.IO.File.Exists(libFile))
                    System.IO.File.Delete(libFile);
                System.IO.File.Create(libFile).Close();

                using (System.IO.StreamWriter StreamWriter1 = new System.IO.StreamWriter(libFile, true))
                {
                    StreamWriter1.WriteLine("\n:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::");
                    StreamWriter1.WriteLine("\n" + Assembly.GetExecutingAssembly().FullName);
                    StreamWriter1.WriteLine("\n:Developer::Raghulan Gowthaman:::::::::::::::::::::::::::::::::::::::::::::::::::::::");
                    StreamWriter1.WriteLine("\n:www.raghulangowthaman.com:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::");
                    StreamWriter1.WriteLine("\n:Zcodia Technologies:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::");
                    StreamWriter1.WriteLine("\n:Zcodia Technologies:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::");
                    StreamWriter1.WriteLine("\n:www.zcodia.com::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::");
                    StreamWriter1.WriteLine("\n:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::");
                    StreamWriter1.WriteLine("\n" + message);
                    StreamWriter1.WriteLine("\n:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::");
                }
            }
            catch (System.Exception ex)
            {
                //MessageBox.Show(ex.ToString());
            }
        }
        
    }

    public class ColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            System.Drawing.Color color = (System.Drawing.Color)value;
            var converter = new System.Windows.Media.BrushConverter();
            var myBrush = converter.ConvertFromString(color.Name) as System.Drawing.Brush;

            return myBrush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public MColor ToMediaColor(DColor color)
        {
            return MColor.FromArgb(color.A, color.R, color.G, color.B);
        }

        public static String HexConverter(System.Drawing.Color c)
        {
            return "#" + c.R.ToString("X2") + c.G.ToString("X2") + c.B.ToString("X2");
        }

        public static String RGBConverter(System.Drawing.Color c)
        {
            return "RGB(" + c.R.ToString() + "," + c.G.ToString() + "," + c.B.ToString() + ")";
        }
    }

    public class StartUpManager
    {
        public static void AddApplicationToCurrentUserStartup(string AppTitle, string pathValue)
        {
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true))
            {
                key.SetValue(AppTitle, pathValue);
            }
        }

        public static void AddApplicationToAllUserStartup(string AppTitle, string pathValue)
        {
            using (RegistryKey key = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true))
            {
                key.SetValue(AppTitle, pathValue);
            }
        }

        public static void RemoveApplicationFromCurrentUserStartup(string AppTitle)
        {
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true))
            {
                key.DeleteValue(AppTitle, false);
            }
        }

        public static void RemoveApplicationFromAllUserStartup(string AppTitle)
        {
            using (RegistryKey key = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true))
            {
                key.DeleteValue(AppTitle, false);
            }
        }

        public static bool IsUserAdministrator()
        {
            //bool value to hold our return value
            bool isAdmin;
            try
            {
                //get the currently logged in user
                WindowsIdentity user = WindowsIdentity.GetCurrent();
                WindowsPrincipal principal = new WindowsPrincipal(user);
                isAdmin = principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
            catch (UnauthorizedAccessException ex)
            {
                isAdmin = false;
            }
            catch (Exception ex)
            {
                isAdmin = false;
            }
            return isAdmin;
        }
    }
}
