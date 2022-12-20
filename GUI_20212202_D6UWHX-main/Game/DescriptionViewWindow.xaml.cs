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
    /// Interaction logic for DescreptionViewWindow.xaml
    /// </summary>
    public partial class DescriptionViewWindow : Window
    {
        public DescriptionViewWindow()
        {
            InitializeComponent();
        }
        private void Return_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            StartMenuVm startMenuVm = new StartMenuVm();
            startMenuVm.ShowDialog();
            this.Close();
        }
    }
}
