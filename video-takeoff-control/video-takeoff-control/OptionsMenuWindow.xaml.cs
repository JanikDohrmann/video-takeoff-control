using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace video_takeoff_control
{
    /// <summary>
    /// Interaktionslogik für OptionsMenuWindow.xaml
    /// </summary>
    public partial class OptionsMenuWindow : Window
    {
        public OptionsMenuWindow()
        {
            InitializeComponent();
        }

        private void ChangeControlLine_Click(object sender, RoutedEventArgs e)
        {
            string controlLinePosition = textControlLinePosition.Text;

            if (Regex.IsMatch(controlLinePosition, "\\d+"))
            {
                Settings.controlLineX = Int32.Parse(textControlLinePosition.Text);
            }
        }

        private void SaveVideoStorage_Click(object sender, RoutedEventArgs e)
        {
            Settings.storageFolderPath = textVideoStorageFolderPath.Text;
        }
    }
}
