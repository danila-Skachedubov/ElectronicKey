using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ElectronicKey
{
   
    public partial class MainWindow : Window
    {
        char[] characters = new char[] { '#', '1', '2', '3', '4', '5', '6', '7', '8', '9', '0', '-' };
        string sourceFilePath = "";
        string signFilePath = "";
        public MainWindow()
        {
            InitializeComponent();
        }
        private void buttonEncrypt_Click(object sender, EventArgs e)
        {
            if ((textBox_p.Text.Length > 0) && (textBox_q.Text.Length > 0) && (sourceFilePathTextBox.Text.Length > 0) && (signFilePathTextBox.Text.Length > 0))
            {
                long p = Convert.ToInt64(textBox_p.Text);
                long q = Convert.ToInt64(textBox_q.Text);

                if (IsTheNumberSimple(p) && IsTheNumberSimple(q))
                {
                    string hash = File.ReadAllText(sourceFilePath).GetHashCode().ToString();

                    long n = p * q;
                    long m = (p - 1) * (q - 1);
                    long d = Calculate_d(m);
                    long e_ = Calculate_e(d, m);

                    List<string> result = RSA_Endoce(hash, e_, n);

                    StreamWriter sw = new StreamWriter(signFilePath);
                    foreach (string item in result)
                        sw.WriteLine(item);
                    sw.Close();

                    textBox_d.Text = d.ToString();
                    textBox_n.Text = n.ToString();


                }
                else
                    MessageBox.Show("p или q - не простые числа!");
            }
            else
                MessageBox.Show("Введите p и q и пути к файлам!");
        }
        private bool IsTheNumberSimple(long n)
        {
            if (n < 2)
                return false;

            if (n == 2)
                return true;

            for (long i = 2; i < n; i++)
                if (n % i == 0)
                    return false;

            return true;
        }


        private List<string> RSA_Endoce(string s, long e, long n)
        {
            List<string> result = new List<string>();

            BigInteger bi;

            for (int i = 0; i < s.Length; i++)
            {
                int index = Array.IndexOf(characters, s[i]);

                bi = new BigInteger(index);
                bi = BigInteger.Pow(bi, (int)e);

                BigInteger n_ = new BigInteger((int)n);

                bi = bi % n_;

                result.Add(bi.ToString());
            }

            return result;
        }

        private string RSA_Dedoce(List<string> input, long d, long n)
        {
            string result = "";

            BigInteger bi;

            foreach (string item in input)
            {
                bi = new BigInteger(Convert.ToDouble(item));
                bi = BigInteger.Pow(bi, (int)d);

                BigInteger n_ = new BigInteger((int)n);

                bi = bi % n_;

                int index = Convert.ToInt32(bi.ToString());

                result += characters[index].ToString();
            }

            return result;
        }

        private long Calculate_d(long m)
        {
            long d = m - 1;

            for (long i = 2; i <= m; i++)
                if ((m % i == 0) && (d % i == 0)) 
                {
                    d--;
                    i = 1;
                }

            return d;
        }


        private long Calculate_e(long d, long m)
        {
            long e = 10;

            while (true)
            {
                if ((e * d) % m == 1)
                    break;
                else
                    e++;
            }

            return e;
        }

        private void sourceFileButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();

            ofd.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            


            if (ofd.ShowDialog() == true)
            {
                FileInfo fileInfo = new FileInfo(ofd.FileName);
                sourceFilePath = ofd.FileName;
                sourceFilePathTextBox.Text = fileInfo.Name;
            }
        }

        private void signFileButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();

            ofd.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";

            if (ofd.ShowDialog() == true)
            {
                FileInfo fileInfo = new FileInfo(ofd.FileName);
                signFilePath = ofd.FileName;
                signFilePathTextBox.Text = fileInfo.Name;
            }
        }

        private void buttonDecipher_Click(object sender, EventArgs e)
        {
            if ((textBox_d.Text.Length > 0) && (textBox_n.Text.Length > 0) && (sourceFilePathTextBox.Text.Length > 0) && (signFilePathTextBox.Text.Length > 0))
            {
                long d = Convert.ToInt64(textBox_d.Text);
                long n = Convert.ToInt64(textBox_n.Text);

                List<string> input = new List<string>();

                StreamReader sr = new StreamReader(signFilePath);

                while (!sr.EndOfStream)
                {
                    input.Add(sr.ReadLine());
                }

                sr.Close();

                string result = RSA_Dedoce(input, d, n);

                string hash = File.ReadAllText(sourceFilePath).GetHashCode().ToString();

                if (result.Equals(hash))
                    MessageBox.Show("Файл подлинный. Подпись верна.");
                else
                    MessageBox.Show("Внимание! Файл НЕ подлинный!!!");
            }
            else
                MessageBox.Show("Введите секретный ключ и пути к файлам!");
        }

        private void instruction(object sender, EventArgs e)
        {
            MessageBox.Show("1. Выберите файл на который требуется электронная подпись" + Environment.NewLine
                          + "2. Выберите файл с подписью" + Environment.NewLine 
                          + "3. Введите два взаимно простых числа" + Environment.NewLine
                          + "4. Нажмите копку подписать" + Environment.NewLine
                          + "5. Дя проверки подписи нажмите соответсвующую кнопку"
                );
        }

    }
}
