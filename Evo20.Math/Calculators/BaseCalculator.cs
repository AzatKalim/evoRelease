using System.Collections.Generic;
using Evo20.Packets;
using Evo20.Sensors;
using Evo20.Utils;

namespace Evo20.Math
{
    public abstract class BaseCalculator
    {
        private double[][][] ComputeCalibrationCoefficents(double[][][] a, double[][] b)
        {
            var result = new double[a.Length][][];
            for (var i = 0; i < result.Length; i++)
            {
                result[i] = OneCompute(a[i], b);
            }

            return result;
        }
        public abstract double[][] GetModelVectors(Position[] profile);

        public abstract double[][] ComputeTemperatureCalibrationCoefficents(List<PacketsCollection> packetsCollection,
            int positionCount);

        protected abstract double[][][] GetAMatrix(List<PacketsCollection> packetsCollections, int degree);

        public double[][] OneCompute(double[][] a, double[][] b)
        {
            var aTrans = a.Transpose();
            var step1 = Matrix.Multiply(aTrans, a);
            var step2 = step1.Inverse();
            var step3 = Matrix.Multiply(aTrans, b);
            return Matrix.Multiply(step2, step3);
        }


        public double[][][] ComputeCalibrationCoefficents(List<PacketsCollection> packetsCollection,
            Position[] profile, int degree)
        {
            Log.Instance.Info($"Рассчет {GetType().FullName}");
            var a = GetAMatrix(packetsCollection, degree);
            var b = GetModelVectors(profile);
            return ComputeCalibrationCoefficents(a, b);
        }
    }
}
