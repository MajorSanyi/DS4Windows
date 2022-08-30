using System;
using System.Xml;
using DS4Windows.InputDevices;

namespace DS4WinWPF.DS4Control.ControllerServiceDeviceOptions
{
    public class JoyConControllerOptions : ControllerOptionsStore
    {
        private bool enableHomeLED = true;
        public bool EnableHomeLED
        {
            get => enableHomeLED;
            set
            {
                if (enableHomeLED == value) return;
                enableHomeLED = value;
                EnableHomeLEDChanged?.Invoke(this, EventArgs.Empty);
            }
        }
        public event EventHandler EnableHomeLEDChanged;

        public JoyConControllerOptions(InputDeviceType deviceType) :
            base(deviceType)
        {
        }

        public override void PersistSettings(XmlDocument xmlDoc, XmlNode node)
        {
            XmlNode tempOptsNode = node.SelectSingleNode("JoyConSupportSettings");
            if (tempOptsNode == null)
            {
                tempOptsNode = xmlDoc.CreateElement("JoyConSupportSettings");
            }
            else
            {
                tempOptsNode.RemoveAll();
            }

            XmlNode tempElement = xmlDoc.CreateElement("EnableHomeLED");
            tempElement.InnerText = enableHomeLED.ToString();
            tempOptsNode.AppendChild(tempElement);

            node.AppendChild(tempOptsNode);
        }

        public override void LoadSettings(XmlDocument xmlDoc, XmlNode node)
        {
            XmlNode baseNode = node.SelectSingleNode("JoyConSupportSettings");
            if (baseNode != null)
            {
                XmlNode item = baseNode.SelectSingleNode("EnableHomeLED");
                if (bool.TryParse(item?.InnerText ?? "", out bool temp))
                {
                    enableHomeLED = temp;
                }
            }
        }
    }
}
