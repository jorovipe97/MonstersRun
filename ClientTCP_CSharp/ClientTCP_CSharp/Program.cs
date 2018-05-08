using System;
using System.Net.Sockets;
using System.Threading;
using System.Collections.Generic;
using System.Text;


namespace ClientTCP_CSharp
{
    class Program
    {
        // UNE_HFC_5D50
        // ADADC4F8
        public static string host = "10.9.4.155";
        public static int port = 80;

        static NetworkStream stream;
        // private static Object thisLock = new Object();
        private static object thisObj = new object();
        
        static void ReadServer()
        {
            while (true)
            {
                if (stream.CanRead)
                {
                    // Buffer to store the response bytes.
                    Byte[] myReadBuffer = new Byte[1024];
                    StringBuilder myCompleteMessage = new StringBuilder();
                    int numberOfBytesRead = 0;

                    // Incoming message may be larger than the buffer size.
                    do
                    {
                        //Byte[] dummy = new byte[] { 16 };
                        //stream.Read(dummy, 0, 1);                  
                        numberOfBytesRead = stream.Read(myReadBuffer, 0, myReadBuffer.Length);
                        myCompleteMessage.AppendFormat("{0}", Encoding.ASCII.GetString(myReadBuffer, 0, numberOfBytesRead));
                    }
                    while (stream.DataAvailable);
                    char userCode = myCompleteMessage[0];
                    StringBuilder msg = myCompleteMessage.Remove(0, 1);
                    // Print out the received message to the console.
                    Console.WriteLine("User {0}: {1}", userCode, msg);
                }
                else
                {
                    Console.WriteLine("Sorry you cannot read");
                }
                Thread.Sleep(500);
            }
        }
        
        static void Main(string[] args)
        {
            // Documentation about the System.Net.Sockets.TcpClient
            // https://msdn.microsoft.com/es-es/library/system.net.sockets.tcpclient(v=vs.110).aspx
            // Creates a new TCP client
            TcpClient tcpClient = new TcpClient();

            Console.WriteLine("Connecting to {0}:{1}", host, port);
            try
            {
                // Begins an asychronous conection with the server
                tcpClient.Connect(host, port);
            }
            catch (SocketException e)
            {
                Console.WriteLine("{0}", e.ToString());
            }
            

            // Blocks program while tcpClient is not connected
            while (!tcpClient.Connected)
            { }
            Console.WriteLine("Connected succesfully");
            // Get a client stream for reading and writing.
            stream = tcpClient.GetStream();

            // Starts the thread which reads the server for new messages
            // C#1.0 way of instantiate a delegate
            ThreadStart requesterRef = new ThreadStart(ReadServer);
            Thread readServerThread = new Thread(requesterRef);
            readServerThread.Start();
            // Different ways of instantiate delegates https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/delegates/how-to-declare-instantiate-and-use-a-delegate
            
            do
            {
                // Input chat promt
                // Console.Write(">>> ");
                string message = Console.ReadLine();
                if (message.Equals(""))
                {
                    continue;
                }
                // Close client
                else if (message.Equals(":q"))
                {
                    break;
                }
                // Console.WriteLine("You: {0}", message);


                Byte[] msgBytes = PrepareMessage(message);

                try
                {
                    stream.Write(msgBytes, 0, msgBytes.Length);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                    Console.WriteLine("Error writing");
                }                         
            }
            while (true);
            stream.Close();
            tcpClient.Close();
        }

        static string ProcessResponse(Byte[] responseData, Int32 bytes)
        {
            string msg = System.Text.Encoding.ASCII.GetString(responseData, 0, bytes);
            return msg;
        }

        static Byte[] PrepareMessage(string msg, bool debug = false)
        {
            string message = msg + "\n";
            Byte[] messageAscii = System.Text.Encoding.ASCII.GetBytes(message);
            if (debug)
            {
                foreach (Byte b in messageAscii)
                {
                    Console.Write(b);
                    Console.Write(" ");
                }
                Console.Write("\n");
            }            
            return messageAscii;
        }

        static Byte[] PrepareRequest()
        {
            string request = "GET /  HTTP/1.1\r\n" +
                "Host: " + host + "\r\n" +
                "Connection: close\r\n" +
                "\r\n";

            // Translate the passed message into ASCII and store it as a Byte array.
            Byte[] reqAscii = System.Text.Encoding.ASCII.GetBytes(request);
            
            return reqAscii;
        }
    }
}
