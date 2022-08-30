using System;
using System.Xml;
using DS4Windows.InputDevices;

namespace DS4WinWPF.DS4Control.ControllerServiceDeviceOptions
{
    public class DualSenseControllerOptions : ControllerOptionsStore
    {
        public enum LEDBarMode : ushort
        {
            Off,
            MultipleControllers,
            BatteryPercentage,
            On,
        }

        public enum MuteLEDMode : ushort
        {
            Off,
            On,
            Pulse
        }

        private bool enableRumble = true;
        public bool EnableRumble
        {
            get => enableRumble;
            set
            {
                if (enableRumble == value) return;
                enableRumble = value;
                EnableRumbleChanged?.Invoke(this, EventArgs.Empty);
            }
        }
        public event EventHandler EnableRumbleChanged;

        private DualSenseDevice.HapticIntensity hapticIntensity = DualSenseDevice.HapticIntensity.Medium;
        public DualSenseDevice.HapticIntensity HapticIntensity
        {
            get => hapticIntensity;
            set
            {
                if (hapticIntensity == value) return;
                hapticIntensity = value;
                HapticIntensityChanged?.Invoke(this, EventArgs.Empty);
            }
        }
        public event EventHandler HapticIntensityChanged;

        private LEDBarMode ledMode = LEDBarMode.MultipleControllers;
        public LEDBarMode LedMode
        {
            get => ledMode;
            set
            {
                if (ledMode == value) return;
                ledMode = value;
                LedModeChanged?.Invoke(this, EventArgs.Empty);
            }
        }
        public event EventHandler LedModeChanged;

        private MuteLEDMode muteLedMode = MuteLEDMode.Off;
        public MuteLEDMode MuteLedMode
        {
            get => muteLedMode;
            set
            {
                if (muteLedMode == value) return;
                muteLedMode = value;
                MuteLedModeChanged?.Invoke(this, EventArgs.Empty);
            }
        }
        public event EventHandler MuteLedModeChanged;

        public DualSenseControllerOptions(InputDeviceType deviceType) :
            base(deviceType)
        {
        }

        public override void PersistSettings(XmlDocument xmlDoc, XmlNode node)
        {
            XmlNode tempOptsNode = node.SelectSingleNode("DualSenseSupportSettings");
            if (tempOptsNode == null)
            {
                tempOptsNode = xmlDoc.CreateElement("DualSenseSupportSettings");
            }
            else
            {
                tempOptsNode.RemoveAll();
            }

            XmlNode tempRumbleNode = xmlDoc.CreateElement("EnableRumble");
            tempRumbleNode.InnerText = enableRumble.ToString();
            tempOptsNode.AppendChild(tempRumbleNode);

            XmlNode tempRumbleStrengthNode = xmlDoc.CreateElement("RumbleStrength");
            tempRumbleStrengthNode.InnerText = hapticIntensity.ToString();
            tempOptsNode.AppendChild(tempRumbleStrengthNode);

            XmlNode tempLedMode = xmlDoc.CreateElement("LEDBarMode");
            tempLedMode.InnerText = ledMode.ToString();
            tempOptsNode.AppendChild(tempLedMode);

            XmlNode tempMuteLedMode = xmlDoc.CreateElement("MuteLEDMode");
            tempMuteLedMode.InnerText = muteLedMode.ToString();
            tempOptsNode.AppendChild(tempMuteLedMode);

            node.AppendChild(tempOptsNode);
        }

        public override void LoadSettings(XmlDocument xmlDoc, XmlNode node)
        {
            XmlNode baseNode = node.SelectSingleNode("DualSenseSupportSettings");
            if (baseNode != null)
            {
                XmlNode item = baseNode.SelectSingleNode("EnableRumble");
                if (bool.TryParse(item?.InnerText ?? "", out bool temp))
                {
                    enableRumble = temp;
                }

                XmlNode itemStrength = baseNode.SelectSingleNode("RumbleStrength");
                if (Enum.TryParse(itemStrength?.InnerText ?? "",
                    out DualSenseDevice.HapticIntensity tempHap))
                {
                    hapticIntensity = tempHap;
                }

                XmlNode itemLedMode = baseNode.SelectSingleNode("LEDBarMode");
                if (Enum.TryParse(itemLedMode?.InnerText ?? "",
                    out LEDBarMode tempLED))
                {
                    ledMode = tempLED;
                }

                XmlNode itemMuteLedMode = baseNode.SelectSingleNode("MuteLEDMode");
                if (Enum.TryParse(itemMuteLedMode?.InnerText ?? "",
                    out MuteLEDMode tempMuteLED))
                {
                    muteLedMode = tempMuteLED;
                }
            }
        }
    }
}
