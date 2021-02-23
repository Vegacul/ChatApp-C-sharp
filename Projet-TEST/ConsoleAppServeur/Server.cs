using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Communication;

namespace ConsoleAppServeur
{
    public class Server
    {
        private int port;

        //on creer un dictionnaire pour pouvoir parcourir facilement avec foreach mais aussi verifier plus facilement la presence d'une cle ou d'une valeur
        static Dictionary<String, UserInfo> users_list = new Dictionary<String, UserInfo>();


        //le constructeur est utiliser pour donner un port de connexion on peut le changer en créant durectement un serveur server(port)
        public Server(int port)
        {
            this.port = port;
        }

        //methode qui permet de faire fonctionne le serveur peut importe la connexion l'address ip étant changeante selon l'odinateur et ou la box internet 
        //ic on recupoere directement l'ip a utiliser 
        private string GetLocalIP()
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
            //on lance la connection sur serveur 
            //on config ip grace a getlocalip
            IPAddress localAddr = IPAddress.Parse(GetLocalIP());
            //le serveur ecoute ce qu'il se passe sur l'ip X et le port 
            TcpListener l = new TcpListener(localAddr, port);
            // on lance l'ecoute et par conséquent le serveur 
            l.Start();
            //on met un msg dans la console qui indique que le serv est a l'ecoute sur l'ip X et le port Y
            Console.WriteLine("Server " + localAddr + " is launch and waitting for clients on port " + port);


            //le serveur acccepte tout les nouvelle demande de connexion
            while (true)
            {
                TcpClient comm = l.AcceptTcpClient();
                //on ecrit dans le serv des qu'unne nouvelle co arrive 
                Console.WriteLine("Connection established @" + comm);
                //pour chaque nouveau user on lance la boucle d'operation sous forme de thread
                new Thread(new Receiver(comm).doOperation).Start();
            }
        }

        // ================================================================== on enregister des object userinfo qui contient le client TCP et sa position actuelle

        //l'objet user info stock la connection du user et sa position (le topic dans lequel il parle)
        public class UserInfo
        {
            TcpClient client = new TcpClient();
            //de base le client parle pas donc n'a pas de position
            String position = ""; 
            public UserInfo(TcpClient client, String position)
            {
                this.client = client;
                this.position = position;
            }

            public void setPosition(String position)
            {
                this.position = position;
            }
            public String getPosition()
            {
                return position;
            }
            public TcpClient getClient()
            {
                return client;
            }
        }
        // ================================================================== RECEIVER CLASS 
        class Receiver
        {

            // on creer des semaphore pour les action qui necessite un enregistrement et un gestion fichier /memoire
            //on attend que le procs en cours soit fini pour lancer le deuxieme 

            private Semaphore LoginSemaphore = new Semaphore(1, 1);
            // LoginSemaphore.WaitOne();
            // LoginSemaphore.Release();
            private Semaphore SigninSemaphore = new Semaphore(1, 1);
            // SigninSemaphore.WaitOne();
            // SigninSemaphore.Release();

            private Semaphore addTopicSemaphore = new Semaphore(1, 1);
            // addTopicSemaphore.WaitOne();
            // addTopicSemaphore.Release();
            private Semaphore readTopicSemaphore = new Semaphore(1, 1);
            // readTopicSemaphore.WaitOne();
            // readTopicSemaphore.Release();
            private Semaphore msgTopicSemaphore = new Semaphore(1, 1);
            // msgTopicSemaphore.WaitOne();
            // msgTopicSemaphore.Release();

            private Semaphore readPMSemaphore = new Semaphore(1, 1);
            // readPMSemaphore.WaitOne();
            // readPMSemaphore.Release();
            private Semaphore msgPMSemaphore = new Semaphore(1, 1);
            // msgPMSemaphore.WaitOne();
            // msgPMSemaphore.Release();


