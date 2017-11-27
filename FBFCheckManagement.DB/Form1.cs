using System;
using System.Windows.Forms;
using FBFCheckManagement.Infrastructure.EntityFramework;

namespace FBFCheckManagement.DB
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DbCreator d = new DbCreator();
            d.Create();
        }
    }
}
