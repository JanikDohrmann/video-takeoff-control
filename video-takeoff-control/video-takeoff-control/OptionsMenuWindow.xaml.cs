using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows;

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

            textControlLinePosition.Text = Settings.controlLineX.ToString();
            textControlLineWidth.Text = Settings.controlLineWidth.ToString();
            checkShowControlLine.IsChecked = Settings.showControlLine;
            
            List<String> controlLineColorOptions = new List<String>();
            controlLineColorOptions.Add(Settings.controlLineColor.Name);
            comboControlLineColor.ItemsSource = controlLineColorOptions;
            comboControlLineColor.SelectedIndex = 0;

            textVideoStoragePath.Text = Settings.storageFolderPath;
            textFrameRate.Text = Settings.framerate.ToString();
        }

        private void ChangeControlLine_Click(object sender, RoutedEventArgs e)
        {
            string controlLinePosition = textControlLinePosition.Text;

            if (Regex.IsMatch(controlLinePosition, "\\d+"))
            {
                Settings.controlLineX = Int32.Parse(controlLinePosition);
            }

            string controlLineWidth = textControlLineWidth.Text;

            if (Regex.IsMatch(controlLineWidth, "\\d+"))
            {
                Settings.controlLineWidth = Int32.Parse(controlLineWidth);
            }

            Settings.showControlLine = checkShowControlLine.IsChecked.GetValueOrDefault(true);
        }

        private void SaveVideoStorage_Click(object sender, RoutedEventArgs e)
        {
            Settings.storageFolderPath = textVideoStoragePath.Text;

            string framerate = textFrameRate.Text;

            if (Regex.IsMatch(framerate, "\\d+"))
            {
                Settings.framerate = Int32.Parse(framerate);
            }
        }
    }
}
