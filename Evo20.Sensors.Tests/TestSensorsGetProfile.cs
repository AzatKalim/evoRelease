using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Evo20.Sensors;
using System.IO;
using System.Xml.Serialization;
using System.Xml;

namespace Evo20.Sensors.Tests
{
    public class TestSensorsGetProfile
    {
        static void Main()
        {
            var sensors = new List<ISensor>();
            //sensors.Add(new DLY());
            sensors.Add(new DYS());
            StreamWriter stream = new StreamWriter("profile.txt");

            foreach (var sensor in sensors)
	        {
                var profile = sensor.CalibrationProfile;
                Console.WriteLine(sensor.Name);
                int number=0;
                foreach (var profPart in profile)
                {
                    stream.WriteLine(profPart.SpeedY);
                    number++;
                }
	        }
            stream.Close();
            //Console.ReadLine();
        }
    }
}
