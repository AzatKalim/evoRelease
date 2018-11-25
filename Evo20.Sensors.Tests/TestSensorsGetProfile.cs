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
            var sensors = new List<BKV1>();
            sensors.Add(new DLY());
            sensors.Add(new DYS());

            foreach (var sensor in sensors)
	        {
                sensor.WriteCalibrationProfile();
                //sensor.GetCalibrationProfileFromFile();
	        }
            //Console.ReadLine();
        }
    }
}
