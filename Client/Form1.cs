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

namespace Client
{
    public partial class Form1 : Form
    {
        byte[] readBuffer = new byte[1024];
        byte[] writeBuffer = new byte[1024];
        int size = 25;
        string h;
        //int chet = 0;
        //byte[] data = new byte[256];
        string message;
        //public TimeSpan reaction;
        //double res = 0;
        //public DateTime time = DateTime.Now;
        //private Point targetposition = Point.Empty;
        //private Point direction = Point.Empty;
        int[,] map = new int[16, 8];


        NetworkStream stream1;
        //NetworkStream streamWrite;
        public Form1(NetworkStream stream)
        {
            stream1 = stream;
            InitializeComponent();
        }

        private void keyFunc(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Up:
                    writeBuffer = Encoding.Unicode.GetBytes("1");
                    stream1.Write(writeBuffer, 0, writeBuffer.Length);
                    Invalidate();

                    break;
                case Keys.Down:
                    writeBuffer = Encoding.Unicode.GetBytes("2");
                    stream1.Write(writeBuffer, 0, writeBuffer.Length);
                    break;
                case Keys.Right:
                    writeBuffer = Encoding.Unicode.GetBytes("3");
                    stream1.Write(writeBuffer, 0, writeBuffer.Length);
                    break;
                case Keys.Left:
                    writeBuffer = Encoding.Unicode.GetBytes("4");
                    stream1.Write(writeBuffer, 0, writeBuffer.Length);
                    break;
            }
        }

        public void DrawMap(int[,] map, Graphics e)
        {
            for (int i = 0; i < 16; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (map[i, j] == 1)
                    {
                        e.FillRectangle(Brushes.Red, new Rectangle(50 + j * (size) + 1, 50 + i * (size) + 1, size - 1, size - 1));
                    }
                    if (map[i, j] == 2)
                    {
                        e.FillRectangle(Brushes.Yellow, new Rectangle(50 + j * (size) + 1, 50 + i * (size) + 1, size - 1, size - 1));
                    }
                    if (map[i, j] == 3)
                    {
                        e.FillRectangle(Brushes.Green, new Rectangle(50 + j * (size) + 1, 50 + i * (size) + 1, size - 1, size - 1));
                    }
                    if (map[i, j] == 4)
                    {
                        e.FillRectangle(Brushes.Blue, new Rectangle(50 + j * (size) + 1, 50 + i * (size) + 1, size - 1, size - 1));
                    }
                    if (map[i, j] == 5)
                    {
                        e.FillRectangle(Brushes.Purple, new Rectangle(50 + j * (size) + 1, 50 + i * (size) + 1, size - 1, size - 1));
                    }
                }
            }
        }

        public void ClientMessage()
        {
            while (true)
            {
                //получение матрицы поля
                int readBufferLength = stream1.Read(readBuffer, 0, readBuffer.Length);
                message = Encoding.Unicode.GetString(readBuffer, 0, readBufferLength);

                map = ToIntArray(message);
                Thread.Sleep(100);
            }
        }

        //отрисовка клеток карты
        public void DrawGrid(Graphics g)
        {
            for (int i = 0; i <= 16; i++)
            {
                g.DrawLine(Pens.Black, new Point(50, 50 + i * size), new Point(50 + 8 * size, 50 + i * size));
            }
            for (int i = 0; i <= 8; i++)
            {
                g.DrawLine(Pens.Black, new Point(50 + i * size, 50), new Point(50 + i * size, 50 + 16 * size));
            }
        }

        private void OnPaint(object sender, PaintEventArgs e)
        {
            //получаем сообщение что все клиенты подключились
            int readBufferLength = stream1.Read(readBuffer, 0, readBuffer.Length);
            message = Encoding.Unicode.GetString(readBuffer, 0, readBufferLength);
            if (message == "go")
            {
                writeBuffer = Encoding.Unicode.GetBytes("2");//отправляем серверу что готовы к игре
                stream1.Write(writeBuffer, 0, writeBuffer.Length);//отправляем
            }
            //while (message == "go") { Thread.Sleep(10); }
            Thread th = new Thread(ClientMessage);
            th.Start();

            // тут говно пошло
            //while (true)
            //{
                //получение матрицы поля
                //readBufferLength = stream1.Read(readBuffer, 0, readBuffer.Length);
                //message = Encoding.Unicode.GetString(readBuffer, 0, readBufferLength);

                //map = ToIntArray(message);

                DrawGrid(e.Graphics);
                DrawMap(map, e.Graphics);
                Thread.Sleep(10);
            //}
            //Убираем эту отрисовку новой фигуры , нафиг надо
            //ShowNextShape(e.Graphics);
        }

        public static int[,] ToIntArray (string inputString)
        {
            int[,] resultArrayInt = new int[16, 8];
            string[] resultArrayLines = inputString.Split(new char[] { '/' });
            string[] resultArrayElements;

            for (int y = 0; y < 16; y++)
            {
                resultArrayElements = resultArrayLines[y].Split(new char[] { ' ' });
                for (int x = 0; x < 8; x++)
                {
                    resultArrayInt[y, x] = Convert.ToInt32(resultArrayElements[x]);
                }
            }
            return resultArrayInt;
        }
    }
}
