using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace ProjektKlient_Server
{
    class MyTcpListener
    {
        TcpListener server = null;
        TcpClient client = null;
        MyTcpListenerEventArgs myargs = null;
        public event EventHandler StateInfo = null;
        public bool ServerOn = false;
        public bool ClientConnected = false;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="port">Port na którym serwer będzie nasłuchiwać</param>
        /// <param name="filePath">Scieżka do pliku</param>
        /// <param name="fileContent">Zawartość pliku</param>
        public void CreateAndStartListeningServer(Int32 port, MainWindow mainWindow)
        {
            try
            {
                IPAddress localAddr = IPAddress.Parse("127.0.0.1");

                //Nowy TcpListener z adresem lokalnym i portem z kontrolki PortComboBox
                server = new TcpListener(localAddr, port);
                if(mainWindow.fs == null) { mainWindow.WybierzPlik_Click(this, null); }
                server.Start();  // this will start the server
                mainWindow.LogBox.Text += "Serwer aktywny\n";
                mainWindow.ServerStateDisp.Text = "Serwer aktywny";
                while (true)   //we wait for a connection
                {
                    client = server.AcceptTcpClient();  //if a connection exists, the server will accept it ( tylko 1 )
                    mainWindow.LogBox.Text += "Klient połączony\n";
                    mainWindow.ClientStateDisp.Text = "Klient połączony";
                    NetworkStream ns = client.GetStream();
                    mainWindow.fs.CopyTo(ns); // like ns.write simplier
                    ns.Close();
                    mainWindow.LogBox.Text += "Wysyłanie danych zakończone\n";
                    break;
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
                ServerOn = false;
                ClientConnected = false;
                server.Stop();
            }
            catch (Exception e)
            {
                mainWindow.LogBox.Text += e;
                ServerOn = false;
                ClientConnected = false;
                server.Stop();
            }
            finally
            {
                mainWindow.LogBox.Text += "Serwer zakończył nadawanie, zostaje wyłączony";
                mainWindow.PortComboBox.IsEnabled = true;
                mainWindow.ServerStateDisp.Text = "Serwer wyłączony";
                mainWindow.ClientStateDisp.Text = "Brak klienta";
                ServerOn = false;
                ClientConnected = false;
                client.Close();
                server.Stop();
            }
        }
        /// <summary>
        /// Delegat aktualizacji stanu serwera i połączenia
        /// </summary>
        /// <param name="e"></param>
        public void NewState(MyTcpListenerEventArgs e)
        {
            EventHandler newStateInfo = StateInfo;
            if (newStateInfo != null)
            {

                newStateInfo(this, e);
            }
        }
        
        //Zatrzymaj serwer
        public void StopServer()
        {
            server.Stop();
        }
    }
    class MyTcpListenerEventArgs : EventArgs
    {
        public bool ServerState { get; set; }
        public bool ClientState { get; set; }
        public MyTcpListenerEventArgs(bool ServerState, bool ClientState)
        {
            this.ServerState = ServerState;
            this.ClientState = ClientState;
        }
    }
}

