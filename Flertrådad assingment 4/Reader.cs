using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flertrådad_assingment_4
{
    class Reader
    {
        private BoundedBuffer cBuffer;   // The shared resource
        bool isRunning = false;
        public Reader(BoundedBuffer buffer, int nrOfStrings)
        {
            //count = nrOfStrings;
            cBuffer = buffer;

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
        public void ReadLoop()
        {
            //wrong
            while (isRunning)
            {
                cBuffer.ReadData();
                
            }
        }
    }
}
