using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Communication;
using System.Threading;

namespace ConsoleAppClient
{
    class Program
    {
        private static String login = "";
        //int attempt = 3;
        public string GetLocalIP()
        {
            IPHostEntry host;
            host = Dns.GetHostEntry(Dns.GetHostName());

            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            return "127.0.0.1";

        }
        public void start()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("========================================================================================================");
                Console.WriteLine("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
                Console.WriteLine("\t\t Connexion Menu");
                Console.WriteLine("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
                Console.WriteLine("");
                Console.WriteLine(" \t\t 1 > Sign in \t");
                Console.WriteLine(" \t\t 2 > Log in \t");
                Console.WriteLine("");
                Console.WriteLine("========================================================================================================");
                String choice = Console.ReadLine();

                if (choice.Equals("1"))
                {
                    SignIn();
                }
                else if (choice.Equals("2"))
                {
                    LogIn();
                }
                else
                {
                    Console.WriteLine(" This option is only in your dream");
                }
            }
        }

        //============================MENU enregistrement =====================================
        public void SignIn()
        {
            string host = GetLocalIP();
            int port = 123;

            TcpClient client = new TcpClient(host, port);
            Boolean registerVerif = false;
            while (registerVerif == false)
            {
                List<String> msg = new List<String>();//Créer une nouvelle list à chaque fois
                Console.WriteLine("Enter your login > ");
                String login = Console.ReadLine();

                Console.WriteLine("Enter your password > ");
                String password = Console.ReadLine();

                Console.WriteLine("Repeat your password > ");
                String repeatPassword = Console.ReadLine();

                if (password.Equals(repeatPassword))
                {
                    msg.Add("signin");
                    msg.Add(login);
                    msg.Add(password);
                    Message.sendMsg(client.GetStream(), msg);
                    if (Message.rcvMsg(client.GetStream())[0].Equals("you are know register in the serveur"))
                    {
                        Console.WriteLine("Register success ");
                        registerVerif = true;
                    }

                }
                else
                {
                    Console.WriteLine("You don't enter the two same password");
                }

                if (registerVerif == false)
                {
                    Console.WriteLine("========================================================================================================");
                    Console.WriteLine("");
                    Console.WriteLine(" 1 > Repeat ");
                    Console.WriteLine(" 2 > Go back to menu ");
                    Console.WriteLine("");
                    Console.WriteLine("========================================================================================================");

                    if (Console.ReadLine().Equals("2"))
                    {
                        start();
                        registerVerif = true;
                    }
                }

            }
        }
        //============================MENU login =================================
        public void LogIn()
        {
            string host = GetLocalIP();
            int port = 123;

            TcpClient client = new TcpClient(host, port);
            Boolean loginVerif = false, exit = false;
            while ((loginVerif == false) && (exit == false))
            {
                List<String> msg = new List<String>();//Create a new List each time of you failed to Log, to send the new data
                Console.WriteLine("Enter your login > ");
                login = Console.ReadLine();

                Console.WriteLine("Enter your password > ");
                String password = Console.ReadLine();

                msg.Add("login");
                msg.Add(login);
                msg.Add(password);
                Message.sendMsg(client.GetStream(), msg);//I get a list of lenght 1 or 2, 1 if you are already connectd 2 if you succed or have wrong password or login
                List<String> LogSuccess = Message.rcvMsg(client.GetStream());
                if (LogSuccess[0].Equals("login request accepted"))
                {
                    Console.WriteLine("Login Success ");
                    List<String> NewUser = new List<String>();
                    NewUser.Add(login);
                    Message.sendMsg(client.GetStream(), NewUser);
                    loginVerif = true;
                }
                else if (LogSuccess[0].Equals("login request rejected"))
                {
                    Console.WriteLine("Wrong password or login");
                }
                else if (LogSuccess[0].Equals("login request banned"))
                {
                    while (true)
                    {
                        Console.WriteLine("YOU ARE BANNED");
                    }

                }



                if (loginVerif == false)
                {
                    Console.WriteLine("========================================================================================================");
                    Console.WriteLine("");
                    Console.WriteLine(" 1 > Try again ");
                    Console.WriteLine(" 2 > Go back to menu ");
                    Console.WriteLine("");
                    Console.WriteLine("========================================================================================================");

                    if (Console.ReadLine().Equals("2"))
                    {
                        exit = true;
                    }
                }

            }

            if (loginVerif == true)
            {
                MenuApp(client);//Go to the mune of the application
            }
        }
        //============================MENU principale =========================================
        static void MenuApp(TcpClient client)
        {
            Boolean exit = false;
            while (exit == false)
            {
                Console.Clear();
                Console.WriteLine("========================================================================================================");
                Console.WriteLine("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
                Console.WriteLine("\t\t Chat App Menu ");
                Console.WriteLine("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
                Console.WriteLine("");
                Console.WriteLine(" \t\t 0 > Log out \t");
                Console.WriteLine(" \t\t 1 > Chat in a Topic  \t");
                Console.WriteLine(" \t\t 2 > Chat by Private message  \t");
                Console.WriteLine("");
                Console.WriteLine("========================================================================================================");
                String choice = Console.ReadLine();
                if (choice.Equals("0"))
                {
                    List<String> Logout = new List<String>();
                    Logout.Add("Logout");
                    Message.sendMsg(client.GetStream(), Logout);
                    exit = true;
                }
                else if (choice.Equals("1"))
                {
                    Topic(client);
                }
                else if (choice.Equals("2"))
                {
                    PrivateMessage(client);
                }
                else
                {
                    Console.WriteLine(" This option is only in your dream");
                }
            }
        }
        //==========================  MENU topic ===========================================
        static void Topic(TcpClient client)
        {
            Boolean exit = false;
            while (exit == false)
            {
                Console.Clear();
                Console.WriteLine("========================================================================================================");
                Console.WriteLine("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
                Console.WriteLine("\t\t Chat Room Topic Menu");
                Console.WriteLine("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
                Console.WriteLine("");
                Console.WriteLine(" \t\t 0 > Go back to main menu  \t");
                Console.WriteLine(" \t\t 1 > New topic \t");

                List<String> listTopic = new List<String>();
                Dictionary<String, String> listTopicDictionnary = new Dictionary<String, String>();
                listTopic.Add("listTopic");
                Message.sendMsg(client.GetStream(), listTopic);
                listTopic.Clear();
                listTopic = Message.rcvMsg(client.GetStream());
                int i = 2;
                foreach (String topic in listTopic)
                {
                    listTopicDictionnary.Add(i.ToString(), topic);
                    Console.WriteLine("\t\t " + i + " > " + topic);
                    i++;
                }



                Console.WriteLine("");
                Console.WriteLine("========================================================================================================");
                String choice = Console.ReadLine();
                if (choice.Equals("0"))
                {
                    exit = true;
                }
                else if (choice.Equals("1"))
                {
                    NewTopic(client);
                }
                else if (listTopicDictionnary.ContainsKey(choice))
                {
                    TopicMessage(client, listTopicDictionnary[choice]);

                }
                

            }
        }

        public static void NewTopic(TcpClient client)
        {
            Boolean exit = false;
            while (exit == false)
            {
                Console.WriteLine("Name of the new Topic > ");
                List<String> nameTopic = new List<String>();
                nameTopic.Add("addTopic");
                String NAME = Console.ReadLine();
                nameTopic.Add(NAME);
                Message.sendMsg(client.GetStream(), nameTopic);
                nameTopic.Clear();
                nameTopic = Message.rcvMsg(client.GetStream());
                if (nameTopic[0].Equals("Topic already exist"))
                {
                    Console.WriteLine("Topic already exist");
                    Console.WriteLine("0 > Go back to menu ");
                    Console.WriteLine("1 > Try another Topic");
                    if (Console.ReadLine().Equals("0"))
                    {
                        exit = true;
                    }
                }
                else
                {
                    Console.WriteLine("Topic has been created");
                    exit = true;
                }
            }


        }

        public static void TopicMessage(TcpClient client, String nameTopic)
        {
            Console.Clear();
            Console.WriteLine("========================================================================================================");
            Console.WriteLine("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
            Console.WriteLine("\t\t Chat Room on Topic : " + nameTopic);
            Console.WriteLine("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
            Console.WriteLine("");
            Console.WriteLine("Write your messsage or tape /Exit to quit the conversation");
            Console.WriteLine("========================================================================================================");
            List<String> inTopic = new List<String>();
            inTopic.Add("joinTopic");
            inTopic.Add(nameTopic);
            Message.sendMsg(client.GetStream(), inTopic);

            Boolean exit = false;
            inTopic.Clear();

            inTopic.Add("readTopic");
            inTopic.Add(nameTopic);
            Message.sendMsg(client.GetStream(), inTopic);
            inTopic.Clear();

            inTopic = Message.rcvMsg(client.GetStream());
            foreach (String line in inTopic)
            {
                Console.WriteLine(line);
            }

            Thread t1 = new Thread(() => UpdateTopicMessage(client, nameTopic));
            t1.Start();
            while (exit == false)
            {
                List<String> LineToSend = new List<String>();

                String line = Console.ReadLine();
                if (!line.ToUpper().Equals("/Exit".ToUpper()))
                {
                    LineToSend.Add("msgTopic");
                    LineToSend.Add(nameTopic);
                    LineToSend.Add(line);
                    Message.sendMsg(client.GetStream(), LineToSend);
                }
                else
                {
                    //on envoie qu'on quit
                    LineToSend.Add("/Exit");
                    Console.WriteLine("You QUit the Topic chat Room");
                    exit = true;
                    Message.sendMsg(client.GetStream(), LineToSend);
                }

            }

        }

        public static void UpdateTopicMessage(TcpClient client, String nameTopic)
        {
            Boolean exit = false;
            while (exit == false)
            {

                List<String> NewLine = Message.rcvMsg(client.GetStream());
                if (NewLine[0].Equals("Server kich the User from the chat room"))
                {
                    exit = true;
                }
                else if (NewLine[0].Equals(nameTopic))//We verifie that the msg is for our topic and not for an other
                {
                    Console.WriteLine(NewLine[1]);
                }

            }
        }




        public static void PrivateMessage(TcpClient client)
        {
            Boolean exit = false;
            while (exit == false)
            {
                Console.Clear();
                Console.WriteLine("========================================================================================================");
                Console.WriteLine("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
                Console.WriteLine("\t\t Private Chat Room");
                Console.WriteLine("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
                Console.WriteLine("");
                Console.WriteLine("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
                Console.WriteLine("");
                Console.WriteLine("\t\t Your are " + login + "| you can talk with other connected users ");
                Console.WriteLine("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
                Console.WriteLine("");
                Console.WriteLine(" \t\t 0 > Exit \t");

                List<String> listPrivate = new List<String>();
                Dictionary<String, String> listConversationDictionnary = new Dictionary<String, String>();
                listPrivate.Add("listPrivate");
                Message.sendMsg(client.GetStream(), listPrivate);
                listPrivate = Message.rcvMsg(client.GetStream());
                int i = 1;
                foreach (String user in listPrivate)
                {
                    if (!user.Equals(login))
                    {
                        listConversationDictionnary.Add(i.ToString(), user);
                        Console.WriteLine("\t\t " + i + " > " + user);
                        i++;
                    }


                }


                Console.WriteLine("");
                Console.WriteLine("========================================================================================================");
                String choice = Console.ReadLine();
                if (choice.Equals("0"))
                {
                    exit = true;
                }
                else if (listConversationDictionnary.ContainsKey(choice))
                {
                    Console.WriteLine(listConversationDictionnary[choice]);
                    writePrivateMessage(client, listConversationDictionnary[choice]);

                }
            }
        }

        public static void writePrivateMessage(TcpClient client, String ChatterUsername)
        {
            Console.Clear();
            Console.WriteLine("========================================================================================================");
            Console.WriteLine("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
            Console.WriteLine("");
            Console.WriteLine("\t\t Your are " + login + ", And you're chatting with" + ChatterUsername);
            Console.WriteLine("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
            Console.WriteLine("");
            Console.WriteLine("Write your messsage or tape /Exit to quit the conversation");
            Console.WriteLine("========================================================================================================");
            List<String> inConversation = new List<String>();
            inConversation.Add("joinPrivate");
            inConversation.Add(ChatterUsername);
            Message.sendMsg(client.GetStream(), inConversation);

            Boolean exit = false;
            List<String> readFileConversation = new List<String>();

            readFileConversation.Add("readPrivate");
            readFileConversation.Add(ChatterUsername);
            Message.sendMsg(client.GetStream(), readFileConversation);
            readFileConversation = Message.rcvMsg(client.GetStream());
            foreach (String conv in readFileConversation)
            {
                Console.WriteLine(conv);
            }
            Thread t1 = new Thread(() => UpdatePMessage(client, ChatterUsername));
            t1.Start();
            while (exit == false)
            {
                List<String> lineSend = new List<String>();

                String line = Console.ReadLine();
                if (!line.ToUpper().Equals("/Exit".ToUpper()))
                {
                    lineSend.Add("msgPrivate");
                    lineSend.Add(ChatterUsername);
                    lineSend.Add(line);
                    Message.sendMsg(client.GetStream(), lineSend);
                }
                else
                {
                    lineSend.Add("/Exit");
                    exit = true;
                    Message.sendMsg(client.GetStream(), lineSend);
                }

            }


        }
        public static void UpdatePMessage(TcpClient client, String ChatterUsername)
        {
            Boolean exit = false;
            while (exit == false)
            {

                List<String> NewMessage = Message.rcvMsg(client.GetStream());
                if (NewMessage[0].Equals("Server kich the User from the chat room"))
                {
                    exit = true;
                }
                else if ((NewMessage[0].Equals(login + "and" + ChatterUsername)) || (NewMessage[0].Equals(ChatterUsername + "and" + login)))
                {
                    Console.WriteLine(NewMessage[1]);
                }

            }

        }

        static void Main(string[] args)
        {
            Program serv = new Program();
            serv.start();

        }
    }
}
