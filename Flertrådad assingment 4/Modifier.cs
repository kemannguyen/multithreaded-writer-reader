using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flertrådad_assingment_4
{
    class Modifier
    {
        private BoundedBuffer cBuffer;   // The shared resource
        bool isRunning;
        public Modifier(BoundedBuffer buffer, int nrOfStrings)
        {
            cBuffer = buffer;
            isRunning = true;
        }
        public void Start()
        {
            isRunning = true;
        }

        public void Stop()
        {
            isRunning = false;
        }
        public void ModifierLoop()
        {
            while (isRunning)
            {
                cBuffer.Modify();
            }
        }
    }
}

