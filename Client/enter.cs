using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Client
{
    public partial class enter : Form
    {
        byte[] bytes = new byte[256];
        TcpClient client;
        NetworkStream stream = null;

        public enter()
        {
            InitializeComponent();
        }    

        //connect
        private void button3_Click(object sender, EventArgs e)
        {
            int port = Convert.ToInt32(textBox2.Text);
            client = new TcpClient(textBox1.Text, port);
            stream = client.GetStream();
            bytes = Encoding.Unicode.GetBytes(textBox3.Text);//send name
            stream.Write(bytes, 0, bytes.Length);
            label6.Text = "установлено";
        }

        //play
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                bytes = Encoding.Unicode.GetBytes("1");
                stream.Write(bytes, 0, bytes.Length);
                TetrisForm fr1 = new TetrisForm(stream);
                fr1.Show();
                this.Hide();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void ClientMessage()
        {
            int size;
            string message = null;
            byte[] data = new byte[256];
            size = stream.Read(data, 0, data.Length);
            message = Encoding.Unicode.GetString(data, 0, size);
            if (message == "go")
            {
                TetrisForm fr1 = new TetrisForm(stream);
                fr1.Show();
            }
        }

        //exit
        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }
        
    }
}
