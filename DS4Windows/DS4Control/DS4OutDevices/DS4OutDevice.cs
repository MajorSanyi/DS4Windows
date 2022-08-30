using System.Collections.Generic;
using DS4WinWPF.DS4Control.Common;
using Nefarius.ViGEm.Client;
using Nefarius.ViGEm.Client.Targets;

namespace DS4WinWPF.DS4Control.DS4OutDevices
{
    abstract class DS4OutDevice : OutputDevice
    {
        private const string devtype = "DS4";

        //public DualShock4FeedbackReceivedEventHandler forceFeedbackCall;
        public Dictionary<int, DualShock4FeedbackReceivedEventHandler> ForceFeedbacksDict => new();
        public override string GetDeviceType => devtype;

        public IDualShock4Controller Cont { get; set; }

        public DS4OutDevice(ViGEmClient client)
        {
            Cont = client.CreateDualShock4Controller();
            //cont = client.CreateDualShock4Controller(0x054C, 0x09CC);
            Cont.AutoSubmitReport = false;
        }

        public override void Connect()
        {
            Cont.Connect();
            connected = true;
        }

        public override void Disconnect()
        {
            foreach (KeyValuePair<int, DualShock4FeedbackReceivedEventHandler> pair in ForceFeedbacksDict)
            {
                Cont.FeedbackReceived -= pair.Value;
            }

            ForceFeedbacksDict.Clear();

            connected = false;
            Cont.Disconnect();
            //cont.Dispose();
            Cont = null;
        }

        public override void RemoveFeedbacks()
        {
            foreach (KeyValuePair<int, DualShock4FeedbackReceivedEventHandler> pair in ForceFeedbacksDict)
            {
                Cont.FeedbackReceived -= pair.Value;
            }

            ForceFeedbacksDict.Clear();
        }

        public override void RemoveFeedback(int inIdx)
        {
            if (ForceFeedbacksDict.TryGetValue(inIdx, out DualShock4FeedbackReceivedEventHandler handler))
            {
                Cont.FeedbackReceived -= handler;
                ForceFeedbacksDict.Remove(inIdx);
            }
        }
    }
}
