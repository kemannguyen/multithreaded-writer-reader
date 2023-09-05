using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Flertrådad_assingment_4
{
    class Writer
    {
        BoundedBuffer cBuffer;
        List<string> cList;
        bool isRunning = false;
        int max;
        public Writer(BoundedBuffer buffer, List<string> list)
        {
            cBuffer = buffer;
            cList = list;
            max = list.Count;
            Console.WriteLine("max : " +max);
            isRunning = true;
        }
        public void Stop()
        {
            isRunning = false;
        }
        public void Start()
        {
            isRunning = true;
        }
        public void Writeloop()
        {
            while (isRunning && cBuffer.wordsWritten() < max)
            {
                cBuffer.WriteData(cList[cBuffer.wordsWritten()]);
            }
        }
    }
}
