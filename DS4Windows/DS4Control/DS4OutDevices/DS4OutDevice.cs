using System.Collections.Generic;
using DS4Windows;
using DS4WinWPF.DS4Control.Common;
using Nefarius.ViGEm.Client;
using Nefarius.ViGEm.Client.Targets;
using Nefarius.ViGEm.Client.Targets.DualShock4;

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

        protected abstract void MapAllAxis(DS4State state, SASteeringWheelEmulationAxisType steeringWheelMappedAxis);

        protected static DualShock4DPadDirection GetDpadDirection(DS4State state)
        {
            return state switch
            {
                { DpadUp: true, DpadRight: true } => DualShock4DPadDirection.Northeast,
                { DpadUp: true, DpadLeft: true } => DualShock4DPadDirection.Northeast,
                { DpadUp: true } => DualShock4DPadDirection.North,
                { DpadRight: true, DpadDown: true } => DualShock4DPadDirection.Southeast,
                { DpadRight: true } => DualShock4DPadDirection.East,
                { DpadDown: true, DpadLeft: true } => DualShock4DPadDirection.East,
                { DpadDown: true } => DualShock4DPadDirection.East,
                { DpadLeft: true } => DualShock4DPadDirection.East,
                _ => DualShock4DPadDirection.None,
            };
        }
        protected static DS4AxisDto SetAllAxis(DS4State state, SASteeringWheelEmulationAxisType steeringWheelMappedAxis)
        {
            DS4AxisDto dS4AxisDto = new();

            switch (steeringWheelMappedAxis)
            {
                case SASteeringWheelEmulationAxisType.None:
                    dS4AxisDto.LX = state.LX;
                    dS4AxisDto.LY = state.LY;
                    dS4AxisDto.RX = state.RX;
                    dS4AxisDto.RY = state.RY;
                    break;

                case SASteeringWheelEmulationAxisType.LX:
                    dS4AxisDto.LX = (byte)state.SASteeringWheelEmulationUnit;
                    dS4AxisDto.LY = state.LY;
                    dS4AxisDto.RX = state.RX;
                    dS4AxisDto.RY = state.RY;
                    break;

                case SASteeringWheelEmulationAxisType.LY:
                    dS4AxisDto.LX = state.LX;
                    dS4AxisDto.LY = (byte)state.SASteeringWheelEmulationUnit;
                    dS4AxisDto.RX = state.RX;
                    dS4AxisDto.RY = state.RY;
                    break;

                case SASteeringWheelEmulationAxisType.RX:
                    dS4AxisDto.LX = state.LX;
                    dS4AxisDto.LY = state.LY;
                    dS4AxisDto.RX = (byte)state.SASteeringWheelEmulationUnit;
                    dS4AxisDto.RY = state.RY;
                    break;

                case SASteeringWheelEmulationAxisType.RY:
                    dS4AxisDto.LX = state.LX;
                    dS4AxisDto.LY = state.LY;
                    dS4AxisDto.RX = state.RX;
                    dS4AxisDto.RY = (byte)state.SASteeringWheelEmulationUnit;
                    break;

                case SASteeringWheelEmulationAxisType.L2R2:
                    dS4AxisDto.LeftTrigger = dS4AxisDto.RightTrigger = 0;
                    if (state.SASteeringWheelEmulationUnit >= 0) dS4AxisDto.LeftTrigger = (byte)state.SASteeringWheelEmulationUnit;
                    else dS4AxisDto.RightTrigger = (byte)state.SASteeringWheelEmulationUnit;
                    goto case SASteeringWheelEmulationAxisType.None;

                case SASteeringWheelEmulationAxisType.VJoy1X:
                case SASteeringWheelEmulationAxisType.VJoy2X:
                    DS4Windows.VJoyFeeder.vJoyFeeder.FeedAxisValue(state.SASteeringWheelEmulationUnit, ((uint)steeringWheelMappedAxis - (uint)SASteeringWheelEmulationAxisType.VJoy1X) / 3 + 1, DS4Windows.VJoyFeeder.HID_USAGES.HID_USAGE_X);
                    goto case SASteeringWheelEmulationAxisType.None;

                case SASteeringWheelEmulationAxisType.VJoy1Y:
                case SASteeringWheelEmulationAxisType.VJoy2Y:
                    DS4Windows.VJoyFeeder.vJoyFeeder.FeedAxisValue(state.SASteeringWheelEmulationUnit, ((uint)steeringWheelMappedAxis - (uint)SASteeringWheelEmulationAxisType.VJoy1X) / 3 + 1, DS4Windows.VJoyFeeder.HID_USAGES.HID_USAGE_Y);
                    goto case SASteeringWheelEmulationAxisType.None;

                case SASteeringWheelEmulationAxisType.VJoy1Z:
                case SASteeringWheelEmulationAxisType.VJoy2Z:
                    DS4Windows.VJoyFeeder.vJoyFeeder.FeedAxisValue(state.SASteeringWheelEmulationUnit, ((uint)steeringWheelMappedAxis - (uint)SASteeringWheelEmulationAxisType.VJoy1X) / 3 + 1, DS4Windows.VJoyFeeder.HID_USAGES.HID_USAGE_Z);
                    goto case SASteeringWheelEmulationAxisType.None;

                default:
                    // Should never come here but just in case use the NONE case as default handler....
                    goto case SASteeringWheelEmulationAxisType.None;
            }
            return dS4AxisDto;
        }

    }
}
