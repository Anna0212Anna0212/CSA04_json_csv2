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
using System.Text.Json;
using System.Globalization;

namespace CSA04
{
    public partial class Form1: Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        Dictionary<string, string> record = new Dictionary<string, string>();
        public static List<DateTime> list_date = new List<DateTime>();
        public static bool date_tr;

        public static void add_date(DateTime date1,DateTime date2)
        {
            while (date1 <= date2)
            {
                date1 = date1.AddDays(1);
                list_date.Add(date1);
            }
        }
        public static void remove_date(DateTime date1, DateTime date2)
        {
            while (date1 <= date2)
            {
                date1 = date1.AddDays(1);
                list_date.Remove(date1);
            }
        }
        public static void che_date(DateTime date1, DateTime date2)
        {
            date_tr = true;
            foreach (var date in list_date)
            {
                if (date == date1 || date == date2)
                {
                    date_tr = false;
                    break;
                }
            }
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            dateTimePicker1.MinDate = DateTime.Now.AddDays(1);
            
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            dateTimePicker2.MinDate = dateTimePicker1.Value.AddDays(1);
            dateTimePicker2.Value = dateTimePicker2.MinDate;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(!string.IsNullOrWhiteSpace(comboBox1.Text) && !string.IsNullOrWhiteSpace(comboBox2.Text) && !record.ContainsKey(dateTimePicker1.Value.ToString("yyyy年MM月dd日") + "-"))
            {
                che_date(dateTimePicker1.Value.Date, dateTimePicker2.Value.Date);
                if(date_tr)
                {
                    record.Add(dateTimePicker1.Value.ToString("yyyy年MM月dd日") + "-", $"{dateTimePicker2.Value.ToString("yyyy年MM月dd日")}-{comboBox1.Text} 付現：{(radioButton1.Checked ? "是" : "否")} {comboBox2.Text}");
                    button5_Click(sender, e);
                    comboBox1.Text = null;
                    comboBox2.Text = null;
                    radioButton1.Checked = true;
                    add_date(dateTimePicker1.Value.Date, dateTimePicker2.Value.Date);
                    MessageBox.Show("加入成功");
                }
                else
                {
                    MessageBox.Show("日期已存在活動");
                }
            }
            else
            {
                MessageBox.Show("錯誤資料");
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (record.ContainsKey(dateTimePicker1.Value.ToString("yyyy年MM月dd日") + "-"))
            {
                record.Remove(dateTimePicker1.Value.ToString("yyyy年MM月dd日") + "-");
                button5_Click(sender, e);
                comboBox1.Text = null;
                comboBox2.Text = null;
                radioButton1.Checked = true;
                remove_date(dateTimePicker1.Value.Date, (DateTime.Parse(record[dateTimePicker1.Value.ToString("yyyy年MM月dd日") + "-"].Split('-')[0]).Date));
                MessageBox.Show("刪除成功");
            }
            else
            {
                MessageBox.Show("查無資料");
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (record.ContainsKey(dateTimePicker1.Value.ToString("yyyy年MM月dd日") + "-"))
            {
                listBox1.Items.Clear();
                listBox1.Items.Add(dateTimePicker1.Value.ToString("yyyy年MM月dd日") + "-" + record[dateTimePicker1.Value.ToString("yyyy年MM月dd日") + "-"]);
                MessageBox.Show("查詢成功");
            }
            else
            {
                MessageBox.Show("查無資料");
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
            foreach (var item in record)
            {
                listBox1.Items.Add(item.Key+item.Value);
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
            List<string> list = new List<string>();
            foreach(var item in record)
            {
                list.Add(item.Key);
            }
            list.Sort();
            foreach (var item in list)
            {
                listBox1.Items.Add(item + record[item]);
            }
        }

        private void button6_Click(object sender, EventArgs e)//匯入csv
        {
            OpenFileDialog open_csv = new OpenFileDialog();
            open_csv.Filter = "CSV File|*.csv";
            open_csv.Title = "選擇csv檔";
            if(open_csv.ShowDialog() == DialogResult.OK)
            {
                List<string> file_csv = File.ReadAllLines(open_csv.FileName).ToList();
                file_csv.RemoveAt(0);
                foreach (var item in file_csv)
                {
                    string[] line_csv = item.Split(',');
                    if(line_csv.Length == 5 && !record.ContainsKey(line_csv[0] + "-") && DateTime.Parse(line_csv[0]) >= DateTime.Now)
                    {
                        che_date(DateTime.Parse(line_csv[0]).Date, DateTime.Parse(line_csv[1]).Date);
                        if (date_tr)
                        {
                            add_date(DateTime.Parse(line_csv[0]).Date, DateTime.Parse(line_csv[1]).Date);
                            record.Add($"{line_csv[0]}-", $"{line_csv[1]}-{line_csv[2]} 付款：{line_csv[3]} {line_csv[4]}");
                        }
                    }
                }
                button5_Click(sender, e);
                MessageBox.Show("匯入csv成功");
            }
        }

        private void button7_Click(object sender, EventArgs e)//匯出csv
        {
            SaveFileDialog save_csv = new SaveFileDialog();
            save_csv.Filter = "CSV File|*.csv";
            save_csv.Title = "儲存csv檔";
            if(save_csv.ShowDialog() == DialogResult.OK)
            {
                File.WriteAllText(save_csv.FileName, "入住日期,退房日期,人數,付款方式,房型");
                foreach (var item in record)
                {
                    File.AppendAllText(save_csv.FileName,item.Key.Replace('-',',')+item.Value.Replace('-',',').Replace(" ",",").Replace("付現：", "") + "\n");
                }
                MessageBox.Show("匯出csv成功");
            }
        }

        private void button3_Click(object sender, EventArgs e)//匯入json
        {
            OpenFileDialog open_json = new OpenFileDialog();
            open_json.Filter = "JSON File|*.json";
            open_json.Title = "選擇json檔";
            if(open_json.ShowDialog() == DialogResult.OK)
            {
                string json_file = File.ReadAllText(open_json.FileName);
                var json = JsonSerializer.Deserialize<List<Class1>>(json_file);

                foreach (var item in json)
                {
                    DateTime date1 = DateTime.Parse(item.入住日期).Date;
                    DateTime date2 = DateTime.Parse(item.退房日期).Date;

                    che_date(date1,date2);
                    if (date_tr && date1 >= DateTime.Now && !record.ContainsKey(date1+"-"))
                    {
                        record.Add(date1.ToString("yyyy年MM月dd日") + "-", $"{date2.ToString("yyyy年MM月dd日")}-{item.人數} 付現：{item.付款方式.Replace("付款：","")} {item.房型}");
                        add_date(date1, date2);
                    }
                }
                button5_Click(sender, e);
                MessageBox.Show("匯入json成功");
            }
        }

        private void button9_Click(object sender, EventArgs e)//匯出json
        {
            SaveFileDialog save_json = new SaveFileDialog();
            save_json.Filter = "JSON File|*.json";
            save_json.Title = "儲存json檔";
            if (save_json.ShowDialog() == DialogResult.OK)
            {
                var json = record.Select(x => new Class1 { 入住日期 = x.Key.Replace("-", ""), 退房日期 = x.Value.Split('-')[0], 人數 = x.Value.Split('-')[1].Split(' ')[0], 付款方式 = x.Value.Split('-')[1].Split(' ')[1].Replace("付現：", ""), 房型 = x.Value.Split('-')[1].Split(' ')[2] });
                string file_json = JsonSerializer.Serialize(json, new JsonSerializerOptions { WriteIndented = true});
                File.WriteAllText(save_json.FileName, file_json);
                MessageBox.Show("匯出json檔完成");
            }
        }
    }
}
