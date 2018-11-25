using System.Xml.Serialization;
using System.Xml;
using System;
using System.IO;

namespace Evo20.Sensors
{

    public struct ProfilePart
    {
        int firstPosition;
        [XmlElement("FirstPosition")]
        public int FirstPosition
        {
            set
            {
                firstPosition = value;
            }
            get
            {
                return firstPosition;
            }
        }

        int secondPosition;
        [XmlElement("SecondPosition")]
        public int SecondPosition
        {
            set
            {
                secondPosition = value;
            }
            get
            {
                return secondPosition;
            }
        }

        int speedFirst;
        [XmlElement("SpeedFirst")]
        public int SpeedFirst
        {
            set
            {
                speedFirst = value;
            }
            get
            {
                return speedFirst;
            }
        }

        int speedSecond;
        [XmlElement("SpeedSecond")]
        public int SpeedSecond
        {
            set
            {
                speedSecond = value;
            }
            get
            {
                return speedSecond;
            }
        }

        public ProfilePart(int firstPosition, int secondPosition, int SpeedFirst, int SpeedSecond)
        {
            this.firstPosition = firstPosition;
            this.secondPosition = secondPosition;
            this.speedFirst = SpeedFirst;
            this.speedSecond = SpeedSecond;
        }
        public ProfilePart(int firstPosition, int secondPosition)
            : this(firstPosition, secondPosition, 0, 0) { }

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
