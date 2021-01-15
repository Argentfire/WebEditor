using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;
using System.Collections.Specialized;
using System.IO;
using System.Threading;

#pragma warning disable IDE1006 // Naming Styles
#pragma warning disable IDE0017 // Simpliasfy object initialization
#pragma warning disable CS0168 // Variable is declared but never used

namespace WebBrowser
{
    public partial class Form1 : Form
    {
        string tmpSaveDir = AppDomain.CurrentDomain.BaseDirectory + "tmp";
        string tmpSaveFileName =  "Temporary_Page.html";

        System.Timers.Timer autoSaveTimer = new System.Timers.Timer();
        System.Timers.Timer tagColoringTimer = new System.Timers.Timer();

        string cutText = string.Empty;
        string copyText = string.Empty;

        public Form1()
        {
            InitializeComponent();

            autoSaveTimer.Interval = 30000;
            autoSaveTimer.AutoReset = true;
            autoSaveTimer.Enabled = false;
            autoSaveTimer.Elapsed += Timer_Elapsed;

            tagColoringTimer.Interval = 2000;
            tagColoringTimer.AutoReset = true;
            //tagColoringTimer.Enabled = true;
            tagColoringTimer.Elapsed += TagColoringTimer_Elapsed;
        }

        private void TagColoringTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                HighlightPhrase(txtEditor, "<", ">", "</", Color.Blue, Color.Black);
            }
            catch (Exception ex) { }
        }

        private void SaveFile(string directory, string fileName)
        {
            string content = txtEditor.Text;
            FileStream fParameter = new FileStream(Path.Combine(directory, fileName), FileMode.OpenOrCreate, FileAccess.Write);
            StreamWriter m_WriterParameter = new StreamWriter(fParameter);

            m_WriterParameter.Write(content);
            m_WriterParameter.Close();
            m_WriterParameter.Dispose();
        }

        private void SaveFile(string path)
        {
            string content = txtEditor.Text;
            FileStream fParameter = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write);
            StreamWriter m_WriterParameter = new StreamWriter(fParameter);

            m_WriterParameter.Write(content);
            m_WriterParameter.Close();
            m_WriterParameter.Dispose();
        }


        private void OpenPage(string link)
        {
            webBrowser.Url = new Uri(link);
            txtWebURL.Text = link;

            txtEditor.Text = File.ReadAllText(link);
        }

        private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            SaveFile(tmpSaveDir, tmpSaveFileName);
        }

        private void txtWebURL_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (txtWebURL.Text != string.Empty)
                {
                    webBrowser.Navigate(txtWebURL.Text);
                }
            }
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            if(webBrowser.CanGoBack)
                webBrowser.GoBack();
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            webBrowser.Refresh();
        }

        private void btnForward_Click(object sender, EventArgs e)
        {
            if (webBrowser.CanGoForward)
                webBrowser.GoForward();
        }

        private void webBrowser_Navigating(object sender, WebBrowserNavigatingEventArgs e)
        {
            txtWebURL.Text = webBrowser.Url.ToString();
        }

        private void autoSaveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string _autoSaver = ConfigurationManager.AppSettings.Get("AutoSave");
            if (!Convert.ToBoolean(_autoSaver))
            {
                autoSaveTimer.Enabled = true;
                autoSaveToolStripMenuItem.BackColor = SystemColors.ControlDark;
            }
            else
            {
                autoSaveTimer.Enabled = false;
                autoSaveToolStripMenuItem.BackColor = SystemColors.Control;
            }
        }

        private void reloadBrowserToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (File.Exists(Path.Combine(tmpSaveDir, tmpSaveFileName)))
                File.Delete(Path.Combine(tmpSaveDir, tmpSaveFileName));

            SaveFile(Path.Combine(tmpSaveDir, tmpSaveFileName));
            OpenPage(Path.Combine(tmpSaveDir, tmpSaveFileName));
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                if(ofd.ShowDialog() == DialogResult.OK)
                {
                    OpenPage(ofd.FileName);
                }
            }
        }

        private void savePageAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.AddExtension = true;
                sfd.Filter = "HTML (.html)|*.html|PHP (.php)|*.php|CSS (.css)|*.css|JavaScript (.js)|*.js";
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    SaveFile(Path.GetDirectoryName(sfd.FileName),sfd.FileName);
                }
            }
        }

        private void cutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (txtEditor.SelectedText.Length != 0)
            {
                cutText = txtEditor.SelectedText;
                txtEditor.Cut();
                MessageBox.Show(cutText);
            }
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (txtEditor.SelectedText.Length != 0)
            {
                copyText = txtEditor.SelectedText;
                txtEditor.Copy();
                MessageBox.Show(copyText);
            }
        }

        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void HighlightPhrase(RichTextBox box, string StartTag, string EndTag, string ControlTag, Color color1, Color color2)
        {
            int pos = box.SelectionStart;
            string s = box.Text;
            for (int ix = 0; ;)
            {
                int jx = s.IndexOf(StartTag, ix, StringComparison.CurrentCultureIgnoreCase);
                if (jx < 0) break;
                int ex = s.IndexOf(EndTag, ix, StringComparison.CurrentCultureIgnoreCase);
                box.SelectionStart = jx;
                box.SelectionLength = ex - jx + 1;
                box.SelectionColor = color1;

                int bx = s.IndexOf(ControlTag, ix, StringComparison.CurrentCultureIgnoreCase);
                int bxtest = s.IndexOf(StartTag, (ex + 1), StringComparison.CurrentCultureIgnoreCase);
                if (bx == bxtest)
                {
                    box.SelectionStart = ex + 1;
                    box.SelectionLength = bx - ex + 1;
                    box.SelectionColor = color2;
                }

                ix = ex + 1;
            }
            box.SelectionStart = pos;
            box.SelectionLength = 0;
        }

        private void COLORTAGSToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                HighlightPhrase(txtEditor, "<", ">", "</", Color.Blue, Color.Black);
            }
            catch (Exception ex) { }
        }

        private void btnGO_Click(object sender, EventArgs e)
        {
            if (txtWebURL.Text != string.Empty)
            {
                webBrowser.Navigate(txtWebURL.Text);
            }
            else
                webBrowser.Navigate("About:Blank");
        }
    }
}