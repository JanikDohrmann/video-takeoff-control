using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace video_takeoff_control
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static void ChangeLanguage(string cultureCode)
        {
            // Kultur setzen
            CultureInfo culture = new CultureInfo(cultureCode);
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;

            // Neues ResourceDictionary laden
            ResourceDictionary dict = new ResourceDictionary();
            switch (cultureCode)
            {
                case "de-DE":
                    dict.Source = new Uri("resources/locals/Strings.de-DE.xaml", UriKind.Relative);
                    break;
                default:
                    dict.Source = new Uri("resources/locals/Strings.xaml", UriKind.Relative);
                    break;
            }

            // Altes Dictionary ersetzen
            Application.Current.Resources.MergedDictionaries.Clear();
            Application.Current.Resources.MergedDictionaries.Add(dict);
        }
    }
}
