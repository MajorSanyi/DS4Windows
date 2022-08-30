using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DS4WinWPF.DS4Control.DS4OutDevices
{
    internal class DS4AxisDto
    {
        public byte LX { get; set; }
        public byte LY { get; set; }
        public byte RX { get; set; }
        public byte RY { get; set; }
        public byte LeftTrigger { get; set; }
        public byte RightTrigger { get; set; }
    }
}
