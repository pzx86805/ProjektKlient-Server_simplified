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
        /// <param name="port">Podaj port z którego ma korzystać serwer</param>
        public void CreateAndStartListeningServer(Int32 port)
        {
            try
            {
                IPAddress localAddr = IPAddress.Parse("127.0.0.1");
                server = new TcpListener(localAddr, port);

                // Start listening for client requests.
                server.Start();
                this.ServerOn = true;
                myargs = new MyTcpListenerEventArgs(this.ServerOn, this.ClientConnected);
                NewState(myargs);
                // Enter the listening loop.
                while (true)
                {
                    // Wait for Client
                    client = server.AcceptTcpClient();
                    if (client != null) { ClientConnected = true; }
                    MyTcpListenerEventArgs myargs = new MyTcpListenerEventArgs(this.ServerOn, this.ClientConnected);
                    NewState(myargs);
                    // Buffer for reading data
                    Byte[] bytes = new Byte[256];
                    String data = null;
                    data = null;

                    // Get a stream object for reading and writing
                    NetworkStream stream = client.GetStream();

                    int i;

                    // Loop to receive all the data sent by the client.
                    while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                    {
                        // Translate data bytes to a ASCII string.
                        // Received
                        data = System.Text.Encoding.ASCII.GetString(bytes, 0, i);
                        Console.WriteLine("Received: {0}", data);

                        // Process the data sent by the client.
                        // Processing
                        data = data.ToUpper();

                        byte[] msg = System.Text.Encoding.ASCII.GetBytes(data);

                        // Send back a response.
                        // Response
                        stream.Write(msg, 0, msg.Length);
                        Console.WriteLine("Sent: {0}", data);
                    }
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
                ServerOn = false;
                ClientConnected = false;
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

