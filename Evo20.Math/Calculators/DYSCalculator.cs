using System.Collections.Generic;
using Evo20.Packets;
using Evo20.Sensors;

namespace Evo20.Math.Calculators
{
    public class DYSCalculator : BaseCalculator
    {
        public override double[][] GetModelVectors(Position[] dysProfile)
        {
            var result = new double[dysProfile.Length][];
            for (int i = 0; i < dysProfile.Length; i++)
            {
                result[i] = new double[3];
                result[i][0] = dysProfile[i].SecondPosition == 90 || dysProfile[i].SecondPosition == -90
                    ? dysProfile[i].SpeedFirst
                    : 0;
                result[i][1] = dysProfile[i].SpeedSecond;
                result[i][2] = dysProfile[i].SecondPosition == 90 || dysProfile[i].SecondPosition == -90
                    ? 0
                    : dysProfile[i].SpeedFirst;
            }

            return result;
        }

        public override double[][] ComputeTemperatureCalibrationCoefficents(List<PacketsCollection> packetsCollection,
            int positionCount)
        {
            var mean = new double[packetsCollection.Count][];
            for (var i = 0; i < mean.Length; i++)
            for (var j = 0; j < positionCount; j++)
                mean[i] = packetsCollection[i].MeanUw(j);
            return mean;
        }

        protected override double[][][] GetAMatrix(List<PacketsCollection> packetsCollections, int degree)
        {
            var a = new double[packetsCollections.Count][][];
            for (var i = 0; i < a.Length; i++)
            {
                a[i] = new double[packetsCollections[i].PositionCount][];
                for (var j = 0; j < packetsCollections[i].PositionCount; j++)
                {
                    a[i][j] = new double[1 + 3 * degree];
                    var meanA = packetsCollections[i].MeanW(j);
                    var counter = 0;
                    a[i][j][0] = 1;
                    for (var k = 1; k < degree + 1; k++)
                    {
                        a[i][j][++counter] = System.Math.Pow(meanA[0], k);
                        a[i][j][++counter] = System.Math.Pow(meanA[1], k);
                        a[i][j][++counter] = System.Math.Pow(meanA[2], k);
                    }
                }
            }
            return a;
        }
    }
}
 