﻿using System;
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
    
    public partial class TopicChat : Form
    {
        private TcpClient client = new TcpClient();
        public TopicChat(TcpClient client, String nameTopic)
        {
            InitializeComponent();
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void TopicChat_Load(object sender, EventArgs e)
        {

        }
    }
}
