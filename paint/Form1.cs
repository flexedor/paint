using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace paint
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            frec();
        }
      
        Bitmap bmp;
        Color color = Color.Black;
        List<Point> lp = new List<Point>();
        float roz = 4;


        private void Form1_Load(object sender, EventArgs e)
        {
            bmp = new Bitmap(pictureBox1.Width, pictureBox1.Height);
           
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                lp.Add(new Point(e.X, e.Y));
                fsend(e.X, e.Y);
                Graphics g = Graphics.FromImage(bmp);
                if (lp.Count()>1)
                {
                    g.DrawLines
                       
                        (new Pen(color, roz), lp.ToArray());
                    
                }
                pictureBox1.Image = bmp;
            }
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            lp.Clear();
        }

        private void colorToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (colorDialog1.ShowDialog() == DialogResult.OK)
                color = colorDialog1.Color;

        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            roz = Convert.ToSingle(numericUpDown1.Value);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            pictureBox1.Image = null;
            bmp = null;
            bmp = new Bitmap(pictureBox1.Width, pictureBox1.Height);

        }
        
  
        private void saveFileDialog1_FileOk(object sender, CancelEventArgs e)
        {

        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {

        }
        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {

            try
            {
                saveFileDialog1.Filter = "(bmp)|*.bmp|(jpeg)|*.jpg";
                DialogResult dr = saveFileDialog1.ShowDialog();
                if (dr == DialogResult.OK)
                {
                    bmp.Save(saveFileDialog1.FileName);
                };
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void openToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            try
            {
                openFileDialog1.Filter = "(bmp)|*.bmp|(jpeg)|*.jpg|(png)|*.png";
                DialogResult dr1 = openFileDialog1.ShowDialog();
                if (dr1 == DialogResult.OK)
                {
                    bmp = new Bitmap(openFileDialog1.FileName);
                    pictureBox1.Image = bmp;

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

          
        }

        private void horizontToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int sr;
            int f = 0;
            int k = 0;
            for (int x = 0; x < bmp.Width; x++)
                for (int y = 0; y < bmp.Height - 10; y++)
                {
                    //int sr=((bmp.GetPixel(x, y)).R+(bmp.GetPixel(x, y)).G+ (bmp.GetPixel(x, y)).B)/3;


                    int r = (bmp.GetPixel(x, y)).R;
                    int g = (bmp.GetPixel(x, y)).G;
                    int b = (bmp.GetPixel(x, y)).B;
                    sr = (r + g + b);

                    int r1 = (bmp.GetPixel(x, y + 1)).R;
                    int g1 = (bmp.GetPixel(x, y + 1)).G;
                    int b1 = (bmp.GetPixel(x, y + 1)).B;
                    int sr1 = (r1 + g1 + b1);
                    if ((Math.Abs(r - r1) >= 1 || Math.Abs(b - b1) >= 1 || Math.Abs(b - b1) >= 1) && (r1 <= 165 && g1 <= 175 && b1 <= 165))
                    {
                        int[] ac = new int[10];
                        int[] bc = new int[10];
                        int[] cc = new int[10];
                        for (int i = 0; i < 10; ++i)
                        {
                            ac[i] = (bmp.GetPixel(x, y + i)).R;
                            bc[i] = (bmp.GetPixel(x, y + i)).G;
                            cc[i] = (bmp.GetPixel(x, y + i)).B;
                            if ((Math.Abs(r - ac[i]) >= 60 || Math.Abs(b - bc[i]) >= 5 || Math.Abs(b - cc[i]) >= 15) && (ac[i] <= 165 && bc[i] <= 165 && cc[i] <= 150) && bc[i] >= cc[i] && ac[i] <= 160)
                            {
                                k++;
                            }
                        }
                        if (k >= 8)
                        {
                            if (lp.Count < 2) lp.Add(new Point(x, y));
                            else
                            {
                                Point p2 = new Point(x + 1, y);
                                Graphics gr = Graphics.FromImage(bmp);
                                gr.DrawLine(new Pen(Color.Red, 3), lp.First(), p2);
                                gr.DrawLine(new Pen(Color.Red, 3), lp.First(), lp.Last());

                                lp.Clear();
                                lp.Add(new Point(x, y));
                            }
                            f++;
                        }
                    }
                    if (f == 1)
                    {
                        k = 0; f = 0; break;
                    }


                }
            pictureBox1.Image = bmp;
            lp.Clear();
        }
        void fsend(int x, int y)
        {

            byte[] bytCommand = new byte[] { };
            UdpClient udpClient = new UdpClient();
            udpClient.Connect(IPAddress.Parse("10.0.0.212"), 8000);
            bytCommand = Encoding.ASCII.GetBytes("#(" + x + "," + y + ")");
            udpClient.Send(bytCommand, bytCommand.Length);
            pictureBox1.CreateGraphics().FillEllipse(Brushes.Black, x - 2, y - 2, 4, 4);


        }


        void frec()
        {
            CheckForIllegalCrossThreadCalls = false;

            byte[] bytCommand = new byte[] { };
            var receivingUdpClient = new System.Net.Sockets.UdpClient(8003);

            Thread th = new Thread(() => {
                IPEndPoint iep = new System.Net.IPEndPoint(System.Net.IPAddress.Any, 0);
                while (true)
                {
                    byte[] receiveBytes = receivingUdpClient.Receive(ref iep);

                    string str = System.Text.Encoding.ASCII.GetString(receiveBytes);

                    int x = Convert.ToInt32(str.Replace("#", "").Replace("(", "").Replace(")", "").Split(',')[0]);
                    int y = Convert.ToInt32(str.Replace("#", "").Replace("(", "").Replace(")", "").Split(',')[1]);

                    pictureBox1.CreateGraphics().DrawEllipse(new Pen(Brushes.Black, 4), x - 2, y - 2, 4, 4);

                }

            });
            th.IsBackground = true;
            th.Start();

        }
     
    }
}
