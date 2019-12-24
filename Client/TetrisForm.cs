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
    public partial class TetrisForm : Form
    {
        byte[] readBuffer = new byte[1024];
        byte[] writeBuffer = new byte[1024];
        static int size = 25; //размер 1 клетки поля
        string messageFromClient;
        static readonly int mapWidth = 12;

        int[,] map = new int[16, mapWidth];

        NetworkStream getMapFromServerStream;

        public TetrisForm(NetworkStream stream)
        {
            getMapFromServerStream = stream;
            InitializeComponent();
        }

        private void OnLoad(object sender, EventArgs e)
        {
            //получаем сообщение что все клиенты подключились
            int readBufferLength = getMapFromServerStream.Read(readBuffer, 0, readBuffer.Length);
            messageFromClient = Encoding.Unicode.GetString(readBuffer, 0, readBufferLength);
            if (messageFromClient == "go")
            {
                writeBuffer = Encoding.Unicode.GetBytes("2");//отправляем серверу что готовы к игре
                getMapFromServerStream.Write(writeBuffer, 0, writeBuffer.Length);//отправляем
            }

            Thread getMapFromServer = new Thread(GetMapFromServer);
            getMapFromServer.Start();

            Thread drawGridLinesThread = new Thread(DrawFullMap);
            drawGridLinesThread.Start();

        }

        //событие отрисовки формы
        private void OnPaint(object sender, PaintEventArgs e) { }

        //обработка нажатий клавиш
        public void PressKeyHandler(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Up:
                    writeBuffer = Encoding.Unicode.GetBytes("Up");
                    getMapFromServerStream.Write(writeBuffer, 0, writeBuffer.Length);
                    break;
                case Keys.Down:
                    writeBuffer = Encoding.Unicode.GetBytes("Down");
                    getMapFromServerStream.Write(writeBuffer, 0, writeBuffer.Length);
                    break;
                case Keys.Right:
                    writeBuffer = Encoding.Unicode.GetBytes("Right");
                    getMapFromServerStream.Write(writeBuffer, 0, writeBuffer.Length);
                    break;
                case Keys.Left:
                    writeBuffer = Encoding.Unicode.GetBytes("Left");
                    getMapFromServerStream.Write(writeBuffer, 0, writeBuffer.Length);
                    break;
                case Keys.W:
                    writeBuffer = Encoding.Unicode.GetBytes("WUp");
                    getMapFromServerStream.Write(writeBuffer, 0, writeBuffer.Length);
                    break;
                case Keys.S:
                    writeBuffer = Encoding.Unicode.GetBytes("SDown");
                    getMapFromServerStream.Write(writeBuffer, 0, writeBuffer.Length);
                    break;
                case Keys.D:
                    writeBuffer = Encoding.Unicode.GetBytes("DRight");
                    getMapFromServerStream.Write(writeBuffer, 0, writeBuffer.Length);
                    break;
                case Keys.A:
                    writeBuffer = Encoding.Unicode.GetBytes("ALeft");
                    getMapFromServerStream.Write(writeBuffer, 0, writeBuffer.Length);
                    break;
            }
        }

        //отрисовка карты
        public static void DrawMap(int[,] map, Graphics e)
        {
            try
            {
                for (int i = 0; i < 16; i++)
                {
                    for (int j = 0; j < mapWidth; j++)
                    {
                        if (map[i, j] == 1)
                        {
                            e.FillRectangle(Brushes.Red, new Rectangle(50 + j * (size) + 1, 50 + i * (size) + 1, size - 1, size - 1));
                        }
                        if (map[i, j] == 2)
                        {
                            e.FillRectangle(Brushes.Orange, new Rectangle(50 + j * (size) + 1, 50 + i * (size) + 1, size - 1, size - 1));
                        }
                        if (map[i, j] == 3)
                        {
                            e.FillRectangle(Brushes.Yellow, new Rectangle(50 + j * (size) + 1, 50 + i * (size) + 1, size - 1, size - 1));
                        }
                        if (map[i, j] == 4)
                        {
                            e.FillRectangle(Brushes.Green, new Rectangle(50 + j * (size) + 1, 50 + i * (size) + 1, size - 1, size - 1));
                        }
                        if (map[i, j] == 5)
                        {
                            e.FillRectangle(Brushes.MidnightBlue, new Rectangle(50 + j * (size) + 1, 50 + i * (size) + 1, size - 1, size - 1));
                        }
                        if (map[i, j] == 6)
                        {
                            e.FillRectangle(Brushes.Blue, new Rectangle(50 + j * (size) + 1, 50 + i * (size) + 1, size - 1, size - 1));
                        }
                        if (map[i, j] == 7)
                        {
                            e.FillRectangle(Brushes.Purple, new Rectangle(50 + j * (size) + 1, 50 + i * (size) + 1, size - 1, size - 1));
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                
            }
        }

        //отрисовка клеток карты
        public void DrawGrid(Graphics g)
        {
            try
            {
                for (int i = 0; i <= 16; i++)
                {
                    g.DrawLine(Pens.Black, new Point(50, 50 + i * size), new Point(50 + mapWidth * size, 50 + i * size));
                }
                for (int i = 0; i <= mapWidth; i++)
                {
                    g.DrawLine(Pens.Black, new Point(50 + i * size, 50), new Point(50 + i * size, 50 + 16 * size));
                }
            }
            catch(Exception e)
            {
                Close();
            }
        }

        //получение новых данных матрицы с сервера
        public void GetMapFromServer()
        {
            try
            {
                while (true)
                {
                    //получение матрицы поля
                    int readBufferLength = getMapFromServerStream.Read(readBuffer, 0, readBuffer.Length);
                    messageFromClient = Encoding.Unicode.GetString(readBuffer, 0, readBufferLength);

                    map = ToIntArray(messageFromClient);
                    Thread.Sleep(200);
                }

            }
            catch (Exception e)
            {
                getMapFromServerStream.Close();
            }
        }

        //метод для отрисовки карты в отдельном потоке   
        private void DrawFullMap()
        {
            Graphics drawMapGraphics = this.CreateGraphics();
            while (true)
            {
                DrawGrid(drawMapGraphics);
                DrawMap(map, drawMapGraphics);
                Thread.Sleep(300);
                Invalidate();
                DrawGrid(drawMapGraphics);
            }
        }

        //преобразование входной строки матрицы в двумерную матрицу
        public static int[,] ToIntArray(string inputString)
        {
            int[,] resultArrayInt = new int[16, mapWidth];
            string[] resultArrayLines = inputString.Split(new char[] { '/' });
            string[] resultArrayElements;

            for (int y = 0; y < 16; y++)
            {
                resultArrayElements = resultArrayLines[y].Split(new char[] { ' ' });
                for (int x = 0; x < mapWidth; x++)
                {
                    resultArrayInt[y, x] = Convert.ToInt32(resultArrayElements[x]);
                }
            }
            return resultArrayInt;
        }

        private void OnClose(object sender, FormClosingEventArgs e)
        {
            Dispose();
            Close();
        }
    }
}
