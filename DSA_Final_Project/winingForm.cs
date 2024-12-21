using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DSA_Final_Project
{
    public partial class winingForm: Form
    {
        public winingForm(string message)
        {
            this.StartPosition = FormStartPosition.Manual; 
            this.StartPosition = FormStartPosition.CenterScreen;
            InitializeComponent();
            Label winLabel = new Label
            {
                Text = message,
                Font = new Font("Verdana", 14, FontStyle.Bold),
                Location = new Point(50, 50),
                AutoSize = true
            };
            this.Controls.Add(winLabel);
        }
       
        private void winingForm_Load(object sender, EventArgs e)
        {
            int x = (Screen.PrimaryScreen.WorkingArea.Width - this.Width) / 2;
            int y = (Screen.PrimaryScreen.WorkingArea.Height - this.Height) / 2;
            this.Location = new Point(x, y);  
        }

        private void guna2Button3_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void guna2Button2_Click(object sender, EventArgs e)
        {
            Start start = new Start();
            start.Show();
            this.Hide();
        }
        public void myName()
        {
            Console.WriteLine("My name is ahmad");
        }
    }
}
