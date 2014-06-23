﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Jojatekok.MoneroGUI
{
    static class Helper
    {
        public const string DefaultLanguageCode = "default";

        public static readonly CultureInfo InvariantCulture = CultureInfo.InvariantCulture;
        public static readonly string NewLineString = Environment.NewLine;

        public static string UppercaseFirst(this string input)
        {
            return char.ToUpper(input[0], InvariantCulture) + input.Substring(1);
        }

        public static string ReWrap(this string input)
        {
            return Regex.Replace(input.TrimEnd(), " (\r\n|\n)", " ");
        }

        public static string ToStringReadable(this TimeSpan timeSpan)
        {
            var days = timeSpan.Days;
            if (days > 0) {
                if (days == 1) return "1 " + Properties.Resources.StatusBarSyncTextDaySingular;
                return days + " " + Properties.Resources.StatusBarSyncTextDayPlural;
            }

            var hours = timeSpan.Hours;
            if (hours > 0) {
                if (hours == 1) return "1 " + Properties.Resources.StatusBarSyncTextHourSingular;
                return hours + " " + Properties.Resources.StatusBarSyncTextHourPlural;
            }

            var minutes = timeSpan.Minutes;
            if (minutes == 1) return "1 " + Properties.Resources.StatusBarSyncTextMinuteSingular;
            return minutes + " " + Properties.Resources.StatusBarSyncTextMinutePlural;
        }

        public static string GetRelativePath(string path)
        {
            var uriBase = new Uri(StaticObjects.ApplicationDirectory, UriKind.Absolute);
            var uriPath = new Uri(path);

            var decodedUrl = HttpUtility.UrlDecode(uriBase.MakeRelativeUri(uriPath).ToString());
            return decodedUrl != null ? decodedUrl.Replace('/', '\\') : string.Empty;
        }

        public static ImageSource ToImageSource(this Icon icon)
        {
            return Imaging.CreateBitmapSourceFromHIcon(icon.Handle, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
        }

        public static BitmapImage ToImageSource(this Image image, bool isTransparent)
        {
            var bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();

            var stream = new MemoryStream();
            image.Save(stream, isTransparent ? ImageFormat.Png : ImageFormat.Bmp);

            bitmapImage.StreamSource = stream;
            bitmapImage.EndInit();
            return bitmapImage;
        }

        public static Bitmap ToBitmap(this BitmapSource bitmapSource)
        {
            using (var stream = new MemoryStream()) {
                var encoder = new BmpBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(bitmapSource));
                encoder.Save(stream);
                return new Bitmap(stream);
            }
        }

        public static T GetAssemblyAttribute<T>() where T : Attribute
        {
            return (T)Attribute.GetCustomAttribute(StaticObjects.ApplicationAssembly, typeof(T), false);
        }

        public static int IndexOf(this IList<SettingsManager.ConfigElementContact> collection, string label)
        {
            for (var i = collection.Count - 1; i >= 0; i--) {
                if (collection[i].Label == label) {
                    return i;
                }
            }

            return -1;
        }
    }

    static class NativeMethods
    {
        private const int GWL_STYLE = -16,
                          WS_MAXIMIZEBOX = 0x10000,
                          WS_MINIMIZEBOX = 0x20000;

        [DllImport("user32.dll")]
        extern private static int GetWindowLong(IntPtr hwnd, int index);

        [DllImport("user32.dll")]
        extern private static int SetWindowLong(IntPtr hwnd, int index, int value);

        public static void SetWindowButtons(this Window window, bool isMinimizable, bool isMaximizable)
        {
            var hwnd = new WindowInteropHelper(window).Handle;
            var style = GetWindowLong(hwnd, GWL_STYLE);

            if (isMaximizable) {
                style |= WS_MAXIMIZEBOX;
            } else {
                style &= ~WS_MAXIMIZEBOX;
            }

            if (isMinimizable) {
                style |= WS_MINIMIZEBOX;
            } else {
                style &= ~WS_MINIMIZEBOX;
            }

            SetWindowLong(hwnd, GWL_STYLE, style);
        }
    }
}
