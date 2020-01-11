using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
using System.IO;
using Microsoft.Win32;


namespace ProjektKlient_Server
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        MyTcpListener server = null;
        public Stream fs = null;
        public string fileContent = string.Empty;
        public string filePath = string.Empty;
        public MainWindow()
        {
            server = new MyTcpListener();
            //server.StateInfo += Server_StateInfo;
            InitializeComponent();

        }

        //private void Server_StateInfo(object sender, EventArgs e)
        //{
        //    if ( e is MyTcpListenerEventArgs)
        //    {
        //        MyTcpListenerEventArgs myArgs = e as MyTcpListenerEventArgs;
        //        if (myArgs.ServerState == true)
        //        {
        //            this.ServerStateDisp.Text = "Serwer włączony";
        //            this.ServerStateDisp.Foreground = Brushes.Green;
        //        }
        //        else
        //        {
        //            this.ServerStateDisp.Text = "Serwer wyłączony";
        //            this.ServerStateDisp.Foreground = Brushes.Red;
        //        }
        //        if (myArgs.ClientState == true)
        //        {
        //            this.ClientStateDisp.Text = "Klient podłączony";
        //            this.ClientStateDisp.Foreground = Brushes.Green;
        //        }
        //        else 
        //        { 
        //            this.ClientStateDisp.Text = "Klient niepodłączony";
        //            this.ClientStateDisp.Foreground = Brushes.Red;
        //        }
        //    }
            
        //}

        public void WybierzPlik_Click(object sender, RoutedEventArgs e)
        {
            fileContent = string.Empty;
            filePath = string.Empty;
            OpenFileDialog openFileDialog = new OpenFileDialog();
            try
            {
                openFileDialog.InitialDirectory = "c:\\";
                openFileDialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
                openFileDialog.FilterIndex = 2;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == true)
                {
                    //Get the path of specified file
                    filePath = openFileDialog.FileName;
                    FilePathDisp.Text = filePath.ToString();
                    FileSizeDisp.Text = new FileInfo(filePath).Length.ToString() + " bytes";
                    //Read the contents of the file into a stream
                    fs = openFileDialog.OpenFile();
                }
            }
            catch (Exception ex)
            {
                LogBox.Text += ex.ToString();
                throw;
            }
        }

        private void ToggleStartStopServer_Click(object sender, RoutedEventArgs e)
        {
            if (server.ServerOn == false)
            {
                //Start serwera na porcie wskazanym w comboboxie
                server.CreateAndStartListeningServer(Int32.Parse(this.PortComboBox.Text.ToString()), this);
                server.ServerOn = true;
                this.PortComboBox.IsEnabled = false;
                this.StartStopButton.Content = "Stop Server";
            }
            else
            {
                //Zatrzymanie serwera
                server.StopServer();
                server.ServerOn = false;
                this.StartStopButton.Content = "Start Server";
                this.PortComboBox.IsEnabled = true;
            }
        }
    }
}
