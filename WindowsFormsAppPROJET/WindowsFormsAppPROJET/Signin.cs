using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using Communication;

namespace WindowsFormsAppPROJET
{
    public partial class Signin : Form
    {

        public static string GetLocalIP()
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

        private TcpClient client = new TcpClient();
        string host = GetLocalIP();
        int port = 123;

        public Signin(int port)
        {
            InitializeComponent();
            client = new TcpClient(host, port);
        }

        public void SignIn()
        {

            TcpClient client = new TcpClient(host, port);
            Boolean registerVerif = false;
            while (registerVerif == false)
            {
                List<String> msg = new List<String>();//Créer une nouvelle list à chaque fois
                String login = textBoxUsername.Text;

                String password = textBoxPassword.Text;
                String repeatPassword = textBoxPassword2.Text;
                

                if (password.Equals(repeatPassword))
                {
                    msg.Add("signin");
                    msg.Add(login);
                    msg.Add(password);
                    Communication.Message.sendMsg(client.GetStream(), msg);
                    if (Communication.Message.rcvMsg(client.GetStream())[0].Equals("you are know register in the serveur"))
                    {
                        registerVerif = true;
                    }

                }
                else
                {
                    this.label2.Text = "You don't enter the two same password";
                }

                if (registerVerif == false)
                {
                    this.label2.Text = "OK bro";
                    this.Close();
                    registerVerif = true;

                }

            }
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }
    }
}
