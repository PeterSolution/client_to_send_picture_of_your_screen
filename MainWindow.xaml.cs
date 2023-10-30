using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ss_klient
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        
        string iplocal= (Dns.GetHostEntry(Dns.GetHostName())).AddressList[0].ToString();

        int commandport = 1978;
        IPAddress serwerip = IPAddress.Parse("192.168.0.62");
        int portdata = 25000;
        Bitmap bm;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            

            Task.Run(async () =>
            {
                while (true)
                {
                    TcpClient client1 = new TcpClient();
                    NetworkStream ns = client1.GetStream();
                    Byte[] bytes = new Byte[5];
                    int read = ns.Read(bytes, 0, bytes.Length);
                    String S = Encoding.ASCII.GetString(bytes);
                    String message = Encoding.ASCII.GetString(bytes);
                    if (message == "##S##")
                    {
                        bm = executebm();
                        MemoryStream ms = new MemoryStream();
                        bm.Save(ms, ImageFormat.Jpeg);
                        byte[] bmbyte = ms.GetBuffer();
                        ms.Close();
                        try
                        {
                            TcpClient klient = new TcpClient(serwerip.ToString(), portdata);
                            NetworkStream ns = klient.GetStream();
                            using (BinaryWriter bw = new BinaryWriter(ms))
                            {
                                bw.Write((int)bmbyte.Length);
                                bw.Write(bmbyte);
                            }


                        }
                        catch
                        {

                        }
                    }
                }
                
            });
            wyslijwiadomoscUDP(iplocal + "HI");

        }


        void wyslijwiadomoscUDP(string wiadomosc)
        {
            UdpClient udp = new UdpClient();
            byte[] bufor = Encoding.ASCII.GetBytes(wiadomosc);
            udp.Send(bufor,bufor.Length);
            udp.Close();
        }
        

        void settext(string tekst)
        {
            list.Items.Add(tekst);
        }

        Bitmap executebm()
        {
            Bitmap bitmap;
            bitmap = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height, System.Drawing.Imaging.PixelFormat.Format32bppRgb);
            Graphics sc = Graphics.FromImage(bitmap);
            sc.CopyFromScreen(Screen.PrimaryScreen.Bounds.X, Screen.PrimaryScreen.Bounds.Y, 0, 0, Screen.PrimaryScreen.Bounds.Size, CopyPixelOperation.SourceCopy);
            return bitmap;
        }

        private void endofconnection(object sender, System.ComponentModel.CancelEventArgs e)
        {
            wyslijwiadomoscUDP(iplocal + "BYE");
        }
    }
}
