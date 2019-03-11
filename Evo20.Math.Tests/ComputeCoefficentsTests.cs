﻿using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Evo20.Math.Tests
{
    [TestClass]
    public class ComputeCoefficentsTests
    {
        [TestMethod]
        public void TestComputeReferenceAccelerationVectorsDly()
        {
            var result = CalculatorCoefficients.DLYModelVectors();
            var etalon = new[]
            {
                new[] {0, 9.807, 0},
                new[] {-2.538238375, 9.472834578, 0},
                new[] {-4.9035, 8.493111135, 0},
                new[] {-6.934596203, 6.934596203, 0},
                new[] {-8.493111135, 4.9035, 0},
                new[] {-9.472834578, 2.538238375, 0},
                new[] {-9.807, 6.00752E-16, 0},
                new[] {-9.472834578, -2.538238375, 0},
                new[] {-8.493111135, -4.9035, 0},
                new[] {-6.934596203, -6.934596203, 0},
                new[] {-4.9035, -8.493111135, 0},
                new[] {-2.538238375, -9.472834578, 0},
                new[] {-1.2015E-15, -9.807, 0},
                new[] {2.538238375, -9.472834578, 0},
                new[] {4.9035, -8.493111135, 0},
                new[] {6.934596203, -6.934596203, 0},
                new[] {8.493111135, -4.9035, 0},
                new[] {9.472834578, -2.538238375, 0},
                new[] {9.807, -1.80225E-15, 0},
                new[] {9.472834578, 2.538238375, 0},
                new[] {8.493111135, 4.9035, 0},
                new[] {6.934596203, 6.934596203, 0},
                new[] {4.9035, 8.493111135, 0},
                new[] {2.538238375, 9.472834578, 0},
                new[] {0, 9.807, 0},
                new[] {-1.55486E-16, 9.472834578, -2.538238375},
                new[] {-3.00376E-16, 8.493111135, -4.9035},
                new[] {-4.24795E-16, 6.934596203, -6.934596203},
                new[] {-5.20266E-16, 4.9035, -8.493111135},
                new[] {-5.80281E-16, 2.538238375, -9.472834578},
                new[] {-6.00752E-16, 6.00752E-16, -9.807},
                new[] {-5.80281E-16, -2.538238375, -9.472834578},
                new[] {-5.20266E-16, -4.9035, -8.493111135},
                new[] {-4.24795E-16, -6.934596203, -6.934596203},
                new[] {-3.00376E-16, -8.493111135, -4.9035},
                new[] {-1.55486E-16, -9.472834578, -2.538238375},
                new[] {-7.3601E-32, -9.807, -1.2015E-15},
                new[] {1.55486E-16, -9.472834578, 2.538238375},
                new[] {3.00376E-16, -8.493111135, 4.9035},
                new[] {4.24795E-16, -6.934596203, 6.934596203},
                new[] {5.20266E-16, -4.9035, 8.493111135},
                new[] {5.80281E-16, -2.538238375, 9.472834578},
                new[] {6.00752E-16, -1.80225E-15, 9.807},
                new[] {5.80281E-16, 2.538238375, 9.472834578},
                new[] {5.20266E-16, 4.9035, 8.493111135},
                new[] {4.24795E-16, 6.934596203, 6.934596203},
                new[] {3.00376E-16, 8.493111135, 4.9035},
                new[] {1.55486E-16, 9.472834578, 2.538238375},
                new[] {6.00752E-16, 6.00752E-16, -9.807},
                new[] {2.538238375, 6.00752E-16, -9.472834578},
                new[] {4.9035, 6.00752E-16, -8.493111135},
                new[] {6.934596203, 6.00752E-16, -6.934596203},
                new[] {8.493111135, 6.00752E-16, -4.9035},
                new[] {9.472834578, 6.00752E-16, -2.538238375},
                new[] {9.807, 6.00752E-16, 0},
                new[] {9.472834578, 6.00752E-16, 2.538238375},
                new[] {8.493111135, 6.00752E-16, 4.9035},
                new[] {6.934596203, 6.00752E-16, 6.934596203},
                new[] {4.9035, 6.00752E-16, 8.493111135},
                new[] {2.538238375, 6.00752E-16, 9.472834578},
                new[] {6.00752E-16, 6.00752E-16, 9.807},
                new[] {-2.538238375, 6.00752E-16, 9.472834578},
                new[] {-4.903, 6.00752E-16, 8.493111135},
                new[] {-6.934596203, 6.00752E-16, 6.934596203},
                new[] {-8.493111135, 6.00752E-16, 4.9035},
                new[] {-9.472834578, 6.00752E-16, 2.538238375},
                new[] {-9.807, 6.00752E-16, 1.2015E-15},
                new[] {-9.472834578, 6.00752E-16, -2.538238375},
                new[] {-8.493111135, 6.00752E-16, -4.9035},
                new[] {-6.934596203, 6.00752E-16, -6.934596203},
                new[] {-4.9035, 6.00752E-16, -8.493111135},
                new[] {-2.538238375, 6.00752E-16, -9.472834578}
            };
            Assert.AreEqual(result.Length, etalon.Length);
            for (var i = 0; i < result.Length; i++)
            {
                Assert.AreEqual(result[i].Length, etalon[i].Length);
                for (var j = 0; j < result[i].Length; j++)
                    Assert.AreEqual(result[i][j], result[i][j]);
            }
        }

        [TestMethod]
        public void ComputeAMatrix_DYS()
        {
            var result = CalculatorCoefficients.DLYModelVectors();
            var etalon = new[]
            {
                new[] {0, 9.807, 0},
                new[] {-2.538238375, 9.472834578, 0},
                new[] {-4.9035, 8.493111135, 0},
                new[] {-6.934596203, 6.934596203, 0},
                new[] {-8.493111135, 4.9035, 0},
                new[] {-9.472834578, 2.538238375, 0},
                new[] {-9.807, 6.00752E-16, 0},
                new[] {-9.472834578, -2.538238375, 0},
                new[] {-8.493111135, -4.9035, 0},
                new[] {-6.934596203, -6.934596203, 0},
                new[] {-4.9035, -8.493111135, 0},
                new[] {-2.538238375, -9.472834578, 0},
                new[] {-1.2015E-15, -9.807, 0},
                new[] {2.538238375, -9.472834578, 0},
                new[] {4.9035, -8.493111135, 0},
                new[] {6.934596203, -6.934596203, 0},
                new[] {8.493111135, -4.9035, 0},
                new[] {9.472834578, -2.538238375, 0},
                new[] {9.807, -1.80225E-15, 0},
                new[] {9.472834578, 2.538238375, 0},
                new[] {8.493111135, 4.9035, 0},
                new[] {6.934596203, 6.934596203, 0},
                new[] {4.9035, 8.493111135, 0},
                new[] {2.538238375, 9.472834578, 0},
                new[] {0, 9.807, 0},
                new[] {-1.55486E-16, 9.472834578, -2.538238375},
                new[] {-3.00376E-16, 8.493111135, -4.9035},
                new[] {-4.24795E-16, 6.934596203, -6.934596203},
                new[] {-5.20266E-16, 4.9035, -8.493111135},
                new[] {-5.80281E-16, 2.538238375, -9.472834578},
                new[] {-6.00752E-16, 6.00752E-16, -9.807},
                new[] {-5.80281E-16, -2.538238375, -9.472834578},
                new[] {-5.20266E-16, -4.9035, -8.493111135},
                new[] {-4.24795E-16, -6.934596203, -6.934596203},
                new[] {-3.00376E-16, -8.493111135, -4.9035},
                new[] {-1.55486E-16, -9.472834578, -2.538238375},
                new[] {-7.3601E-32, -9.807, -1.2015E-15},
                new[] {1.55486E-16, -9.472834578, 2.538238375},
                new[] {3.00376E-16, -8.493111135, 4.9035},
                new[] {4.24795E-16, -6.934596203, 6.934596203},
                new[] {5.20266E-16, -4.9035, 8.493111135},
                new[] {5.80281E-16, -2.538238375, 9.472834578},
                new[] {6.00752E-16, -1.80225E-15, 9.807},
                new[] {5.80281E-16, 2.538238375, 9.472834578},
                new[] {5.20266E-16, 4.9035, 8.493111135},
                new[] {4.24795E-16, 6.934596203, 6.934596203},
                new[] {3.00376E-16, 8.493111135, 4.9035},
                new[] {1.55486E-16, 9.472834578, 2.538238375},
                new[] {6.00752E-16, 6.00752E-16, -9.807},
                new[] {2.538238375, 6.00752E-16, -9.472834578},
                new[] {4.9035, 6.00752E-16, -8.493111135},
                new[] {6.934596203, 6.00752E-16, -6.934596203},
                new[] {8.493111135, 6.00752E-16, -4.9035},
                new[] {9.472834578, 6.00752E-16, -2.538238375},
                new[] {9.807, 6.00752E-16, 0},
                new[] {9.472834578, 6.00752E-16, 2.538238375},
                new[] {8.493111135, 6.00752E-16, 4.9035},
                new[] {6.934596203, 6.00752E-16, 6.934596203},
                new[] {4.9035, 6.00752E-16, 8.493111135},
                new[] {2.538238375, 6.00752E-16, 9.472834578},
                new[] {6.00752E-16, 6.00752E-16, 9.807},
                new[] {-2.538238375, 6.00752E-16, 9.472834578},
                new[] {-4.903, 6.00752E-16, 8.493111135},
                new[] {-6.934596203, 6.00752E-16, 6.934596203},
                new[] {-8.493111135, 6.00752E-16, 4.9035},
                new[] {-9.472834578, 6.00752E-16, 2.538238375},
                new[] {-9.807, 6.00752E-16, 1.2015E-15},
                new[] {-9.472834578, 6.00752E-16, -2.538238375},
                new[] {-8.493111135, 6.00752E-16, -4.9035},
                new[] {-6.934596203, 6.00752E-16, -6.934596203},
                new[] {-4.9035, 6.00752E-16, -8.493111135},
                new[] {-2.538238375, 6.00752E-16, -9.472834578}
            };
            Assert.AreEqual(result.Length, etalon.Length);
            for (var i = 0; i < result.Length; i++)
            {
                Assert.AreEqual(result[i].Length, etalon[i].Length);
                for (var j = 0; j < result[i].Length; j++)
                    Assert.AreEqual(result[i][j], result[i][j]);
            }
        }
    }
}