            private TcpClient comm;
            //list des identifiant  sous forme d'identifiant voir dessous
            static List<Identifiant> IdentifiantList = new List<Identifiant>();
            //list des topic sont for de chaine de caractere
            static List<string> TopicList = new List<string>();



            public Receiver(TcpClient s)
            {
                comm = s;
            }
            // l'objet identifiant stocke le nom et le mdp en clair
            [Serializable]
            public class Identifiant
            {
                public string Username { get; set; }
                public string Password { get; set; }

            }

            //=========================================== serialization des fichiers 

            //================ ficher sauvegarde des identifiants
            public void serializeID()
            {
                Console.WriteLine("\n ==== SERIALIZATION identifiant ==== ");
                Stream stream = File.Open("Identifiant.dat", FileMode.Create);

                BinaryFormatter bf = new BinaryFormatter();

                bf.Serialize(stream, IdentifiantList);
                stream.Close();
            }

            public void deserializeID()
            {
                Console.WriteLine("\n ==== DESERIALIZATION identifiant ==== ");
                Stream stream = File.Open("Identifiant.dat", FileMode.Open);
                BinaryFormatter bf = new BinaryFormatter();

                IdentifiantList = (List<Identifiant>)bf.Deserialize(stream);
                stream.Close();
            }

            //=====================ficher sauvegarde liste des topics
            public void serializeTOPIC()
            {
                Console.WriteLine("\n ==== SERIALIZATION Topic ==== ");
                Stream stream = File.Open("Topic.dat", FileMode.Create);

                BinaryFormatter bf = new BinaryFormatter();

                bf.Serialize(stream, TopicList);
                stream.Close();
            }

            public void deserializeTOPIC()
            {
                Console.WriteLine("\n ==== DESERIALIZATION Topic ==== ");
                Stream stream = File.Open("Topic.dat", FileMode.Open);
                BinaryFormatter bf = new BinaryFormatter();

                TopicList = (List<string>)bf.Deserialize(stream);
                stream.Close();
            }

