using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace zpotts_wmp_a5_client
{
    class Program
    {
        static void Main()
        {
            bool valid_username = false;   
            bool valid_ip = false;
            bool valid_port = false;

            //Some variables for user input - validation will be performed
            string client_username = "";
            string client_desired_ip = "";

            //Start the chat + collect some information from the user
            Console.WriteLine("-------------------------------------------------------------");
            Console.WriteLine("\t\t\tTCP/IP Chat!!");
            Console.WriteLine("-------------------------------------------------------------");

            while (valid_username == false)
            {
                Console.WriteLine("Please enter your username: ");      //Prompt
                client_username = Console.ReadLine();                   //Username for the chat

                if (client_username == "")                              //Check to make sure the users name is not blank
                {
                    Console.WriteLine("\nError: Username cannot be blank!");  //Error msg  
                }
                else                             //The users name is valid as long as it is not blank
                {
                    valid_username = true;      //Set loop exit condition
                }
            }

            while (valid_ip == false)
            {
                Console.WriteLine("Please enter the IP address that you want to connect to: ");      //Prompt
                client_desired_ip = Console.ReadLine();                                      //Server IP

                if (client_desired_ip == "127.0.0.1") //Check to make sure the ip entered by the user matchs the one set by the server
                {
                    valid_ip = true;
                }
                else //If the ip specificed by the users is different from the one specified by the server they will not be able to connect
                {
                    Console.WriteLine("Error: There is no availble chat with the IP Address you specified!");        //Error msg 
                    Console.WriteLine("Please try again!");         //Error msg
                }
            }

            Console.WriteLine("Please enter the port that you want to connect to: ");
            string client_desired_port = Console.ReadLine();        //Port on the server to connect to 
            //Each client must connect to the server on a different port

            try 
            {
                //Int32 port = Int32.Parse(client_desired_port);     //This needs to be the same as specified by the server
                Int32 port = 13000;     //This needs to be the same as specified by the server
                                        //client_desired_ip: This is the ip address the client wants to connect to
                
                TcpClient client = new TcpClient(client_desired_ip, port); //Connect using the ip + port specified by the client

                string client_connection = client_username + " has connected to the chat!"; //Create a message for the server
                
                // Translate the passed message into ASCII and store it as a Byte array.
                Byte[] data = System.Text.Encoding.ASCII.GetBytes(client_connection);
        
                bool open_stream = true;        //This variable will determine whether the stream is open or closed
                bool new_chat = true;           //This variable is used to display a welcome message to the user
                string message = "";            //This variable will hold the message the user would like to send to the server

                NetworkStream stream = client.GetStream();  //Open a stream to send and receive data

                stream.Write(data, 0, data.Length);         //Send the translated message to the server

                while (open_stream == true)                 //Stream is open on default
                {
                    if (new_chat == true)                   //Chat is new on default
                    {
                        //Display some welcome messages to the user
                        Console.WriteLine("Welcome " + client_username + ", you are now connected to the chat!");
                        Console.WriteLine("Enter 'q' when you wish to exit the chat.\n");
                        new_chat = false;                   //Chat is now "old", the above messsages do not need to be displayed again
                    }
                    else
                    {
                        Console.WriteLine(client_username +":");        //Show the users their name to give a "chat vibe"
                        message = Console.ReadLine();                   //Read the messages from the user

                        message = client_username + ": " + message;
                        Byte[] client_message = System.Text.Encoding.ASCII.GetBytes(message);   //Convert the users message to ASCII
                        stream.Write(client_message, 0 ,client_message.Length);         //Send the message to the server

                        if (message == "q")         //If the user enters 'q' - the exit condition
                        {
                            open_stream = false;    //Set the stream to false so that this loop can be exited + the stream can be closed
                        }

                    }
                }

                 data = new Byte[256];          //Store the response bytes

                String response_data = String.Empty;        //store response

                Int32 bytes = stream.Read(data, 0, data.Length);                //Read response
                response_data = System.Text.Encoding.ASCII.GetString(data, 0, bytes);       //Convert to ASCII
                Console.WriteLine("Received: {0}", response_data);

                Console.WriteLine("You have disconnected from the chat!");  //Tell the user they are disconnected from the chat
                
                stream.Close(); //Close the stream
                client.Close(); //Disconnect the client from the server
            }
            catch (SocketException e)                                //Exception
            {
                Console.WriteLine("SocketException: {0}", e);       //Error msg
            }
        }
    }
}
