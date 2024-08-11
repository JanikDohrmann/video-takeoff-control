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

namespace video_takeoff_control
{
    /// <summary>
    /// Interaktionslogik für CompetitionNameModal.xaml
    /// </summary>
    public partial class CompetitionNameModal : Window
    {
        public MainWindow mainWindow { get; set; }

        public CompetitionNameModal()
        {
            InitializeComponent();
        }

        private void saveCompetitionName_Click(object sender, RoutedEventArgs e)
        {
            Settings.competitionName = textCompetitionName.Text;
            mainWindow.updateCompetitionName();
            this.Close();
        }
    }
}
