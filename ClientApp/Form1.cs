using System;
using System.ComponentModel;
using System.Windows.Forms;

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

        private void button1_Click(object sender, EventArgs e)
        {
            label1.Text = DateTime.Now.ToLongTimeString();
            /*
            var walker = new Walker(textBox1.Text);
            var rootElement = AutomationElement.FromHandle(this.Handle);
            var found = walker.Find(rootElement);
            System.Diagnostics.Debug.WriteLine(found.Count());*/
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
