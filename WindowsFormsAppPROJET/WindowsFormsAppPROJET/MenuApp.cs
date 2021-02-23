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
    public partial class MenuApp : Form
    {

        private TcpClient client = new TcpClient();
        public MenuApp(TcpClient client)
        {
            this.client = client;
            InitializeComponent();
        }

        private void buttonTopic_Click(object sender, EventArgs e)
        {
            this.Close();
            var Topic = new Topic(client);
            Topic.Show();
        }

        private void buttonPM_Click(object sender, EventArgs e)
        {
            this.Close();
            var privateMess = new PM(client);
            privateMess.Show();
        }

        private void buttonLogOUT_Click(object sender, EventArgs e)
        {

        }
    }
}
