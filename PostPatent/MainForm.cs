
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Web;

namespace PostPatent
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }
        /// <summary>
        /// url
        /// </summary>
        private const string URL = "https://patentscope.wipo.int/search/zh/result.jsf";

        private void button1_Click(object sender, EventArgs e)
        {

            Thread th = new Thread(new ThreadStart(Todo));
            //启动线程
            th.Start();
        }

        private  void Todo()
        {
            //加载页面
            var web = new HtmlWeb();
            HtmlAgilityPack.HtmlDocument doc = web.Load(URL);
            HtmlNode rootnode = doc.DocumentNode;
            //获取script 节点
            HtmlNode node_script = rootnode.SelectSingleNode("//head/script[2]");
            //获取script脚本节点的src值
            string scriptSrc = node_script.Attributes["src"].Value;
            //得到cookie
            string cookie = scriptSrc.Substring(scriptSrc.LastIndexOf(";") + 1).Replace("jsessionid", "JSESSIONID");
            //获取隐藏域节点
            HtmlNode node_ViewState = rootnode.SelectSingleNode("//input[@name=\"javax.faces.ViewState\"]");
            //得到隐藏域的值
            string viewsatte = node_ViewState.Attributes["value"].Value;

            StringBuilder postDataStr = new StringBuilder("javax.faces.ViewState="+viewsatte+"&resultListFormTop=resultListFormTo&resultListFormTop%3ArefineSearchField="+this.textBox1.Text.Trim()+"&resultListFormTop%3AgoToPage=1");

            string html= HttpPost(URL, postDataStr.ToString(), cookie);

            this.webBrowser1.Invoke((Action)(() => {
                //this.webBrowser1.Navigate("about:blank");
                this.webBrowser1.Navigate("about:blank");
                this.webBrowser1.Document.Write(html);
                this.webBrowser1.Refresh();
            }));

        }

        private string HttpPost(string Url, string postDataStr, string  cookie)
        {

            WebClient webClient = new WebClient();
            webClient.Headers.Add("content-Type", "application/x-www-form-urlencoded");//采取POST方式必须加的header，如果改为GET方式的话就去掉这句话即可  
            webClient.Headers.Add("cookie", cookie);
            webClient.Headers.Add("cache-control", "no-cache");
            byte[] responseData = webClient.UploadData(Url, "POST", Encoding.UTF8.GetBytes(postDataStr));//得到返回字符流  
            string srcString = Encoding.UTF8.GetString(responseData);//解码  
            return srcString;
         
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            this.webBrowser1.Navigate(URL);
        }
    }
}
