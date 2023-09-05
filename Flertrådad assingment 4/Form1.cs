using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Flertrådad_assingment_4
{
    public partial class Form1 : Form
    {
        Thread update;
        public static string filename;
        public string[] words;
        List<string> strings = new List<string>();
        List<string> allwords = new List<string>();
        List<string> destinationList = new List<string>();

        string findstring, replacestring, destinationtext;

        delegate void uppt();
        delegate void writedele();

        //bool fileopen = false;
        BoundedBuffer buffer;

        Thread read;
        Thread mod;
        Thread wr;
        public Form1()
        {
            InitializeComponent();
        }

        Writer writer;
        Reader reader;
        Modifier modifier;


        private void Form1_Load(object sender, EventArgs e)
        {
            update = new Thread(UppdateT);
            update.Start();
            //richTextBox1.Visible = false;
           
        }


        //open file
        private async void Button1_Click(object sender, EventArgs e)
        {
            allwords.Clear();
            using (OpenFileDialog openFileDialog = new OpenFileDialog() { Filter = "Text File | *.txt", Multiselect = false })
            {
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    StreamReader sr = new StreamReader(openFileDialog.FileName);

                    filename = openFileDialog.FileName;

                    //make a string list and save all the words as different strings so you can find and change them easier
                    GetWords();

                    richTextBox1.Text = await sr.ReadToEndAsync();

                    //Exempel, inte säker hur buffer ska användas än
                    //buffer = new BoundedBuffer(10, richTextBox1, false, findstring, replacestring);
                }
            }
            //writer = new Writer(buffer, allwords);
            //reader = new Reader(buffer, allwords.Count);
            //modifier = new Modifier(buffer, allwords.Count);

            //wr = new Thread(new ThreadStart(writer.Writeloop));
            //mod = new Thread(new ThreadStart(modifier.ModifierLoop));
            //read = new Thread(new ThreadStart(reader.ReadLoop));

            //wr.Start();
            //mod.Start();
            //read.Start();
        }


        //creates a list with all the words in it
        public void GetWords()
        {
            //int x2 = 0;
            strings = ReadFromFile(filename);
            //checks every row
            for (int y = 0; y < strings.Count; y++)
            {
                //seperates the words
                words = strings[y].Split(' ');
                for (int i = 0; i < words.Length; i++)
                {
                    allwords.Add(words[i]);
                }
                allwords.Add("\n");
            }
            //for(int i = 0; i < strings.Count;i++)
            //{
            //    Console.WriteLine("new" + strings[i]);
            //}
            //for (int i = 0; i < allwords.Count; i++)
            //{
            //    Console.Write(allwords[i]);
            //}
        }

        private void UppdateT()
        {
            while (true)
            {
                Thread.Sleep(100);
                Invoke(new uppt(Uppdate));
            }
        }


        //makes tab1 and tab2 change richbox
        private void Uppdate()
        {
            if (tabControl1.SelectedTab.Name == "tabPage1")
            {
                richTextBox1.Visible = true;
                richTextBox2.Visible = false;
            }
            else if (tabControl1.SelectedTab.Name == "tabPage2")
            {
                richTextBox2.Visible = true;
                richTextBox1.Visible = false;
            }
            findstring = textBox1.Text;
            replacestring = textBox2.Text;
            //if (buffer != null)
            //{
            //    buffer.GetModText(destinationList);
            //}
        }
        
        private void TabPage1_Click(object sender, EventArgs e)
        {

        }


        //reads a file and saves the text in different strings
        public List<string> ReadFromFile(string fileName)
        {
            StreamReader sr = new StreamReader(fileName);
            List<string> tempStrings = new List<string>();

            //sparar varje rad i txt filen i olika strings
            while (!sr.EndOfStream)
            {
                string s = sr.ReadLine();
                tempStrings.Add(s);
            }
            sr.Close();
            return tempStrings;
        }


        //highlight
        private void Button3_Click(object sender, EventArgs e)
        {
            //possible to enter more than one word

            if (richTextBox1.Text != "")
            {
                for (int i = 0; i < richTextBox1.TextLength; i++)
                {
                    richTextBox1.Find(textBox1.Text, i, RichTextBoxFinds.MatchCase);
                    richTextBox1.SelectionBackColor = Color.Red;
                }
            }
            else
            {
                for (int i = 0; i < richTextBox1.TextLength; i++)
                {
                    richTextBox1.SelectAll();
                    richTextBox1.SelectionBackColor = Color.White;
                }
            }
        }


        //clear highlight
        private void Button2_Click_1(object sender, EventArgs e)
        {
            richTextBox1.SelectAll();
            richTextBox1.SelectionBackColor = Color.White;
            Console.WriteLine("clear");
        }


        //copy over to destination, makes all the add all the lines into one string
        private void Button4_Click(object sender, EventArgs e)
        {
            if(buffer != null)
            {
                buffer.readcount = 0;
                wr.Abort();
                mod.Abort();
                read.Abort();
            }

            buffer = new BoundedBuffer(10, richTextBox1, findstring, replacestring);
            writer = new Writer(buffer, allwords);
            reader = new Reader(buffer, allwords.Count);
            modifier = new Modifier(buffer, allwords.Count);

            wr = new Thread(new ThreadStart(writer.Writeloop));
            mod = new Thread(new ThreadStart(modifier.ModifierLoop));
            read = new Thread(new ThreadStart(reader.ReadLoop));

            wr.Start();
            mod.Start();
            read.Start();

            //pause here till it was read all the words
            while (buffer.readcount < allwords.Count)
            {
                ;
            }

            buffer.GetModText(destinationList);
            StringBuilder builder = new StringBuilder();
            foreach (string line in destinationList)
            {
                builder.Append(line);
                if (line != "\n")
                {
                    builder.Append(" ");
                }

            }
            destinationtext = builder.ToString();
            richTextBox2.Text = destinationtext;
        }

        private void Button5_Click(object sender, EventArgs e)
        {
            System.IO.File.WriteAllText(@"E:\Programmering\Flertrådad assingment 4\Flertrådad assingment 4\Output.txt", destinationtext);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (update != null)
            {
                update.Abort();
            }
            if (wr != null)
            {
                wr.Abort();
            }
            if (mod != null)
            {
                mod.Abort();
            }
            if (read != null)
            {
                read.Abort();
            }

            Console.WriteLine("Closing app");
            base.OnFormClosing(e);
        }
    }
}
