using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Doko.UI.Forms
{
    public partial class GetOneValueForm : Form
    {

        public event Action<string> NewValue;

        private string value = null;

        public static string Show(string title, string text)
        {
            var dialog = new GetOneValueForm(title, text, null);


            dialog.CenterToScreen();
            dialog.ShowDialog();

            return dialog.value;
        }

        public GetOneValueForm(string title, string text, Action<string> action)
        {
            InitializeComponent();
            
            this.Name = title;
            label.Text = text;

            NewValue += action;

            textBox.KeyPress += GetOneValueForm_KeyPress;
        }

        private void GetOneValueForm_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == Convert.ToChar(Keys.Enter))
            {
                okButton_Click(null, null);
            }
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            value = textBox.Text;
            if (string.IsNullOrEmpty(value))
            {
                return;
            }

            NewValue?.Invoke(value);

            this.Close();
        }

        private void abortButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
