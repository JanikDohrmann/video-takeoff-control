using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using video_takeoff_control.settings;

namespace video_takeoff_control
{
    /// <summary>
    /// Interaktionslogik für CompetitionNameModal.xaml
    /// </summary>
    public partial class CompetitionNameModal : Window
    {
        private MainWindow _mainWindow;
        private Settings settings;

        public CompetitionNameModal(MainWindow mainWindow, Settings settings)
        {
            _mainWindow = mainWindow;
            this.settings = settings;
            InitializeComponent();
        }

        private void saveCompetitionName_Click(object sender, RoutedEventArgs e)
        {
            settings.competitionName = textCompetitionName.Text;
            _mainWindow.updateCompetitionName();
            this.Close();
        }
    }
}
