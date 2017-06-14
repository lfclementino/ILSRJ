using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Collections;
using System.Net;
using System.Web;
using System.Xml;
using System.Security.Permissions;
using System.Text.RegularExpressions;
using System.ServiceModel.Syndication;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Xml.Linq;
using System.Runtime.InteropServices;

namespace Transito
{
    public partial class Form1 : Form
    {
        private ArrayList Lugares;

        private string Data()
        {
            string tt = DateTime.Now.ToString("ddd").ToUpper() + " " + DateTime.Now.ToString("HH") + ":" + DateTime.Now.ToString("mm");
            return tt;
        }

        public Form1()
        {
            InitializeComponent();

            Lugares = new ArrayList();
        }

        private int Contador;

        private void button1_Click(object sender, EventArgs e)
        {
            Contador = 0;
            Lista.Items.Clear();
            Verifica.Enabled = false;
            Controle.Enabled = false;
            Controle.Enabled = true;

            Http.Navigate("http://www.google.com");
        }

        private void LerTransitoPrefeitura()
        {
            try
            {
                Verifica.Enabled = false;

                StringReader sr = new StringReader(Http.Document.Body.InnerHtml);

                Lugar NovoLugar = null;

                string input;
                bool Item = false;
                int i = 0;

                numt = 0;

                while ((input = sr.ReadLine()) != null)
                {
                    if (Item)
                    {
                        i++;
                        if (i == 1)
                        {
                            NovoLugar.Rua = input.Replace("<TD>", "").Replace("</TD>", "");
                        }

                        if (i == 2)
                        {
                            string[] t1 = input.Split('/');
                            string[] t2 = t1[1].Split('.');
                            NovoLugar.Status = TrocaCor(t2[0].Replace("leg", "").Replace("Maior", ""));

                            VerificaRua(RemoveAcentos(NovoLugar.Rua.Trim()), NovoLugar.Status);

                            Item = false;
                        }
                    }

                    if (input.Contains("class=par") || input.Contains("class=impar"))
                    {
                        Item = true;
                        i = 0;
                        NovoLugar = new Lugar();
                    }
                }

                Contador = 2;
                Http.Navigate("about:blank");
                Controle.Enabled = false;
                this.Text = "Transito Prefeitura - " + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString();

                System.Threading.Thread.Sleep(3000);
                Controle.Enabled = false;
                LerTransitoMapLink();

            }
            catch
            {
                //Verifica.Enabled = cbLigar.Checked;
                LerTransitoMapLink();
            }
        }

        private void LerTransitoMapLink()
        {
            try
            {
                Controle.Enabled = false;

                numt = 0;

                ArrayList IFs = new ArrayList();
                IFs.Add(new InfoTransito("Al Sao Boaventura", "http://maplink.uol.com.br/v2/corredores/Rio-de-Janeiro/Al.-Sao-Boa-Aventura-389-RJ.htm"));
                IFs.Add(new InfoTransito("Av Feliciano Sodre", "http://maplink.uol.com.br/v2/corredores/Rio-de-Janeiro/Av-Feliciano-Sodre-381-RJ.htm"));
                IFs.Add(new InfoTransito("Av Brasil", "http://maplink.uol.com.br/v2/corredores/Rio-de-Janeiro/Av.-Brasil-316-RJ.htm"));
                IFs.Add(new InfoTransito("Av Francisco Bicalho", "http://maplink.uol.com.br/v2/corredores/Rio-de-Janeiro/Av.-Francisco-Bicalho-331-RJ.htm"));
                IFs.Add(new InfoTransito("Linha Amarela", "http://maplink.uol.com.br/v2/corredores/Rio-de-Janeiro/Linha-Amarela-383-RJ.htm"));
                IFs.Add(new InfoTransito("Linha Vermelha", "http://maplink.uol.com.br/v2/corredores/Rio-de-Janeiro/Linha-Vermelha-384-RJ.htm"));
                IFs.Add(new InfoTransito("Mario Ribeiro", "http://maplink.uol.com.br/v2/corredores/Rio-de-Janeiro/Mario-Ribeiro-342-RJ.htm"));
                IFs.Add(new InfoTransito("Mrq Parana e Roberto Silveira", "http://maplink.uol.com.br/v2/corredores/Rio-de-Janeiro/Marques-do-Parana-e-Roberto-Silveira-Jansem-de-Melo-382-RJ.htm"));
                IFs.Add(new InfoTransito("Ponte", "http://maplink.uol.com.br/v2/corredores/Rio-de-Janeiro/Ponte-Rio-Niteroi-386-RJ.htm"));
                IFs.Add(new InfoTransito("Tn Zuzu Angel", "http://maplink.uol.com.br/v2/corredores/Rio-de-Janeiro/Tunel-Zuzu-Angel-392-RJ.htm"));
                IFs.Add(new InfoTransito("Av das Americas", "http://maplink.uol.com.br/v2/corredores/Rio-de-Janeiro/Av.-das-Americas-301-RJ.htm"));
                IFs.Add(new InfoTransito("Tn Reboucas", "http://maplink.uol.com.br/v2/corredores/Rio-de-Janeiro/Tunel-Reboucas-387-RJ.htm"));
                IFs.Add(new InfoTransito("Tn Sta Barbara", "http://maplink.uol.com.br/v2/corredores/Rio-de-Janeiro/Tunel-Santa-Barbara-388-RJ.htm"));
                IFs.Add(new InfoTransito("Radial Oeste", "http://maplink.uol.com.br/v2/corredores/Rio-de-Janeiro/Radial-Oeste-354-RJ.htm"));

                foreach (InfoTransito info in IFs)
                {
                    HttpWebRequest myWebRequest = (HttpWebRequest)HttpWebRequest.Create(info.Url);
                    myWebRequest.Method = "GET";
                    HttpWebResponse myWebResponse = (HttpWebResponse)myWebRequest.GetResponse();
                    StreamReader myWebSource = new StreamReader(myWebResponse.GetResponseStream(), Encoding.UTF8);
                    string myPageSource = myWebSource.ReadToEnd();
                    myWebResponse.Close();

                    StringReader sr = new StringReader(myPageSource);

                    string Sentido = null;
                    string Altura = null;
                    string Status = null;
                    string input;
                    bool Item = false;
                    int i = 0;


                    while ((input = sr.ReadLine()) != null)
                    {
                        if (Item)
                        {
                            i++;
                            if (i == 1)
                            {
                                Sentido = RemoveHTML(input).Trim();
                            }

                            if (i == 2)
                            {
                                string[] t1 = Regex.Split(input, "<tr>");
                                //string[] t2 = t1[1].Split('.');
                                t1[0] = null;
                                foreach (string item in t1)
                                {
                                    if (!String.IsNullOrEmpty(item))
                                    {
                                        string[] t2 = Regex.Split(item, "<td>");
                                        string[] t3 = Regex.Split(t2[1], "</a></td><td");
                                        Altura = RemoveHTML(t3[0]).Trim();
                                        Status = RemoveHTML("<" + t3[1]).Replace("Ver mapa", "").Trim();
                                        VerificaRua(RemoveAcentos("MAPLINK: " + info.Nome + " sent " + Sentido + " altura " + Altura), Status);
                                    }
                                }

                                Item = false;
                            }
                        }

                        if (input.Contains("<!-- abre dados do corredor"))
                        {
                            Item = true;
                            i = 0;
                        }
                    }
                }
                this.Text = "Transito MAPLINK/APONTADOR - " + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString();

                System.Threading.Thread.Sleep(3000);
                LerJB();

            }
            catch
            {
                LerJB();
            }
        }

        private StringBuilder sb;
        private StreamReader sr;
        private StreamWriter sw;

        private int numt;

        private void VerificaRua(string Rua, string Status)
        {
            try
            {
                if (Status.Trim() != "" && numt < 2)
                {
                    sr = new StreamReader("ruas.txt");
                    sb = new StringBuilder();

                    bool Achou = false;
                    string input = null;
                    while ((input = sr.ReadLine()) != null)
                    {
                        string[] t1 = input.Split(';');
                        if (t1[0] == Rua)
                        {
                            Achou = true;
                            if (t1[1] != Status)
                            {
                                string imagem = null;
                                if (!Rua.Contains("MAPLINK"))
                                {
                                    imagem = Camera(Rua, Status);
                                }
                                else
                                {
                                    imagem = "";
                                }
                                Lista.Items.Add(Data() + " " + Rua + " - " + Status + imagem);
                                if (Status != "Bom" && Status != "Livre" && cbPostar.Checked)
                                {
                                    //if (cbPostar.Checked && numt < 4)
                                    //{
                                    Log(RemoveAcentos(Data()) + " " + Rua + " - " + Status + imagem);
                                    EscreverTweet(RemoveAcentos(Data()) + " " + Rua + " - " + Status + imagem);
                                    numt++;
                                    System.Threading.Thread.Sleep(10000);
                                    //}
                                }
                                sb.AppendLine(Rua + ";" + Status);
                                //}                                else
                                //{
                                //    sb.AppendLine(Rua + ";" + t1[1]);
                                //}

                            }
                            else
                            {
                                sb.AppendLine(input);
                            }
                        }
                        else
                        {
                            sb.AppendLine(input);
                        }
                    }
                    if (!Achou)
                    {
                        sb.AppendLine(Rua + ";" + Status);
                    }
                    sr.Close();
                    sr.Dispose();

                    sw = new StreamWriter("ruas.txt", false);
                    sw.Write(sb.ToString());
                    sw.Close();
                }
            }
            catch
            {
                sr.Close();
                sr.Dispose();
                sw.Close();
                sw.Dispose();
                sw = new StreamWriter("ruas.txt", false);
                sw.Write(sb.ToString());
                sw.Close();
                sw.Dispose();
                //Verifica.Enabled = cbLigar.Checked;
            }
        }

        private WebBrowser wb;
        private WebClient wc;
        private TwitPic tw;

        private string Camera(string Rua, string Status)
        {
            try
            {
                if (Status != "Via Fechada" && Status != "Bom")
                {
                    //for(int i = 1;i<=94;i++)
                    bool achou = false;
                    int i = 1;
                    while (i <= 94 && !achou)
                    {
                        wb = new WebBrowser();
                        wb.Navigate("http://transito.rio.rj.gov.br/iframeTrechosCameraBranca.cfm?codigo=" + i.ToString());

                        while (wb.ReadyState != WebBrowserReadyState.Complete)
                        {
                            Application.DoEvents();
                        }

                        if (RemoveAcentos(wb.Document.Body.InnerText).ToUpper().Contains(Rua.ToUpper()))
                        {
                            achou = true;
                            wb.Dispose();
                            wb = null;
                        }
                        else
                        {
                            i++;
                            wb.Dispose();
                            wb = null;
                        }
                    }

                    if (i <= 94)
                    {
                        wc = new WebClient();
                        tw = new TwitPic();
                        string url = tw.UploadPhoto(wc.DownloadData("http://transito.rio.rj.gov.br/imagens1/" + i + ".jpg"), "image/jpeg", "", "camera.bmp", "ILSRJ", "iiqsto00").ToString();
                        wc.Dispose();
                        wc = null;
                        tw = null;
                        return " " + url;
                    }
                    else
                    {
                        return "";
                    }
                }
                else
                {
                    return "";
                }
            }
            catch
            {
                wc.Dispose();
                wb.Dispose();
                tw = null;
                wc = null;
                wb = null;
                return "";
            }
        }

        oAuthTwitter oAuth;

        private byte[] BmpToBytes_MemStream(Bitmap bmp)
        {
            MemoryStream ms = new MemoryStream();
            // Save to memory using the Jpeg format
            bmp.Save(ms, ImageFormat.Jpeg);

            // read to end
            byte[] bmpBytes = ms.GetBuffer();
            bmp.Dispose();
            ms.Close();

            return bmpBytes;
        }

        private void EscreverTweet(string tweet)
        {
            try
            {
                oAuth = new oAuthTwitter();

                string url = "";
                string xml = "";

                url = "http://twitter.com/statuses/update.xml";
                xml = oAuth.oAuthWebRequest(oAuthTwitter.Method.POST, url, "status=" + HttpUtility.UrlEncode(tweet));
            }
            catch
            {
            }
        }

        public static void PostTweet(string username, string password, string tweet)
        {
            try
            {
                // encode the username/password
                string user = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(username + ":" + password));
                // determine what we want to upload as a status
                byte[] bytes = System.Text.Encoding.ASCII.GetBytes("status=" + tweet);
                // connect with the update page
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://twitter.com/statuses/update.xml");
                // set the method to POST
                request.Method = "POST";
                request.ServicePoint.Expect100Continue = false; // thanks to argodev for this recent change!
                // set the authorisation levels
                request.Headers.Add("Authorization", "Basic " + user);
                request.ContentType = "application/x-www-form-urlencoded";
                // set the length of the content
                request.ContentLength = bytes.Length;

                // set up the stream
                Stream reqStream = request.GetRequestStream();
                // write to the stream
                reqStream.Write(bytes, 0, bytes.Length);
                // close the stream
                reqStream.Close();
            }
            catch {/* DO NOTHING */}
        }

