using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using Communication;

namespace WindowsFormsAppPROJET
{
    public partial class Accueil : Form
    {
        private Server server;
        private String host;
        private int port;
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

        public Accueil(int port)
        {
            InitializeComponent();
            this.host = GetLocalIP();
            this.port = port;
            server = new Server(port);
            Thread serv = new Thread(new ThreadStart(server.start));
            serv.Start();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var Login = new Login(port);
            Login.Show();
        }

        private void buttonSIGNIN_Click(object sender, EventArgs e)
        {
            var Signin = new Signin(port);
            Signin.Show();
        }
    }
}
