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
    public partial class Topic : Form
    {

        private TcpClient client = new TcpClient();
        private Dictionary<int, String> listTopicDictionnary = new Dictionary<int, String>();
        public Topic(TcpClient client)
        {
            this.client = client;
            InitializeComponent();
            buildListTopic();
        }
        public void buildListTopic()
        {
            List<String> listTopic = new List<String>();
            listTopic.Add("listTopic");
            Communication.Message.sendMsg(client.GetStream(), listTopic);
            listTopic = Communication.Message.rcvMsg(client.GetStream());
            int i = 0;
            foreach (String topic in listTopic)
            {
                listTopicDictionnary.Add(i, topic);
                listTopicSelect.Items.Add(topic);
                i++;
            }
        }

        private void buttonAdd_Click(object sender, EventArgs e)
        {
            this.Close();
            var NewTopic = new TopicNew(client);
            NewTopic.Show();
        }


        void listTopicSelect_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            int index = this.listTopicSelect.IndexFromPoint(e.Location);
            if (index != System.Windows.Forms.ListBox.NoMatches)//If you double click on an element which exist
            {

                this.Close();
                var WriteTopic = new TopicChat(client, listTopicDictionnary[index]);
                WriteTopic.Show();
            }


        }

        private void buttonBack_Click(object sender, EventArgs e)
        {

            this.Close();
            var Menu = new MenuApp(client);
            Menu.Show();
        }

        private void Topic_Load(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void listTopicSelect_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