        private string TrocaCor(string cor)
        {
            switch (cor)
            {
                case "Verde":
                    return "Bom";
                case "Amarelo":
                    return "Intenso";
                case "Laranja":
                    return "Lento";
                case "Vermelho":
                    return "Congestionado";
                case "Cinza":
                    return "Via Fechada";
                default:
                    return " ";
            }
        }

        private string RemoveStatus(string Msg)
        {
            if (Msg.Length > 0)
            {
                int p = Msg.LastIndexOf(" ") + 1;
                return Msg.Substring(0, p);
            }

            return Msg;
        }

        private string RemoveAcentos(string palavra)
        {
            string palavraSemAcento = null;
            string caracterComAcento = "áàãâäéèêëíìîïóòõôöúùûüçÁÀÃÂÄÉÈÊËÍÌÎÏÓÒÕÖÔÚÙÛÜÇñÑ";
            string caracterSemAcento = "aaaaaeeeeiiiiooooouuuucAAAAAEEEEIIIIOOOOOUUUUCnN";

            for (int i = 0; i < palavra.Length; i++)
            {
                if (caracterComAcento.IndexOf(Convert.ToChar(palavra.Substring(i, 1))) >= 0)
                {
                    int car = caracterComAcento.IndexOf(Convert.ToChar(palavra.Substring(i, 1)));
                    palavraSemAcento += caracterSemAcento.Substring(car, 1);
                }
                else
                {
                    palavraSemAcento += palavra.Substring(i, 1);
                }
            }

            return palavraSemAcento;
        }

        private bool loop;

        private void Http_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            string url = Http.Document.Url.AbsoluteUri;
            if (url.Contains("google.com"))
            {

                Http.Navigate("http://transito.rio.rj.gov.br/entrada.cfm?pagina=6&testeMapaTodo=1");
            }
            else if (url.Contains("transito.rio.rj"))
            {
                if (Contador == 1)
                {
                    if (url.Substring(0, 5) != "about" && !loop)
                    {
                        loop = true;
                        Controle.Enabled = false;
                        LerTransitoPrefeitura();
                    }
                }
                else if (Contador == 0)
                {
                    Http.Refresh();
                }
                Contador++;
            }
            //else if (url.Contains("ask.com"))
            //{
            //    ParaHttp.Enabled = false;
            //    ParaHttp.Enabled = true;
            //    Http.Navigate("http://jbonline.terra.com.br/editorias/rio/transito/temporeal/index.asp");
            //}
            //else if (url.Contains("jbonline.terra"))
            //{
            //    if (Contador == 1)
            //    {

            //        if (url.Substring(0, 5) != "about")
            //        {
            //            LerJB();
            //        }
            //    }
            //    else if (Contador == 0)
            //    {
            //        Http.Refresh();
            //    }
            //    Contador++;
            //}
            //else if (url.Contains("altavista.com"))
            //{
            //    ParaHttp.Enabled = false;
            //    ParaHttp.Enabled = true;
            //    Http.Navigate("http://extra.globo.com/plantao/pTransito.asp");
            //}
            //else if (url.Contains("extra.globo.com"))
            //{
            //    if (Contador == 1)
            //    {

            //        if (url.Substring(0, 5) != "about")
            //        {
            //            LerEXTRA();
            //        }
            //    }
            //    else if (Contador == 0)
            //    {
            //        Http.Refresh();
            //    }
            //    Contador++;
            //}
            //else if (url.Contains("floc.net"))
            //{
            //    ParaHttp.Enabled = false;
            //    ParaHttp.Enabled = true;
            //    Http.Navigate("http://maplink.uol.com.br/v2/NoticiasLista.aspx?idcatnot=8");
            //}
            //else if (url.Contains("maplink.uol"))
            //{
            //    if (Contador == 1)
            //    {

