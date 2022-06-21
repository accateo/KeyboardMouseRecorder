using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace KeyboardMouseRecorder
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static MainWindow window_istance;
        private const string configFilePath = "C:\\CustomFolder\\rec_config.json";
        //stato visualizzazione finestra
        private bool windowShowState = false;

        public MainWindow()
        {
            InitializeComponent();
            if (File.Exists(configFilePath))
            {
                //leggo da file di configurazione
                JObject data = JObject.Parse(File.ReadAllText(configFilePath));
                windowShowState = (Int32.Parse(data["progressWindow"].ToString()) == 0 ? false : true);
                if (windowShowState)
                    progressCheckbox.IsChecked = true;
            }
            if (!File.Exists("C:\\CustomFolder\\"))
            {

                Directory.CreateDirectory("C:\\CustomFolder\\");
            }
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            window_istance = this;
        }

        

        //blocco gli spazi nella textbox
        private void regName_TextChanged(object sender, EventArgs e)
        {
            if (sender is TextBox tb)
            {
                if (!tb.Text.Contains(" "))
                {
                    btn1.IsEnabled = true;
                }
                else
                {
                    btn1.IsEnabled = false;
                    MessageBox.Show("Inserire un nome del file valido");
                }
                //il nome del file deve esistere
                if (tb.Text.Length == 0)
                {
                    btn1.IsEnabled = false;
                }
                
            }
            
        }

        private void OnClick1(object sender, RoutedEventArgs e)
        {
            
            //this.WindowState = WindowState.Minimized;
            this.Visibility = Visibility.Hidden;
            //apro finestrella registrazione
            RecWindow recwindow = new RecWindow();
            if(windowShowState)
                recwindow.Show();
            recwindow.Topmost = true;
            //salvo la descrizione
            string descrizione = BoxDesc.Text;
            //istanzio registrazione
            Registration reg = new Registration(regName.Text,descrizione);
            

            
        }

        //riapro finestra principale
        public void openWindow()
        {
            this.Visibility = Visibility.Visible;
            Activate();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void progressCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            //aggiorno stato
            windowShowState = true;
            progressCheckbox.IsChecked = true;
        }
        private void progressCheckbox_Unchecked(object sender, RoutedEventArgs e)
        {
            //aggiorno stato
            windowShowState = false;
            progressCheckbox.IsChecked = false;
        }

        private void saveIcon_MouseDown(object sender, RoutedEventArgs e)
        {

                //salvo su file json
                JObject json = JObject.Parse("{'progressWindow':" + (windowShowState == true ? 1 : 0) + "}");
                File.WriteAllText(configFilePath, JsonConvert.SerializeObject(json));
                //avviso l'utente
                System.Windows.MessageBox.Show("Configurazione salvata correttamente");
            
        }
    }
    
    

}
