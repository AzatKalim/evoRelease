using System.Xml.Serialization;
using System.Xml;
using System;
using System.IO;

namespace Evo20.Sensors
{

    public struct ProfilePart
    {
        int axisX;
        [XmlElement("AxisX")]
        public int AxisX
        {
            set
            {
                axisX = value;
            }
            get
            {
                return axisX;
            }
        }

        int axisY;
        [XmlElement("AxisY")]
        public int AxisY
        {
            set
            {
                axisY = value;
            }
            get
            {
                return axisY;
            }
        }

        int speedX;
        [XmlElement("SpeedX")]
        public int SpeedX
        {
            set
            {
                speedX = value;
            }
            get
            {
                return speedX;
            }
        }

        int speedY;
        [XmlElement("SpeedY")]
        public int SpeedY
        {
            set
            {
                speedY = value;
            }
            get
            {
                return speedY;
            }
        }

        public ProfilePart(int x, int y, int speedX, int speedY)
        {
            axisX = x;
            axisY = y;
            this.speedX = speedX;
            this.speedY = speedY;
        }
        public ProfilePart(int x, int y)
            : this(x, y, 0, 0) { }

        public string Serialize()
        {
            XmlSerializer xmler = new XmlSerializer(GetType());
            StringWriter writer = new StringWriter();
            xmler.Serialize(writer, this);
            return writer.ToString();
        }

        public static ProfilePart Deserialize(string text)
        {
            try
            {
                XmlSerializer xmler = new XmlSerializer(typeof(ProfilePart));
                StringReader reader = new StringReader(text);
                return (ProfilePart)xmler.Deserialize(reader);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new ProfilePart();
            }
        }
    }
}