            //        if (url.Substring(0, 5) != "about")
            //        {
            //            LerMAPLINK();
            //        }
            //    }
            //    else if (Contador == 0)
            //    {
            //        Http.Refresh();
            //    }
            //    Contador++;
            //}
        }

        private void Verifica_Tick(object sender, EventArgs e)
        {
            btAtualizar.PerformClick();
        }

        private void cbLigar_CheckedChanged(object sender, EventArgs e)
        {
            Verifica.Enabled = cbLigar.Checked;
        }

        private string RemoveHTML(string text)
        {
            return System.Text.RegularExpressions.Regex.Replace(text, @"<(.|\n)*?>", " ");
        }

        private string RemoveHora(string text)
        {
            return text.Substring(text.IndexOf(':') + 3);
        }

        private void LerJB()
        {
            try
            {
                string Novas = null;
                string NovasLink = null;

                string url = "http://www.jb.com.br/transito/";

                HttpWebRequest myWebRequest = (HttpWebRequest)HttpWebRequest.Create(url);
                myWebRequest.Method = "GET";
                HttpWebResponse myWebResponse = (HttpWebResponse)myWebRequest.GetResponse();
                StreamReader myWebSource = new StreamReader(myWebResponse.GetResponseStream(), Encoding.UTF8);
                string myPageSource = myWebSource.ReadToEnd();
                myWebResponse.Close();

                StringReader sr = new StringReader(myPageSource);
                string input;
                string temp = null;
                string temp2 = null;
                bool foi = false;

                while ((input = sr.ReadLine()) != null && !foi)
                {
                    if (input.ToLower().Contains("&uacute;ltimas not&iacute;cias"))
                    {
                        temp = input;
                        break;
                    }
                }

                if (temp.Length > 0)
                {
                    string[] Noticias = Regex.Split(temp, "<figure>");
                    string[] Sites = temp.Split('"');

                    Noticias[0] = "";

                    foreach (string Noticia in Noticias)
                    {
                        if (Noticia.Length > 0)
                        {
                            temp2 = RemoveAcentos(HttpUtility.HtmlDecode(RemoveHTML(Noticia)).Replace("\t", "")).Trim();
                            temp2 = temp2.Substring(0, temp2.IndexOf("   ")).Trim();
                            if (!String.IsNullOrEmpty(temp2))
                                Novas = Novas + ";" + temp2;
                        }
                    }

                    foreach (string Site in Sites)
                    {
                        if (Site.Contains("/transito/noticias"))
                        {
                            NovasLink = NovasLink + ";" + "http://www.jb.com.br" + Site;
                        }
                    }
                }

                Verificatxt("jb.txt", "JB: ", Novas.Trim(';'), NovasLink.Trim(';'));

                this.Text = "Transito JB - " + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString();

                System.Threading.Thread.Sleep(3000);
                LerJB2();
            }
            catch
            {
                //Verifica.Enabled = cbLigar.Checked;
                LerJB2();
            }
        }

        private void LerJBB()
        {
            try
            {
                string Novas = null;
                string NovasLink = null;

                string url = "http://jbonline.terra.com.br/editorias/rio/transito/temporeal/index.asp";

                HttpWebRequest myWebRequest = (HttpWebRequest)HttpWebRequest.Create(url);
                myWebRequest.Method = "GET";
                HttpWebResponse myWebResponse = (HttpWebResponse)myWebRequest.GetResponse();
                StreamReader myWebSource = new StreamReader(myWebResponse.GetResponseStream(), Encoding.Default);
                string myPageSource = myWebSource.ReadToEnd();
                myWebResponse.Close();

                StringReader sr = new StringReader(myPageSource);
                string input;
                string temp;
                string temp2;
                bool Item = false;
                bool foi = false;
                int i = 0;

                while ((input = sr.ReadLine()) != null && !foi)
                {
                    if (input.Length > 0)
                    {
                        if (Item)
                        {
                            i++;
                            if (input.Contains("</ul>"))
                            {
                                Item = false;
                                foi = true;
                            }
                            else if (i >= 2)
                            {
                                if (input.Trim(' ') != "")
                                {
                                    temp2 = PegaSiteJB(input.Split('"')[1]);
                                    NovasLink = NovasLink + ";" + temp2;
                                    temp = RemoveEmailsESites(RemoveHTML(RemoveAcentos(input))).Trim();
                                    if (temp.Length > 5)
                                    {
                                        Novas = Novas + ";" + temp.Trim(' ');
                                    }
                                }
                            }

                        }
                    }

                    if (input.Contains("Tempo real - TRÂNSITO</"))
                    {
                        Item = true;
                        i = 0;
                    }
                }

                Verificatxt("jb.txt", "JB: ", Novas.Trim(';'), NovasLink.Trim(';'));

                this.Text = "Transito JB - " + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString();

                System.Threading.Thread.Sleep(3000);
                LerJB2();
            }
            catch
            {
                //Verifica.Enabled = cbLigar.Checked;
                LerJB2();
            }
        }

        private string PegaSiteJB(string p)
        {
            string[] t1 = p.Split('/');
            string t2 = null;
            foreach (string t3 in t1)
            {
                t2 = t3;
            }

            return "http://jbonline.terra.com.br/pextra/" + DateTime.Now.Year.ToString("0000") + "/" + DateTime.Now.Month.ToString("00") + "/" + DateTime.Now.Day.ToString("00") + "/" + t2.Substring(1);
        }

        private void LerJB2()
        {
            try
            {
                //string Novas = null;
                //string NovasLink = null;

                //string url = "http://jbonline.terra.com.br/pextra/rio.asp";

                //HttpWebRequest myWebRequest = (HttpWebRequest)HttpWebRequest.Create(url);
                //myWebRequest.Method = "GET";
                //HttpWebResponse myWebResponse = (HttpWebResponse)myWebRequest.GetResponse();
                //StreamReader myWebSource = new StreamReader(myWebResponse.GetResponseStream(), Encoding.Default);
                //string myPageSource = myWebSource.ReadToEnd();
                //myWebResponse.Close();

                //StringReader sr = new StringReader(myPageSource);
                //string input;
                //string temp;
                //string temp2;
                //bool Item = false;
                //bool foi = false;
                //int i = 0;

                //StreamWriter ssw = new StreamWriter("jb2.txt", false);
                //ssw.Write("www");
                //ssw.Close();

                //while ((input = sr.ReadLine()) != null && !foi)
                //{
                //    if (input.Length > 0)
                //    {
                //        if (Item)
                //        {
                //            i++;
                //            if (input.Contains("</ul>"))
                //            {
                //                Item = false;
                //                foi = true;
                //            }
                //            else if (i >= 2)
                //            {
                //                if (input.Trim(' ') != "" && input.ToLower().Contains("radares") && DateTime.Now.Hour <= 12 && DateTime.Now.Hour >= 6 && (DateTime.Now.Hour % 2) == 0 && DateTime.Now.Minute <= 5)
                //                {
                //                    temp2 = input.Split('"')[1];
                //                    NovasLink = NovasLink + ";" + temp2;
                //                    temp = RemoveHora(RemoveHTML(RemoveAcentos(input))).Trim();
                //                    if (temp.Length > 5)
                //                    {
                //                        Novas = Novas + ";" + temp.Trim(' ');
                //                    }
                //                    foi = true;
                //                }
                //            }

                //        }
                //    }

                //    if (input.Contains("Tempo real - Rio</"))
                //    {
                //        Item = true;
                //        i = 0;
                //    }
                //}

                //if (!String.IsNullOrEmpty(Novas))
                //{
                //    Verificatxt("jb2.txt", "JB: ", Novas.Trim(';'), NovasLink.Trim(';'));
                //}

                this.Text = "Transito JB2 - " + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString();

                System.Threading.Thread.Sleep(3000);
                LerJB3();
            }
            catch
            {
                //Verifica.Enabled = cbLigar.Checked;
                LerJB3();
            }
        }

        private void LerJB3()
        {
            try
            {
                string Novas = null;
                string NovasLink = null;

                string url = "http://jbonline.terra.com.br/pextra/rio.asp";

                HttpWebRequest myWebRequest = (HttpWebRequest)HttpWebRequest.Create(url);
                myWebRequest.Method = "GET";
                HttpWebResponse myWebResponse = (HttpWebResponse)myWebRequest.GetResponse();
                StreamReader myWebSource = new StreamReader(myWebResponse.GetResponseStream(), Encoding.Default);
                string myPageSource = myWebSource.ReadToEnd();
                myWebResponse.Close();

                StringReader sr = new StringReader(myPageSource);
                string input;
                string temp;
                string temp2;
                bool Item = false;
                bool foi = false;
                int i = 0;

                while ((input = sr.ReadLine()) != null && !foi)
                {
                    if (input.Length > 0)
                    {
                        if (Item)
                        {
                            i++;
                            if (input.Contains("</ul>"))
                            {
                                Item = false;
                                foi = true;
                            }
                            else if (i >= 2)
                            {
                                if (input.Trim(' ') != "")
                                {
                                    if (input.ToLower().Contains("aeroporto") || input.ToLower().Contains("detro") || input.ToLower().Contains("detran") || input.ToLower().Contains("detro") || input.ToLower().Contains("santos dumont") || input.ToLower().Contains("galeão") || input.ToLower().Contains("previsão"))
                                    {
                                        temp2 = input.Split('"')[1];
                                        NovasLink = NovasLink + ";" + temp2;
                                        temp = RemoveHora(RemoveHTML(RemoveAcentos(input))).Trim();
                                        if (temp.Length > 5)
                                        {
                                            Novas = Novas + ";" + temp.Trim(' ');
                                        }
                                    }
                                }
                            }

                        }
                    }

                    if (input.Contains("Tempo real - Rio</"))
                    {
                        Item = true;
                        i = 0;
                    }
                }

                if (!String.IsNullOrEmpty(Novas))
                {
                    Verificatxt("jb3.txt", "JB: ", Novas.Trim(';'), NovasLink.Trim(';'));
                }

                this.Text = "Transito JB3 - " + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString();

                System.Threading.Thread.Sleep(3000);
                //LerILSRJ();
                LerEXTRA();
            }
            catch
            {
                //Verifica.Enabled = cbLigar.Checked;
                //LerILSRJ();
                LerEXTRA();
            }
        }

        private void LerEXTRA()
        {
            try
            {
                string Novas = null;
                string NovasLink = null;

                string url = "http://extra.globo.com/plantao/pTransito.asp";

                HttpWebRequest myWebRequest = (HttpWebRequest)HttpWebRequest.Create(url);
                myWebRequest.Method = "GET";
                HttpWebResponse myWebResponse = (HttpWebResponse)myWebRequest.GetResponse();
                StreamReader myWebSource = new StreamReader(myWebResponse.GetResponseStream(), Encoding.Default);
                string myPageSource = myWebSource.ReadToEnd();
                myWebResponse.Close();

                StringReader sr = new StringReader(myPageSource);
                string input;
                string temp;
                string temp2;

                bool Item = false;
                bool foi = false;
                int i = 0;

                while ((input = sr.ReadLine()) != null && !foi)
                {
                    if (input.Length > 0)
                    {
                        if (Item)
                        {
                            i++;
                            if (input.Contains("</ul>"))
                            {
                                Item = false;
                                foi = true;
                            }
                            else if (i >= 3)
                            {
                                if (input != "")
                                {
                                    temp2 = input.Split('"')[3];
                                    if (temp2.Length > 5)
                                    {
                                        NovasLink = NovasLink + ";http://extra.globo.com" + temp2.ToLower();
                                    }
                                    temp = RemoveHTML(RemoveAcentos(input)).Trim();
                                    if (temp.Length > 5)
                                    {
                                        Novas = Novas + ";" + temp.Substring(5, temp.Length - 5).Trim(' ');
                                    }
                                }
                            }

                        }

                        if (input.Contains("<h4>RIO - TRÂNSITO</h4>"))
                        {
                            Item = true;
                            i = 0;
                        }
                    }
                }

                Novas = Novas.Trim(';', ' ');
                Verificatxt("extra.txt", "EXTRA: ", Novas, NovasLink.Trim(';'));

                this.Text = "Transito EXTRA - " + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString();

                System.Threading.Thread.Sleep(3000);
                LerMAPLINK();
            }
            catch
            {
                //Verifica.Enabled = cbLigar.Checked;
                LerMAPLINK();
            }
        }

        private void LerMAPLINK()
        {
            try
            {
                string Novas = null;
                string NovasLink = null;

                string url = "http://maplink.uol.com.br/v2/NoticiasLista.aspx?idcatnot=8";

                HttpWebRequest myWebRequest = (HttpWebRequest)HttpWebRequest.Create(url);
                myWebRequest.Method = "GET";
                HttpWebResponse myWebResponse = (HttpWebResponse)myWebRequest.GetResponse();
                StreamReader myWebSource = new StreamReader(myWebResponse.GetResponseStream(), Encoding.UTF8);
                string myPageSource = myWebSource.ReadToEnd();
                myWebResponse.Close();

                StringReader sr = new StringReader(myPageSource);
                string input;
                bool foi = false;

                while ((input = sr.ReadLine()) != null && !foi)
                {
                    if (input.Length > 0)
                    {
                        if (input.ToUpper().Contains("DIVLISTACOMPLETANOTICIASANTESBANNER"))
                        {
                            string t1 = RemoveHTML(RemoveAcentos(input)).Replace("Rio de Janeiro", "").Trim(' ', '"');
                            string t1l = RemoveAcentos(input).Replace("Rio de Janeiro", "").Trim(' ', '"');

                            string[] t1ll = t1l.Split(';');
                            string[] t4 = t1.Split(';');
                            t4[0] = "";
                            t1ll[0] = "";
                            foreach (string t2 in t4)
                            {
                                if (t2.Length > 0)
                                {
                                    if (t2.Contains("&"))
                                    {
                                        Novas = Novas + ";" + t2.Trim(' ').Substring(0, t2.Trim(' ').Length - 12).Trim(' ');
                                    }
                                    else
                                    {
                                        Novas = Novas + ";" + t2.Trim(' ');
                                    }
                                }
                            }

                            foreach (string t1lll in t1ll)
                            {
                                if (t1lll.Length > 0)
                                {
                                    NovasLink = NovasLink + ";http://maplink.uol.com.br" + t1lll.Split('\'')[5].ToLower();
                                }
                            }
                        }

                        if (input.ToUpper().Contains("</BODY"))
                        {
                            foi = true;
                        }
                    }
                }

                Novas = Novas.Trim(';', ' ');
                Verificatxt("maplink.txt", "MAPLINK: ", Novas, NovasLink.Trim(';'));

                this.Text = "Transito MAPLINK - " + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString();

                System.Threading.Thread.Sleep(3000);
                LerMAPLINK2();

            }
            catch
            {
                //Verifica.Enabled = cbLigar.Checked;
                LerMAPLINK2();
            }
        }

        private bool VerificaHorario(string Data)
        {
            DateTime convertedDate = DateTime.Parse(Data);
            DateTime dt = TimeZone.CurrentTimeZone.ToLocalTime(convertedDate);

            DateTime Agora = DateTime.Now;
            TimeSpan ts = Agora - dt;
            bool Resp = true;

            int Horas = (ts.Days * 24) + ts.Hours;

            if (Horas < 1)
            {
                Resp = true;
            }
            else
            {
                Resp = false;
            }

            return Resp;
        }

        private void LerRSS()
        {
            try
            {
                ArrayList RSSs = new ArrayList();
                RSSs.Add(new InfoTransito("PONTE: ", "http://www.ponte.com.br/xml/toflash/cfg/boletins.rss.axd"));
                RSSs.Add(new InfoTransito("NOVADUTRA: ", "http://www.novadutra.com.br/xml/toflash/cfg/boletins.rss.axd"));

                string Novas = null;
                string NovasLink = null;

                foreach (InfoTransito RSS in RSSs)
                {
                    Novas = null;
                    NovasLink = null;

                    rssReader = new XmlTextReader(RSS.Url);
                    rssDoc = new XmlDocument();
                    rssDoc.Load(rssReader);

                    for (int i = 0; i < rssDoc.ChildNodes.Count; i++)
                    {
                        if (rssDoc.ChildNodes[i].Name == "rss")
                        {
                            nodeRss = rssDoc.ChildNodes[i];
                        }
                    }

                    for (int i = 0; i < nodeRss.ChildNodes.Count; i++)
                    {
                        if (nodeRss.ChildNodes[i].Name == "channel")
                        {
                            nodeChannel = nodeRss.ChildNodes[i];
                        }
                    }

                    for (int i = (nodeChannel.ChildNodes.Count - 1); i >= 0; i--)
                    {
                        if (nodeChannel.ChildNodes[i].Name == "item")
                        {
                            nodeItem = nodeChannel.ChildNodes[i];
                            Noticia = RemoveBetween(RemoveAcentos(RemoveEmailsESites(nodeItem["title"].InnerText)).Replace(";", ""), '(', ')').Trim(' ', ':');
                            if (VerificaHorario(nodeItem["pubDate"].InnerText) && Noticia.Length > 1)
                            {
                                Novas = Novas + ";" + RemoveAcentos(nodeItem["title"].InnerText.Trim(' '));
                                NovasLink = NovasLink + ";" + nodeItem["link"].InnerText;
                            }
                        }
                    }

                    if (!String.IsNullOrEmpty(Novas))
                    {
                        Verificatxt("rss\\rss_" + RSS.Nome.Trim(' ', ':').Replace(" ", "_") + ".txt", RSS.Nome, Novas.Trim(';'), NovasLink.Trim(';'), false);
                    }

                    this.Text = "Transito " + RSS.Nome.Trim(' ', ':') + " - " + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString();
                }
                //Verificatxt("leiseca.txt", "@LeiSecaRJ ", Novas.Trim(';'), "");

                this.Text = "Transito - " + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString();

                System.Threading.Thread.Sleep(3000);
                LerCRT();
            }
            catch
            {
                LerCRT();
            }
        }

        private void LerCCR()
        {
            try
            {
                ArrayList Lugares = new ArrayList();
                //Lugares.Add("http://www.rodoviadoslagos.com.br/concessionaria/rodoviaaovivo/index.cfm?objectId=52D0EFF9-1321-0A28-C5B595D9950177F8");
                //Lugares.Add("http://www.rodoviadoslagos.com.br/concessionaria/rodoviaaovivo/index.cfm?objectId=52D06638-1321-0A28-C54B2E8033DA495D");
                Lugares.Add("http://www.ponte.com.br/concessionaria/rodoviaaovivo/index.cfm?objectId=D4BBB6CD-1321-0A28-C5C19925BA3CE151");
                Lugares.Add("http://www.ponte.com.br/concessionaria/rodoviaaovivo/index.cfm?objectId=D4B847E5-1321-0A28-C57FEB65A978FC95");
                Lugares.Add("http://www.novadutra.com.br/concessionaria/rodoviaaovivo/index.cfm?objectId=F4F064AD-1321-0A9F-1BD1B181D147BE91");
                Lugares.Add("http://www.novadutra.com.br/concessionaria/rodoviaaovivo/index.cfm?objectId=F4F0147A-1321-0A9F-1B61006D52BFCFD7");
                Lugares.Add("http://www.novadutra.com.br/concessionaria/rodoviaaovivo/index.cfm?objectId=F4E8E59C-1321-0A9F-1B663C237191992A");
                Lugares.Add("http://www.novadutra.com.br/concessionaria/rodoviaaovivo/index.cfm?objectId=F4E8A5D3-1321-0A9F-1B119049F9955392");

                string Novas = null;

                foreach (string url in Lugares)
                {
                    HttpWebRequest myWebRequest = (HttpWebRequest)HttpWebRequest.Create(url);
                    myWebRequest.Method = "GET";
                    try
                    {
                        HttpWebResponse myWebResponse = (HttpWebResponse)myWebRequest.GetResponse();
                        StreamReader myWebSource = new StreamReader(myWebResponse.GetResponseStream(), Encoding.Default);
                        string myPageSource = myWebSource.ReadToEnd();
                        myWebResponse.Close();

                        StringReader sr = new StringReader(myPageSource);
                        string input;
                        bool Item = false;
                        bool foi = false;
                        int i = 0;
                        string NotTemp = "";

                        while ((input = sr.ReadLine()) != null && !foi)
                        {
                            if (input.Length > 0)
                            {
                                if (Item)
                                {
                                    i++;
                                    if (input.Contains("bac_lin_titulo.gif") || input.ToUpper().Contains("ÚLTIMA ATUALIZAÇÃO") || input.ToLower().Contains("script"))
                                    {
                                        Item = false;
                                        //foi = true;
                                    }
                                    else if (i > 10)
                                    {
                                        if (input.Trim() != "")
                                        {
                                            NotTemp = NotTemp.Trim() + " " + RemoveHTML(RemoveAcentos(HttpUtility.HtmlDecode(input.Replace("\t", "")))).Trim();

                                        }
                                    }

                                }
                            }

                            if (input.ToLower().Contains("txt_destaque txt_menor"))
                            {
                                NotTemp = RemoveHTML(RemoveAcentos(HttpUtility.HtmlDecode(input.Replace("\t", "")))) + " ";
                                Item = true;
                                i = 0;
                            }
                        }
                        NotTemp = NotTemp.Replace("   ", " ").Replace("  ", " ").Replace("Visibilidade", ". Visibilidade").Replace(" . ", ".").Replace(" : ", ":").Replace(" , ", ",").Replace(" Trafego:", "").Replace("Rodovia Presidente Dutra", "Dutra").Replace("Ponte Rio Niteroi", "Ponte").Replace("Rio de Janeiro", "Rio").Replace(" Obs:", "").Replace("trecho", "sent").Trim();

                        Novas = Novas + ";" + NotTemp;
                    }
                    catch (WebException webex)
                    {

                    }
                }

                Verificatxt("ccr.txt", "CCR: ", Novas.Trim(';'), "", false);
                this.Text = "Transito CCR - " + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString();

                System.Threading.Thread.Sleep(3000);
                LerCRT();
            }
            catch (WebException webex)
            {
                //MessageBox.Show(webex.Status.ToString());
                LerCRT();
            }
            catch
            {
                LerCRT();
            }
        }

        private void LerCRT()
        {
            try
            {
                ArrayList Lugares = new ArrayList();
                Lugares.Add("http://www.crt.com.br/condicoes.asp");

                string Novas = null;

                foreach (string url in Lugares)
                {
                    HttpWebRequest myWebRequest = (HttpWebRequest)HttpWebRequest.Create(url);
                    myWebRequest.Method = "GET";
                    HttpWebResponse myWebResponse = (HttpWebResponse)myWebRequest.GetResponse();
                    StreamReader myWebSource = new StreamReader(myWebResponse.GetResponseStream(), Encoding.Default);
                    string myPageSource = myWebSource.ReadToEnd();
                    myWebResponse.Close();

                    StringReader sr = new StringReader(myPageSource);
                    string input;
                    bool Item = false;
                    bool foi = false;
                    int i = 0;
                    string NotTemp = "";

                    while ((input = sr.ReadLine()) != null && !foi)
                    {
                        if (input.Length > 0)
                        {
                            if (Item)
                            {
                                i++;
                                if (input.ToUpper().Contains("*CRT DESEJA"))
                                {
                                    Item = false;
                                }
                                else if (i > 0)
                                {
                                    if (input.Trim() != "")
                                    {
                                        NotTemp = NotTemp.Trim() + " " + RemoveHTML(RemoveAcentos(HttpUtility.HtmlDecode(input.Replace("\t", "")))).Trim();

                                    }
                                }

                            }
                        }

                        if (input.ToLower().Contains("***telefones de emer"))
                        {
                            Item = true;
                            i = 0;
                        }
                    }
                    NotTemp = NotTemp.Replace("   ", " ").Replace("  ", " ").Replace(" . ", ".").Replace(" : ", ":").Replace(" , ", ",").Replace("Rio de Janeiro", "Rio").Trim();

                    foreach (string Noti in NotTemp.Split('*'))
                    {
                        if (!String.IsNullOrEmpty(Noti))
                            Novas = Novas + ";" + Noti.Trim();
                    }
                }

                Verificatxt("crt.txt", "CRT: ", Novas.Trim(';'), "", false);
                this.Text = "Transito CRT - " + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString();

                System.Threading.Thread.Sleep(3000);
                LerCONCER();
            }
            catch
            {
                LerCONCER();
            }
        }

        private void LerG1()
        {
            try
            {
                string Novas = null;
                string NovasLink = null;

                string url = "http://busca2.globo.com/Busca/globo/?query=transito&ordenacao=descending&offset=1&xargs=&formato=&requisitor=globo&aba=todos&filtro=&on=false&formatos=65995%2C60158%2C5123%2C420%2C290%2C0%2C0%2C0%2C0%2C0%2C4";

                //HttpWebRequest myWebRequest = (HttpWebRequest)HttpWebRequest.Create(url);
                //myWebRequest.Method = "GET";
                //HttpWebResponse myWebResponse = (HttpWebResponse)myWebRequest.GetResponse();
                //StreamReader myWebSource = new StreamReader(myWebResponse.GetResponseStream(), Encoding.UTF8);
                //string myPageSource = myWebSource.ReadToEnd();
                //myWebResponse.Close();

                WebBrowser webr = new WebBrowser();
                webr.ScriptErrorsSuppressed = true;
                webr.Navigate(url);

                while (webr.ReadyState != WebBrowserReadyState.Complete)
                {
                    Application.DoEvents();
                }

                string myPageSource = webr.Document.Body.InnerHtml;

                webr.Dispose();
                webr = null;

                StringReader sr = new StringReader(myPageSource);
                string input;
                string temp;
                string temp2;
                bool Item = false;
                int i = 0;

                while ((input = sr.ReadLine()) != null)
                {
                    if (input.Trim().Length > 0)
                    {
                        if (Item)
                        {
                            i++;
                            if (input.Contains("-- /resultado --"))
                            {
                                Item = false;
                                //foi = true;
                            }
                            else if (i == 1)
                            {
                                temp2 = input.Split('"')[3];
                                if (temp2.Contains("/rio"))
                                {
                                    NovasLink = NovasLink + ";" + temp2;
                                    Novas = Novas + ";" + RemoveAcentos(RemoveHTML(input.Trim())).Trim();
                                }
                                else
                                {
                                    Item = false;
                                }
                            }
                        }
                    }

                    if (input.ToLower().Contains("-- resultado --"))
                    {
                        Item = true;
                        i = 0;
                    }
                }

                Verificatxt("g1.txt", "G1: ", Novas.Trim(';'), NovasLink.Trim(';'), true);

                this.Text = "Transito G1 - " + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString();

                System.Threading.Thread.Sleep(3000);
                //LerG2();
                LerNITEROI();
            }
            catch (WebException ex)
            {
                //LerG2();
                LerNITEROI();
            }
            catch
            {
                //LerG2();
                LerNITEROI();
            }
        }

        private void LerODIA()
        {
            try
            {
                string Novas = null;
                string NovasLink = null;

                string url = "http://odia.terra.com.br/portal/rio/html/2010/1/dois_carros_batem_na_linha_vermelha_59798.html";

                HttpWebRequest myWebRequest = (HttpWebRequest)HttpWebRequest.Create(url);
                myWebRequest.Method = "GET";
                HttpWebResponse myWebResponse = (HttpWebResponse)myWebRequest.GetResponse();
                StreamReader myWebSource = new StreamReader(myWebResponse.GetResponseStream(), Encoding.UTF8);
                string myPageSource = myWebSource.ReadToEnd();
                myWebResponse.Close();

                StringReader sr = new StringReader(myPageSource);
                string input;
                string temp = null;
                string temp2 = null;
                bool foi = false;

                while ((input = sr.ReadLine()) != null && !foi)
                {
                    if (input.ToLower().Contains("pontilhado_relacionadas.gif"))
                    {
                        temp = input;
                        break;
                    }
                }

                if (temp.Length > 0)
                {
                    string[] Noticias = Regex.Split(temp, "<p>");
                    string[] Sites = temp.Split('"');

                    foreach (string Noticia in Noticias)
                    {
                        temp2 = RemoveAcentos(HttpUtility.HtmlDecode(RemoveHTML(Noticia)).Replace("\t", "")).Trim();
                        if (!String.IsNullOrEmpty(temp2))
                            Novas = Novas + ";" + temp2;
                    }

                    foreach (string Site in Sites)
                    {
                        if (Site.Contains(".html"))
                        {
                            NovasLink = NovasLink + ";" + "http://odia.terra.com.br" + Site;
                        }
                    }
                }

                Verificatxt("odia.txt", "ODIA: ", Novas.Trim(';'), NovasLink.Trim(';'));

                this.Text = "Transito O Dia - " + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString();

                System.Threading.Thread.Sleep(3000);
                LerODIA2();
            }
            catch
            {
                LerODIA2();
            }
        }

        private void LerODIA2()
        {
            try
            {
                string Novas = null;
                string NovasLink = null;

                string url = "http://odia.terra.com.br/portal/rio/html/2010/11/choque_de_ordem_nas_praias_reboca_109_veiculos_e_multa_307_por_estacionamento_irregular_125981.html";

                HttpWebRequest myWebRequest = (HttpWebRequest)HttpWebRequest.Create(url);
                myWebRequest.Method = "GET";
                HttpWebResponse myWebResponse = (HttpWebResponse)myWebRequest.GetResponse();
                StreamReader myWebSource = new StreamReader(myWebResponse.GetResponseStream(), Encoding.UTF8);
                string myPageSource = myWebSource.ReadToEnd();
                myWebResponse.Close();

                StringReader sr = new StringReader(myPageSource);
                string input;
                string temp = null;
                string temp2 = null;
                bool foi = false;

                while ((input = sr.ReadLine()) != null && !foi)
                {
                    if (input.ToLower().Contains("pontilhado_relacionadas.gif"))
                    {
                        temp = input;
                        break;
                    }
                }

                if (temp.Length > 0)
                {
                    string[] Noticias = Regex.Split(temp, "<p>");
                    string[] Sites = temp.Split('"');

                    foreach (string Noticia in Noticias)
                    {
                        temp2 = RemoveAcentos(HttpUtility.HtmlDecode(RemoveHTML(Noticia)).Replace("\t", "")).Trim();
                        if (!String.IsNullOrEmpty(temp2))
                            Novas = Novas + ";" + temp2;
                    }

                    foreach (string Site in Sites)
                    {
                        if (Site.Contains(".html"))
                        {
                            NovasLink = NovasLink + ";" + "http://odia.terra.com.br" + Site;
                        }
                    }
                }

                Verificatxt("odia2.txt", "ODIA: ", Novas.Trim(';'), NovasLink.Trim(';'));

                this.Text = "Transito O Dia 2 - " + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString();

                System.Threading.Thread.Sleep(3000);
                LerG1();
            }
            catch
            {
                LerG1();
            }
        }

        private void LerCONCER()
        {
            try
            {
                ArrayList Lugares = new ArrayList();
                Lugares.Add("http://www.concer.com.br/Scripts/baixada.dll");
                Lugares.Add("http://www.concer.com.br/scripts/serra.dll");
                Lugares.Add("http://www.concer.com.br/scripts/itaipava.dll");

                string Novas = null;

                foreach (string url in Lugares)
                {

                    HttpWebRequest myWebRequest = (HttpWebRequest)HttpWebRequest.Create(url);
                    myWebRequest.Method = "GET";
                    HttpWebResponse myWebResponse = (HttpWebResponse)myWebRequest.GetResponse();
                    StreamReader myWebSource = new StreamReader(myWebResponse.GetResponseStream(), Encoding.Default);
                    string myPageSource = myWebSource.ReadToEnd();
                    myWebResponse.Close();

                    StringReader sr = new StringReader(myPageSource);
                    string input;
                    string temp = null;
                    string temp2 = null;
                    bool foi = false;

                    while ((input = sr.ReadLine()) != null && !foi)
                    {
                        if (input.ToLower().Contains("<font face=\"arial\" size=\"2\"><b>") || input.ToLower().Contains("<b><font face=\"arial\" size=\"2\">"))
                        {
                            temp = input;
                            break;
                        }
                    }

                    if (temp.Length > 0)
                    {
                        temp2 = RemoveAcentos(HttpUtility.HtmlDecode(RemoveHTML(temp)).Replace("\t", "").Trim());
                        string nott = temp2.Substring(temp2.IndexOf(":") + 1).Trim();
                        if (nott.ToLower().Contains("ultima atualizacao"))
                        {
                            Novas = Novas + ";" + url.Split('/')[4].ToUpper().Replace(".DLL", "") + " " + "O trafego esta normal Com tempo bom e visibilidade boa";
                        }
                        else
                        {
                            Novas = Novas + ";" + url.Split('/')[4].ToUpper().Replace(".DLL", "") + " " + nott;
                        }
                    }
                }

                Verificatxt("concer.txt", "CONCER: ", Novas.Trim(';'), "", false);

                this.Text = "Transito CONCER - " + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString();

                System.Threading.Thread.Sleep(3000);
                LerTERRA();
            }
            catch
            {
                LerTERRA();
            }
        }

        private void LerR7()
        {
            try
            {
                string Novas = null;
                string NovasLink = null;

                string url = "http://noticias.r7.com/transito/transito-em-tempo-real/uf/rj.html?UF=RJ";

                HttpWebRequest myWebRequest = (HttpWebRequest)HttpWebRequest.Create(url);
                myWebRequest.Method = "GET";
                HttpWebResponse myWebResponse = (HttpWebResponse)myWebRequest.GetResponse();
                StreamReader myWebSource = new StreamReader(myWebResponse.GetResponseStream(), Encoding.UTF8);
                string myPageSource = myWebSource.ReadToEnd();
                myWebResponse.Close();

                StringReader sr = new StringReader(myPageSource);
                string input;
                string temp = null;
                string temp2 = null;
                bool foi = false;

                while ((input = sr.ReadLine()) != null && !foi)
                {
                    if (input.ToLower().Contains("ultimas listavalores"))
                    {
                        temp = input;
                        break;
                    }
                }

                if (temp.Length > 0)
                {
                    string[] Noticias = Regex.Split(temp.Substring(temp.IndexOf("<li>")), "<li>");
                    string[] Sites = temp.Substring(temp.IndexOf("<li>")).Split('"');

                    foreach (string Noticia in Noticias)
                    {
                        if (!String.IsNullOrEmpty(Noticia))
                        {
                            temp2 = RemoveAcentos(HttpUtility.HtmlDecode(RemoveHTML(Noticia)).Replace("\t", "").Trim()).Substring(18).Replace(" << Anterior", "").Trim();
                            Novas = Novas + ";" + temp2;
                        }
                    }

                    foreach (string Site in Sites)
                    {
                        if (Site.Contains("</ul>"))
                            break;
                        if (Site.Contains(".html"))
                        {
                            NovasLink = NovasLink + ";" + Site;
                        }
                    }
                }

                Verificatxt("r7.txt", "R7: ", Novas.Trim(';'), NovasLink.Trim(';'));

                this.Text = "Transito R7 - " + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString();

                System.Threading.Thread.Sleep(3000);
                //LerCCR();
                //LerRSS();
                LerCRT();
            }
            catch
            {
                //LerRSS();
                LerCRT();
                //LerCCR();
            }
        }
        //private void LerR7()
        //{
        //    try
        //    {
        //        string Novas = null;
        //        string NovasLink = null;

        //        string url = "http://noticias.r7.com/transito/";

        //        HttpWebRequest myWebRequest = (HttpWebRequest)HttpWebRequest.Create(url);
        //        myWebRequest.Method = "GET";
        //        HttpWebResponse myWebResponse = (HttpWebResponse)myWebRequest.GetResponse();
        //        StreamReader myWebSource = new StreamReader(myWebResponse.GetResponseStream(), Encoding.UTF8);
        //        string myPageSource = myWebSource.ReadToEnd();
        //        myWebResponse.Close();

        //        StringReader sr = new StringReader(myPageSource);
        //        string input;
        //        string temp = null;
        //        string temp2 = null;
        //        bool foi = false;

        //        while ((input = sr.ReadLine()) != null && !foi)
        //        {
        //            if (input.ToLower().Contains("icn_rio.gif"))
        //            {
        //                temp = input;
        //                break;
        //            }
        //        }

        //        if (temp.Length > 0)
        //        {
        //            string NoticiaT = temp.Substring(temp.LastIndexOf("<h3>"));
        //            string[] Site = NoticiaT.Split('"');

        //            temp2 = RemoveAcentos(HttpUtility.HtmlDecode(RemoveHTML(NoticiaT)).Replace("\t", "").Trim());
        //            Novas = temp2;
        //            NovasLink = Site[1];
        //        }

        //        Verificatxt("r7.txt", "R7: ", Novas.Trim(';'), NovasLink.Trim(';'));

        //        this.Text = "Transito R7 - " + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString();

        //        System.Threading.Thread.Sleep(3000);
        //        LerTERRA();
        //    }
        //    catch
        //    {
        //        LerTERRA();
        //    }
        //}

        private void LerG2()
        {
            try
            {
                string Novas = null;
                string NovasLink = null;

                string url = "http://busca2.globo.com/Busca/g1/?query=chuva&ordenacao=descending&offset=1&xargs=&formato=&requisitor=g1&aba=todos&filtro=&on=false&formatos=48323%2C42961%2C5088%2C274%2C0%2C0%2C0%2C0%2C0%2C0%2C0";

                //HttpWebRequest myWebRequest = (HttpWebRequest)HttpWebRequest.Create(url);
                //myWebRequest.Method = "GET";
                //HttpWebResponse myWebResponse = (HttpWebResponse)myWebRequest.GetResponse();
                //StreamReader myWebSource = new StreamReader(myWebResponse.GetResponseStream(), Encoding.UTF8);
                //string myPageSource = myWebSource.ReadToEnd();
                //myWebResponse.Close();

                WebBrowser webr = new WebBrowser();
                webr.ScriptErrorsSuppressed = true;
                webr.Navigate(url);

                while (webr.ReadyState != WebBrowserReadyState.Complete)
                {
                    Application.DoEvents();
                }

                string myPageSource = webr.Document.Body.InnerHtml;

                webr.Dispose();
                webr = null;

                StringReader sr = new StringReader(myPageSource);
                string input;
                string temp;
                string temp2;
                bool Item = false;
                int i = 0;

                while ((input = sr.ReadLine()) != null)
                {
                    if (input.Trim().Length > 0)
                    {
                        if (Item)
                        {
                            i++;
                            if (input.Contains("-- /resultado --"))
                            {
                                Item = false;
                                //foi = true;
                            }
                            else if (i == 1)
                            {
                                temp2 = input.Split('"')[3];
                                if (temp2.Contains("/rio"))
                                {
                                    NovasLink = NovasLink + ";" + temp2;
                                    Novas = Novas + ";" + RemoveAcentos(RemoveHTML(input.Trim())).Trim();
                                }
                                else
                                {
                                    Item = false;
                                }
                            }
                        }
                    }

                    if (input.ToLower().Contains("-- resultado --"))
                    {
                        Item = true;
                        i = 0;
                    }
                }

                Verificatxt("g2.txt", "G1: ", Novas.Trim(';'), NovasLink.Trim(';'), true);

                this.Text = "Transito G2 - " + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString();

                System.Threading.Thread.Sleep(3000);
                LerNITEROI();
            }
            catch (WebException ex)
            {
                LerNITEROI();
            }
            catch
            {
                LerNITEROI();
            }
        }

        private void LerG3()
        {
            try
            {
                string Novas = null;
                string NovasLink = null;

                string url = "http://g1.globo.com/Noticias/0,,LTM0-5597-28650,00.html";

                HttpWebRequest myWebRequest = (HttpWebRequest)HttpWebRequest.Create(url);
                myWebRequest.Method = "GET";
                HttpWebResponse myWebResponse = (HttpWebResponse)myWebRequest.GetResponse();
                StreamReader myWebSource = new StreamReader(myWebResponse.GetResponseStream(), Encoding.UTF8);
                string myPageSource = myWebSource.ReadToEnd();
                myWebResponse.Close();

                StringReader sr = new StringReader(myPageSource);
                string input;
                string temp;
                string temp2;
                bool Item = false;
                bool foi = false;
                int i = 0;

                while ((input = sr.ReadLine()) != null && !foi)
                {
                    if (input.Length > 0)
                    {
                        if (Item)
                        {
                            i++;
                            if (input.Contains("</li>"))
                            {
                                Item = false;
                                //foi = true;
                            }
                            else if (i == 4)
                            {
                                if (input.Trim(' ') != "")
                                {
                                    temp2 = input.Split('"')[1];
                                    NovasLink = NovasLink + ";" + temp2;
                                    temp = RemoveHTML(RemoveAcentos(input)).Trim();
                                    if (temp.Length > 5)
                                    {
                                        Novas = Novas + ";" + temp.Trim(' ');
                                    }
                                }
                            }

                        }
                    }

                    if (input.ToLower().Contains("| rio de janeiro"))
                    {
                        Item = true;
                        i = 0;
                    }
                }

                Verificatxt("g3.txt", "G1: ", Novas.Trim(';'), NovasLink.Trim(';'));

                this.Text = "Transito G3 - " + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString();

                System.Threading.Thread.Sleep(3000);
                //LerLEISECA();
                LerG4();
            }
            catch
            {
                //Verifica.Enabled = cbLigar.Checked;
                LerG4();
            }
        }

        private void LerG4()
        {
            try
            {
                string Novas = null;
                string NovasLink = null;

                string url = "http://g1.globo.com/Noticias/0,,LTM0-5597-206,00.html";

                HttpWebRequest myWebRequest = (HttpWebRequest)HttpWebRequest.Create(url);
                myWebRequest.Method = "GET";
                HttpWebResponse myWebResponse = (HttpWebResponse)myWebRequest.GetResponse();
                StreamReader myWebSource = new StreamReader(myWebResponse.GetResponseStream(), Encoding.UTF8);
                string myPageSource = myWebSource.ReadToEnd();
                myWebResponse.Close();

                StringReader sr = new StringReader(myPageSource);
                string input;
                string temp;
                string temp2;
                bool Item = false;
                bool foi = false;
                int i = 0;

                while ((input = sr.ReadLine()) != null && !foi)
                {
                    if (input.Length > 0)
                    {
                        if (Item)
                        {
                            i++;
                            if (input.Contains("</li>"))
                            {
                                Item = false;
                                //foi = true;
                            }
                            else if (i == 4)
                            {
                                if (input.Trim(' ') != "")
                                {
                                    temp2 = input.Split('"')[1];
                                    NovasLink = NovasLink + ";" + temp2;
                                    temp = RemoveHTML(RemoveAcentos(input)).Trim();
                                    if (temp.Length > 5)
                                    {
                                        Novas = Novas + ";" + temp.Trim(' ');
                                    }
                                }
                            }

                        }
                    }

                    if (input.ToLower().Contains("| rio de janeiro"))
                    {
                        Item = true;
                        i = 0;
                    }
                }

                Verificatxt("g4.txt", "G1: ", Novas.Trim(';'), NovasLink.Trim(';'));

                this.Text = "Transito G4 - " + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString();

                System.Threading.Thread.Sleep(3000);
                LerG5();
            }
            catch
            {
                LerG5();
            }
        }

        private void LerG5()
        {
            try
            {
                string Novas = null;
                string NovasLink = null;

                string url = "http://g1.globo.com/Noticias/0,,LTM0-5597-12090,00.html";

                HttpWebRequest myWebRequest = (HttpWebRequest)HttpWebRequest.Create(url);
                myWebRequest.Method = "GET";
                HttpWebResponse myWebResponse = (HttpWebResponse)myWebRequest.GetResponse();
                StreamReader myWebSource = new StreamReader(myWebResponse.GetResponseStream(), Encoding.UTF8);
                string myPageSource = myWebSource.ReadToEnd();
                myWebResponse.Close();

                StringReader sr = new StringReader(myPageSource);
                string input;
                string temp;
                string temp2;
                bool Item = false;
                bool foi = false;
                int i = 0;

                while ((input = sr.ReadLine()) != null && !foi)
                {
                    if (input.Length > 0)
                    {
                        if (Item)
                        {
                            i++;
                            if (input.Contains("</li>"))
                            {
                                Item = false;
                                //foi = true;
                            }
                            else if (i == 4)
                            {
                                if (input.Trim(' ') != "")
                                {
                                    temp2 = input.Split('"')[1];
                                    NovasLink = NovasLink + ";" + temp2;
                                    temp = RemoveHTML(RemoveAcentos(input)).Trim();
                                    if (temp.Length > 5)
                                    {
                                        Novas = Novas + ";" + temp.Trim(' ');
                                    }
                                }
                            }

                        }
                    }

                    if (input.ToLower().Contains("| rio de janeiro"))
                    {
                        Item = true;
                        i = 0;
                    }
                }

                Verificatxt("g5.txt", "G1: ", Novas.Trim(';'), NovasLink.Trim(';'));

                this.Text = "Transito G5 - " + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString();

                System.Threading.Thread.Sleep(3000);
                LerNITEROI();
            }
            catch
            {
                LerNITEROI();
            }
        }

        private void LerNITEROI()
        {
            try
            {
                string Novas = null;
                string NovasLink = null;

                string url = "http://www.niteroi.rj.gov.br/novo/index.php";

                HttpWebRequest myWebRequest = (HttpWebRequest)HttpWebRequest.Create(url);
                myWebRequest.Method = "GET";
                HttpWebResponse myWebResponse = (HttpWebResponse)myWebRequest.GetResponse();
                StreamReader myWebSource = new StreamReader(myWebResponse.GetResponseStream(), Encoding.UTF8);
                string myPageSource = myWebSource.ReadToEnd();
                myWebResponse.Close();

                StringReader sr = new StringReader(myPageSource);
                string input = string.Empty;
                string temp = string.Empty;
                string temp2 = string.Empty;
                bool Item = false;
                bool foi = false;
                int i = 0;

                while ((input = sr.ReadLine()) != null && !foi)
                {
                    if (input.Length > 0)
                    {
                        if (Item)
                        {
                            i++;
                            if (input.Contains("</td>"))
                            {
                                Item = false;
                                //foi = true;
                            }
                            else if (i == 1)
                            {
                                if (input.Trim(' ') != "")
                                {
                                    temp2 = input.Split('"')[1];
                                }
                            }
                            else if (i == 2)
                            {
                                if (input.Trim(' ') != "")
                                {
                                    temp = RemoveHTML(RemoveAcentos(input)).Trim();
                                    if (temp.ToLower().Contains("boletim") || temp.ToLower().Contains("prefeit") || temp.ToLower().Contains("interdi") || temp.ToLower().Contains("transito") || temp.ToLower().Contains("engarrafa") || temp.ToLower().Contains("acidente"))
                                    {
                                        Novas = Novas + ";" + temp.Trim(' ');
                                        NovasLink = NovasLink + ";http://www.niteroi.rj.gov.br" + temp2;
                                    }
                                }
                            }

                        }
                    }

                    if (input.ToLower().Contains("contentheading"))
                    {
                        Item = true;
                        i = 0;
                    }
                }

                Verificatxt("niteroi.txt", "NITEROI: ", Novas.Trim(';'), NovasLink.Trim(';'));

                this.Text = "Transito NITEROI - " + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString();

                System.Threading.Thread.Sleep(3000);
                LerR7();
            }
            catch
            {
                LerR7();
            }
        }

        private void LerTERRA()
        {
            try
            {
                StreamReader sm = new StreamReader("terra.txt");
                string SM = sm.ReadLine();
                sm.Close();

                string Novas = null;
                string NovasLink = null;

                rssReader = new XmlTextReader("http://rss.terra.com.br/0,,EI11777,00.xml");
                rssDoc = new XmlDocument();
                rssDoc.Load(rssReader);

                for (int i = 0; i < rssDoc.ChildNodes.Count; i++)
                {
                    if (rssDoc.ChildNodes[i].Name == "rss")
                    {
                        nodeRss = rssDoc.ChildNodes[i];
                    }
                }

                for (int i = 0; i < nodeRss.ChildNodes.Count; i++)
                {
                    if (nodeRss.ChildNodes[i].Name == "channel")
                    {
                        nodeChannel = nodeRss.ChildNodes[i];
                    }
                }

                for (int i = (nodeChannel.ChildNodes.Count - 1); i >= 0; i--)
                {
                    if (nodeChannel.ChildNodes[i].Name == "item")
                    {
                        nodeItem = nodeChannel.ChildNodes[i];
                        //Noticia = RemoveAcentos(HttpUtility.HtmlDecode(nodeItem["title"].InnerText)) + " " + RemoveAcentos(HttpUtility.HtmlDecode(nodeItem["description"].InnerText));
                        Noticia = HttpUtility.HtmlDecode(RemoveHTML(VerificaConteudo(nodeItem["link"].InnerText.Replace("///", "//"), "<!-- miolo -->", "<!-- fim coluna 1-->")));
                        if (VerificaHora(nodeItem["pubDate"].InnerText, 0))
                        {
                            if (Noticia.ToUpper().Contains(" NO RIO") || Noticia.ToUpper().Contains("RIO DE JANEIRO"))
                            {
                                if (SM.IndexOf(RemoveAcentos(nodeItem["title"].InnerText)) < 0)
                                {
                                    Novas = Novas + ";" + RemoveAcentos(nodeItem["title"].InnerText.Trim(' '));
                                    NovasLink = NovasLink + ";" + nodeItem["link"].InnerText;
                                }
                            }
                        }
                    }
                }

                if (!String.IsNullOrEmpty(Novas))
                {
                    Verificatxt("terra2.txt", "TERRA: ", Novas.Trim(';'), NovasLink.Trim(';'), true);
                }

                this.Text = "Transito TERRA - " + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString();

                System.Threading.Thread.Sleep(3000);
                //LerAUTOPISTA();
                LerTEMPO();
            }
            catch
            {
                //Verifica.Enabled = cbLigar.Checked;
                //LerAUTOPISTA();
                LerTEMPO();
            }
        }

        private void LerAUTOPISTA()
        {
            try
            {
                string Novas = null;

                Random random = new Random();

                rssReader = new XmlTextReader("http://www.autopistafluminense.com.br/?link=js.get_events_xml&target=fluminense&rand=" + random.Next());
                rssDoc = new XmlDocument();
                rssDoc.Load(rssReader);

                for (int i = 0; i < rssDoc.ChildNodes.Count; i++)
                {
                    if (rssDoc.ChildNodes[i].Name == "events")
                    {
                        nodeRss = rssDoc.ChildNodes[i];
                    }
                }


                for (int i = (nodeRss.ChildNodes.Count - 1); i >= 0; i--)
                {
                    if (nodeRss.ChildNodes[i].Name == "event")
                    {
                        nodeItem = nodeRss.ChildNodes[i];
                        //Noticia = RemoveAcentos(HttpUtility.HtmlDecode(nodeItem["title"].InnerText)) + " " + RemoveAcentos(HttpUtility.HtmlDecode(nodeItem["description"].InnerText));
                        Noticia = HttpUtility.HtmlDecode(RemoveHTML(nodeItem["text"].InnerText)).Replace("  ", " ");
                        Novas = Novas + ";" + RemoveAcentos(Noticia.Trim(' '));
                    }
                }

                if (!String.IsNullOrEmpty(Novas))
                {
                    Verificatxt("autopista.txt", "AUTOPISTA: ", Novas.Trim(';'), "");
                }

                this.Text = "Transito AUTOPISTA - " + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString();

                System.Threading.Thread.Sleep(3000);
                LerAUTOPISTA2();
            }
            catch
            {
                //Verifica.Enabled = cbLigar.Checked;
                LerAUTOPISTA2();
            }
        }

        private void LerAUTOPISTA2()
        {
            try
            {
                string Novas = null;
                string NovasLink = null;

                string url = "http://www.autopistafluminense.com.br/index.php?link=noticias.todas";

                HttpWebRequest myWebRequest = (HttpWebRequest)HttpWebRequest.Create(url);
                myWebRequest.Method = "GET";
                HttpWebResponse myWebResponse = (HttpWebResponse)myWebRequest.GetResponse();
                StreamReader myWebSource = new StreamReader(myWebResponse.GetResponseStream(), Encoding.UTF8);
                string myPageSource = myWebSource.ReadToEnd();
                myWebResponse.Close();

                StringReader sr = new StringReader(myPageSource);
                string input = string.Empty;
                string temp = string.Empty;
                string temp2 = string.Empty;
                bool Item = false;
                bool foi = false;
                int i = 0;

                while ((input = sr.ReadLine()) != null && !foi)
                {
                    if (input.Length > 0)
                    {
                        if (Item)
                        {
                            i++;
                            if (input.Contains("</h2>"))
                            {
                                Item = false;
                                //foi = true;
                            }
                            else if (i == 3)
                            {
                                if (input.Trim(' ') != "")
                                {
                                    Novas = Novas + ";" + RemoveHTML(RemoveAcentos(HttpUtility.HtmlDecode(input.Trim(' ', '-')))).Trim();
                                }
                            }
                            else if (i == 2)
                            {
                                if (input.Trim(' ') != "")
                                {
                                    NovasLink = NovasLink + ";http://www.autopistafluminense.com.br/index.php" + input.Split('"')[1];
                                }
                            }
                        }
                    }

                    if (input.ToLower().Contains("<h2>"))
                    {
                        Item = true;
                        i = 0;
                    }
                }

                Verificatxt("autopista2.txt", "AUTOPISTA: ", Novas.Trim(';'), NovasLink.Trim(';'));

                this.Text = "Transito AUTOPISTA2 - " + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString();

                System.Threading.Thread.Sleep(3000);
                LerTEMPO();
            }
            catch
            {
                LerTEMPO();
            }
        }

        private void LerTEMPO()
        {
            try
            {
                string Novas = null;
                string NovasLink = null;

                ArrayList List = new ArrayList();

                string url = "http://www.climatempo.com.br/";

                HttpWebRequest myWebRequest = (HttpWebRequest)HttpWebRequest.Create(url);
                myWebRequest.Method = "GET";
                HttpWebResponse myWebResponse = (HttpWebResponse)myWebRequest.GetResponse();
                StreamReader myWebSource = new StreamReader(myWebResponse.GetResponseStream(), Encoding.UTF8);
                string myPageSource = myWebSource.ReadToEnd();
                myWebResponse.Close();

                StringReader sr = new StringReader(myPageSource);
                string input;
                string temp;

                bool Item = false;
                bool foi = false;
                int i = 0;

                while ((input = sr.ReadLine()) != null && !foi)
                {
                    if (input.Length > 0)
                    {
                        if (Item)
                        {
                            if (input.Contains("</ul>"))
                            {
                                Item = false;
                                foi = true;
                            }
                            else
                            {
                                if (input.Trim(' ') != "")
                                {
                                    temp = RemoveAcentos(HttpUtility.HtmlDecode(input)).Trim();
                                    if (temp.Length > 8)
                                    {
                                        if (temp.ToUpper().Contains("RIO-DE-JANEIRO") || temp.ToUpper().Contains("RIO DE JANEIRO") || temp.ToUpper().Contains("RIO") || temp.ToUpper().Contains("RIO\">"))
                                        {
                                            if (i == 0 && temp.Contains("href"))
                                            {
                                                NovasLink = NovasLink + temp.Split('"')[1];
                                                i++;
                                            }
                                            else
                                            {
                                                Novas = Novas + ";" + RemoveEmailsESites(temp.Substring(18, temp.Length - 18));
                                                i = 0;
                                            }
                                        }
                                    }
                                }
                            }

                        }

                        if (RemoveAcentos(input).Contains("</ul>-->"))
                        {
                            Item = true;
                            //i = 0;
                        }
                    }
                }

                if (!String.IsNullOrEmpty(Novas))
                {
                    Verificatxt("climatempo.txt", "CLIMATEMPO: ", Novas.Trim(';'), NovasLink.Trim(';'), true);
                }

                Http.Navigate("about:blank");
                Controle.Enabled = false;
                this.Text = "Transito CLIMATEMPO - " + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString();

                GravaUsuarios();
                Controle.Enabled = false;
                loop = false;
                Verifica.Enabled = cbLigar.Checked;
            }
            catch
            {
                Http.Navigate("about:blank");
                Controle.Enabled = false;
                loop = false;
                Verifica.Enabled = cbLigar.Checked;
            }
        }

        private XmlTextReader rssReader;
        private XmlDocument rssDoc;
        private XmlNode nodeRss;
        private XmlNode nodeChannel;
        private XmlNode nodeItem;
        private string Noticia;

        private void LerLEISECA()
        {
            try
            {
                string Novas = null;

                rssReader = new XmlTextReader("http://twitter.com/statuses/user_timeline/46152686.rss");
                rssDoc = new XmlDocument();
                rssDoc.Load(rssReader);

                for (int i = 0; i < rssDoc.ChildNodes.Count; i++)
                {
                    if (rssDoc.ChildNodes[i].Name == "rss")
                    {
                        nodeRss = rssDoc.ChildNodes[i];
                    }
                }

                for (int i = 0; i < nodeRss.ChildNodes.Count; i++)
                {
                    if (nodeRss.ChildNodes[i].Name == "channel")
                    {
                        nodeChannel = nodeRss.ChildNodes[i];
                    }
                }

                for (int i = (nodeChannel.ChildNodes.Count - 1); i >= 0; i--)
                {
                    if (nodeChannel.ChildNodes[i].Name == "item")
                    {
                        nodeItem = nodeChannel.ChildNodes[i];
                        Noticia = nodeItem["title"].InnerText.Remove(0, 11);
                        Arruma();
                        Noticia = RemoveEmailsESites(RemoveAcentos(Noticia));
                        if (Noticia.ToUpper().Contains("BLITZ") || Noticia.ToUpper().Contains("LIVRE") || Noticia.ToUpper().Contains("BOLS") || DiaSemana(Noticia.ToUpper()) || Noticia.ToUpper().Contains("ARRASTAO") || Noticia.ToUpper().Contains("TIROTEIO") || Noticia.ToUpper().Contains("ASSALT") || Noticia.ToUpper().Contains("POLICI") || Noticia.ToUpper().Contains("ACIDENTE") || Noticia.ToUpper().Contains("TRANSITO") || Noticia.ToUpper().Contains("RESUMO") || Noticia.ToUpper().Contains("REBOQ"))
                        {
                            if (!Noticia.Contains("TRAPSTER") && !VerificaPalavras(Noticia))
                            {
                                Novas = Novas + ";" + Noticia.Trim(' ', '-');
                            }
                        }
                    }
                }

                Verificatxt("leiseca.txt", "@LeiSecaRJ ", Novas.Trim(';'), "");

                this.Text = "Transito @LeiSecaRJ - " + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString();

                System.Threading.Thread.Sleep(3000);
                LerTEMPO();
            }
            catch
            {
                //Verifica.Enabled = cbLigar.Checked;
                LerTEMPO();
            }
        }

        private string AjustaPREF(string text)
        {
            return RemoveBetween(text, ';', '?');
        }

        private bool VerificaCategoria(string site)
        {
            try
            {
                HttpWebRequest myWebRequest = (HttpWebRequest)HttpWebRequest.Create(site);
                myWebRequest.Method = "GET";
                HttpWebResponse myWebResponse = (HttpWebResponse)myWebRequest.GetResponse();
                StreamReader myWebSource = new StreamReader(myWebResponse.GetResponseStream(), Encoding.Default);
                string myPageSource = myWebSource.ReadToEnd();
                myWebResponse.Close();

                StringReader sr = new StringReader(myPageSource);

                string input = null;
                bool Resp = false;
                string categoria = null;

                while ((input = sr.ReadLine()) != null)
                {
                    if (input.Contains("\"categoria\""))
                    {
                        categoria = input.Substring(input.IndexOf("<div class=\"categoria\">") + 25);
                        categoria = categoria.Substring(0, categoria.IndexOf("</div><div class=\"subtit_interna\">"));
                        break;
                    }
                }

                categoria = categoria.Replace("\t", "").Replace("\n", "");

                string categorias = "transito;fiscalizacao;conservacao;ordem publica;cidade;guarda municipal;seguranca";

                foreach (string cat in categorias.Split(';'))
                {
                    if (String.IsNullOrEmpty(categoria))
                    {
                        Resp = true;
                        break;
                    }
                    else if (RemoveAcentos(categoria).ToLower().Trim().Contains(cat))
                    {
                        Resp = true;
                        break;
                    }
                }

                return Resp;
            }
            catch
            {
                return false;
            }
        }

        private void LerPORTALPREFEITURA()
        {
            try
            {
                string Novas = null;
                string NovasLink = null;

                rssReader = new XmlTextReader("http://www.rio.rj.gov.br/web/guest/exibeconteudo/-/journal/rss/10136/NOT%C3%8DCIAS?doAsGroupId=10136&refererPlid=15501");
                rssDoc = new XmlDocument();
                rssDoc.Load(rssReader);

                for (int i = 0; i < rssDoc.ChildNodes.Count; i++)
                {
                    if (rssDoc.ChildNodes[i].Name == "rss")
                    {
                        nodeRss = rssDoc.ChildNodes[i];
                    }
                }

                for (int i = 0; i < nodeRss.ChildNodes.Count; i++)
                {
                    if (nodeRss.ChildNodes[i].Name == "channel")
                    {
                        nodeChannel = nodeRss.ChildNodes[i];
                    }
                }

                for (int i = (nodeChannel.ChildNodes.Count - 1); i >= 0; i--)
                {
                    if (nodeChannel.ChildNodes[i].Name == "item")
                    {
                        nodeItem = nodeChannel.ChildNodes[i];
                        string link = AjustaPREF(nodeItem["link"].InnerText).Replace("exibeconteudo", "exibeconteudo?");
                        if (VerificaHora(nodeItem["pubDate"].InnerText, 2) && VerificaCategoria(link))
                        {
                            Novas = Novas + ";" + RemoveAcentos(nodeItem["title"].InnerText.Trim(' '));
                            NovasLink = NovasLink + ";" + link;
                        }
                    }
                }

                if (!String.IsNullOrEmpty(Novas))
                {
                    Verificatxt("prefeituraportal.txt", "PREFEITURA: ", Novas.Trim(';'), NovasLink.Trim(';'));
                }

                this.Text = "Transito Prefeitura Portal - " + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString();

                System.Threading.Thread.Sleep(3000);
                LerODIA();
            }
            catch
            {
                //Verifica.Enabled = cbLigar.Checked;
                LerODIA();
            }
        }

        private void LerMAPLINK2()
        {
            try
            {
                StreamReader sm = new StreamReader("maplink.txt");
                string SM = sm.ReadLine();
                sm.Close();

                string Novas = null;
                string NovasLink = null;

                rssReader = new XmlTextReader("http://maplink.uol.com.br/v2/rss/noticiasmaplink.xml");
                rssDoc = new XmlDocument();
                rssDoc.Load(rssReader);

                for (int i = 0; i < rssDoc.ChildNodes.Count; i++)
                {
                    if (rssDoc.ChildNodes[i].Name == "rss")
                    {
                        nodeRss = rssDoc.ChildNodes[i];
                    }
                }

                for (int i = 0; i < nodeRss.ChildNodes.Count; i++)
                {
                    if (nodeRss.ChildNodes[i].Name == "channel")
                    {
                        nodeChannel = nodeRss.ChildNodes[i];
                    }
                }

                for (int i = (nodeChannel.ChildNodes.Count - 1); i >= 0; i--)
                {
                    if (nodeChannel.ChildNodes[i].Name == "item")
                    {
                        nodeItem = nodeChannel.ChildNodes[i];
                        //Noticia = RemoveAcentos(HttpUtility.HtmlDecode(nodeItem["title"].InnerText)) + " " + RemoveAcentos(HttpUtility.HtmlDecode(nodeItem["description"].InnerText));
                        Noticia = VerificaConteudo(nodeItem["link"].InnerText, "Notícias e informações", "Compartilhe essa notícia");
                        if (VerificaHora(nodeItem["pubDate"].InnerText, 2))
                        {
                            if (Noticia.ToUpper().Contains("CET-RIO") || Noticia.ToUpper().Contains("RJ"))
                            {
                                if (SM.IndexOf(RemoveAcentos(nodeItem["title"].InnerText)) < 0)
                                {
                                    Novas = Novas + ";" + RemoveAcentos(nodeItem["title"].InnerText.Trim(' '));
                                    NovasLink = NovasLink + ";" + nodeItem["link"].InnerText;
                                }
                            }
                        }
                    }
                }

                if (!String.IsNullOrEmpty(Novas))
                {
                    Verificatxt("maplink2.txt", "MAPLINK: ", Novas.Trim(';'), NovasLink.Trim(';'));
                }

                this.Text = "Transito MAPLINK2 - " + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString();

                System.Threading.Thread.Sleep(3000);
                LerPORTALPREFEITURA();
            }
            catch
            {
                //Verifica.Enabled = cbLigar.Checked;
                LerPORTALPREFEITURA();
            }
        }

        private string VerificaConteudo(string http, string p1, string p2)
        {
            try
            {
                HttpWebRequest myWebRequest = (HttpWebRequest)HttpWebRequest.Create(http);
                myWebRequest.Method = "GET";
                HttpWebResponse myWebResponse = (HttpWebResponse)myWebRequest.GetResponse();
                StreamReader myWebSource = new StreamReader(myWebResponse.GetResponseStream(), Encoding.UTF8);
                string myPageSource = myWebSource.ReadToEnd();
                myWebResponse.Close();

                //StringReader sr = new StringReader(myPageSource);
                int i1 = myPageSource.IndexOf(p1);
                int i2 = myPageSource.IndexOf(p2) - i1;
                return myPageSource.Substring(i1, i2);
            }
            catch
            {
                return "...";
            }
        }

        private bool VerificaHora(string Hora, int gmt)
        {
            //"Tue, 15 Dec 2009 21:22:00 GMT"
            double t1 = Convert.ToDouble(Hora.Split(' ')[4].Split(':')[0]) - gmt;
            double t2 = Convert.ToDouble(DateTime.Now.Hour) - t1;

            double t3 = Convert.ToDouble(Hora.Split(' ')[1]);
            double t4 = Convert.ToDouble(DateTime.Now.Day) - t3;

            if (Math.Abs(t2) > 1 || t4 > 0)
                return false;
            else
                return true;
        }

        private void Arruma()
        {
            if (Noticia.Substring(0, 4) == "RT: ")
            {
                int pos = Noticia.IndexOf(":", 5);
                Noticia = Noticia.Substring(pos + 1, Noticia.Length - pos - 1).Trim();
            }
        }

        private bool DiaSemana(string texto)
        {
            if (texto.Length >= 4)
            {
                string dia = texto.Substring(0, 4);
                if (dia == "SEG ")
                    return true;
                else if (dia == "TER ")
                    return true;
                else if (dia == "QUA ")
                    return true;
                else if (dia == "QUI ")
                    return true;
                else if (dia == "SEX ")
                    return true;
                else if (dia == "SAB ")
                    return true;
                else if (dia == "DOM ")
                    return true;
                else
                    return false;
            }
            else
                return false;
        }

        private string RemoveEmailsESites(string str)
        {
            string temp = str;
            string[] tem = temp.Split(' ');
            StringBuilder sb = new StringBuilder();
            foreach (string palavra in tem)
            {
                if (!palavra.Contains("@") && !palavra.ToUpper().Contains("HTTP://") && !palavra.ToUpper().Contains("RT:") && !palavra.Contains(":") && !DiaSemana(palavra.ToUpper() + " "))
                {
                    sb.Append(palavra + " ");
                }
            }

            temp = sb.ToString();

            return temp.Substring(0, temp.Length - 1);
        }

        private void Log(string log)
        {
            StreamWriter sw = new StreamWriter("logs\\log" + DateTime.Now.Date.ToShortDateString().Replace('/', '_') + ".txt", true);
            sw.Write(log + Environment.NewLine);
            sw.Close();
        }

        private void GravaUsuarios()
        {
            if (DateTime.Now.Minute <= 5 && DateTime.Now.Hour == 0)
            {
                //---
                WebClient twitterClient = new WebClient();
                NetworkCredential credential = new NetworkCredential("ilsrj", "iiqsto00");
                twitterClient.Credentials = credential;

                string rssFeed = ASCIIEncoding.Default.GetString(twitterClient.DownloadData("http://www.twitter.com/users/ilsrj.xml"));
                XmlDocument doc = new XmlDocument();

                doc.LoadXml(rssFeed);

                //----
                string numero = null;
                string tweets = null;
                //foreach (var feeditem in feed.Items)
                foreach (XmlNode node in doc.SelectNodes("/user/followers_count"))
                {
                    numero = node.InnerText;
                }

                foreach (XmlNode node in doc.SelectNodes("/user/statuses_count"))
                {
                    tweets = node.InnerText;
                }

                StreamWriter sw = new StreamWriter("usuarios.txt", true);
                sw.Write(DateTime.Now.Date.ToShortDateString() + ";" + numero + ";" + tweets + Environment.NewLine);
                sw.Close();
            }
        }

        private void Verificatxt(string arquivo, string nome, string Novas, string Links)
        {
            Verificatxt(arquivo, nome, Novas, Links, false);
        }

        private void Verificatxt(string arquivo, string nome, string Novas, string Links, bool VerificaLogs)
        {
            try
            {
                bitly b = new bitly();

                string t1 = "blablabla";

                if (File.Exists(arquivo))
                {
                    StreamReader sr = new StreamReader(arquivo);
                    t1 = sr.ReadLine();
                    sr.Close();
                }

                string[] t2 = Novas.Split(';');
                string[] t1l = null;
                if (!String.IsNullOrEmpty(Links))
                {
                    t1l = Links.Split(';');
                }
                int i = 0;

                foreach (string t3 in t2)
                {
                    if (t1.IndexOf(t3) < 0)
                    {
                        string ITEM;
                        if (!String.IsNullOrEmpty(Links))
                        {
                            ITEM = RemoveAcentos(Data()) + " " + nome + t3 + " " + b.shorten(t1l[i]);
                        }
                        else
                        {
                            ITEM = RemoveAcentos(Data()) + " " + nome + t3;
                        }
                        if (ITEM.Length > 140)
                            ITEM = ITEM.Substring(0, 140);

                        if (!VerificaSeLista(t3) || !VerificaLogs)
                        {
                            if (!VerificaPostagens(t3))
                            {
                                Lista.Items.Add(ITEM);
                                if (cbPostar.Checked)
                                {
                                    Log(ITEM);
                                    EscreverTweet(ITEM);
                                    System.Threading.Thread.Sleep(10000);
                                }
                            }
                            else
                            {
                                Log(ITEM);
                            }
                        }
                    }
                    i++;
                }

                StreamWriter sw = new StreamWriter(arquivo);
                sw.Write(Novas);
                sw.Close();
            }
            catch
            {
                //Verifica.Enabled = cbLigar.Checked;
            }
        }

        private void LerILSRJ()
        {
            try
            {
                string Novas = null;

                XmlReader reader = XmlReader.Create("http://search.twitter.com/search.atom?q=%23ILSRJ");
                SyndicationFeed feed = SyndicationFeed.Load(reader);
                string title = String.Empty;
                string link = String.Empty;

                foreach (var feeditem in feed.Items)
                {
                    title = feeditem.Title.Text.Replace(";", "");
                    link = feeditem.Authors[feeditem.Authors.Count - 1].Uri;
                    Noticia = "@" + PegaUsuario(link) + " " + RemoveBetween(RemoveEmailsESites(RemoveAcentos(HttpUtility.HtmlDecode(title))).Replace("  ", " ").Replace("\n", "").Replace("\r", "").Trim(), '(', ')').Trim();

                    if (Noticia.Length > 1)
                    {
                        if (!VerificaPalavras(Noticia) && !Noticia.ToUpper().Contains("TRAPSTER") && !Noticia.ToUpper().Contains(" RT ") && !Noticia.ToUpper().Contains("?") && (PegaUsuario(link).ToUpper() != "ILSRJ"))// && (title.ToUpper().Contains("LEISECARJ") || (PegaUsuario(link).ToUpper() == "LEISECARJ")))
                        {
                            Novas = Novas + ";" + "RT " + Noticia.Trim(' ', '-');
                        }
                    }
                }

                if (!String.IsNullOrEmpty(Novas))
                {
                    Verificatxt("ilsrj.txt", "", Novas.Trim(';'), "");
                }

                this.Text = "Transito ILSRJ - " + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString();

                System.Threading.Thread.Sleep(3000);
                LerILSRJ2();
            }
            catch
            {
                LerILSRJ2();
            }
        }

        private void LerILSRJ2()
        {
            try
            {
                //if ((DateTime.Now.Hour % 2) == 0 && DateTime.Now.Minute <= 5)
                //{
                //    int num = DateTime.Now.Day + DateTime.Now.Month + DateTime.Now.Year + DateTime.Now.Hour + DateTime.Now.Minute + DateTime.Now.Second + DateTime.Now.Millisecond;
                //    EscreverTweet("Escreva a tag #ILSRJ na sua mensagem para aparecer aqui automaticamente ! " + num.ToString());
                //}

                //this.Text = "Transito ILSRJ2 - " + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString();

                //System.Threading.Thread.Sleep(3000);
                LerILSRJ3();
            }
            catch
            {
                LerILSRJ3();
            }
        }

        private void LerILSRJ3()
        {
            try
            {
                string Novas = null;

                string title = String.Empty;
                string link = String.Empty;

                WebClient twitterClient = new WebClient();
                NetworkCredential credential = new NetworkCredential("ilsrj", "iiqsto00");
                twitterClient.Credentials = credential;

                string rssFeed = ASCIIEncoding.Default.GetString(twitterClient.DownloadData("http://twitter.com/statuses/mentions.xml"));
                XmlDocument doc = new XmlDocument();

                doc.LoadXml(rssFeed);

                string usuario = String.Empty;

                foreach (XmlNode node in doc.SelectNodes("/statuses/status/text"))
                {
                    title = node.InnerText.Replace(";", "");
                    usuario = node.ParentNode.ChildNodes.Item(9).ChildNodes.Item(2).InnerText;
                    Noticia = "@" + usuario + " " + RemoveBetween(RemoveEmailsESites(RemoveAcentos(HttpUtility.HtmlDecode(title))).Replace("  ", " ").Replace("\n", "").Replace("\r", "").Trim(), '(', ')').Trim();

                    if (Noticia.Length > 1)
                    {
                        if (!VerificaPalavras(Noticia) && !Noticia.ToUpper().Contains("TRAPSTER") && !Noticia.ToUpper().Contains(" RT ") && !Noticia.ToUpper().Contains("?") && (usuario != "ILSRJ") && (usuario != "RADARBLITZ") && (usuario != "LEISECARJ") && VerificaPalavrasOK(Noticia))//(PegaUsuario(link).ToUpper() != "ILSRJ"))// && (title.ToUpper().Contains("LEISECARJ") || (PegaUsuario(link).ToUpper() == "LEISECARJ")))
                        {
                            Novas = Novas + ";" + "RT " + Noticia.Trim(' ', '-');
                        }
                    }
                }

                if (!String.IsNullOrEmpty(Novas))
                {
                    Verificatxt("ilsrj3.txt", "", Novas.Trim(';'), "");
                }

                this.Text = "Transito ILSRJ3 - " + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString();

                System.Threading.Thread.Sleep(3000);
                LerEXTRA();
            }
            catch
            {
                LerEXTRA();
            }
        }

        private bool VerificaPalavras(string texto)
        {
            StreamReader Palavras = new StreamReader("palavras.txt");
            string[] Proibidas = Palavras.ReadLine().Split(';');
            Palavras.Close();

            string[] Verificar = texto.Split(' ');

            bool Resposta = false;

            foreach (string PalavraProibida in Proibidas)
            {
                //foreach (string PalavraDaVez in Verificar)
                //{
                //    if (PalavraDaVez.ToUpper().Trim(',', '.', '?', '!', '(', ')') == PalavraProibida.ToUpper().Trim(',', '.', '?', '!', '(', ')'))
                //        Resposta = true;
                //}
                if (texto.ToUpper().Trim(',', '.', '?', '!', '(', ')').Contains(PalavraProibida.ToUpper()))
                {
                    Resposta = true;
                }
            }

            return Resposta;
        }

        private bool VerificaPalavrasOK(string texto)
        {
            StreamReader Palavras = new StreamReader("palavrasok.txt");
            string[] OK = Palavras.ReadLine().Split(';');
            Palavras.Close();

            bool Resposta = false;

            foreach (string PalavraOK in OK)
            {
                if (texto.ToUpper().Trim(',', '.', '?', '!', '(', ')').Contains(PalavraOK.ToUpper()))
                    Resposta = true;
            }

            return Resposta;
        }

        string RemoveBetween(string s, char begin, char end)
        {
            Regex regex = new Regex(string.Format("\\{0}.*?\\{1}", begin, end));
            return regex.Replace(s, string.Empty);
        }

        private string PegaUsuario(string http)
        {
            string[] tt = http.Split('/');
            return tt[3];
        }

        private bool VerificaSeLista(string ITEM)
        {
            bool Resp = false;

            string tt2 = RemoveHTML(ITEM.Substring(9)).Trim();

            string directory = Application.StartupPath + "\\logs";
            DirectoryInfo dirinfo = new DirectoryInfo(directory);

            FileInfo[] files = dirinfo.GetFiles("*.txt");

            foreach (FileInfo file in files)
            {
                //foreach (object item in Lista.Items)
                //{
                //    if (RemoveEmailsESites(item.ToString()) == RemoveEmailsESites(ITEM))
                //        Resp = true;
                //}

                string t1 = "blabla";
                //if (File.Exists("logs\\log" + DateTime.Now.Date.ToShortDateString().Replace('/', '_') + ".txt"))
                //{
                //StreamReader sr = new StreamReader("logs\\log" + DateTime.Now.Date.ToShortDateString().Replace('/', '_') + ".txt");
                StreamReader sr = new StreamReader(file.FullName);
                t1 = sr.ReadToEnd();
                sr.Close();
                //}

                //string[] t2 = ITEM.Substring(12).Trim().Split(' ');
                //string tt2 = null;

                //int j = 0;
                //while (t2[j].Contains("@") || (t2[j] == "RT") || (t2[j] == "RT:") || (DiaSemana(t2[j] + " ")))
                //{
                //    j++;
                //}

                //for (int i = j; i < t2.Length; i++)
                //    tt2 = tt2 + " " + t2[i];

                //tt2 = tt2.Trim();

                if (t1.IndexOf(tt2) > 0)
                {
                    Resp = true;
                    break;
                }


            }



            return Resp;
        }

        private bool VerificaPostagens(string ITEM)
        {
            bool Resp = false;

            foreach (string item in Lista.Items)
            {
                if (RemoveEmailsESites(item.ToString()) == RemoveEmailsESites(ITEM))
                {
                    Resp = true;
                    break;
                }
            }

            return Resp;
        }

        private string msgstr = "Transito";
        private uint msg;
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern uint RegisterWindowMessage(string lpString);

        private void Form1_Load(object sender, EventArgs e)
        {
            cbLigar.Checked = true;
            msg = RegisterWindowMessage(msgstr);
            if (msg == 0)
            {
                MessageBox.Show(Marshal.GetLastWin32Error().ToString());
            }
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == msg)
            {
                btAtualizar.PerformClick();
            }
            base.WndProc(ref m);
        }

        private void Controle_Tick(object sender, EventArgs e)
        {
            Verifica.Enabled = false;
            Controle.Enabled = false;
            Contador = 2;
            Http.Navigate("about:blank");
            Controle.Enabled = false;
            this.Text = "Transito Prefeitura ERRO - " + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString();

            System.Threading.Thread.Sleep(3000);
            Controle.Enabled = false;
            LerTransitoMapLink();
        }

        //private void VerificaJB(string oNoticia)
        //{
        //    try
        //    {
        //        string Noticia = oNoticia.Substring(0, oNoticia.Length);
        //        StreamReader sr = new StreamReader("jb.txt");
        //        sb = new StringBuilder();

        //        bool modificou = false;

        //        string arq = sr.ReadToEnd();
        //        if (arq.Length > 0)
        //        {
        //            arq = arq.Replace("\r\n\r\n", "\r\n");
        //            arq = arq.Trim(' ', '\r', '\n');//.Replace(" ", "");
        //            sb.AppendLine(arq);
        //        }
        //        if (!arq.Contains(Noticia))
        //        {
        //            Lista.Items.Add(Data() + " JB: " + Noticia);
        //            //PostTweet("ilsrj", "iiqsto00", RemoveAcentos(Data()) + " JB: " + Noticia);
        //            EscreverTweet(RemoveAcentos(Data()) + " JB: " + Noticia);
        //            System.Threading.Thread.Sleep(10000);
        //            sb.AppendLine(Noticia);
        //            modificou = true;
        //        }
        //        sr.Close();



        //        if (modificou)
        //        {
        //            using (StreamWriter sw = new StreamWriter(@"jb.txt", false))
        //            {
        //                sw.Write(sb.ToString());
        //            }
        //        }
        //    }
        //    catch
        //    {
        //    }
        //}
    }

    public class Lugar
    {
        private string oRua;
        private string oStatus;

        public string Rua
        {
            get
            {
                return oRua;
            }
            set
            {
                oRua = value;
            }
        }
        public string Status
        {
            get
            {
                return oStatus;
            }
            set
            {
                oStatus = value;
            }
        }
    }

    class bitly
    {
        private string loginAccount;
        private string apiKeyForAccount;
        private string submitPath = @"http://api.bit.ly/shorten?version=2.0.1&format=xml";
        private int errorStatus = 0;
        private string errorMessage = "";


        /// <summary>
        /// Default constructor which will login with demo credentials
        /// </summary>
        /// <returns>A bitly class object</returns>
        public bitly()
            : this("ilsrj", "R_73a9e71b0cdaf5c20da3b2ed3236d44b")
        {

        }


        /// <summary>
        /// Overloaded constructor that takes a bit.ly login and apikey (if applicable)
        /// </summary>
        /// <returns>A bitly class object</returns>
        public bitly(string login, string APIKey)
        {
            loginAccount = login;
            apiKeyForAccount = APIKey;

            submitPath += "&login=" + loginAccount + "&apiKey=" + apiKeyForAccount;
        }


        // Properties to retrieve error information.
        public int ErrorCode
        {
            get { return errorStatus; }
        }

        public string ErrorMessage
        {
            get { return errorMessage; }
        }


        /// <summary>
        /// Shortens a provided URL
        /// </summary>
        /// <param name="url">A URL</param>
        /// <returns>A shortened bit.ly URL String</returns>
        public string shorten(string url)
        {
            errorStatus = 0;
            errorMessage = "";

            XmlDocument doc = buildDocument(url);

            if (doc.DocumentElement != null)
            {
                XmlNode shortenedNode = doc.DocumentElement.SelectSingleNode("results/nodeKeyVal/shortUrl");

                if (shortenedNode != null)
                {
                    return shortenedNode.InnerText;
                }
                else
                {
                    errorCode(doc);
                }
            }
            else
            {
                this.errorStatus = -1;
                this.errorMessage = "Unable to connect to bit.ly for shortening of URL";
            }

            return "";

        }


        // Sets error code and message in the situation we receive a response, but there was
        // something wrong with our submission.
        private void errorCode(XmlDocument doc)
        {
            XmlNode errorNode = doc.DocumentElement.SelectSingleNode("errorCode");
            XmlNode errorMessage = doc.DocumentElement.SelectSingleNode("errorMessage");

            if (errorNode != null)
            {
                this.errorStatus = Convert.ToInt32(errorNode.InnerText);
                this.errorMessage = errorMessage.InnerText;
            }
        }


        // Builds an XmlDocument using the XML returned by bit.ly in response
        // to our URL being submitted
        private XmlDocument buildDocument(string url)
        {
            XmlDocument doc = new XmlDocument();

            try
            {
                // Load the XML response into an XML Document and return it.
                doc.LoadXml(readSource(submitPath + "&longUrl=" + HttpUtility.UrlEncode(url)));
                return doc;
            }
            catch
            {
                return new XmlDocument();
            }
        }


        // Fetches a result from bit.ly provided the URL submitted
        private string readSource(string url)
        {
            WebClient client = new WebClient();

            try
            {
                using (StreamReader reader = new StreamReader(client.OpenRead(url)))
                {
                    // Read all of the response

                    return reader.ReadToEnd();
                    reader.Close();
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }

    public class InfoTransito
    {
        public InfoTransito(string oNome, string oUrl)
        {
            Nome = oNome;
            Url = oUrl;
        }

        private string iNome;
        private string iUrl;

        public string Nome
        {
            get
            {
                return iNome;
            }
            set
            {
                iNome = value;
            }
        }

        public string Url
        {
            get
            {
                return iUrl;
            }
            set
            {
                iUrl = value;
            }
        }
    }

    public class TwitPic
    {
        private const string TWITPIC_UPLADO_API_URL = "http://twitpic.com/api/upload";
        private const string TWITPIC_UPLOAD_AND_POST_API_URL = "http://twitpic.com/api/uploadAndPost";

        /// <summary>
        /// Uploads the photo and sends a new Tweet
        /// </summary>
        /// <param name="binaryImageData">The binary image data.</param>
        /// <param name="tweetMessage">The tweet message.</param>
        /// <param name="filename">The filename.</param>
        /// <returns>Return true, if the operation was succeded.</returns>
        public string UploadPhoto(byte[] binaryImageData, string ContentType, string tweetMessage, string filename, string Username, string Password)
        {
            // Documentation: http://www.twitpic.com/api.do
            string boundary = Guid.NewGuid().ToString();
            string requestUrl = String.IsNullOrEmpty(tweetMessage) ? TWITPIC_UPLADO_API_URL : TWITPIC_UPLOAD_AND_POST_API_URL;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(requestUrl);
            string encoding = "iso-8859-1";

            request.PreAuthenticate = true;
            request.AllowWriteStreamBuffering = true;
            request.ContentType = string.Format("multipart/form-data; boundary={0}", boundary);
            request.Method = "POST";

            string header = string.Format("--{0}", boundary);
            string footer = string.Format("--{0}--", boundary);

            StringBuilder contents = new StringBuilder();
            contents.AppendLine(header);

            string fileContentType = ContentType;
            string fileHeader = String.Format("Content-Disposition: file; name=\"{0}\"; filename=\"{1}\"", "media", filename);
            string fileData = Encoding.GetEncoding(encoding).GetString(binaryImageData);

            contents.AppendLine(fileHeader);
            contents.AppendLine(String.Format("Content-Type: {0}", fileContentType));
            contents.AppendLine();
            contents.AppendLine(fileData);

            contents.AppendLine(header);
            contents.AppendLine(String.Format("Content-Disposition: form-data; name=\"{0}\"", "username"));
            contents.AppendLine();
            contents.AppendLine(Username);

            contents.AppendLine(header);
            contents.AppendLine(String.Format("Content-Disposition: form-data; name=\"{0}\"", "password"));
            contents.AppendLine();
            contents.AppendLine(Password);

            if (!String.IsNullOrEmpty(tweetMessage))
            {
                contents.AppendLine(header);
                contents.AppendLine(String.Format("Content-Disposition: form-data; name=\"{0}\"", "message"));
                contents.AppendLine();
                contents.AppendLine(tweetMessage);
            }

            contents.AppendLine(footer);

            byte[] bytes = Encoding.GetEncoding(encoding).GetBytes(contents.ToString());
            request.ContentLength = bytes.Length;

            using (Stream requestStream = request.GetRequestStream())
            {
                requestStream.Write(bytes, 0, bytes.Length);

                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                    {
                        string result = reader.ReadToEnd();

                        XDocument doc = XDocument.Parse(result);

                        XElement rsp = doc.Element("rsp");
                        string status = rsp.Attribute(XName.Get("status")) != null ? rsp.Attribute(XName.Get("status")).Value : rsp.Attribute(XName.Get("stat")).Value;
                        string mediaurl = rsp.Element("mediaurl").Value;
                        //return status.ToUpperInvariant().Equals("OK");
                        return mediaurl;
                    }
                }
            }
        }
    }
}
