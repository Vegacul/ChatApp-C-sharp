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
    public partial class TopicNew : Form
    {
        private TcpClient client = new TcpClient();
        public TopicNew(TcpClient client)
        {
            this.client = client;
            InitializeComponent();
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {

            List<String> nameTopic = new List<String>();
            nameTopic.Add("addTopic");
            String NAME = Console.ReadLine();
            nameTopic.Add(NAME);
            Communication.Message.sendMsg(client.GetStream(), nameTopic);
            nameTopic.Clear();
            nameTopic = Communication.Message.rcvMsg(client.GetStream());
            if (nameTopic[0].Equals("Topic already exist"))
            {
                this.label2.Text = ("Topic already exist");

            }
            else
            {
                this.label2.Text = ("Topic has been created");

            }
        }

        private void buttonGoback_Click(object sender, EventArgs e)
        {
            this.Close();
            var Topic = new Topic(client);
            Topic.Show();
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }
    }
}
