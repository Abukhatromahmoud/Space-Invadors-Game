using GUI_20212202_D6UWHX.Properties;
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

namespace GUI_20212202_D6UWHX
{
    /// <summary>
    /// Interaction logic for StartMenuVm.xaml
    /// </summary>
    public partial class StartMenuVm : Window
    {
        System.Media.SoundPlayer _soundPlayer = new System.Media.SoundPlayer("CreepyAlien.wav");
        System.Media.SoundPlayer _soundPlayer2 = new System.Media.SoundPlayer("Polynomial.wav");
        public StartMenuVm()
        {
            InitializeComponent();
            _soundPlayer.Play();
        }

        private void Start_Click(object sender, RoutedEventArgs e)
        {
            _soundPlayer.Stop();  
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            _soundPlayer2.Play();
            (this).Close();
        }
        private void Description_Click(object sender, RoutedEventArgs e)
        {
            DescriptionViewWindow descriptionViewWindow = new DescriptionViewWindow();
            descriptionViewWindow.Show();
            (this).Close();
        }
        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            (this).Close();
        }
    }
}
