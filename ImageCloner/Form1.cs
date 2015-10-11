using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace ImageCloner
{
    public partial class Form1 : Form
    {
        private string _img_file;
        private List<string> _patterns = new List<string>();
        private Image _img;
        private Bitmap _bitmap;
        
        public Form1()
        {
            InitializeComponent();
            bindSettings();
        }

        private void img_button_Click(object sender, EventArgs e)
        {
            if (openFileDialog2.ShowDialog() == DialogResult.OK)
            {
                _img_file = openFileDialog2.FileName;
                _img = Image.FromFile(_img_file);
                _bitmap = new Bitmap(_img);
                textBox1.Text = "image taken";
            }
        }

        private void generate_button_Click(object sender, EventArgs e)
        {
            int ind = comboBox1.SelectedIndex;
            int num_pattern = Properties.Settings.Default.num_pattern;

            if (ind > -1 && num_pattern > ind)
            {
                textBox1.Text = "started to generate images";
                string[] arr0 = Properties.Settings.Default.pattern_list.Split('-');
                string pattern_str = arr0[ind];    
                string[] arr2 = pattern_str.Split(' ');

                for (int i = 1; i < arr2.Length; i++)
                {
                    string[] nums = arr2[i].Split('x');
                    int wid = int.Parse(nums[0]);
                    int hei = int.Parse(nums[1]);
                    generateImg(wid, hei);
                }
            }
            else
            {
                textBox1.Text = "index error";
            }
           
        }

        private void generateImg(int wid, int hei)
        {
            int original_wid = _img.Width;
            int original_hei = _img.Height;

            double original_ratio = original_wid / original_hei;
            double ratio = wid / hei;

            if (ratio == original_ratio)
            {
                resizeImage(_img, new Size(wid, hei)).Save(wid + "x" + hei + ".png");
            }
            else if (ratio > original_ratio)
            {
                int mid_hei = (int)(wid / original_ratio);
                Image mid_img = resizeImage(_img, new Size(wid, mid_hei));
                Bitmap mid_bitmap = new Bitmap(mid_img);
                cropBitmap(mid_bitmap, 0, (mid_hei - hei) / 2, wid, hei).Save(wid + "x" + hei + ".png");
            }
            else
            {
                int mid_wid = (int)(hei * original_ratio);
                Image mid_img = resizeImage(_img, new Size(mid_wid, hei));
                Bitmap mid_bitmap = new Bitmap(mid_img);
                cropBitmap(mid_bitmap, (mid_wid - wid) / 2, 0, wid, hei).Save(wid + "x" + hei + ".png");
            }

        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void add_button_Click(object sender, EventArgs e)
        {
            string ptrn = textBox2.Text + " " + textBox3.Text;
            addPattern(ptrn);
            _patterns.Add(ptrn);
            bindSettings();
            textBox1.Text = "Restart Reqired!";
        }

        private void delete_button_Click(object sender, EventArgs e)
        {
            int ind = comboBox1.SelectedIndex;
            if (ind > -1 && _patterns.Count > ind)
            {
                deletePattern(ind);
                bindSettings();
            }
        }

        private void bindSettings()
        {
            string str = Properties.Settings.Default.pattern_list;
            string[] arr = str.Split('-');
            for (int i = 0; i < arr.Length; i++)
            {
                if (arr[i] != "")
                {
                    _patterns.Add(arr[i]);
                }
            }
            
            comboBox1.DataSource = _patterns;
            textBox1.Text = "Patterns binded";
        }

        private void addPattern(string pattern)
        {
            if (pattern != "" && pattern != null)
            {
                int num_pattern = Properties.Settings.Default.num_pattern;
                Properties.Settings.Default.num_pattern += 1;
                if (num_pattern > 0)
                {
                    Properties.Settings.Default.pattern_list += "-" + pattern;
                }
                else
                {
                    Properties.Settings.Default.pattern_list = pattern;
                }
                
                Properties.Settings.Default.Save();
                textBox1.Text = "Pattern added";
            }
        }

        private void deletePattern(int ind)
        {
            if (ind > -1)
            {
                Properties.Settings.Default.num_pattern -= 1;
                string[] arr = Properties.Settings.Default.pattern_list.Split('-');
                string str = "";

                for (int i = 0; i < arr.Length; i++)
                {
                    if (ind != i)
                    {
                        if (str == "")
                        {
                            str = arr[i];
                        }
                        else
                        {
                            str += "-" + arr[i];
                        }
                    }
                }

                Properties.Settings.Default.pattern_list = str;
                Properties.Settings.Default.Save();
            }
            else
            {
                textBox1.Text = "Invalid index";
            }
        }
                
        private void refreshPatterns()
        {
            Properties.Settings.Default.num_pattern = 0;
            Properties.Settings.Default.pattern_list = "";
            Properties.Settings.Default.Save();

            textBox1.Text = "Database refreshed";
        }

        public static Image resizeImage(Image imgToResize, Size size)
        {
            return (Image)(new Bitmap(imgToResize, size));
        }

        public Bitmap cropBitmap(Bitmap bitmap, int cropX, int cropY, int cropWidth, int cropHeight)
        {
            Rectangle rect = new Rectangle(cropX, cropY, cropWidth, cropHeight);
            Bitmap cropped = bitmap.Clone(rect, System.Drawing.Imaging.PixelFormat.DontCare);
            return cropped;
        }

        private void refreshSettings()
        {
            Properties.Settings.Default.num_pattern = 0;
            Properties.Settings.Default.pattern_list = "";
            Properties.Settings.Default.Save();
        }
    }
}
