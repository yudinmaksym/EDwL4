using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EDwL4
{
    public partial class Form1 : Form
    {
        int i = 0;
        int count = 0;
        public Form1()
        {
            InitializeComponent();

        }

        private void button1_Click(object sender, EventArgs e)
        {
            string urlAddress = textBox1.Text;

            Uri myUri = new Uri(urlAddress);
            var ip = Dns.GetHostAddresses(myUri.Host)[0];
            string[] row = { urlAddress + "-"+ ip};
            richTextBox1.AppendText(urlAddress + "-" + ip);
            richTextBox1.AppendText(System.Environment.NewLine);

            int nrounds = Convert.ToInt32(textBox2.Text);


            GetAllLinks(urlAddress,Convert.ToString(ip),nrounds);


        }
        public void GetAllLinks(string urlAddress, string ip, int nrounds)
        {
            
            if (count < nrounds)
            {
                count++;
                string[] strok = File.ReadAllLines("out.txt");
                if (strok.Length == 0)
                {

                }
                else
                {
                    urlAddress = Regex.Replace(strok[i], @"\s([^>]*)", "");
                    Uri myUri = new Uri(urlAddress);
                    var newip = Dns.GetHostAddresses(myUri.Host)[0];
                    ip = Convert.ToString(newip);
                    i += 2;
                }

                Match m;
                string HRefPattern = @"\b(?:https?|ftp):\/\/[a-z0-9-+&@#\/%?=~_|!:,.;]*[a-z0-9-+&@#\/%=~_|]";
                //@"(http|ftp|https):\/\/([\w\-_]+(?:(?:\.[\w\-_]+)+))([\w\-\.,@?^=%&amp;:/~\+#]*[\w\-\@?^=%&amp;/~\+#])?"

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(urlAddress);
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    Stream receiveStream = response.GetResponseStream();
                    StreamReader readStream = null;

                    if (response.CharacterSet == null)
                    {
                        readStream = new StreamReader(receiveStream);
                    }
                    else
                    {
                        readStream = new StreamReader(receiveStream, Encoding.GetEncoding("Windows-1250"));
                    }

                    string data = readStream.ReadToEnd();
                    data = Regex.Replace(data, @"<meta([^>]*)>", "");
                    //data = Regex.Replace(data, @"<style>([^>]*)<\/style>", "");
                    //data = Regex.Replace(data, @"<script>([^>]*)<\/script>", "");



                    try
                    {
                        m = Regex.Match(data, HRefPattern,
                                        RegexOptions.IgnoreCase | RegexOptions.Compiled,
                                        TimeSpan.FromSeconds(1));
                        while (m.Success)
                        {
                            Uri myUri = new Uri(m.Value);
                            var ip2 = Dns.GetHostAddresses(myUri.Host)[0];
                            richTextBox1.AppendText(m.Value + " " + ip2);
                            richTextBox1.AppendText(System.Environment.NewLine);
                            m = m.NextMatch();

                            if (ip == Convert.ToString(ip2))
                            {
                                string appendText = m.Value + " " + ip2 + '\n';
                                File.AppendAllText("in.txt", appendText, Encoding.UTF8);
                                File.AppendAllText("in.txt", System.Environment.NewLine, Encoding.UTF8);
                            }
                            else
                            {
                                string appendText = m.Value + " " + ip2 + '\n';
                                File.AppendAllText("out.txt", appendText, Encoding.UTF8);
                                File.AppendAllText("out.txt", System.Environment.NewLine, Encoding.UTF8);
                            }
                        }
                    }
                    catch (RegexMatchTimeoutException)
                    {
                        Console.WriteLine("The matching operation timed out.");
                    }
                     GetAllLinks(urlAddress, ip, nrounds);
                }
            }
            return;
            
            
        }

    }
}
