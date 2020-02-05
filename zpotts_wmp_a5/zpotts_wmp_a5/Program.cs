using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace zpotts_wmp_a5
{
    class Program
    {
        public static void Main(string[] args)
        {

            TcpListener server = null;                                  

            try
            {
                Int32 port = 13000;                                     //Set the port to 13000
                IPAddress local_ad = IPAddress.Parse("127.0.0.1");      //Set the IP address

                server = new TcpListener(local_ad, port);               //Set the server to listen on the port + ip

                server.Start();                                         //Start the server

                Console.WriteLine("Waiting for a connection ... ");     //This is displayed in the server console

                TcpClient client = server.AcceptTcpClient();            //Accept new client         

                Console.WriteLine("Connected !!");                      //This is displayed in the server console
                
                while (true)
                {

                    ParameterizedThreadStart thread_start = new ParameterizedThreadStart(AddClient);

                    Thread client_thread = new Thread(thread_start);

                    client_thread.Start(client);

                }
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }
            finally
            {
                server.Stop();
            }
        }


        public static void AddClient(object o)
        {
            Byte[] bytes = new Byte[256];

            string data = "";

            int i;

            string usersname = "";          //This will hold the users name

            bool welcome_client = true;

            bool disconnect_client = false;

            TcpClient server = (TcpClient)o;                //Create a new client

            NetworkStream stream = server.GetStream();  //Open a stream to send and receive data

            while ((i = stream.Read(bytes, 0, bytes.Length)) != 0) 
            {
                if (disconnect_client == false)
                {

                    if (welcome_client == true)                            //If the client has just entered the chat 
                    {
                        data = System.Text.Encoding.ASCII.GetString(bytes, 0, bytes.Length);       //Convert data to ASCII values

                        Console.WriteLine("Received: {0}", data);     //This is displayed on the server console: username has connected to the chat

                        usersname = data;

                        string server_welcome = "Welcome " + usersname + ", You are now connected to the chat!"; //This is the message we want to send to the client

                        byte[] server_message = System.Text.Encoding.ASCII.GetBytes(server_welcome);        //Translate this message into ASCII

                        stream.Write(server_message, 0, server_message.Length);                             //Send the welcome message to the client

                        welcome_client = false;         //This is now false as the client has been welcomed to the chat
                    }
                    else
                    { 

                        Console.WriteLine("Received: {0}", data);     //This is displayed on the server console: username has connected to the chat

                        string server_response = "Message received!"; //This is the message we want to send to the client

                        byte[] server_message = System.Text.Encoding.ASCII.GetBytes(server_response);        //Translate this message into ASCII

                        stream.Write(server_message, 0, server_message.Length);                             //Send the welcome message to the client
                                                                                                            //Console.WriteLine("Sent: {0}", data);
                    }
                }
            }

            server.Close();     //Disconnect the client from the server
        }
    }
}
