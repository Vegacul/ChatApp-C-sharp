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
    public partial class Login : Form
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

        public Login(int port)
        {
            InitializeComponent();
            client = new TcpClient(host, port);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            TcpClient client = new TcpClient(host, port);

            Boolean loginVerif = false, exit = false;
            while ((loginVerif == false) && (exit == false))
            {
                List<String> msg = new List<String>();//Créer une nouvelle list à chaque fois
                String login = textBoxUsername.Text;
                String password = textBoxPassword.Text;
                msg.Add("login");
                msg.Add(login);
                msg.Add(password);
                Communication.Message.sendMsg(client.GetStream(), msg);//I get a list of lenght 1 or 2, 1 if you are already connectd 2 if you succed or have wrong password or login
                List<String> LogSuccess = Communication.Message.rcvMsg(client.GetStream());
                if (LogSuccess[0].Equals("login request accepted"))
                {
                    this.label2.Text = ("Login Success ");
                    List<String> NewUser = new List<String>();
                    NewUser.Add(login);
                    Communication.Message.sendMsg(client.GetStream(), NewUser);
                    loginVerif = true;
                }
                else if (LogSuccess[0].Equals("login request rejected"))
                {
                    this.label2.Text = ("Wrong password or login");
                }
                else if (LogSuccess[0].Equals("login request banned"))
                {
                    while (true)
                    {
                        this.label2.Text = ("YOU ARE BANNED");
                    }

                }

            }

            if (loginVerif == true)
            {
                this.label2.Text = ("Connected");
                var Menu = new MenuApp(client);
                this.Close();
                Menu.Show();

            }
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }
    }
}
