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
                this.ServerOn = true;
                mainWindow.LogBox.Text += "Serwer aktywny\n";
                myargs = new MyTcpListenerEventArgs(this.ServerOn, this.ClientConnected);
                NewState(myargs);
                while (true)   //we wait for a connection
                {
                    client = server.AcceptTcpClient();  //if a connection exists, the server will accept it ( tylko 1 )
                    this.ClientConnected = true;
                    mainWindow.LogBox.Text += "Klient połączony\n";
                    myargs = new MyTcpListenerEventArgs(this.ServerOn, this.ClientConnected);
                    NewState(myargs);
                    NetworkStream ns = client.GetStream();
                    mainWindow.fs.CopyTo(ns); // like ns.write simplier
                    ns.Close();
                    mainWindow.LogBox.Text += "Wysyłanie danych zakończone\n";
                    break;
                }
            }
            catch (SocketException e)
            {
                mainWindow.LogBox.Text+=string.Format("SocketException: {0}", e);
                StopServer();
            }
            catch (Exception e)
            {
                mainWindow.LogBox.Text += string.Format("Exception: {0}", e);
                StopServer();
            }
            finally
            {
                mainWindow.LogBox.Text += "Serwer zakończył nadawanie\n";
                StopServer();
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
            client.Close();
            this.ClientConnected = false;
            server.Stop();
            this.ServerOn = false;
            myargs = new MyTcpListenerEventArgs(this.ServerOn, this.ClientConnected);
            NewState(myargs);
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

