using Notifications.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using GV = ZSharpUIHelper.Global.variable;

namespace ZSharpUIHelper
{
    public class UIHelper
    {
        public static bool result;

        public static Visibility getVisilityOpp(bool visilitystatus)
        {
            if (visilitystatus)
            {
                return System.Windows.Visibility.Collapsed;
            }
            else
            {
                return System.Windows.Visibility.Visible;
            }
        }

        public static bool toogleUIVisisbility(object control)
        {
            //get the type of UI
            switch (control.GetType().ToString())
            {
                case "System.Windows.Controls.StackPanel":
                    {
                        StackPanel sPanel = (StackPanel)control;
                        sPanel.Visibility = getVisilityOpp(sPanel.IsVisible);
                        result = sPanel.IsVisible;
                    }
                    break;
                case "System.Windows.Controls.Button":
                    {
                        Button btn = (Button)control;
                        btn.Visibility = getVisilityOpp(btn.IsVisible);
                        result = btn.IsVisible;
                    }
                    break;
                case "System.Windows.Controls.Grid":
                    {
                        Grid grid = (Grid)control;
                        grid.Visibility = getVisilityOpp(grid.IsVisible);
                        result = grid.IsVisible;
                    }
                    break;
            }

            return result;
        }

        public static Uri getResURI(string resource)
        {
            string Add_Icon = resource;
            Uri imgURI = new Uri(Add_Icon, UriKind.Relative);
            return imgURI;
        }

        public static Image setImage(string res)
        {
            Image Content = new Image
            {
                Source = new BitmapImage(getResURI(res)),
                VerticalAlignment = VerticalAlignment.Center
            };
            return Content;
        }

        public static Brush convertHEXtoBrush(string hexCode)
        {
            var converter = new System.Windows.Media.BrushConverter();
            var brush = (Brush)converter.ConvertFromString(hexCode);
            return brush;
        }

        public static void toastIT(string appName, string message, string title, string notificationType)
        {
            NotificationType NType;
            if (notificationType == "Success")
            {
                NType = NotificationType.Success;
            }
            else if (notificationType == "Error")
            {
                NType = NotificationType.Error;
            }
            else if (notificationType == "Warning")
            {
                NType = NotificationType.Warning;
            }
            else
            { NType = NotificationType.Information; }
                

            var notificationManager = new NotificationManager();

            notificationManager.Show(new NotificationContent
            {
                Title = appName + " | " + title,
                Message = message,
                Type = NType
            });
        }

        public static void DoEvents()
        {
            DispatcherFrame frame = new DispatcherFrame();
            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background,
                new DispatcherOperationCallback(ExitFrame), frame);
            Dispatcher.PushFrame(frame);
        }

        public static object ExitFrame(object f)
        {
            ((DispatcherFrame)f).Continue = false;

            return null;
        }
    }
}
