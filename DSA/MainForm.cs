using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace console
{
    public partial class MainForm : Form
    {
        byte[] sourceBytes;
        string sourceText;
        string filename;
        public int q;
        public int p;
        public int h;
        public int x;
        public int k;
        public int g;
        public int y;
        
        public MainForm()
        {
            InitializeComponent();
        }

        public Boolean ValidatePapams()
        {
            if (filename == null)
            {
                MessageBox.Show("Файл не открыт");
                return false;
            }
            
            try
            {
                q = Convert.ToInt32(tbQ.Text);

                if (q<1 || !DSA.IsPrime(q,20))
                {
                    throw new Exception();
                }
               
            }
            catch (Exception)
            {
                MessageBox.Show("Параметр 'q' должен быть простым числом");
                return false;
            }

            try
            {
                p = Convert.ToInt32(tbP.Text);
                if (p<1 || !DSA.IsPrime(p,20))
                { 
                    throw new Exception();
                }
            }
            catch
            {
                MessageBox.Show("Параметр 'p' должен быть простым числом");
                return false;
            }

            try
            {
                if ((p -1) % q != 0)
                {
                    throw new Exception();
                }

            }
            catch
            {
                MessageBox.Show("q должен быть делителем (p-1)");
                return false;
            }

            try
            {
                h = Convert.ToInt32(tbH.Text);

                if (h<=1 || h >= (p - 1))
                {
                    throw new Exception();
                }
            }
            catch
            {
                MessageBox.Show("h должно принадлежать интервалу (1, р-1)");
                return false;
            }

            try
            {
                g = DSA.Power(h, (p - 1) / q, p);

                if (g<2)
                {
                    throw new Exception();
                }
            }
            catch
            {
                MessageBox.Show("g должно быть больше 1, где g = h^((p-1)/q) mod p");
                return false;
            }

            try
            {
                x = Convert.ToInt32(tbX.Text);

                if (x<=0 || x>=q)
                {
                    throw new Exception();
                }
            }
            catch
            {
                MessageBox.Show("x должен принадлежать интервалу (0, q)");
                return false;
            }

            try
            {
                k = Convert.ToInt32(tbK.Text);

                if (k<=0 || k>=q)
                {
                    throw new Exception();
                }
            }
            catch
            {
                MessageBox.Show("k должно принадлежать интервалу (0, q)");
                return false;
            }

            try
            {
                y = DSA.Power(g, x, p);
            }
            catch
            {
                MessageBox.Show("Ошибка при вычислении y");
                return false;
            }



            return true;
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            if (odOpen.ShowDialog() == DialogResult.Cancel)
                return;

            filename = odOpen.FileName;
            sourceText = System.IO.File.ReadAllText(filename);
            tbSource.Text = sourceText;

            sourceBytes = System.IO.File.ReadAllBytes(filename);
            MessageBox.Show("Файл открыт");
        }

        private void btnSign_Click(object sender, EventArgs e)
        {
            if (!ValidatePapams())
                return;
            int h;
            int r;
            int s;
            (h, r, s) = DSA.GetSignature(sourceBytes, q, p, x, k, g);

            if (r == 0)
            {
                MessageBox.Show("Выберите другое k, т. к. r=0");
                return;
            }

            if (s == 0)
            {
                MessageBox.Show("Выберите другое k, т. к. s=0");
                return;
            }

            tbResult.Text = "h=" + h.ToString() + ' ' + "r=" + r.ToString() + ' ' + "s=" + s.ToString();

            sourceText += ' '+r.ToString()+' '+s.ToString();

            tbSource.Text = sourceText;

            System.IO.File.WriteAllText(filename, sourceText);
        }

        private void btnCheck_Click(object sender, EventArgs e)
        {
            if (!ValidatePapams())
                return;

            int _s;
            int _r;
            int _h;
            
            string[] words = sourceText.TrimEnd().Split(' ');
            try 
            {
                _s = Convert.ToInt32(words[words.Length - 1]);
                _r = Convert.ToInt32(words[words.Length - 2]);
            }
            catch
            {
                MessageBox.Show("Файл не подписан");
                return;
            }

            int len = words[words.Length - 1].Length + words[words.Length - 2].Length + 2;

            string text = sourceText.TrimEnd().Substring(0, sourceText.Length - len);

            byte[] data = Encoding.UTF8.GetBytes(text);

            _h = DSA.GetHash(data, q);

            int w = DSA.Power(_s, q - 2, q);
            int u1 = (_h * w) % q;
            int u2 = (_r * w) % q;
            int v = DSA.Power(g, u1, p) * DSA.Power(y, u2, p)% p % q;

            string res = "h="+_h.ToString()+' ';

            int s = (DSA.Power(k, q - 2, q) * (_h + x * _r)) % q;
            

            if (v == _r)
            {
                res += _r.ToString() + '=' + v.ToString() + ' ';
            }
            else
            {
                res += _r.ToString() + "!=" + v.ToString() + ' ';
            }

            if (s == _s)
            {
                res += _s.ToString() + '=' + s.ToString() + ' ';
            }
            else
            {
                res += _s.ToString() + "!=" + s.ToString() + ' ';
            }

            if (v == _r && _s == s)
            {
                MessageBox.Show("Подпись подлинна");
            }
            else
            {
                MessageBox.Show("Подпись не подлинна");
            }

            tbResult.Text = res;





        }
    }
}
