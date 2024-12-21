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
    public partial class Start : Form
    {
        public SoundManager soundManager;
        public Start()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.Manual; 
        }

        private void Start_Load(object sender, EventArgs e)
        {
          
            int x = (Screen.PrimaryScreen.WorkingArea.Width - this.Width) / 2;
            int y = (Screen.PrimaryScreen.WorkingArea.Height - this.Height) / 2;
            this.Location = new Point(x, y);  
        }
        private void guna2Button2_Click(object sender, EventArgs e)
        {
            HardMode hardMode = new HardMode();
            hardMode.Show();
            this.Hide();
        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            Customized easyMode = new Customized();
            easyMode.Show();
            this.Hide();

        }

        private void guna2Button3_Click(object sender, EventArgs e)
        {
            Application.Exit();

        }

      
        private void guna2Button4_Click(object sender, EventArgs e)
        {
            EasyMode easyMode = new EasyMode();
            easyMode.Show();
            this.Hide();
        }

        private void guna2Button5_Click(object sender, EventArgs e)
        {
            MediumMode mediumMode = new MediumMode();
            mediumMode.Show();
            this.Hide();
        }
    }
}