            // ============fichier sauvegarde des chats
            public List<String> DEserializeChatTOPIC(String nameTopic)
            {
                try
                {
                    List<String> TopicChat = new List<String>();
                    Stream streamTopic = File.Open(nameTopic + ".dat", FileMode.Open);
                    BinaryFormatter bf3 = new BinaryFormatter();
                    try
                    {//2eme try car si le fichier est vide on deserialize un truc null et ça marche pas et du coup on peut plus fermer le stream donc plus l'ouvrir
                        TopicChat = (List<String>)bf3.Deserialize(streamTopic);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                    streamTopic.Close();
                    return TopicChat;
                }
                catch (Exception e)
                {

                    Console.WriteLine(e);
                    return new List<String>();
                }

            }

            public void SerializeChatTOPIC(List<String> TopicChat, String nameTopic)//Give all the conversation to put in the file and the name of the file
            {
                try
                {
                    if (!(nameTopic.Equals("Topic.dat")) && (!(nameTopic.Equals("Identifiant.dat"))))/// because
                    {
                        ///Add to the file which contain all topic the new topic
                        Stream streamTopic = File.Open(nameTopic + ".dat", FileMode.Create);

                        BinaryFormatter bf1 = new BinaryFormatter();
                        bf1.Serialize(streamTopic, TopicChat);
                        streamTopic.Close();
                    }


                }
                catch (Exception e)
                {
                    Console.Write(e);
                }
            }

            public List<String> deSerializePM(String nameUser1, String nameUser2)
            {
                try
                {
                    List<String> Conversation = new List<String>();
                    Stream streamTopic = File.Open(nameUser1 + "and" + nameUser2 + ".dat", FileMode.Open);
                    BinaryFormatter bf3 = new BinaryFormatter();
                    try
                    {//2eme try car si le fichier est vide on deserialize un truc null et ça marche pas et du coup on peut plus fermer le stream donc plus l'ouvrir
                        Conversation = (List<String>)bf3.Deserialize(streamTopic);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                    streamTopic.Close();
                    return Conversation;
                }
                catch (Exception e)
                {

                    Console.WriteLine(e);
                    return new List<String>();
                }

            }


            public void SerializePM(List<String> conversation, String nameUser1, String nameUser2)
            {
                try
                {
                    ///Add to the file which contain all topic the new topic
                    Stream streamTopic = File.Open(nameUser1 + "and" + nameUser2 + ".dat", FileMode.Create);

                    BinaryFormatter bf1 = new BinaryFormatter();
                    bf1.Serialize(streamTopic, conversation);
                    streamTopic.Close();

                    Stream streamTopic2 = File.Open(nameUser2 + "and" + nameUser1 + ".dat", FileMode.Create);

                    BinaryFormatter bf2 = new BinaryFormatter();
                    bf2.Serialize(streamTopic2, conversation);
                    streamTopic2.Close();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }


            // ==================================== fonction  du serveur 
            public void doOperation()
            {

                Console.WriteLine("Computing operation");
                String username = "";

                while (true)
                {

                    // on a def dans communication qu'on recoit des list 
                    List<String> msg = new List<String>();
                    //le serv attent les nouveaux messages 
                    msg = (List<String>)Message.rcvMsg(comm.GetStream());
                    //on ecrit dans le serveur qu'on a bien recu une nouvelle demande 
                    Console.WriteLine("\nexpression received");


                    // j'ai choisi de def que le 1er element donc en 0 sera le type d'action et ensuite le contenu 
                    // send result
                    switch (msg[0])
                    {
                        //losque le premier element de la list recu est 
                        case "logout":
                            // reussir a ferme le truc client sans que ca eteigne le serveur 
                            break;

                        case "login":
                            //on lance le Semaphore pour que plusieur login entre pas en conflic 
                            LoginSemaphore.WaitOne();
                            //on recupere les login stocké dans le fichier 
                            deserializeID();
                            //la structure envoyé étant user puis log , on les recup
                            username = (msg[1]);
                            string password = (msg[2]);
                            //variable de connexion 
                            string conn = "non";
                            //variable d'essai
                            int attempt = 3;
                            //on verifie si les id recu sont dans la liste stocké
                            if (IdentifiantList.Any(x => x.Username == username && x.Password == password))
                            {
                                //si oui  alors
                                attempt = 0;
                                Console.WriteLine(username + " is connected to the serveur ");
                                UserInfo userInfo = new UserInfo(comm, "");
                                Console.WriteLine(username + " is ADDDED TO USERLIST ");
                                //on ajoute cet utilisateur a notre dico de user connecté
                                users_list.Add(username, userInfo);
                                //on vide le msg recu et envoi la reponse
                                msg.Clear();
                                msg.Add("login request accepted");
                                Message.sendMsg(comm.GetStream(), msg);
                                //la connexion est reussi 
                                conn = "oui";
                            }
                            //si la connexion est pas valide et qu'on a toujours des essai alors 
                            else if ((attempt > 0) && (conn == "non"))
                            {
                                //on perd un essai
                                --attempt;
                                //on note sur le serv la tentative de co
                                Console.WriteLine(username + "try to connected to the serveur ");
                                //on renvoie la rep du serv 
                                msg.Clear();
                                msg.Add("login request rejected");

                                Message.sendMsg(comm.GetStream(), msg);
                            }
                            else
                            {
                                //si co rate et plus d'essai alors c fini
                                Console.WriteLine(username + "are banned from the serveur ");
                                //on renvoie la rep du serv 
                                msg.Clear();
                                msg.Add("login request banned");

                                Message.sendMsg(comm.GetStream(), msg);
                            }
                            //le log est fini alors on termine le semaphore
                            LoginSemaphore.Release();
                            break;

                        case "signin":
                            SigninSemaphore.WaitOne();
                            //on charge les id
                            deserializeID();
                            username = (msg[1]);
                            string password2 = (msg[2]);
                            //on ajoute le nouvel id
                            IdentifiantList.Add(new Identifiant { Username = username, Password = password2 });
                            //on re save les id 
                            serializeID();

                            Console.WriteLine(username + " are know register in the serveur ");

                            //on renvoie la rep du serv 
                            msg.Clear();
                            msg.Add("you are know register in the serveur");
                            Message.sendMsg(comm.GetStream(), msg);
                            //le log est fini alors on termine le semaphore
                            SigninSemaphore.Release();
                            break;

                        /// ================================================== TOPIC SERVEUR FUNCTION ============================================================
                        case "listTopic": //renvoyer la liste des topics
                            //on recupere la list des topic
                            deserializeTOPIC();
                            //et on a renvoi
                            Message.sendMsg(comm.GetStream(), TopicList);
                            break;

                        case "addTopic": //ajouter dans la liste le topic demandé
                            addTopicSemaphore.WaitOne();
                            //on charge la liste de topic sauvegarde
                            deserializeTOPIC();
                            //on verifie  que le topic n existe pas deja
                            if (!TopicList.Contains(msg[1]))
                            {
                                // on recup le nom du nouveau topic
                                string topicName = (msg[1]);
                                //on ajoute a la liste
                                TopicList.Add(topicName);
                                //on save
                                serializeTOPIC();
                                //on renvoie la reponse
                                msg.Clear();
                                msg.Add("Topic create");
                                Message.sendMsg(comm.GetStream(), msg);
                                //on creer un ficher de sauvegarde pour ce nouveau topic
                                Stream streamFileTopic = File.Open(topicName + ".dat", FileMode.Create);
                                streamFileTopic.Close();
                                addTopicSemaphore.Release();
                            }
                            else
                            {
                                //si le topic existe deja 
                                //on renvoie un msg specific 
                                msg.Clear();
                                msg.Add("Topic already exist");
                                Message.sendMsg(comm.GetStream(), msg);
                                addTopicSemaphore.Release();
                            }
                            break;

                        case "joinTopic": //rejoindre un topic existant
                            //on parcour le dico des user connecté
                            foreach (var user in users_list)
                            {
                                //on cherche le user en question 
                                if (user.Key.ToString().Equals(username)) // si c'est bien nous dans la liste 
                                {
                                    //on chage la position de l'utilisateur 
                                    //Console.WriteLine(user.Key.ToString());
                                    Console.WriteLine(username + " enter the room " + msg[1]);
                                    user.Value.setPosition(msg[1]);//on positionne cet utilisateur dans le topic qu'il a rejoint
                                }

                            }
                            break;

                        case "readTopic": //renvoyer le chat sauvegarder du topic en question     
                            readTopicSemaphore.WaitOne();
                            //on charge le chat du topic sauvegardé
                            List<String> conversationTopic = new List<String>();
                            conversationTopic = DEserializeChatTOPIC(msg[1]);//Msg[1] contain the name of the topic that you want read
                            //on le save
                            SerializeChatTOPIC(conversationTopic, msg[1]);
                            //on envoie la rep 
                            Message.sendMsg(comm.GetStream(), conversationTopic);
                            readTopicSemaphore.Release();
                            break;

                        case "msgTopic": //ajouter un msg au doc de ce topic et envoyer l'update a tout les user connecté
                            msgTopicSemaphore.WaitOne();
                            //on charge le msg envoyé par le user
                            List<String> lineSend = new List<String>();//Msg[1] contain the name of the topic that you want read
                            lineSend.Add(msg[1]); //on ajoute le topic destination
                            Console.WriteLine("Envoie de " + username + " > " + msg[2]); //on formate le message 
                            lineSend.Add(username + " > " + msg[2]);


                            List<String> NewconversationTopic = DEserializeChatTOPIC(msg[1]); //on re charge le ficher
                            NewconversationTopic.Add(lineSend[1]);    // on ajoute la nouvelle ligne dans un le chat 
                            SerializeChatTOPIC(NewconversationTopic, msg[1]); // on sauvegarde le chat



                            foreach (var user in users_list) //pour tout le user 
                            {
                                //Console.WriteLine(user);
                                //Console.WriteLine("VERIFFFFF");
                                if (!user.Key.ToString().Equals(username)) //si ce n'ets pas nous meme
                                {

                                    if (user.Value.getPosition().Equals(msg[1])) //si la position est bien la meme donc le meme topics
                                    {

                                        Message.sendMsg(user.Value.getClient().GetStream(), lineSend); //on envoie a ce client la nouvelle ligne 
                                    }

                                }

                            }
                            msgTopicSemaphore.Release();
                            break;

                        case "/Exit":
                            //on parcour le dico des user connecté
                            foreach (var user in users_list)
                            {
                                //on cherche le user en question 
                                if (user.Key.ToString().Equals(username)) // si c'est bien nous dans la liste 
                                {
                                    //on chage la position de l'utilisateur 
                                    //Console.WriteLine(user.Key.ToString());
                                    Console.WriteLine(username + " quit the room "+ user.Value.getPosition());
                                    user.Value.setPosition("");//on positionne cet utilisateur en defaultt
                                }
                                List<String> ServeurRespond = new List<String>();
                                ServeurRespond.Add("Server kich the User from the chat room");
                                Message.sendMsg(comm.GetStream(), ServeurRespond);
                            }
                            break;

                        /// ================================================== PRIVATE MESSAGE SERVEUR FUNCTION ============================================================



                        case "listPrivate":
                            List<String> PVlist = new List<String>();
                            
                            foreach (var user in users_list) //pour tout le user 
                            {
                                if (!user.Key.ToString().Equals(username)) //si ce n'ets pas nous meme
                                {
                                    PVlist.Add(user.Key.ToString()); 
                                }

                            }
                            Message.sendMsg(comm.GetStream(), PVlist);
                            break;


                        case "joinPrivate":
                            foreach (var user in users_list)
                            {

                                if (user.Key.ToString().Equals(username))
                                {
                                    Console.WriteLine(username + " enter the conv" + username + "and" + msg[1]);
                                    user.Value.setPosition(username + "and" + msg[1]);//Set position att name of conversation file
                                }

                            }
                            break;

                        case "readPrivate":
                            readPMSemaphore.WaitOne();
                            List<String> conversation = new List<String>();
                            conversation = deSerializePM(username, msg[1]);//Msg[1] contain the name of the topic that you want read
                            SerializePM(conversation, username, msg[1]);
                            Message.sendMsg(comm.GetStream(), conversation);
                            readPMSemaphore.Release();
                            break;

                        case "msgPrivate":
                            msgPMSemaphore.WaitOne();
                            // 
                            List<String> NewMsg = new List<String>();//Msg[1] contain the name of the topic that you want read
                            NewMsg.Add(username + "and" + msg[1]);
                            Console.WriteLine("Envoie de " + username + " > " + msg[2]);
                            NewMsg.Add(username + " > " + msg[2]);
                            List<String> convWrite = deSerializePM(username, msg[1]);
                            convWrite.Add(NewMsg[1]);
                            SerializePM(convWrite, username, msg[1]);
                            foreach (var user in users_list)
                            {
                                if (!user.Key.ToString().Equals(username))
                                {
                                    if ((user.Value.getPosition().Equals(username + "and" + msg[1])) || (user.Value.getPosition().Equals(msg[1] + "and" + username)))
                                    {
                                        Message.sendMsg(user.Value.getClient().GetStream(), NewMsg);
                                    }

                                }

                            }
                            msgPMSemaphore.Release();
                            break;

                        default:

                            Console.WriteLine("Default case");
                            break;







                    }

                }

            }
        }


    }
}
