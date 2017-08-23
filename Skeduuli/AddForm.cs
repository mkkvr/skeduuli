using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Skeduuli
{
    public partial class AddForm : Form
    {

        CalendarForm mainForm;
        DateTime start;
        DateTime end;

        public AddForm(CalendarForm mainForm, DateTime start, DateTime end)
        {
            this.mainForm = mainForm;
            this.start = start;
            this.end = end;
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (this.textBox1.Text != string.Empty)
            {
                mainForm.addNewEvent(textBox1.Text, start, end);
            }

            this.Close();
        }
    }
}
