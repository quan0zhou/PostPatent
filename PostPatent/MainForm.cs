
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
        /// 简单搜索URL
        /// </summary>
        private const string simpleSearchURL = "https://patentscope.wipo.int/search/zh/search.jsf";

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                Thread th = new Thread(new ThreadStart(Todo));
                //启动线程
                th.Start();
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message, "系统提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        
        }

        private  void Todo()
        {
            //加载页面
            var web = new HtmlWeb();
            HtmlAgilityPack.HtmlDocument doc = web.Load(simpleSearchURL);
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

            StringBuilder postDataStr = new StringBuilder();
            postDataStr.Append("simpleSearchSearchForm=simpleSearchSearchForm");
            postDataStr.Append("&simpleSearchSearchForm:j_idt380="+this.comboBox1.SelectedValue);
            postDataStr.Append("&simpleSearchSearchForm:fpSearch=" + this.textBox1.Text.Trim());
            postDataStr.Append("&simpleSearchSearchForm:commandSimpleFPSearch=检索");
            postDataStr.Append("&simpleSearchSearchForm:j_idt451=workaround");
            postDataStr.Append("&javax.faces.ViewState=" + viewsatte);

            string html= HttpPost(simpleSearchURL, postDataStr.ToString(), cookie);

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
            this.webBrowser1.Navigate(simpleSearchURL);
            DataTable dt = new DataTable();
            dt.Columns.Add("Value", typeof(string));
            dt.Columns.Add("Text", typeof(string));
            dt.Rows.Add(new string[] { "FP","首页" });
            dt.Rows.Add(new string[] { "ALL", "任意字段" });
            dt.Rows.Add(new string[] { "ALLTXT", "全文" });
            dt.Rows.Add(new string[] { "ZH_ALLTXT", "中文文本" });
            dt.Rows.Add(new string[] { "ALLNUM", "识别码/编号" });
            dt.Rows.Add(new string[] { "IC", "国际分类（国际专利分类）" });
            dt.Rows.Add(new string[] { "ALLNAMES", "名称" });
            dt.Rows.Add(new string[] { "DP", "日期" });
            this.comboBox1.DataSource = dt;
            this.comboBox1.ValueMember = "Value";
            this.comboBox1.DisplayMember = "Text";
            
        }
    }
}
