using System;
using m = System.Math;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace EventHorizon.Blazor.BabylonJS.Data
{
    /// <summary>
    /// The standard fill algorithm uses approaches developed by GOAL (Gower optimal algorithms Ltd) to produce in most cases as optimum answer for 
    /// a layer of multiple items into a single layer.
    /// </summary>
    public static class standardFillAlg
    {
        private static List<rectPos> StandardLayerRectPack(double[] startPosition, double[] item, int x_count, int y_count)
        {
            List<rectPos> retValue = new List<rectPos>();
            for (int a = 0; a < x_count; a++)
            {
                for (int b = 0; b < y_count; b++)
                {
                    retValue.Add(new rectPos()
                    {
                        startPos = new double[] { startPosition[0] + a * item[0], startPosition[1]+ b * item[1], startPosition[2]+ 0 },
                        endPos = new double[] { startPosition[0] + (a + 1) * item[0], startPosition[1] + (b + 1) * item[1], startPosition[2] + item[2] }
                    });
                }
            }
            return retValue;
        }
        private static List<int[]> CreateAcceptableCombinations(ref double spaceGap, int dimension,double[] item, double[] space)
        {
           
            List<int[]> combiSpaceDim = new List<int[]>();
            spaceGap = space[dimension];

            #region creating List of acceptable Combinations of rotations in the Pallet Dimension 1
            for (int jj = 0; jj <= (int)(space[dimension] / item[0]); jj++)
            {
                combiSpaceDim.Add(new int[2] { jj, (int)((space[dimension] - jj * item[0]) / item[1]) });
                spaceGap = spaceGap > space[dimension] - combiSpaceDim[jj][0] * item[0] - combiSpaceDim[jj][1] * item[1] ? space[dimension] - combiSpaceDim[jj][0] * item[0] - combiSpaceDim[jj][1] * item[1] : spaceGap;
            }
            #endregion
            return combiSpaceDim;
        }
        public static int CreateUpperBound(double[] item, double[] space)
        {
            double space1Gap = 0;
            List<int[]> combiSpace1 = CreateAcceptableCombinations(ref space1Gap, 0, item, space);

            double space2Gap = 0;
            List<int[]> combiSpace2 = CreateAcceptableCombinations(ref space2Gap, 1, item, space);

            return (int)((space[0] - space1Gap) * (space[1] - space2Gap) / (item[0] * item[1]));
        }

        /// <summary>
        /// Pack takes the dimensions of an RECTANGULAR item, the dimensions of a packing space, typically pallet or container, and produces the local optimial soltuion
        /// for those constraints, it doesnt take, any decisions on quality of the soultion proposed, just the solution that produces the most items per layer.
        /// </summary>
        /// <param name="item">The item dimensions are passed in the form of a double array [x..,y..,z..] although the algorithm only packs a layer the z dimesion is used to give absolute locations</param>
        /// <param name="space">The space dimensions are passed in the form of a double array [x..,y..,z..] this represents the space availble, not necessary the whole space </param>
         /// <returns>A single list of the locations that the items are packed in</returns>
        public static List<rectPos> Pack(double [] item, double [] space) 
        {

            if (item[2] > space[2])
                return new List<rectPos>();

            
            List<rectPos> retValue = new List<rectPos>();
           
            int maxPack = (int)(space[0] / item[0]) * (int)(space[1] / item[1]);
            retValue = StandardLayerRectPack(new double[]
                {
                (space[0] - (int)(space[0] / item[0])*item[0])/2,
                (space[1] - (int)(space[1] / item[1])*item[1])/2,
                0},
                new double[3] { item[0], item[1], item[2] },
                (int)(space[0] / item[0]),
                (int)(space[1] / item[1]));

            if ((int)(space[0] / item[1]) * (int)(space[1] / item[0]) > maxPack)
            {
                maxPack = (int)(space[0] / item[1]) * (int)(space[1] / item[0]);
                retValue = StandardLayerRectPack(new double[]
                {
                (space[0] - (int)(space[0] / item[1])*item[1])/2,
                (space[1] - (int)(space[1] / item[0])*item[0])/2,
                0},
                new double[3] { item[1], item[0], item[2] },
                (int)(space[0] / item[1]),
                (int)(space[1] / item[0]));
            }

            double space1Gap = 0;
            List<int[]> combiSpace1 = CreateAcceptableCombinations(ref space1Gap, 0, item, space);

            double space2Gap = 0;
            List<int[]> combiSpace2 = CreateAcceptableCombinations(ref space2Gap, 1, item, space);

            int upperBound = (int)((space[0] - space1Gap) * (space[1] - space2Gap) / (item[0] * item[1]));

            if (maxPack == upperBound)
                return CentreLoad(retValue,space);

            #region Actual Algorithm NEED TO UNDERSTAND HOW VERTICAL AND HORIZONTAL OVERHANGS ARE EXPLORED 

            int lowibb = 0;
            int limibb = (int)(space[1] / item[0]);

            for (int ial = 0; ial <= (space[0] / item[0]); ial++)
            {
                for (int iar = ial; iar <= (space[0] / item[0]); iar++)
                {
                    for (int ibt = 0; ibt <= (space[1] / item[0]); ibt++)
                    {
                        if (((ial) * item[0]) > combiSpace1[iar][1] * item[1])
                        {
                            if ((int)(space[0] / item[0]) - (ial) != (iar))
                            {
                                int numbb = ((int)((space[1] - combiSpace2[ibt][1] * item[1]) / item[1]));

                                int numba = (int)((space[1] - numbb * item[1]) / item[0]);
                                lowibb = numba;
                                if (combiSpace2[numba][1] != numbb)
                                    lowibb = lowibb + 1;


                                lowibb = lowibb + 1;
                            }
                        }
                        else
                        {
                            if ((int)(space[0] / item[1]) - combiSpace1[iar][1] != combiSpace1[ial][1])
                                limibb = (int)((space[1] - (ibt) * item[0]) / item[0]);

                        }
                        for (int ibb = lowibb; ibb <= limibb; ibb++)
                        {
                            if (ial == iar && ibt > ibb)
                                break;

                            double olp15h = space[1] - combiSpace2[ibt][1] * item[1] - combiSpace2[ibb][1] * item[1];
                            double olp15v = space[0] - combiSpace1[ial][0] * item[0] - combiSpace1[iar][0] * item[0];
                            double olp24v = space[0] - combiSpace1[ial][1] * item[1] - combiSpace1[iar][1] * item[1];
                            double olp24h = space[1] - combiSpace2[ibt][0] * item[0] - combiSpace2[ibb][0] * item[0];
                            double vert = olp15v * olp24v;
                            double DimensionASpace = 0;
                            double DimensionBSpace = 0;

                            if (vert >= 0.0)
                            {
                                double horz = olp15h * olp24h;
                                if (horz >= 0.0)
                                {
                                    DimensionASpace = m.Min(olp15v, olp24v);
                                    DimensionBSpace = m.Min(olp15h, olp24h);
                                }
                                else
                                {
                                    DimensionBSpace = m.Max(olp15h, olp24h);
                                    if (olp15h == DimensionBSpace)
                                        DimensionASpace = olp24v;
                                    DimensionASpace = olp15v;
                                }
                            }
                            else
                            {
                                DimensionASpace = m.Max(olp15v, olp24v);
                                if (olp15v == DimensionASpace)
                                    DimensionBSpace = olp24h;
                                else
                                    DimensionBSpace = olp15h;
                            }
                            int Area5Amount = (int)(DimensionASpace / item[0]) * (int)(DimensionBSpace / item[1]);
                            int A5a = (int)(DimensionASpace / item[0]);
                            int A5b = (int)(DimensionBSpace / item[1]);
                            bool A5Right = true;
                            if (Area5Amount < (int)(DimensionASpace / item[1]) * (int)(DimensionBSpace / item[0]))
                            {
                                Area5Amount = (int)(DimensionASpace / item[1]) * (int)(DimensionBSpace / item[0]);
                                A5a = (int)(DimensionASpace / item[1]);
                                A5b = (int)(DimensionBSpace / item[0]);
                                A5Right = false;
                            }

                            int[] areaAntiClockwiseCentre = new int[5] { combiSpace1[ial][0] * combiSpace2[ibt][1],
                                combiSpace2[ibt][0] * combiSpace1[iar][1],
                                combiSpace1[iar][0] * combiSpace2[ibb][1],
                                combiSpace2[ibb][0] * combiSpace1[ial][1],
                                Area5Amount };





                            if (areaAntiClockwiseCentre.Sum() > maxPack)
                            {
                                #region hid for moment
                                //////////////////////////////////////////////////////
                                //|___|___|___|___|___|___|Ma1             | | | | |//
                                //|___|___|___|___|___|___|                |_|_|_|_|// 
                                //                   ^                 Ma2 | | | | |// 
                                //                   |                     |_|_|_|_|//     
                                // _ _               |Ma4                  | | | | |// 
                                //| | |<-------------|-------------------->|_|_|_|_|// 
                                //|_|_|              |                              //
                                //| | |             _v_ ___ ___ ___ ___ ___ ___ ___ //
                                //|_|_|        Ma3 |___|___|___|___|___|___|___|___|//
                                //| | |Ma0         |___|___|___|___|___|___|___|___|// 
                                //|_|_|            |___|___|___|___|___|___|___|___|//
                                //////////////////////////////////////////////////////
                                #endregion

                                //////////////////////////////////////////////////////
                                //| | |            |___|___|___|___|___|___|___|___|//
                                //|_|_|        Ma2 |___|___|___|___|___|___|___|___|//
                                //| | |            |___|___|___|___|___|___|___|___|//
                                //|_|_|Ma1           ^                      _ _ _ _ //
                                //| | |<---------------------------------->| | | | |//
                                //|_|_|              |  Ma4                |_|_|_|_|//
                                //                   |                  Ma3| | | | |//
                                // ___ ___ ___ ___   v                     |_|_|_|_|//
                                //|___|___|___|___|Ma0                     | | | | |//
                                //|___|___|___|___|                        |_|_|_|_|//
                                //////////////////////////////////////////////////////

                                //Ma0 built up as combiSpace1[ial][0] x combiSpace2[ibt][1] correctly rotated
                                //Ma1 built up as combiSpace1[iar][1] x combiSpace2[ibt][0] incorrectly rotated
                                //Ma2 built up as combiSpace1[iar][0] x combiSpace2[ibb][1] correctly rotated
                                //Ma3 built up as combiSpace1[ial][1] x combiSpace2[ibb][0] incorrectly rotated
                                //Ma4 built up as A5a x A5b correctly/incorrectly rotated based on A5Right
                                maxPack = areaAntiClockwiseCentre.Sum();
                                retValue.Clear();

                                double[] xloc = new double[5] { 0, 0, space[0], space[0],0 };

                                double[] yloc = new double[5] { 0, space[1], space[1], 0,0 };
                                double[] zloc = new double[5]{ 0, 0, 0, 0,0 };
                                //Region Ma0 - items packed correct way from top left
                                if (combiSpace1[ial][0] > 0 && combiSpace2[ibt][1] > 0)
                                {
                                     yloc[0] = 0;
                                     xloc[0] = 0;
                                     zloc[0] = 0;

                                    retValue.AddRange(StandardLayerRectPack(
                                        new double[3] { xloc[0], yloc[0] , zloc[0]  },
                                        new double[3] { item[0], item[1], item[2] },
                                        combiSpace1[ial][0], combiSpace2[ibt][1]));
                                }
                                //Region Ma1 - items packed incorrect way from bottom left (left is 0 or edge of Ma1)
                                if (combiSpace2[ibt][0] > 0 && combiSpace1[iar][1] > 0)
                                {
                                    
                                    yloc[1] = (space[1] - combiSpace2[ibt][0] * item[0]);
                                    xloc[1] = (combiSpace2[ibt][1] * item[1] < yloc[1] ? 0 : combiSpace1[ial][0]*item[0]);
                                    zloc[1] = 0;

                                    retValue.AddRange(StandardLayerRectPack(
                                         new double[3] { xloc[1] , yloc[1], zloc[1]  },
                                        new double[3] { item[1], item[0], item[2] },
                                        combiSpace1[iar][1], combiSpace2[ibt][0]));
                                }
                                //region Ma2 -items packed correct way from top right
                                if (combiSpace1[iar][0] > 0 && combiSpace2[ibb][1] > 0)
                                {
                                    xloc[2] =  (space[0] - combiSpace1[iar][0] * item[0]);
                                    yloc[2] = (xloc[2]> xloc[1] + (combiSpace1[iar][1] * item[1]) ? space[1] - combiSpace2[ibb][1] * item[1]: space[1] - combiSpace2[ibt][0] * item[0]- combiSpace2[ibb][1] * item[1]);
                                    zloc[2] = 0;

                                    retValue.AddRange(StandardLayerRectPack(
                                         new double[3] { xloc[2] , yloc[2] , zloc[2]  },
                                        new double[3] { item[0], item[1], item[2] },
                                        combiSpace1[iar][0], combiSpace2[ibb][1]));
                                }
                                //region Ma3 -items packed incorrect way from bottom right
                                if (combiSpace1[ial][1] > 0 && combiSpace2[ibb][0] > 0)
                                {
                                    xloc[3] = space[0] - combiSpace1[ial][1] * item[1];
                                    yloc[3] = 0;
                                    if (xloc[3] < xloc[0] + combiSpace1[ial][0] * item[0])
                                        yloc[3] = combiSpace2[ibt][1] * item[1];
                                    if (yloc[3] + combiSpace2[ibb][0] * item[0] > yloc[2])
                                        xloc[3] = (space[0] - combiSpace1[iar][0] * item[0]) - combiSpace1[ial][1] * item[1];



                                    zloc[3] = 0;
                                    retValue.AddRange(StandardLayerRectPack(
                                         new double[3] { xloc[3], yloc[3], zloc[3] },
                                        new double[3] { item[1], item[0], item[2] },
                                        combiSpace1[ial][1], combiSpace2[ibb][0]));

                                }
                                //region Ma4 -items packed either way int the middle
                                if (A5a > 0 && A5b > 0)
                                {
                                    yloc[4] =  (yloc[4]+ combiSpace2[ibb][0]*item[0]);
                                    xloc[4] = (xloc[0]+ combiSpace1[ial][0]*item[0]);
                                    zloc[4] = 0;

                                    retValue.AddRange(StandardLayerRectPack(
                                         new double[3] { xloc[4] , yloc[4] , zloc[4]  },
                                        A5Right ? new double[3] { item[0], item[1], item[2] }: new double[3] { item[1], item[0], item[2] },
                                        A5a, A5b));
                                }
                                if (retValue.Count == upperBound)
                                   return CentreLoad(retValue,space);
                            }
                        }
                    }
                }
            }
            #endregion
            return retValue;
        }
        /// <summary>
        /// Pack takes the dimensions of an RECTANGULAR item, the dimensions of a packing space, typically pallet or container, and produces a sequenced list of all the options that were explored during the algorithm creating a series opf altnatives to the local optimial soltuion
        /// for those constraints, it doesnt take, any decisions on quality of the soultion proposed, just the solution that produces the most items per layer.
        /// </summary>
        /// <param name="item">The item dimensions are passed in the form of a double array [x..,y..,z..] although the algorithm only packs a layer the z dimesion is used to give absolute locations</param>
        /// <param name="space">The space dimensions are passed in the form of a double array [x..,y..,z..] this represents the space availble, not necessary the whole space </param>
        /// <returns>A organised list of multiple lists of the locations that the items are packed in</returns>
        public static Dictionary<int,List<List<rectPos>>> PackOptions(double[] item, double[] space)
        {
            

           
            SortedDictionary<int, List<List<rectPos>>> retValue = new SortedDictionary<int, List<List<rectPos>>>();
            if (item[2]>space[2])
                return new Dictionary<int, List<List<rectPos>>>();
            int localPack = (int)(space[0] / item[0]) * (int)(space[1] / item[1]);

            if (!retValue.ContainsKey(localPack))
                retValue.Add(localPack, new List<List<rectPos>>());

            retValue[localPack].Add(StandardLayerRectPack(new double[]
                {
                (space[0] - (int)(space[0] / item[0])*item[0])/2,
                (space[1] - (int)(space[1] / item[1])*item[1])/2,
                0},
                new double[3] { item[0], item[1], item[2] },
                (int)(space[0] / item[0]),
                (int)(space[1] / item[1])));


            localPack = (int)(space[0] / item[1]) * (int)(space[1] / item[0]);

            if (!retValue.ContainsKey(localPack))
                retValue.Add(localPack, new List<List<rectPos>>());
            retValue[localPack].Add(StandardLayerRectPack(new double[]
                {
                (space[0] - (int)(space[0] / item[1])*item[1])/2,
                (space[1] - (int)(space[1] / item[0])*item[0])/2,
                0},
                new double[3] { item[1], item[0], item[2] },
                (int)(space[0] / item[1]),
                (int)(space[1] / item[0])));

           


            double space1Gap = 0;
            List<int[]> combiSpace1 = CreateAcceptableCombinations(ref space1Gap, 0, item, space);

            double space2Gap = 0;
            List<int[]> combiSpace2 = CreateAcceptableCombinations(ref space2Gap, 1, item, space);

            int upperBound = (int)((space[0] - space1Gap) * (space[1] - space2Gap) / (item[0] * item[1]));



            #region Actual Algorithm NEED TO UNDERSTAND HOW VERTICAL AND HORIZONTAL OVERHANGS ARE EXPLORED 

            int lowibb = 0;
            int limibb = (int)(space[1] / item[0]);

            for (int ial = 0; ial <=  (space[0] / item[0]); ial++)
            {
                for (int iar = ial; iar <= (space[0] / item[0]); iar++)
                {
                    for (int ibt = 0; ibt <= (space[1] / item[0]); ibt++)
                    {
                        if (((ial) * item[0]) > combiSpace1[iar][1] * item[1])
                        {
                            if ((int)(space[0] / item[0]) - (ial) != (iar))
                            {
                                int numbb = ((int)((space[1] - combiSpace2[ibt][1] * item[1]) / item[1]));

                                int numba = (int)((space[1] - numbb * item[1]) / item[0]);
                                lowibb = numba;
                                if (combiSpace2[numba][1] != numbb)
                                    lowibb = lowibb + 1;


                                lowibb = lowibb + 1;
                            }
                        }
                        else
                        {
                            if ((int)(space[0] / item[1]) - combiSpace1[iar][1] != combiSpace1[ial][1])
                                limibb = (int)((space[1] - (ibt) * item[0]) / item[0]);

                        }
                        for (int ibb = lowibb; ibb <= limibb; ibb++)
                        {
                            if (ial == iar && ibt > ibb)
                                break;

                            double olp15h = space[1] - combiSpace2[ibt][1] * item[1] - combiSpace2[ibb][1] * item[1];
                            double olp15v = space[0] - combiSpace1[ial][0] * item[0] - combiSpace1[iar][0] * item[0];
                            double olp24v = space[0] - combiSpace1[ial][1] * item[1] - combiSpace1[iar][1] * item[1];
                            double olp24h = space[1] - combiSpace2[ibt][0] * item[0] - combiSpace2[ibb][0] * item[0];
                            double vert = olp15v * olp24v;
                            double DimensionASpace = 0;
                            double DimensionBSpace = 0;

                            if (vert >= 0.0)
                            {
                                double horz = olp15h * olp24h;
                                if (horz >= 0.0)
                                {
                                    DimensionASpace = m.Min(olp15v, olp24v);
                                    DimensionBSpace = m.Min(olp15h, olp24h);
                                }
                                else
                                {
                                    DimensionBSpace = m.Max(olp15h, olp24h);
                                    if (olp15h == DimensionBSpace)
                                        DimensionASpace = olp24v;
                                    DimensionASpace = olp15v;
                                }
                            }
                            else
                            {
                                DimensionASpace = m.Max(olp15v, olp24v);
                                if (olp15v == DimensionASpace)
                                    DimensionBSpace = olp24h;
                                else
                                    DimensionBSpace = olp15h;
                            }
                            int Area5Amount = (int)(DimensionASpace / item[0]) * (int)(DimensionBSpace / item[1]);
                            int A5a = (int)(DimensionASpace / item[0]);
                            int A5b = (int)(DimensionBSpace / item[1]);
                            bool A5Right = true;
                            if (Area5Amount < (int)(DimensionASpace / item[1]) * (int)(DimensionBSpace / item[0]))
                            {
                                Area5Amount = (int)(DimensionASpace / item[1]) * (int)(DimensionBSpace / item[0]);
                                A5a = (int)(DimensionASpace / item[1]);
                                A5b = (int)(DimensionBSpace / item[0]);
                                A5Right = false;
                            }

                            int[] areaAntiClockwiseCentre = new int[5] { combiSpace1[ial][0] * combiSpace2[ibt][1],
                                combiSpace2[ibt][0] * combiSpace1[iar][1],
                                combiSpace1[iar][0] * combiSpace2[ibb][1],
                                combiSpace2[ibb][0] * combiSpace1[ial][1],
                                Area5Amount };






                            //////////////////////////////////////////////////////
                            //| | |            |___|___|___|___|___|___|___|___|//
                            //|_|_|        Ma2 |___|___|___|___|___|___|___|___|//
                            //| | |            |___|___|___|___|___|___|___|___|//
                            //|_|_|Ma1           ^                      _ _ _ _ //
                            //| | |<---------------------------------->| | | | |//
                            //|_|_|              |  Ma4                |_|_|_|_|//
                            //                   |                  Ma3| | | | |//
                            // ___ ___ ___ ___   v                     |_|_|_|_|//
                            //|___|___|___|___|Ma0                     | | | | |//
                            //|___|___|___|___|                        |_|_|_|_|//
                            //////////////////////////////////////////////////////

                            //Ma0 built up as combiSpace1[ial][0] x combiSpace2[ibt][1] correctly rotated
                            //Ma1 built up as combiSpace1[iar][1] x combiSpace2[ibt][0] incorrectly rotated
                            //Ma2 built up as combiSpace1[iar][0] x combiSpace2[ibb][1] correctly rotated
                            //Ma3 built up as combiSpace1[ial][1] x combiSpace2[ibb][0] incorrectly rotated
                            //Ma4 built up as A5a x A5b correctly/incorrectly rotated based on A5Right
                            localPack = areaAntiClockwiseCentre.Sum();


                           

                            List<rectPos> tempRetVal = new List<rectPos>();


                            double[] xloc = new double[5] { 0, 0, space[0], space[0], 0 };

                            double[] yloc = new double[5] { 0, space[1], space[1], 0, 0 };
                            double[] zloc = new double[5] { 0, 0, 0, 0, 0 };
                            //Region Ma0 - items packed correct way from bottom left
                            if (combiSpace1[ial][0] > 0 && combiSpace2[ibt][1] > 0)
                            {
                                yloc[0] = 0;
                                xloc[0] = 0;
                                zloc[0] = 0;

                                tempRetVal.AddRange(StandardLayerRectPack(
                                    new double[3] { xloc[0], yloc[0], zloc[0] },
                                    new double[3] { item[0], item[1], item[2] },
                                    combiSpace1[ial][0], combiSpace2[ibt][1]));


                               
                            }
                            //Region Ma1 - items packed incorrect way from top left (left is 0 or edge of Ma1)
                            if (combiSpace2[ibt][0] > 0 && combiSpace1[iar][1] > 0)
                            {

                                yloc[1] = (space[1] - combiSpace2[ibt][0] * item[0]);
                                xloc[1] = (combiSpace2[ibt][1] * item[1] <= yloc[1] ? 0 : combiSpace1[ial][0] * item[0]);
                                zloc[1] = 0;

                                

                                tempRetVal.AddRange(StandardLayerRectPack(
                                     new double[3] { xloc[1], yloc[1], zloc[1] },
                                    new double[3] { item[1], item[0], item[2] },
                                    combiSpace1[iar][1], combiSpace2[ibt][0]));
                            }
                            //region Ma2 -items packed correct way from top right
                            if (combiSpace1[iar][0] > 0 && combiSpace2[ibb][1] > 0)
                            {
                                xloc[2] = (space[0] - combiSpace1[iar][0] * item[0]);
                                yloc[2] = (xloc[2] >= xloc[1] + (combiSpace1[iar][1] * item[1]) ? space[1] - combiSpace2[ibb][1] * item[1] : space[1] - combiSpace2[ibt][0] * item[0] - combiSpace2[ibb][1] * item[1]);
                                zloc[2] = 0;

                                tempRetVal.AddRange(StandardLayerRectPack(
                                     new double[3] { xloc[2], yloc[2], zloc[2] },
                                    new double[3] { item[0], item[1], item[2] },
                                    combiSpace1[iar][0], combiSpace2[ibb][1]));



                                
                            }
                            //region Ma3 -items packed correct way from bottom right
                            if (combiSpace1[ial][1] > 0 && combiSpace2[ibb][0] > 0)
                            {

                                xloc[3] = space[0] - combiSpace1[ial][1] * item[1];
                                yloc[3] = 0;
                                if (xloc[3] < xloc[0] + combiSpace1[ial][0] * item[0])
                                    yloc[3] = combiSpace2[ibt][1] * item[1];
                                if (yloc[3] + combiSpace2[ibb][0] * item[0] > yloc[2])
                                    xloc[3] = (space[0] - combiSpace1[iar][0] * item[0])- combiSpace1[ial][1] * item[1];



                                zloc[3] = 0;
                                tempRetVal.AddRange(StandardLayerRectPack(
                                     new double[3] { xloc[3], yloc[3], zloc[3] },
                                    new double[3] { item[1], item[0], item[2] },
                                    combiSpace1[ial][1], combiSpace2[ibb][0]));

                              
                            }
                            //region Ma4 -items packed either way int the middle
                            if (A5a > 0 && A5b > 0)
                            {

                                yloc[4] = (yloc[4] + combiSpace2[ibb][0] * item[0]);
                                xloc[4] = (xloc[0] + combiSpace1[ial][0] * item[0]);
                                zloc[4] = 0;
                                

                                tempRetVal.AddRange(StandardLayerRectPack(
                                     new double[3] { xloc[4], yloc[4], zloc[4] },
                                    A5Right ? new double[3] { item[0], item[1], item[2] } : new double[3] { item[1], item[0], item[2] },
                                    A5a, A5b));
                            }

                            if (!retValue.ContainsKey(localPack))
                                retValue.Add(localPack, new List<List<rectPos>>());
                            retValue[localPack].Add(CentreLoad(tempRetVal,space));



                        }
                    }
                }
            }
            #endregion
            return new Dictionary<int, List<List<rectPos>>>(retValue.Reverse());
        }
        private static List<rectPos> CentreLoad(List<rectPos> loadList, double[] space)
        {
            double biggestH = 0;
            double biggestV = 0;

            foreach (rectPos rP in loadList)
            {
                if (rP.endPos[0] > biggestH)
                    biggestH = rP.endPos[0];
                if (rP.endPos[1] > biggestV)
                    biggestV = rP.endPos[1];
            }

            double AdjustrmentH = (space[0] - biggestH) / 2;
            double AdjustrmentV = (space[1] - biggestV) / 2;

            List<rectPos> retur = new List<rectPos>();
            foreach (rectPos rP in loadList)
            {
                retur.Add(new rectPos()
                {
                    startPos = new double[3]
                    {
                       rP.startPos[0]+AdjustrmentH,
                       rP.startPos[1]+AdjustrmentV,
                       rP.startPos[2]
                    },
                    endPos = new double[3]
                    {
                       rP.endPos[0]+AdjustrmentH,
                       rP.endPos[1]+AdjustrmentV,
                       rP.endPos[2]
                    },
                });

            }
            return retur;
        }
    }
}
