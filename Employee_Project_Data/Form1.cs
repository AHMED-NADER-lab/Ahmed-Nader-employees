using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Employee_Project_Data
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

      

        private void button1_Click(object sender, EventArgs e)
        {
            textBox1.Hide();
            dataGridView1.Hide();
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Text files | *.txt"; // file types, that will be allowed to upload
            dialog.Multiselect = false; // allow/deny user to upload more than one file at a time
            if (dialog.ShowDialog() == DialogResult.OK) // if user clicked OK
            {
                List<employee> allemp = new List<employee>();
                var list = new List<string>();
                String path = dialog.FileName; // get name of file
                using (StreamReader reader = new StreamReader(new FileStream(path, FileMode.Open), new UTF8Encoding())) // do anything you want, e.g. read it
                {
                    int x = 0;
                    string line;

                    while ((line = reader.ReadLine()) != null)
                    {
                        if (x != 0)
                        {
                            employee model = new employee();
                            string[] word = line.Split(',');

                            model.empId = int.Parse(word[0].Trim());
                            model.projectId = int.Parse(word[1].Trim());

                            model.startDate = word[2].Trim().ToLower() == "null" ? DateTime.Now : Convert.ToDateTime(word[2]);
                            model.enddate = word[3].Trim().ToLower() == "null" ? DateTime.Now : Convert.ToDateTime(word[3]);

                            allemp.Add(model);
                        }
                        x++;
                    };

                }

                var empdata = allemp.GroupBy(t => new { proid = t.projectId })
.Select(g => new {
    proid = g.Key.proid,
    empData = g.OrderBy(d => d.startDate)
.Select(x => new employee { empId = x.empId, projectId = x.projectId, startDate = x.startDate, enddate = x.enddate }).ToList()
})
.Where(x => x.empData.Count() > 1).ToList();

                List<Showemployee> showData = new List<Showemployee>();
                foreach (var item in empdata)
                {

                    if (Rangedata(item.empData))
                    {
                        Showemployee model = new Showemployee();
                        model.Emp_1 = item.empData[0].empId;
                        model.Emp_2 = item.empData[1].empId;
                        model.Project_Id = item.proid;
                        showData.Add(model);
                    }

                }
                if (showData.Any())
                {
                    textBox1.Hide();
                    dataGridView1.Show();
                    dataGridView1.DataSource = showData;

                }
                else
                {
                    textBox1.Show();
                    textBox1.ReadOnly = true;
                    textBox1.Text = "No Project Shared between employees";
                    dataGridView1.Hide();
                }
            }
        }

        public bool Rangedata(List<employee> emps)
        {

            return emps[1].startDate >= emps[0].startDate && emps[1].startDate < emps[0].enddate;
        }

        private void Form1_Load_1(object sender, EventArgs e)
        {
            textBox1.Hide();
            dataGridView1.Hide();
        }
    }

    public class employee
    {
        public int empId { get; set; }
        public int projectId { get; set; }

        public DateTime startDate { get; set; }

        public DateTime enddate { get; set; }


    }


    public class Showemployee
    {
        public int Emp_1 { get; set; }
        public int Emp_2 { get; set; }
        public int Project_Id { get; set; }



    }
}
