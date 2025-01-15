using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Threading;

namespace WindowsFormsApp2
{
    public partial class Form2 : Form
    {
        private TcpClient client1, client2;
        public Form2(bool isHost, string ip1, string ip2)
        {
            InitializeComponent();
            if (isHost)
            {
                flag = 0;
                client1 = new TcpClient();
                try
                {
                    client1.Connect(ip1, 36000);
                    client2 = new TcpClient();
                    if (client1.Connected)
                    {
                        try
                        {
                            client2.Connect(ip2, 36000);
                            MessageBox.Show("Connected to Server");
                        }
                        catch
                        {
                            MessageBox.Show("Server not found");
                            client2 = null;
                            System.Windows.Forms.Application.Exit();
                        }
                    }
                }
                catch
                {
                    MessageBox.Show("Server not found");
                    client1 = null;
                    System.Windows.Forms.Application.Exit();
                }
            }
            else
            {
                flag = 1;
                client1 = new TcpClient();
                try
                {
                    client1.Connect(ip2, 36000);
                    client2 = new TcpClient();
                    if(client1.Connected)
                    {
                        try
                        {
                            client2.Connect(ip1, 36000);
                            MessageBox.Show("Connected to Server");
                        }
                        catch
                        {
                            MessageBox.Show("Server not found");
                            client2 = null;
                            System.Windows.Forms.Application.Exit();
                        }
                    }
                }
                catch
                {
                    MessageBox.Show("Server not found");
                    client1 = null;
                    System.Windows.Forms.Application.Exit();
                }
            }
        }

        private int flag;
        bool up, down;
        int score1 = 0, score2 = 0;
        private void Form2_Load(object sender, EventArgs e)
        {
            canvas = this.CreateGraphics();
            timer1.Interval = 1000 / 60;
            timer1.Start();


        }

        //player
        float gx = 50, gy = 225;//綠色檔板中心位置
        //enemy
        float bx = 660, by = 225;//藍色檔板中心位置

        float speed = 8;

        bool inbound_up = true, inbound_down = true;
        bool touchOn = false;
        bool winlose = false;

        string a, b;

       public void getform1(string player1,string player2) {
            a = player1;
            b = player2;

        }
        private void update()
        {

            limitblock_green();             //擋板範圍

            limitblock_blue();             //擋板範圍

            if (flag == 1)
            {
                moveblock_green();              //player 左

                moveblock_blue();               //player  右
            }

            else
            {
                moveblock_blue();              //player 右

                moveblock_green();               //player 左
            }

            ballblock_green();              //球根擋板的反彈

            ballblock_blue();              //球根擋板的反彈

            moveball();                     //enemy 球的移動

            ballslide();                    //球的邊界反彈

            get_score();       //得分
           
            touch();
        }
        float ey_now;
        private void get_score()
        {
            //碰到左邊邊界=>玩家2得分
            if (ex<=30 && ey >= 85 && ey <= 435 && touchOn==false)
            {
                touchOn = true;
                score2++;
                ey_now = ey;
                label2.Text = "得分:" + score2;

                //回到起始位置
                ex = 360;
                ey = 210;

                //贏家先攻
                if (espeedx<0) {
                    espeedx = -espeedx;
                    espeedy = -espeedy;
                }
            }

            //碰到右邊邊界=>玩家1得分
            if (ex >= 690 && ey >= 85 && ey <= 435 && touchOn == false)
            {
                touchOn = true;
                score1++;
                ey_now = ey;
                label1.Text = "得分:" + score1;

                //回到起始位置
                ex = 360;
                ey = 210;

                ////贏家先攻
                if (espeedx > 0)
                {
                    espeedx = -espeedx;
                    espeedy = -espeedy;
                }
            }

            //先得到10分的是最終贏家
            if (score1==10)
            {
                timer1.Enabled = false;
            }
            if (score2 == 10)
            {
                timer1.Enabled = false;
            }
        }
        private void touch()
        {
            if (touchOn == true && ey-ey_now != 0)
            {
                touchOn = false;
            }
        }
        private void ballblock_green()                              //***bug*** 擋板上下移動會卡球
        {



            if ((Math.Abs(gx - ex) <= 30) && ((ey + er) >= (gy - 30)) && ((ey - er) <= (gy + 30)))
            {
                espeedx = -espeedx;

            }
            if ((Math.Abs(gy - ey) <= 50) && ((ex + er) > (gx - 10)) && ((ex - er) < (gx + 10)))
            {
                espeedx = -espeedx;
                espeedy = -espeedy;

                if ((ey - gy) > 0)
                {

                    gy = ey - 50 - 9;
                    ey += speed;
                    ballslide();
                    moveball();


                    ballslide();
                    limitblock_green();

                }
                else
                {
                    gy = ey + 50 + 9;
                    ey -= speed;
                    ballslide();
                    limitblock_green();

                }
                moveball();
                ballslide();
                moveball();
                ballslide();
                moveball();
                ballslide();
            }


        }

