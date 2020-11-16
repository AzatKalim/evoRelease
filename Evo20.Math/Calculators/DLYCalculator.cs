using System.Collections.Generic;
using System.Linq;
using Evo20.Packets;
using Evo20.Sensors;
using Evo20.Utils;

namespace Evo20.Math.Calculators
{
    public class DLYCalculator : BaseCalculator
    {
        private const double Factor = 9.807;

        public override double[][] GetModelVectors(Position[] profile)
        {
            Log.Instance.Info("Эталонные вектора ДЛУ");

            var b = new double[profile.Length][];
            var dlyMatrix = ProfileToPositionArray(profile);
            for (var n = 0; n < profile.Length; n++)
            {
                b[n] = new double[3];
                b[n][0] = -Factor * System.Math.Sin(dlyMatrix[n][1] * System.Math.PI / 180) *
                          System.Math.Cos(dlyMatrix[n][0] * System.Math.PI / 180);
                b[n][1] = Factor * System.Math.Cos(dlyMatrix[n][1] * System.Math.PI / 180);
                b[n][2] = -Factor * System.Math.Sin(dlyMatrix[n][1] * System.Math.PI / 180) *
                          System.Math.Sin(dlyMatrix[n][0] * System.Math.PI / 180);
            }
            return b;
        }

        private static int[][] ProfileToPositionArray(Position[] profile)
        {
            return profile.Select(position => new[] {(int) position.SecondPosition, (int) position.FirstPosition})
                .ToArray();
        }
        public override double[][] ComputeTemperatureCalibrationCoefficents(List<PacketsCollection> packetsCollection, int positionCount)
        {
            var mean = new double[packetsCollection.Count][];
            for (var i = 0; i < mean.Length; i++)
            for (var j = 0; j < positionCount; j++)
                mean[i] = packetsCollection[i].MeanUa(j);
            return mean;
        }

        protected override double[][][] GetAMatrix(List<PacketsCollection> packetsCollections, int degree)
        {
            Log.Instance.Info($"{nameof(GetAMatrix)} degree: {degree}");
            var a = new double[packetsCollections.Count][][];
            for (var i = 0; i < a.Length; i++)
            {
                a[i] = new double[packetsCollections[i].PositionCount][];
                for (var j = 0; j < packetsCollections[i].PositionCount; j++)
                {
                    a[i][j] = new double[1 + 3 * degree];
                    var meanA = packetsCollections[i].MeanA(j);
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
