﻿using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Evo20.Math.Tests
{
    [TestClass]
    public class MatrixTests
    {
        [TestMethod]
        public void Transpose_CorrectMatrix_Transposed()
        {
            var testMatrix = new[]
            {
                new[] {1, 0.001514617, 0.041224963, 0.000692487},
                new[] {1, 0.001511852, 0.041230287, 0.000698511},
                new[] {1, 0.001519228, 0.041221091, 0.000697962},
                new[] {1, 0.001525944, 0.041229624, 0.000697654},
                new[] {1, 0.001519112, 0.041221233, 0.000695516},
                new[] {1, 0.001529195, 0.041230677, 0.000698093},
                new[] {1, 0.001523981, 0.041229313, 0.000705972},
                new[] {1, 0.00151074, 0.041222127, 0.000704023},
                new[] {1, 0.001504573, 0.041234232, 0.00071088},
                new[] {1, 0.001506762, 0.041232977, 0.000701833},
                new[] {1, 0.001502421, 0.041228495, 0.000711149},
                new[] {1, 0.001508562, 0.041230616, 0.000691674},
                new[] {1, 0.001502162, 0.041232187, 0.000704754},
                new[] {1, 0.00150467, 0.041227608, 0.000709712},
                new[] {1, 0.001507368, 0.041233016, 0.000704365},
                new[] {1, 0.001514462, 0.04123052, 0.000697819},
                new[] {1, 0.001510351, 0.041230211, 0.00068747},
                new[] {1, 0.001515946, 0.041228545, 0.000711781},
                new[] {1, 0.00150646, 0.04122812, 0.000708392},
                new[] {1, 0.001510948, 0.041232811, 0.00069941},
                new[] {1, 0.001506822, 0.041233342, 0.000701111},
                new[] {1, 0.00150765, 0.04122779, 0.000697367},
                new[] {1, 0.001512178, 0.041234654, 0.000700436},
                new[] {1, 0.001511332, 0.041231724, 0.000709358},
                new[] {1, 0.001511483, 0.041233621, 0.000704921},
                new[] {1, 0.001508484, 0.041235176, 0.000708715},
                new[] {1, 0.001511663, 0.04123951, 0.000705715},
                new[] {1, 0.001511791, 0.041233017, 0.000706388},
                new[] {1, 0.001510786, 0.041238972, 0.000700007},
                new[] {1, 0.001514463, 0.041228209, 0.000705676},
                new[] {1, 0.001506483, 0.041223239, 0.000693929},
                new[] {1, 0.001506884, 0.041233504, 0.000694489},
                new[] {1, 0.001507802, 0.041232365, 0.000708645},
                new[] {1, 0.001508491, 0.041229436, 0.000704978},
                new[] {1, 0.001504569, 0.041221811, 0.000705013},
                new[] {1, 0.001502085, 0.041228754, 0.000705741},
                new[] {1, 0.001510783, 0.041236383, 0.000698818},
                new[] {1, 0.001507575, 0.041233801, 0.000698473},
                new[] {1, 0.001513605, 0.041234339, 0.000691129},
                new[] {1, 0.001506453, 0.041237151, 0.000694209},
                new[] {1, 0.001515155, 0.041230592, 0.000711079},
                new[] {1, 0.001503252, 0.041237822, 0.00069494},
                new[] {1, 0.001506383, 0.041224975, 0.000697217},
                new[] {1, 0.001507413, 0.04123246, 0.000703204},
                new[] {1, 0.001505914, 0.041241641, 0.000700342},
                new[] {1, 0.001505588, 0.041235046, 0.000691785},
                new[] {1, 0.001498904, 0.041236915, 0.000695437},
                new[] {1, 0.001507378, 0.041237202, 0.000701285},
                new[] {1, 0.001504451, 0.041238204, 0.000698079},
                new[] {1, 0.001508158, 0.041235298, 0.000694824},
                new[] {1, 0.001511124, 0.041236489, 0.000707557},
                new[] {1, 0.001519308, 0.041241566, 0.000706405},
                new[] {1, 0.001505805, 0.04123573, 0.000703969},
                new[] {1, 0.001511115, 0.0412407, 0.000702185},
                new[] {1, 0.001517014, 0.041232295, 0.000704706},
                new[] {1, 0.001517423, 0.041235838, 0.000714948},
                new[] {1, 0.001514827, 0.041230341, 0.000706648},
                new[] {1, 0.001511245, 0.041227143, 0.000697455},
                new[] {1, 0.001516522, 0.041232063, 0.000698763},
                new[] {1, 0.001517871, 0.041236705, 0.000698685},
                new[] {1, 0.001515548, 0.041234768, 0.000688486},
                new[] {1, 0.00152115, 0.04124174, 0.000699308},
                new[] {1, 0.001508581, 0.04122967, 0.000700346},
                new[] {1, 0.001515339, 0.041238837, 0.000699954},
                new[] {1, 0.0015131, 0.041249421, 0.000697258},
                new[] {1, 0.001516544, 0.041236985, 0.000704364},
                new[] {1, 0.001512148, 0.041245288, 0.000695847},
                new[] {1, 0.001516427, 0.041237314, 0.000695351},
                new[] {1, 0.001507034, 0.041240832, 0.000688063},
                new[] {1, 0.00151031, 0.041238601, 0.000703522},
                new[] {1, 0.001502587, 0.041233984, 0.000699248},
                new[] {1, 0.001508637, 0.041243435, 0.000691392}
            };
            var etalonDoubles = new[]
            {
                new double []
                {
                    1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
                    1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
                    1, 1, 1, 1, 1, 1
                },
                new[]
                {
                    0.001514617, 0.001511852, 0.001519228, 0.001525944, 0.001519112, 0.001529195, 0.001523981,
                    0.00151074, 0.001504573, 0.001506762, 0.001502421, 0.001508562, 0.001502162, 0.00150467,
                    0.001507368, 0.001514462, 0.001510351, 0.001515946, 0.00150646, 0.001510948, 0.001506822,
                    0.00150765, 0.001512178, 0.001511332, 0.001511483, 0.001508484, 0.001511663, 0.001511791,
                    0.001510786, 0.001514463, 0.001506483, 0.001506884, 0.001507802, 0.001508491, 0.001504569,
                    0.001502085, 0.001510783, 0.001507575, 0.001513605, 0.001506453, 0.001515155, 0.001503252,
                    0.001506383, 0.001507413, 0.001505914, 0.001505588, 0.001498904, 0.001507378, 0.001504451,
                    0.001508158, 0.001511124, 0.001519308, 0.001505805, 0.001511115, 0.001517014, 0.001517423,
                    0.001514827, 0.001511245, 0.001516522, 0.001517871, 0.001515548, 0.00152115, 0.001508581,
                    0.001515339, 0.0015131, 0.001516544, 0.001512148, 0.001516427, 0.001507034, 0.00151031, 0.001502587,
                    0.001508637
                },
                new[]
                {
                    0.041224963, 0.041230287, 0.041221091, 0.041229624, 0.041221233, 0.041230677, 0.041229313,
                    0.041222127, 0.041234232, 0.041232977, 0.041228495, 0.041230616, 0.041232187, 0.041227608,
                    0.041233016, 0.04123052, 0.041230211, 0.041228545, 0.04122812, 0.041232811, 0.041233342, 0.04122779,
                    0.041234654, 0.041231724, 0.041233621, 0.041235176, 0.04123951, 0.041233017, 0.041238972,
                    0.041228209, 0.041223239, 0.041233504, 0.041232365, 0.041229436, 0.041221811, 0.041228754,
                    0.041236383, 0.041233801, 0.041234339, 0.041237151, 0.041230592, 0.041237822, 0.041224975,
                    0.04123246, 0.041241641, 0.041235046, 0.041236915, 0.041237202, 0.041238204, 0.041235298,
                    0.041236489, 0.041241566, 0.04123573, 0.0412407, 0.041232295, 0.041235838, 0.041230341, 0.041227143,
                    0.041232063, 0.041236705, 0.041234768, 0.04124174, 0.04122967, 0.041238837, 0.041249421,
                    0.041236985, 0.041245288, 0.041237314, 0.041240832, 0.041238601, 0.041233984, 0.041243435
                },
                new[]
                {
                    0.000692487, 0.000698511, 0.000697962, 0.000697654, 0.000695516, 0.000698093, 0.000705972,
                    0.000704023, 0.00071088, 0.000701833, 0.000711149, 0.000691674, 0.000704754, 0.000709712,
                    0.000704365, 0.000697819, 0.00068747, 0.000711781, 0.000708392, 0.00069941, 0.000701111,
                    0.000697367, 0.000700436, 0.000709358, 0.000704921, 0.000708715, 0.000705715, 0.000706388,
                    0.000700007, 0.000705676, 0.000693929, 0.000694489, 0.000708645, 0.000704978, 0.000705013,
                    0.000705741, 0.000698818, 0.000698473, 0.000691129, 0.000694209, 0.000711079, 0.00069494,
                    0.000697217, 0.000703204, 0.000700342, 0.000691785, 0.000695437, 0.000701285, 0.000698079,
                    0.000694824, 0.000707557, 0.000706405, 0.000703969, 0.000702185, 0.000704706, 0.000714948,
                    0.000706648, 0.000697455, 0.000698763, 0.000698685, 0.000688486, 0.000699308, 0.000700346,
                    0.000699954, 0.000697258, 0.000704364, 0.000695847, 0.000695351, 0.000688063, 0.000703522,
                    0.000699248, 0.000691392
                }
            };

            var testResult = testMatrix.Transpose();

            AssertEquals(testResult, etalonDoubles);
        }

        [TestMethod]
        public void Invers_CorrectMatrix_Inverted()
        {
            var testMatrix = new[]
            {
                new[] {770408.7388, -1201409.853, -18596518.9, -2566061.553},
                new[] {-1201409.853, 415312589.3, 13906557.62, 730126.8967},
                new[] {-18596518.9, 13906557.62, 449549927.9, 55786398.59},
                new[] {-2566061.553, 730126.8967, 55786398.59, 377718052.4}
            };
            var etalonDoubles = new[]
            {
                new[] {71.99999926, 0.10878299,  2.968795322, 0.050457226},
                new[] {0.10878299 , 0.00016436 , 0.004485478, 7.62346E-05},
                new[] {2.968795322, 0.004485478, 0.122413138, 0.002080516},
                new[] {0.050457226, 7.62346E-05, 0.002080516, 3.53629E-05}
            };

            var testResult = testMatrix.Inverse();
            AssertEquals(testResult, etalonDoubles);
        }
       
        [TestMethod]
        public void Invers_CorrectMatrix_Inverted5()
        {
            var testMatrix = new[]
            {
                new [] {39          ,-0.000647873  ,  -0.000398662,    -0.00088424 ,   1.41720234  ,1.434678161    , 1.391326776  , -0.000344077 ,    0.000104943 ,4.53907E-05     , 0.393569302  ,  0.40325447     , 0.379031788     },
                new [] {-0.000647873,    1.41720234,  0.006036328 , 0.021438833    , -0.000344077  ,  -1.58983E-05 ,   -3.3298E-06,  0.393569302 , 0.001101005    , -0.002759048   , -0.000170646 ,   -4.51085E-06  ,  -4.53899E-07   },
                new [] {-0.000398662,   0.006036328, 1.434678161  , -0.000768489   , -2.85487E-05  ,  0.000104943  ,  -2.81671E-05,  0.000594492 , 0.40325447     , -4.45878E-05   , -8.02614E-06 ,   5.76975E-05   ,  -7.63117E-06   },
                new [] {-0.00088424 ,0.021438833   , -0.000768489 ,   1.391326776  , -4.31641E-05  ,  -3.64512E-05 ,   4.53907E-05,  0.008792703 , -0.0001839     , 0.379031788    ,  -1.36668E-05,    -1.02725E-05 ,   2.83696E-05   },
                new [] {1.41720234  ,-0.000344077  ,  -2.85487E-05,    -4.31641E-05,    0.393569302, 3.90505E-06   ,  0.000216608 , -0.000170646 ,   -2.51381E-08 ,   -1.15998E-08 ,   0.119597384,    9.27418E-07  ,  6.0592E-06     },
                new [] {1.434678161 ,-1.58983E-05  ,  0.000104943 ,   -3.64512E-05 ,   3.90505E-06 ,0.40325447     , 9.1066E-08   , -2.39687E-08 ,   5.76975E-05  ,  1.78833E-09   , 2.73951E-07  ,  0.124013111    , 1.72069E-09     },
                new [] {1.391326776 ,-3.3298E-06   , -2.81671E-05 ,   4.53907E-05  ,   0.000216608 ,9.1066E-08     , 0.379031788  ,  -5.25586E-07,    9.35865E-09 ,   2.83696E-05  ,   5.98894E-05,   2.61445E-08   ,  0.113039376    },
                new [] {-0.000344077,  0.393569302 , 0.000594492  ,  0.008792703   ,  -0.000170646 ,-2.39687E-08   , -5.25586E-07 ,   0.119597384, 2.95152E-09    , 1.29623E-06    , -7.45674E-05 ,   -3.23937E-11  ,  -1.67267E-10   },
                new [] {0.000104943 ,0.001101005   ,  0.40325447  ,   -0.0001839   ,  -2.51381E-08 ,   5.76975E-05 , 9.35865E-09  ,2.95152E-09   , 0.124013111    , -1.24172E-11   , -1.67073E-11 ,   2.7137E-05    , -9.60497E-14    },
                new [] {4.53907E-05 ,-0.002759048  ,  -4.45878E-05,    0.379031788 ,   -1.15998E-08,    1.78833E-09, 2.83696E-05  ,1.29623E-06   , -1.24172E-11   , 0.113039376    ,  -5.34265E-09,    -1.98815E-12 ,   1.33849E-05   },
                new [] {0.393569302 ,-0.000170646  ,  -8.02614E-06,    -1.36668E-05,    0.119597384, 2.73951E-07   , 5.98894E-05  , -7.45674E-05 ,   -1.67073E-11 ,   -5.34265E-09 ,   0.037628439,    2.41483E-12  ,  9.56975E-09    },
                new [] {0.40325447  ,-4.51085E-06  ,  5.76975E-05 ,    -1.02725E-05,    9.27418E-07, 0.124013111   , 2.61445E-08  , -3.23937E-11 ,   2.7137E-05   , -1.98815E-12   , 2.41483E-12  ,   0.039485733   , 1.83989E-15     },
                new [] {0.379031788 ,-4.53899E-07  ,  -7.63117E-06,    2.83696E-05 ,    6.0592E-06 , 1.72069E-09   , 0.113039376  , -1.67267E-10 ,   -9.60497E-14 ,   1.33849E-05  ,   9.56975E-09,    1.83989E-15  ,  0.034904292    }
            };
            var etalonDoubles = new[]
            {
                new [] {0.048815965 ,-1.42786E-05   ,0.000135054 ,0.000237009     ,-0.603241106   ,-0.596366282  ,  -0.614934742 ,   0.000183494 , -0.000503373   , -0.000833324   , 1.407727454   , 1.374485456   , 1.461501787    } ,
                new [] {-1.42786E-05,    8.200446685,-0.033207363,   -0.121310028 ,   0.012365766 ,0.001008107   , 0.000590983   , -26.9768485   ,  0.034996738   ,  0.607215626   , -0.055475804  ,  -0.002090929 ,   -0.001796009 } ,
                new [] {0.000135054 ,-0.033207363   ,8.103259752 , 0.004868985    , 0.000262114   , -0.009456366 ,   0.000469327 ,   0.06864204  , -26.34913872   , -0.013941218   , -0.000530749  ,  0.034585964  ,   -0.001213979 } ,
                new [] {0.000237009 ,-0.121310028   ,0.004868985 , 8.352456849    ,-0.000239027   ,-0.000343875  ,  -0.005435658 ,   -0.214580754,    -0.002369807,    -28.00954037,    0.000344741,    0.000813124,    0.018981681 } ,
                new [] {-0.603241106,    0.012365766,0.000262114 , -0.000239027   , 81.83925767   ,7.368912839   ,  7.560349569  ,  -0.083858787 ,   -0.000147717 ,   0.001570665  ,  -253.8187353 ,   -16.98482302,    -17.94802515} ,
                new [] {-0.596366282,    0.001008107,-0.009456366,   -0.000343875 ,   7.368912839 ,79.93023228   ,  7.512407505  ,  -0.005119273 ,   0.047657995  ,  0.001640871   , -17.19612051  ,  -244.9471349 ,   -17.85456019 } ,
                new [] {-0.614934742,    0.000590983,0.000469327 , -0.005435658   , 7.560349569   ,7.512407505   ,  84.97980192  ,  -0.003211308 ,   -0.000730299 ,   0.028957251  , -17.73306842  ,  -17.31438379 ,   -268.5351647 } ,
                new [] {0.000183494 ,-26.9768485    ,0.06864204  , -0.214580754   , -0.083858787  , -0.005119273 ,   -0.003211308,    97.15192432,   0.015979514  ,  0.059975716   ,  0.334740244  ,   0.010957285 ,    0.008237935 } ,
                new [] {-0.000503373,    0.034996738,-26.34913872,   -0.002369807 ,   -0.000147717,   0.047657995,   -0.000730299,    0.015979514,   93.7430795   ,  -0.001592934  ,  0.00030457   ,   -0.170459706,    0.0020736   } ,
                new [] {-0.000833324,    0.607215626,-0.013941218,   -28.00954037 ,   0.001570665 ,0.001640871   ,   0.028957251 ,  0.059975716  , -0.001592934   , 102.7799496    , -0.003611187  ,  -0.003839119 ,   -0.101373608 } ,
                new [] {1.407727454 ,-0.055475804   ,-0.000530749,   0.000344741  , -253.8187353  , -17.19612051 ,   -17.73306842,    0.334740244,  0.00030457    , -0.003611187   , 818.6122343   ,  39.63729497  ,  42.18655037   } ,
                new [] {1.374485456 ,-0.002090929   ,0.034585964 , 0.000813124    ,-16.98482302   ,-244.9471349  ,  -17.31438379 ,   0.010957285 ,  -0.170459706  ,  -0.003839119  ,  39.63729497  ,   780.5960605 ,   41.15071869  } ,
                new [] {1.461501787 ,-0.001796009   ,-0.001213979,   0.018981681  , -17.94802515  , -17.85456019 ,   -268.5351647,    0.008237935,  0.0020736     , -0.101373608   , 42.18655037   ,  41.15071869  ,   882.447375   }
            };

            var testResult = testMatrix.Inverse();
            AssertEquals(testResult, etalonDoubles, 3);
        }

        [TestMethod]
        public void Invers_CorrectMatrix_Inverted3()
        {
            var testMatrix = new[]
            {
                new[] {72, -0.028824689, 0.007543981, 0.02205759},
                new[] {-0.028824689, 0.038953397, 0.000173764, 0.000645482},
                new[] {0.007543981, 0.000173764, 0.038861074, -2.62605E-05},
                new[] {0.02205759, 0.000645482, -2.62605E-05, 0.03879439}

            };
            var etalonDoubles = new[]
            {
               new[] { 0.013895826 , 0.010428711, -0.002749643,    -0.008076224 },
               new[] { 0.010428711 , 25.68712351, -0.11717496 ,  -0.433404886   },
               new[] { -0.002749643, -0.11717496, 25.73376286 , 0.0209326       },
               new[] { -0.008076224,-0.433404886,    0.0209326,   25.78873997   }

            };

            var testResult = testMatrix.Inverse();           
            AssertEquals(testResult, etalonDoubles, 3);
        }

        [TestMethod]
        public void Invers_OneMatrix_Inverted()
        {
            var testMatrix = new double[][]
            {
                new double [] {1,0,0,0},
                new double [] {0,1,0,0},
                new double [] {0,0,1,0},
                new double [] {0,0,0,1}
            };

            var testResult = testMatrix.Inverse();
            AssertEquals(testResult, testMatrix);
        }

        [TestMethod]
        public void Multiply_CorrectMatrix_Multiplyed()
        {
            var first = new[]
            {
                new[] {770408.7388, -1201409.853, -18596518.9, -2566061.553},
                new[] {-1201409.853, 415312589.3, 13906557.62, 730126.8967},
                new[] {-18596518.9, 13906557.62, 449549927.9, 55786398.59},
                new[] {-2566061.553, 730126.8967, 55786398.59, 377718052.4}  

            };
            var second = new[]
            {
                new[] {2.84217E-14, -1.40037E-14, 5.77316E-15},
                new[] {3.97185E-05, 0.000590964, 3.01476E-05},
                new[] {-1.35806E-05, 0.00016595, 0.000141503},
                new[] {-4.04457E-05, -0.000186737, -0.000214374}
            };

            var etalonDoubles = new[]
            {
                new[] {308.619302, -3316.897338, -2117.591694},
                new[] {16277.21413, 247606.4301, 14331.99457},
                new[] {-7809.117411, 72403.54171, 52072.89772},
                new[] {-16005.68142, -60844.75973, -73056.96864}  
            };

            var testResult = Matrix.Multiply(first, second);
            AssertEquals(testResult, etalonDoubles);
        }

        [TestMethod]
        public void Multiply_CorrectMatrix_Multiplyed2()
        {
            var etalonDoubles = new[]
            {
                new double[] {0, 0, 0},
                new [] {307.4109663, 0.841324668, -2.211972878},
                new [] {0.462860289, 309.3083508, -0.035718913},
                new [] {6.842948928, -0.140773185, 304.5241139},
                new [] {-0.049903594, -1.88182E-05, 4.22902E-06},
                new [] {-1.83696E-05, 0.015194957, 1.44637E-06},
                new [] {-0.000367733, 7.14907E-06, 0.006735529},
                new [] {85.70142175, 1.77743E-06, -3.23038E-05},
                new [] {2.96029E-07, 87.27792884, -1.79686E-10},
                new [] {0.000956105, -8.5543E-09, 83.31618408},
                new [] {-0.029763659, -8.20355E-11, 9.44708E-11},
                new [] {-2.35296E-11, 0.010034133, 1.21092E-14},
                new [] {-1.03201E-07, 8.51979E-13, 0.005018287}
            };
            var a = new[]
            {
                new[]
                {
                    1, -7.67E-07, -2.63E-06, -3.56E-05, 5.89E-13, 6.94E-12, 1.27E-09, -4.52E-19, -1.83E-17, -4.53E-14,
                    3.47E-25, 4.81E-23, 1.61E-18
                },
                new[]
                {
                    1, 2.37E-05, 0.009649086, -3.60E-05, 5.61E-10, 9.31E-05, 1.30E-09, 1.33E-14, 8.98E-07, -4.68E-14,
                    3.14E-19, 8.67E-09, 1.68E-18
                },
                new[]
                {
                    1, 7.63E-05, 0.038612668, -4.47E-05, 5.82E-09, 1.49E-03, 2.00E-09, 4.44E-13, 5.76E-05, -8.93E-14,
                    3.39E-17, 2.22E-06, 3.99E-18
                },
                new[]
                {
                    1, 0.000193108, 0.077184453, -6.48E-05, 3.73E-08, 5.96E-03, 4.20E-09, 7.20E-12, 4.60E-04, -2.72E-13,
                    1.39E-15, 3.55E-05, 1.76E-17
                },
                new[]
                {
                    1, 0.000797403, 0.30521211, -0.000161071, 6.36E-07, 9.32E-02, 2.59E-08, 5.07E-10, 2.84E-02,
                    -4.18E-12, 4.04E-13, 8.68E-03, 6.73E-16
                },
                new[]
                {
                    1, 0.001404801, 0.520573429, -0.000259803, 1.97E-06, 2.71E-01, 6.75E-08, 2.77E-09, 1.41E-01,
                    -1.75E-11, 3.89E-12, 7.34E-02, 4.56E-15
                },
                new[]
                {
                    1, 0.001600894, 0.587967654, -0.000296064, 2.56E-06, 3.46E-01, 8.77E-08, 4.10E-09, 2.03E-01,
                    -2.60E-11, 6.57E-12, 1.20E-01, 7.68E-15
                },
                new[]
                {
                    1, -2.20E-05, -0.009656087, -1.85E-05, 4.85E-10, 9.32E-05, 3.42E-10, -1.07E-14, -9.00E-07,
                    -6.33E-15, 2.35E-19, 8.69E-09, 1.17E-19
                },
                new[]
                {
                    1, -0.000119252, -0.038618893, 2.12E-06, 1.42E-08, 1.49E-03, 4.49E-12, -1.70E-12, -5.76E-05,
                    9.52E-18, 2.02E-16, 2.22E-06, 2.02E-23
                },
                new[]
                {
                    1, -0.000209577, -0.077193145, 9.55E-06, 4.39E-08, 5.96E-03, 9.12E-11, -9.21E-12, -4.60E-04,
                    8.72E-16, 1.93E-15, 3.55E-05, 8.33E-21
                },
                new[]
                {
                    1, -0.000816433, -0.30520179, 0.000109574, 6.67E-07, 9.31E-02, 1.20E-08, -5.44E-10, -2.84E-02,
                    1.32E-12, 4.44E-13, 8.68E-03, 1.44E-16
                },
                new[]
                {
                    1, -0.001424169, -0.520557112, 0.000210772, 2.03E-06, 2.71E-01, 4.44E-08, -2.89E-09, -1.41E-01,
                    9.36E-12, 4.11E-12, 7.34E-02, 1.97E-15
                },
                new[]
                {
                    1, -0.001626416, -0.587881847, 0.000244156, 2.65E-06, 3.46E-01, 5.96E-08, -4.30E-09, -2.03E-01,
                    1.46E-11, 7.00E-12, 1.19E-01, 3.55E-15
                },
                new[]
                {
                    1, -1.77E-06, -6.75E-06, -3.17E-05, 3.13E-12, 4.56E-11, 1.00E-09, -5.55E-18, -3.08E-16, -3.17E-14,
                    9.82E-24, 2.08E-21, 1.00E-18
                },
                new[]
                {
                    1, 7.31E-05, -6.41E-06, -0.009538836, 5.34E-09, 4.11E-11, 9.10E-05, 3.90E-13, -2.63E-16, -8.68E-07,
                    2.85E-17, 1.69E-21, 8.28E-09
                },
                new[]
                {
                    1, 0.000275691, -2.67E-05, -0.038003519, 7.60E-08, 7.10E-10, 1.44E-03, 2.10E-11, -1.89E-14,
                    -5.49E-05, 5.78E-15, 5.05E-19, 2.09E-06
                },
                new[]
                {
                    1, 0.000528739, -6.87E-06, -0.075940876, 2.80E-07, 4.72E-11, 5.77E-03, 1.48E-10, -3.24E-16,
                    -4.38E-04, 7.82E-14, 2.23E-21, 3.33E-05
                },
                new[]
                {
                    1, 0.002116628, 1.37E-05, -0.30028077, 4.48E-06, 1.88E-10, 9.02E-02, 9.48E-09, 2.57E-15, -2.71E-02,
                    2.01E-11, 3.53E-20, 8.13E-03
                },
                new[]
                {
                    1, 0.003711756, 3.93E-05, -0.512470842, 1.38E-05, 1.55E-09, 2.63E-01, 5.11E-08, 6.08E-14, -1.35E-01,
                    1.90E-10, 2.39E-18, 6.90E-02
                },
                new[]
                {
                    1, 0.00424694, 4.87E-05, -0.578963804, 1.80E-05, 2.37E-09, 3.35E-01, 7.66E-08, 1.15E-13, -1.94E-01,
                    3.25E-10, 5.61E-18, 1.12E-01
                },
                new[]
                {
                    1, -6.60E-05, -5.33E-06, 0.009460878, 4.36E-09, 2.84E-11, 8.95E-05, -2.88E-13, -1.51E-16, 8.47E-07,
                    1.90E-17, 8.06E-22, 8.01E-09
                },
                new[]
                {
                    1, -0.00028236, -2.79E-05, 0.037939182, 7.97E-08, 7.77E-10, 1.44E-03, -2.25E-11, -2.17E-14,
                    5.46E-05, 6.36E-15, 6.04E-19, 2.07E-06
                },
                new[]
                {
                    1, -0.000533014, -2.47E-05, 0.0758865, 2.84E-07, 6.10E-10, 5.76E-03, -1.51E-10, -1.51E-14, 4.37E-04,
                    8.07E-14, 3.72E-19, 3.32E-05
                },
                new[]
                {
                    1, -0.002120594, -5.63E-05, 0.300251616, 4.50E-06, 3.17E-09, 9.02E-02, -9.54E-09, -1.79E-13,
                    2.71E-02, 2.02E-11, 1.01E-17, 8.13E-03
                },
                new[]
                {
                    1, -0.003715347, -8.07E-05, 0.512480555, 1.38E-05, 6.52E-09, 2.63E-01, -5.13E-08, -5.26E-13,
                    1.35E-01, 1.91E-10, 4.25E-17, 6.90E-02
                },
                new[]
                {
                    1, -0.004247001, -8.80E-05, 0.579010458, 1.80E-05, 7.75E-09, 3.35E-01, -7.66E-08, -6.82E-13,
                    1.94E-01, 3.25E-10, 6.00E-17, 1.12E-01
                },
                new[]
                {
                    1, -1.64E-05, -4.27E-06, -3.29E-05, 2.67E-10, 1.82E-11, 1.09E-09, -4.37E-15, -7.78E-17, -3.57E-14,
                    7.15E-20, 3.32E-22, 1.18E-18
                },
                new[]
                {
                    1, 0.009577092, 9.27E-06, 0.000180485, 9.17E-05, 8.60E-11, 3.26E-08, 8.78E-07, 7.98E-16, 5.88E-12,
                    8.41E-09, 7.40E-21, 1.06E-15
                },
                new[]
                {
                    1, 0.038339363, 3.02E-05, 0.000805347, 1.47E-03, 9.13E-10, 6.49E-07, 5.64E-05, 2.76E-14, 5.22E-10,
                    2.16E-06, 8.33E-19, 4.21E-13
                },
                new[]
                {
                    1, 0.076653385, 9.66E-05, 0.001614648, 5.88E-03, 9.33E-09, 2.61E-06, 4.50E-04, 9.01E-13, 4.21E-09,
                    3.45E-05, 8.70E-17, 6.80E-12
                },
                new[]
                {
                    1, 0.303127617, 0.000422622, 0.006538528, 9.19E-02, 1.79E-07, 4.28E-05, 2.79E-02, 7.55E-11,
                    2.80E-07, 8.44E-03, 3.19E-14, 1.83E-09
                },
                new[]
                {
                    1, 0.517257869, 0.000758608, 0.011465647, 2.68E-01, 5.75E-07, 1.31E-04, 1.38E-01, 4.37E-10,
                    1.51E-06, 7.16E-02, 3.31E-13, 1.73E-08
                },
                new[]
                {
                    1, 0.584345615, 0.000867187, 0.013104758, 3.41E-01, 7.52E-07, 1.72E-04, 2.00E-01, 6.52E-10,
                    2.25E-06, 1.17E-01, 5.66E-13, 2.95E-08
                },
                new[]
                {
                    1, -0.009602396, -5.06E-05, -0.000239611, 9.22E-05, 2.56E-09, 5.74E-08, -8.85E-07, -1.29E-13,
                    -1.38E-11, 8.50E-09, 6.54E-18, 3.30E-15
                },
                new[]
                {
                    1, -0.038357919, -8.38E-05, -0.000835745, 1.47E-03, 7.03E-09, 6.98E-07, -5.64E-05, -5.89E-13,
                    -5.84E-10, 2.16E-06, 4.94E-17, 4.88E-13
                },
                new[]
                {
                    1, -0.076675986, -0.000135944, -0.001674283, 5.88E-03, 1.85E-08, 2.80E-06, -4.51E-04, -2.51E-12,
                    -4.69E-09, 3.46E-05, 3.42E-16, 7.86E-12
                },
                new[]
                {
                    1, -0.303209735, -0.000464816, -0.006592029, 9.19E-02, 2.16E-07, 4.35E-05, -2.79E-02, -1.00E-10,
                    -2.86E-07, 8.45E-03, 4.67E-14, 1.89E-09
                },
                new[]
                {
                    1, -0.5174149, -0.000794452, -0.011517505, 2.68E-01, 6.31E-07, 1.33E-04, -1.39E-01, -5.01E-10,
                    -1.53E-06, 7.17E-02, 3.98E-13, 1.76E-08
                },
                new[]
                {
                    1, -0.58453578, -0.000909136, -0.013159995, 3.42E-01, 8.27E-07, 1.73E-04, -2.00E-01, -7.51E-10,
                    -2.28E-06, 1.17E-01, 6.83E-13, 3.00E-08
                },
            };
            
            //var testResult = Matrix.Multiply(a.Transpose(), CalculatorCoefficients.DYS_MATRIX);
            //AssertEquals(testResult, etalonDoubles, 3);
        }

        public static void AssertEquals(double[][] testResult, double[][] etalonDoubles,int numbersCount=0)
        {
            Assert.AreEqual(testResult.Length, etalonDoubles.Length);

            for (var i = 0; i < testResult.Length; i++)
            {
                Assert.AreEqual(testResult[i].Length, etalonDoubles[i].Length);
                for (var j = 0; j < testResult[i].Length; j++)
                    Assert.AreEqual(System.Math.Round(testResult[i][j], numbersCount), System.Math.Round(etalonDoubles[i][j], numbersCount));
            }
        }
    }
}