        private void ballblock_blue()                              //***bug*** 擋板上下移動會卡球
        {



            if ((Math.Abs(bx - ex) <= 30) && ((ey + er) >= (by - 30)) && ((ey - er) <= (by + 30)))
            {
                espeedx = -espeedx;

            }
            if ((Math.Abs(by - ey) <= 50) && ((ex + er) > (bx - 10)) && ((ex - er) < (bx + 10)))
            {
                espeedx = -espeedx;
                espeedy = -espeedy;

                if ((ey - by) > 0)
                {

                    by = ey - 50 - 9;
                    ey += speed;
                    ballslide();
                    moveball();


                    ballslide();
                    limitblock_blue();

                }
                else
                {
                    by = ey + 50 + 9;
                    ey -= speed;
                    ballslide();
                    limitblock_blue();

                }
                moveball();
                ballslide();
                moveball();
                ballslide();
                moveball();
                ballslide();
            }


        }
        bool inbound_upb = true, inbound_downb = true;
        private void limitblock_blue()
        {
            if (by <= 40)
            {
                inbound_upb = false;
                by = 40;
            }
            else
                inbound_upb = true;

            if (by >= 380)
            {
                inbound_downb = false;
                by = 380;
            }
            else
                inbound_downb = true;
        }
        private void limitblock_green()
        {
            if (gy <= 40)
            {
                inbound_up = false;
                gy = 40;
            }
            else
                inbound_up = true;

            if (gy >= 380)
            {
                inbound_down = false;
                gy = 380;
            }
            else
                inbound_down = true;
        }

        private void moveblock_blue()
        {
            if (flag == 1)
            {
                if (up && inbound_upb)
                {
                    by -= speed;
                    NetworkStream nwstream = client1.GetStream();
                    byte[] sendcoordinate = ASCIIEncoding.ASCII.GetBytes(Convert.ToString(by));
                    nwstream.Write(sendcoordinate);
                }

                if (down && inbound_downb)
                {
                    by += speed;
                    NetworkStream nwstream = client1.GetStream();
                    byte[] sendcoordinate = ASCIIEncoding.ASCII.GetBytes(Convert.ToString(by));
                    nwstream.Write(sendcoordinate);
                }
            }
            else
                ThreadPool.QueueUserWorkItem(receive);
        }
        
        private void moveblock_green()
        {
            if(flag == 0)
            {
                if (up && inbound_up)
                {
                    gy -= speed;
                    NetworkStream nwstream = client1.GetStream();
                    byte[] sendcoordinate = ASCIIEncoding.ASCII.GetBytes(Convert.ToString(gy));
                    nwstream.Write(sendcoordinate);
                } 
                    
                if (down && inbound_down)
                {
                    gy += speed;
                    NetworkStream nwstream = client1.GetStream();
                    byte[] sendcoordinate = ASCIIEncoding.ASCII.GetBytes(Convert.ToString(gy));
                    nwstream.Write(sendcoordinate);
                }
            }
            else
                ThreadPool.QueueUserWorkItem(receive);
        }
        private void receive(object state)
        {
            NetworkStream stream = client2.GetStream();
            Byte[] data = new Byte[1024];
            String responseData = String.Empty;
            Int32 bytes;
            bytes = stream.Read(data, 0, data.Length);
            if (bytes > 0)
            {
                responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
                if (flag == 1)
                    gy = Int32.Parse(responseData);
                else
                    by = Int32.Parse(responseData);
            }
            Thread.Sleep(10);
        }
        private void moveball()
        {
            ex += espeedx;
            ey += espeedy;
        }
        private void gameloop(object sender, EventArgs e)
        {
            update();
            Refresh();
            if (score1 == 10)
            {
                MessageBox.Show("玩家" + a + "獲勝");
            }
            if (score2 == 10)
            {
                MessageBox.Show("玩家"+b+"獲勝");
            }
        }

        float ex = 360, ey = 220, er = 20;//er球的半徑,ex ey為球的起始位置

        float espeedx = -5, espeedy = -5;  //球的往x,y的速度



        private void ballslide()
        {
            if ((ex == 30 && ey == 30) || (ex == 30 && ey == 390) || (ex == 690 && ey == 30) || (ex == 690 && ey == 390))
            {
                espeedx = -espeedx;
                espeedy = -espeedy;
            }
            else if ((ex - er) <= 10 || (ex + er) >= 710)
                espeedx = -espeedx;
            else if ((ey - er) <= 10 || (ey + er) >= 410)
                espeedy = -espeedy;
            else;

        }
       

        Graphics canvas;


        private void Form2_Paint(object sender, PaintEventArgs e)
        {
            Brush bushg = new SolidBrush(Color.Green);
            Brush bushb = new SolidBrush(Color.Blue);
            Brush bushy = new SolidBrush(Color.Yellow);
            Brush bushr = new SolidBrush(Color.Red);
            canvas.DrawRectangle(Pens.Red, 10, 10, 700, 400);
            canvas.DrawLine(Pens.Black, 360, 10, 360, 410);
            canvas.FillRectangle(bushb, (bx - 10), (by - 30), 20, 60);            //     藍檔板最左上位置
            canvas.FillRectangle(bushg, gx - 10, gy - 30, 20, 60);                      //玩家 綠檔板最左上位置
            canvas.FillEllipse(bushy, ex - er, ey - er, er * 2, er * 2);        //圓球
            canvas.FillRectangle(bushr, 5, 85, 5, 250);
            canvas.FillRectangle(bushr, 710, 85, 5, 250);
        }
        float rx = 10;//長方形半徑(x方向)
        float ry = 30;//長方形半徑(y方向)

        private void Form2_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Up:
                    up = false;
                    break;
                case Keys.Down:
                    down = false;
                    break;

            }
            //            MessageBox.Show(e.KeyCode.ToString());
        }

        private void Form2_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Up:
                    up = true;

                    break;
                case Keys.Down:
                    down = true;
                    break;
            }
            //           MessageBox.Show(e.KeyCode.ToString());
        }
    }
}
