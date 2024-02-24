using System;
using System.Windows.Forms;

namespace SM64DSe.ui.dialogs
{
    public partial class DropdownDialog : Form
    {
        private int selected = -1;
        public DropdownDialog(string message, string[] items, int selectedIndex = -1)
        {
            InitializeComponent();
            this.message.Text = message;
            this.dropdown.Items.AddRange(items);
            if (selectedIndex != -1)
            {
                this.dropdown.SelectedIndex = selectedIndex;
            }
        }
        

        public int GetSelected()
        {
            return selected;
        }

        private void cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ok_Click(object sender, EventArgs e)
        {
            this.selected = this.dropdown.SelectedIndex;
            this.Close();
        }
    }
}