using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Flertrådad_assingment_4
{
    public enum Status
    {
        Empty,
        Checked,
        New
    }
    public class BoundedBuffer
    {
        string[] strArr;
        Status[] status;


        int max, writePos = 0, readPos = 0, findPos = 0, wordswritten = 0;
        public int readcount = 0;
        RichTextBox rtxBox;
        string findstring, replacestring;

        List<string> modifiedText = new List<string>();

        delegate void Marker();
        delegate void Selector();

        public BoundedBuffer(int max, RichTextBox rtxBox, string findstring, string replacestring)
        {
            this.max = max;
            this.rtxBox = rtxBox;
            this.findstring = findstring;
            this.replacestring = replacestring;
            strArr = new string[max];

            status = new Status[max];
            for (int i = 0; i < max; i++)
            {
                status[i] = Status.Empty;
                Console.WriteLine(status[i]);
            }
        }

        //Checks words in buffer with status new, if the find string is the same as the word checked, modify it and change STATUS to CHECKED!
        public void Modify()
        {
            if (Monitor.TryEnter(strArr))
            {
                try
                {

                    while (status[findPos] != Status.New)
                    {
                        Monitor.Wait(strArr);
                    }

                    Console.Write("M " + findPos + " " + status[findPos] + " " + strArr[findPos] + " ");
                    if (strArr[findPos] == findstring)
                    {
                        strArr[findPos] = replacestring;
                        Console.Write("changed to " + strArr[findPos] + " ");
                    }
                    status[findPos] = Status.Checked;
                    Console.WriteLine(status[findPos]);
                    findPos = (findPos + 1) % max;

                    Monitor.PulseAll(strArr);
                }
                finally
                {
                    Monitor.Exit(strArr);
                }
            }
        }

        //Checks all slots in buffer with status checked and adds it into a List. 
        //Reacount is used in FORM to wait for all words to be read before updating the text!
        public string ReadData()
        {

            string returnString = "";
            if (Monitor.TryEnter(strArr))
            {
                try
                {

                    while (status[readPos] != Status.Checked)
                    {
                        Monitor.Wait(strArr);
                    }

                    Console.WriteLine("R " + status[readPos] + " " + strArr[readPos]);
                    returnString = strArr[readPos];
                    status[readPos] = Status.Empty;
                    readPos = (readPos + 1) % max;
                    Pushback(returnString);
                    readcount++;

                    Monitor.PulseAll(strArr);
                }
                finally
                {
                    Monitor.Exit(strArr);
                }
            }
            return returnString;
        }

        //adds a word in the mod text that will appear in rtb2
        public void Pushback(string s)
        {
            modifiedText.Add(s);
        }

        //if buffer got a empty slot, adds a new word into buffer and increase wordswritten by 1, used in writer
        public void WriteData(string s)
        {
            Monitor.Enter(strArr);
            try
            {
                while (status[writePos] != Status.Empty)
                {
                    Monitor.Wait(strArr);
                }

                Console.WriteLine("W " + status[writePos] + " " + writePos);

                strArr[writePos] = s;
                status[writePos] = Status.New;
                Console.WriteLine(status[writePos] + " " + writePos);
                writePos = (writePos + 1) % max;
                wordswritten++;

                Monitor.PulseAll(strArr);
            }
            finally
            {
                Monitor.Exit(strArr);
            }

        }
        public int wordsWritten()
        {
            return wordswritten;
        }

        //used in form1 to show the modded text in richtextbox 2
        public void GetModText(List<string> list)
        {
            list.Clear();
            foreach (string i in modifiedText)
            {
                list.Add(i);
            }
        }

        //public List<string> ReadFromFile(string fileName)
        //{
        //    StreamReader sr = new StreamReader(fileName);
        //    List<string> tempStrings = new List<string>();

        //    //sparar varje rad i txt filen i olika strings
        //    while (!sr.EndOfStream)
        //    {
        //        string s = sr.ReadLine();
        //        tempStrings.Add(s);
        //    }
        //    sr.Close();
        //    return tempStrings;
        //}
    }
}
