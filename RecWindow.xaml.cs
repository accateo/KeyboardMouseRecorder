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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace KeyboardMouseRecorder
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class RecWindow : Window
    {
        public static RecWindow window_istance;

        public RecWindow()
        {
            InitializeComponent();

            Left = System.Windows.SystemParameters.WorkArea.Width - Width;
            Top = System.Windows.SystemParameters.WorkArea.Height - Height;
            this.AllowsTransparency = true;

            window_istance = this;

        }

        //nascondo finestra a fine registrazione
        public void hideWindow()
        {
            this.Close();
        }


    }
    
    

}
