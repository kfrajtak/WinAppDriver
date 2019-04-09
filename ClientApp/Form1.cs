using CodePlex.XPathParser;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Automation;
using System.Windows.Forms;
using WinAppDriver.XPath;

namespace ClientApp
{
    public partial class Form1 : Form
    {
        //private CommandDispatcher commandDispatcher;

        public Form1()
        {
            InitializeComponent();

            //commandDispatcher = new CommandDispatcher(this.Handle);
            //commandDispatcher.Start();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
        }

        private static string EscapeQuotes(string toEscape)
        {
            if (toEscape.IndexOf("\"", StringComparison.OrdinalIgnoreCase) > -1 && toEscape.IndexOf("'", StringComparison.OrdinalIgnoreCase) > -1)
            {
                bool flag = false;
                if (toEscape.LastIndexOf("\"", StringComparison.OrdinalIgnoreCase) == toEscape.Length - 1)
                {
                    flag = true;
                }
                List<string> list = new List<string>(toEscape.Split('"'));
                if (flag && string.IsNullOrEmpty(list[list.Count - 1]))
                {
                    list.RemoveAt(list.Count - 1);
                }
                StringBuilder stringBuilder = new StringBuilder("concat(");
                for (int i = 0; i < list.Count; i++)
                {
                    stringBuilder.Append("\"").Append(list[i]).Append("\"");
                    if (i == list.Count - 1)
                    {
                        if (flag)
                        {
                            stringBuilder.Append(", '\"')");
                        }
                        else
                        {
                            stringBuilder.Append(")");
                        }
                    }
                    else
                    {
                        stringBuilder.Append(", '\"', ");
                    }
                }
                return stringBuilder.ToString();
            }
            if (toEscape.IndexOf("\"", StringComparison.OrdinalIgnoreCase) > -1)
            {
                return string.Format(CultureInfo.InvariantCulture, "'{0}'", new object[1]
                {
                toEscape
                });
            }
            return string.Format(CultureInfo.InvariantCulture, "\"{0}\"", new object[1]
            {
            toEscape
            });
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //label1.Text = DateTime.Now.ToLongTimeString();

            var criteria = "//ComboBox[@AutomationId='comboBox1']/List/ListItem[contains(normalize-space(.),  " + EscapeQuotes("6 7") + ")]";

            comboBox1.Focus();
            comboBox1.DroppedDown = true;
            //AutoIt.AutoItX.MouseMove(Location.X + comboBox1.Location.X + comboBox1.Width - 2, Location.Y + comboBox1.Location.Y + 38, 2);
            //AutoIt.AutoItX.MouseMove(Location.X + comboBox1.Location.X + comboBox1.Width - 2, Location.Y + comboBox1.Location.Y + 39, 2);
            //AutoIt.AutoItX.MouseClick(x: Location.X + comboBox1.Location.X + comboBox1.Width - 2, y: Location.Y + comboBox1.Location.Y + 38);//.ControlClick(Handle, comboBox1.Handle, x: comboBox1.Width - 2, y: 2);

            try
            {
                var walker = new AutomationElementTreeWalker(new XPathParser<IXPathExpression>().Parse(criteria, new WalkerBuilder()));
                var rootElement = AutomationElement.FromHandle(Handle);
                var src = new CancellationTokenSource();
                var found = walker.Find(rootElement, src.Token).ToList();
                MessageBox.Show("Found " + found.Count);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {
            label1.Text = e.KeyChar.ToString();
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            label1.Text += e.KeyCode.ToString();
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Delete)
            {
                textBox2.Text = "";
            }
            else
            {
                textBox2.Text += keyData.ToString();
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }
    }
}
