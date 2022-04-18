using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using m = System.Math;

namespace EventHorizon.Blazor.BabylonJS.Data
{
    /// <summary>
    /// 
    /// </summary>
    public static class stairsFillAlg
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static List<rectPos> Pack(double[] item, double[] space)
        {

            List<rectPos> retValue = new List<rectPos>();
            if (item[2] > space[2])
                return retValue;

            #region diagram
            //              __
            // _____       |  |
            //|_____| = 0  |  |= 1
            //             |__|

            //rotation {0,1} 
            //overlapRotaion {0,1}
            //numHinStep {1,n}
            //numVinStep {1,n}
            //maxHinStep {1.847293}
            //maxVinStep {6.43}
            //numOfSteps {2,n}
            //maxOfSteps{3}
            //numRight_0_Above_1 {1, n} - depending on overlapRotaion
            //maxRight_0_Above_1 {5} - depending on overlapRotaion
            // __ __ ___ ___
            //|  |  |___|___|
            //|__|__|_|  |  |    0 = overlapRotation horizontal x-
            //|___|___|__|__|  

            // ___ __
            //|___|  | 
            //|  ||__|  1= overlapRotation of steps is vertical y-
            //|__|___|
            #endregion

            for (int rotation = 0; rotation < 2; rotation++)//0=normal i.e. item[0] space[0] 1= item[1] space[0]
            {
                for (int overlapRotation = 0; overlapRotation < 2; overlapRotation++)
                {
                    #region setting limits on dimensions of steps
                    double maxHinStep = (space[0] - item[1]) / item[0];
                    double maxVinStep = (0.5 * space[1]) / item[1];
                    if (rotation == 0 && overlapRotation == 1)
                    {
                        maxHinStep = (0.5 * space[0]) / item[0];
                        maxVinStep = (space[1] - item[0]) / item[1];
                    }
                    else if (rotation == 1 && overlapRotation == 0)
                    {
                        maxHinStep = (space[0] - item[0]) / item[1];
                        maxVinStep = (0.5 * space[1]) / item[0];
                    }
                    else if (rotation == 1 && overlapRotation == 1)
                    {
                        maxHinStep = (0.5 * space[0]) / item[1];
                        maxVinStep = (space[1] - item[1]) / item[0];
                    }
                    #endregion
                    for (int numHinStep = 1; numHinStep <= maxHinStep; numHinStep++)
                    {
                        for (int numVinStep = 1; numVinStep <= maxVinStep; numVinStep++)
                        {
                            #region setting limits on number of steps 
                            double maxOfSteps = m.Floor(space[1] / (item[0] * m.Ceiling(numVinStep * item[1] / item[0]))) + 1;
                            if (rotation == 0 && overlapRotation == 1)
                                maxOfSteps = m.Floor(space[0] / (item[1] * m.Ceiling(numHinStep * item[0] / item[1]))) + 1;
                            else if (rotation == 1 && overlapRotation == 0)
                                maxOfSteps = m.Floor(space[1] / (item[1] * m.Ceiling(numVinStep * item[0] / item[1]))) + 1;
                            else if (rotation == 1 && overlapRotation == 1)
                                maxOfSteps = m.Floor(space[0] / (item[0] * m.Ceiling(numHinStep * item[1] / item[0]))) + 1;
                            #endregion
                            for (int numOfSteps = 2; numOfSteps <= maxOfSteps; numOfSteps++)
                            {
                                #region setting limits on number of E 
                                double maxRight_0_Above_1 = m.Min(m.Floor(((numOfSteps - 1) * numHinStep * item[0]) / item[1]), m.Floor((space[0] - numHinStep * item[0]) / item[1]));
                                if (rotation == 0 && overlapRotation == 1)
                                    maxRight_0_Above_1 = m.Min(m.Floor(((numOfSteps - 1) * numVinStep * item[1]) / item[0]), m.Floor((space[1] - numVinStep * item[1]) / item[0]));
                                else if (rotation == 1 && overlapRotation == 0)
                                    maxRight_0_Above_1 = m.Min(m.Floor(((numOfSteps - 1) * numHinStep * item[1]) / item[0]), m.Floor((space[0] - numHinStep * item[1]) / item[0]));
                                else if (rotation == 1 && overlapRotation == 1)
                                    maxRight_0_Above_1 = m.Min(m.Floor(((numOfSteps - 1) * numVinStep * item[0]) / item[1]), m.Floor((space[1] - numVinStep * item[0]) / item[1]));
                                #endregion
                                for (int numRight_0_Above_1 = 1; numRight_0_Above_1 <= maxRight_0_Above_1; numRight_0_Above_1++)
                                {

                                    #region Getting RowBlockWide
                                    int RowBlockWide = (int)m.Ceiling(numVinStep * item[1] / item[0]);
                                    if (rotation == 0 && overlapRotation == 1)
                                        RowBlockWide = (int)m.Ceiling(numHinStep * item[0] / item[1]);
                                    else if (rotation == 1 && overlapRotation == 0)
                                        RowBlockWide = (int)m.Ceiling(numVinStep * item[0] / item[1]);
                                    else if (rotation == 1 && overlapRotation == 1)
                                        RowBlockWide = (int)m.Ceiling(numHinStep * item[1] / item[0]);
                                    #endregion
                                    int NumberOfBlocksInStairs = (RowBlockWide * numRight_0_Above_1 + numHinStep * numVinStep) * numOfSteps;
                                    
                                    #region Getting Answers
                                    double DimHUsed = numHinStep * item[0] + numRight_0_Above_1 * item[1];
                                    double DimVUsed = numVinStep * item[1] + (numOfSteps - 1) * m.Ceiling(numVinStep * item[1] / item[0]) * item[0];
                                    if (rotation==0 && overlapRotation == 1)
                                    {
                                        DimHUsed = numHinStep * item[0] + (numOfSteps - 1) * m.Ceiling(numHinStep * item[0] / item[1]) * item[1];
                                        DimVUsed = numVinStep * item[1] + numRight_0_Above_1 * item[0];
                                    }
                                    else if (rotation == 1 && overlapRotation == 0)
                                    {
                                        DimHUsed = numHinStep * item[1] + numRight_0_Above_1 * item[0];
                                        DimVUsed = numVinStep * item[0] + (numOfSteps - 1) * m.Ceiling(numVinStep * item[0] / item[1]) * item[1];
                                    }
                                    else if (rotation == 1 && overlapRotation==1)
                                    {
                                        DimHUsed = numHinStep * item[1] + (numOfSteps - 1) * m.Ceiling(numHinStep * item[1] / item[0]) * item[0];
                                        DimVUsed = numVinStep * item[0] + numRight_0_Above_1 * item[1];
                                    }
                                    #endregion
                                    
                                    if (space[0] - DimHUsed >= 0 && space[1] - DimVUsed >= 0)
                                    {
                                        List<rectPos> tempRetValue = new List<rectPos>();
                                        tempRetValue.AddRange(StairsDescAlg(rotation, overlapRotation, numHinStep, numVinStep, numOfSteps, numRight_0_Above_1, item));
                                        tempRetValue.AddRange(LeftOverFillAlg(space, item, new double[] { DimHUsed, DimVUsed, 0 }));

                                        if (tempRetValue.Count > retValue.Count)
                                            retValue = CentreLoad(tempRetValue,space);

                                    }
                                }
                            }
                        }
                    }
                }
            }
           
            return retValue;

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <param name="space"></param>
        /// <returns></returns>
        public static Dictionary<int, List<List<rectPos>>> PackOptions(double[] item, double[] space)
        {
            SortedDictionary<int, List<List<rectPos>>> retValue = new SortedDictionary<int, List<List<rectPos>>>();
            if (item[2] > space[2])
                return new Dictionary<int, List<List<rectPos>>>(); 

            #region diagram
            //              __
            // _____       |  |
            //|_____| = 0  |  |= 1
            //             |__|

            //rotation {0,1} 
            //overlapRotaion {0,1}
            //numHinStep {1,n}
            //numVinStep {1,n}
            //maxHinStep {1.847293}
            //maxVinStep {6.43}
            //numOfSteps {2,n}
            //maxOfSteps{3}
            //numRight_0_Above_1 {1, n} - depending on overlapRotaion
            //maxRight_0_Above_1 {5} - depending on overlapRotaion
            // __ __ ___ ___
            //|  |  |___|___|
            //|__|__|_|  |  |    0 = overlapRotation horizontal x-
            //|___|___|__|__|  

            // ___ __
            //|___|  | 
            //|  ||__|  1= overlapRotation of steps is vertical y-
            //|__|___|
            #endregion

            for (int rotation = 0; rotation < 2; rotation++)//0=normal i.e. item[0] space[0] 1= item[1] space[0]
            {
                for (int overlapRotation = 0; overlapRotation < 2; overlapRotation++)
                {
                    #region setting limits on dimensions of steps
                    double maxHinStep = (space[0] - item[1]) / item[0];
                    double maxVinStep = (0.5 * space[1]) / item[1];
                    if (rotation == 0 && overlapRotation == 1)
                    {
                        maxHinStep = (0.5 * space[0]) / item[0];
                        maxVinStep = (space[1] - item[0]) / item[1];
                    }
                    else if (rotation == 1 && overlapRotation == 0)
                    {
                        maxHinStep = (space[0] - item[0]) / item[1];
                        maxVinStep = (0.5 * space[1]) / item[0];
                    }
                    else if (rotation == 1 && overlapRotation == 1)
                    {
                        maxHinStep = (0.5 * space[0]) / item[1];
                        maxVinStep = (space[1] - item[1]) / item[0];
                    }
                    #endregion
                    for (int numHinStep = 1; numHinStep <= maxHinStep; numHinStep++)
                    {
                        for (int numVinStep = 1; numVinStep <= maxVinStep; numVinStep++)
                        {
                            #region setting limits on number of steps 
                            double maxOfSteps = m.Floor(space[1] / (item[0] * m.Ceiling(numVinStep * item[1] / item[0]))) + 1;
                            if (rotation == 0 && overlapRotation == 1)
                                maxOfSteps = m.Floor(space[0] / (item[1] * m.Ceiling(numHinStep * item[0] / item[1]))) + 1;
                            else if (rotation == 1 && overlapRotation == 0)
                                maxOfSteps = m.Floor(space[1] / (item[1] * m.Ceiling(numVinStep * item[0] / item[1]))) + 1;
                            else if (rotation == 1 && overlapRotation == 1)
                                maxOfSteps = m.Floor(space[0] / (item[0] * m.Ceiling(numHinStep * item[1] / item[0]))) + 1;
                            #endregion
                            for (int numOfSteps = 2; numOfSteps <= maxOfSteps; numOfSteps++)
                            {
                                #region setting limits on number of E 
                                double maxRight_0_Above_1 = m.Min(m.Floor(((numOfSteps - 1) * numHinStep * item[0]) / item[1]), m.Floor((space[0] - numHinStep * item[0]) / item[1]));
                                if (rotation == 0 && overlapRotation == 1)
                                    maxRight_0_Above_1 = m.Min(m.Floor(((numOfSteps - 1) * numVinStep * item[1]) / item[0]), m.Floor((space[1] - numVinStep * item[1]) / item[0]));
                                else if (rotation == 1 && overlapRotation == 0)
                                    maxRight_0_Above_1 = m.Min(m.Floor(((numOfSteps - 1) * numHinStep * item[1]) / item[0]), m.Floor((space[0] - numHinStep * item[1]) / item[0]));
                                else if (rotation == 1 && overlapRotation == 1)
                                    maxRight_0_Above_1 = m.Min(m.Floor(((numOfSteps - 1) * numVinStep * item[0]) / item[1]), m.Floor((space[1] - numVinStep * item[0]) / item[1]));
                                #endregion
                                for (int numRight_0_Above_1 = numOfSteps- 1; numRight_0_Above_1 <= maxRight_0_Above_1; numRight_0_Above_1++)
                                {

                                    #region Getting RowBlockWide
                                    int RowBlockWide = (int)m.Ceiling(numVinStep * item[1] / item[0]);
                                    if (rotation == 0 && overlapRotation == 1)
                                        RowBlockWide = (int)m.Ceiling(numHinStep * item[0] / item[1]);
                                    else if (rotation == 1 && overlapRotation == 0)
                                        RowBlockWide = (int)m.Ceiling(numVinStep * item[0] / item[1]);
                                    else if (rotation == 1 && overlapRotation == 1)
                                        RowBlockWide = (int)m.Ceiling(numHinStep * item[1] / item[0]);
                                    #endregion
                                    int NumberOfBlocksInStairs = (RowBlockWide * numRight_0_Above_1 + numHinStep * numVinStep) * numOfSteps;

                                    #region Getting Answers
                                    double DimHUsed = numHinStep * item[0] + numRight_0_Above_1 * item[1];
                                    double DimVUsed = numVinStep * item[1] + (numOfSteps - 1) * m.Ceiling(numVinStep * item[1] / item[0]) * item[0];
                                    if (rotation == 0 && overlapRotation == 1)
                                    {
                                        DimHUsed = numHinStep * item[0] + (numOfSteps - 1) * m.Ceiling(numHinStep * item[0] / item[1]) * item[1];
                                        DimVUsed = numVinStep * item[1] + numRight_0_Above_1 * item[0];
                                    }
                                    else if (rotation == 1 && overlapRotation == 0)
                                    {
                                        DimHUsed = numHinStep * item[1] + numRight_0_Above_1 * item[0];
                                        DimVUsed = numVinStep * item[0] + (numOfSteps - 1) * m.Ceiling(numVinStep * item[0] / item[1]) * item[1];
                                    }
                                    else if (rotation == 1 && overlapRotation == 1)
                                    {
                                        DimHUsed = numHinStep * item[1] + (numOfSteps - 1) * m.Ceiling(numHinStep * item[1] / item[0]) * item[0];
                                        DimVUsed = numVinStep * item[0] + numRight_0_Above_1 * item[1];
                                    }
                                    #endregion

                                    if (space[0] - DimHUsed >= 0 && space[1] - DimVUsed >= 0)
                                    {
                                        List<rectPos> tempRetValue = new List<rectPos>();
                                        tempRetValue.AddRange(StairsDescAlg(rotation, overlapRotation, numHinStep, numVinStep, numOfSteps, numRight_0_Above_1, item));
                                        int bananan = 0;
                                        if (tempRetValue.Count == 20)
                                            bananan++;
                                        tempRetValue.AddRange(LeftOverFillAlg(space, item, new double[] { DimHUsed, DimVUsed, 0 }));
                                       

                                        if (!retValue.ContainsKey(tempRetValue.Count))
                                            retValue.Add(tempRetValue.Count, new List<List<rectPos>>());
                                        retValue[tempRetValue.Count].Add(CentreLoad(tempRetValue, space));

                                    }
                                }
                            }
                        }
                    }
                }
            }

            return new Dictionary<int, List<List<rectPos>>>(retValue.Reverse()); 


        }
        private static List<rectPos> StairsDescAlg(int rotation/*0-lan 1-port*/, int overlapRotation/*0-h 1-v*/, int numHinStep, int numVinStep, int numOfSteps, int numRight_0_Above_1, double [] item)
        {
            List<rectPos> retVal = new List<rectPos>();
            if (overlapRotation == 0 && rotation == 0)
            {
                int indent = 0;
                for (int step = 0; step < numOfSteps; step++)
                {
                    int dimensionsOfBlockRight_0_Above_1 = (int)m.Ceiling(numVinStep * item[1] / item[0]);
                    #region incrementing indent 
                    if (step != 0)
                    {
                        int indentincrement = (int)(m.Ceiling((double)(numRight_0_Above_1 - indent) / (numOfSteps - step)));
                        indent += indentincrement;
                    }
                    #endregion
                    #region pre step items

                    for (int a = 0; a < indent; a++)
                    {
                        for (int b = 0; b < dimensionsOfBlockRight_0_Above_1; b++)
                        {
                            retVal.Add(new rectPos()
                            {
                                startPos = new double[3] {
                                    a*item[1],
                                    (step-1)*item[0]*dimensionsOfBlockRight_0_Above_1 +numVinStep*item[1]+ b*item[0],
                                    0},
                                endPos = new double[3]
                                {
                                     (a+1)*item[1],
                                    (step-1)*item[0]*dimensionsOfBlockRight_0_Above_1 +numVinStep*item[1]+ (b+1)*item[0],
                                    item[2]},

                            });
                        }
                    }
                    #endregion
                    #region Step items
                    for (int h = 0; h < numHinStep; h++)
                    {
                        for (int v = 0; v < numVinStep; v++)
                        {
                            retVal.Add(new rectPos()
                            {
                                startPos = new double[3] {
                                    indent * item[1]+ h *item[0],
                                    step * item[0] * dimensionsOfBlockRight_0_Above_1+ v * item[1] ,
                                    0 },
                                endPos = new double[3]
                                {
                                    indent * item[1]+ (h+1) *item[0],
                                    step * item[0] * dimensionsOfBlockRight_0_Above_1+ (v+1) * item[1] ,
                                    item[2] },

                            });
                        }
                    }
                    #endregion
                    #region post step items
                    for (int a = 0; a < numRight_0_Above_1 - indent; a++)
                    {
                        for (int b = 0; b < dimensionsOfBlockRight_0_Above_1; b++)
                        {

                            retVal.Add(new rectPos()
                            {
                                startPos = new double[3] {
                                    indent * item[1] + numHinStep*item[0]+ a*item[1],
                                    step * item[0]* dimensionsOfBlockRight_0_Above_1 + b*item[0],
                                    0},
                                endPos = new double[3] {
                                    indent * item[1] + numHinStep*item[0]+ (a+1)*item[1],
                                    step * item[0]* dimensionsOfBlockRight_0_Above_1 + (b+1)*item[0],
                                    item[2]},
                            });

                        }
                    }
                    #endregion
                }

            }
            else if (overlapRotation == 0 && rotation == 1)
            {
                int indent = 0;
                for (int step = 0; step < numOfSteps; step++)
                {
                    int dimensionsOfBlockRight_0_Above_1 = (int)m.Ceiling(numVinStep * item[0] / item[1]);
                    #region incrementing indent 
                    if (step != 0)
                    {
                        int indentincrement = (int)(m.Ceiling((double)(numRight_0_Above_1 - indent) / (numOfSteps - step)));
                        indent += indentincrement;
                    }
                    #endregion
                    #region pre step items

                    for (int a = 0; a < indent; a++)
                    {
                        for (int b = 0; b < dimensionsOfBlockRight_0_Above_1; b++)
                        {
                            retVal.Add(new rectPos()
                            {
                                startPos = new double[3] {
                                    a*item[0],
                                    (step-1)*item[1]*dimensionsOfBlockRight_0_Above_1 +numVinStep*item[0]+ b*item[1],
                                    0},
                                endPos = new double[3]
                                {
                                     (a+1)*item[0],
                                    (step-1)*item[1]*dimensionsOfBlockRight_0_Above_1 +numVinStep*item[0]+ (b+1)*item[1],
                                    item[2]},

                            });
                        }
                    }
                    #endregion
                    #region Step items
                    for (int h = 0; h < numHinStep; h++)
                    {
                        for (int v = 0; v < numVinStep; v++)
                        {
                            retVal.Add(new rectPos()
                            {
                                startPos = new double[3] {
                                    indent * item[0]+ h *item[1],
                                    step * item[1] * dimensionsOfBlockRight_0_Above_1+ v * item[0] ,
                                    0 },
                                endPos = new double[3]
                                {
                                    indent * item[0]+ (h+1) *item[1],
                                    step * item[1] * dimensionsOfBlockRight_0_Above_1+ (v+1) * item[0] ,
                                    item[2] },

                            });
                        }
                    }
                    #endregion
                    #region post step items
                    for (int a = 0; a < numRight_0_Above_1 - indent; a++)
                    {
                        for (int b = 0; b < dimensionsOfBlockRight_0_Above_1; b++)
                        {

                            retVal.Add(new rectPos()
                            {
                                startPos = new double[3] {
                                    indent * item[0] + numHinStep*item[1]+ a*item[0],
                                    step * item[1]* dimensionsOfBlockRight_0_Above_1 + b*item[1],
                                    0},
                                endPos = new double[3] {
                                    indent * item[0] + numHinStep*item[1]+ (a+1)*item[0],
                                    step * item[1]* dimensionsOfBlockRight_0_Above_1 + (b+1)*item[1],
                                    item[2]},
                            });

                        }
                    }
                    #endregion
                }
            }
            else if (overlapRotation == 1 && rotation == 0)
            {
                int indent = 0;
                for (int step = 0; step < numOfSteps; step++)
                {
                    int dimensionsOfBlockRight_0_Above_1 = (int)m.Ceiling(numHinStep*item[0]/item[1]);
                    #region incrementing indent 
                    if (step != 0)
                    {
                        int indentincrement = (int)(m.Ceiling((double)(numRight_0_Above_1 - indent) / (numOfSteps - step)));
                        indent += indentincrement;
                    }
                    #endregion
                    #region pre step items

                    for (int a = 0; a < indent; a++)
                    {
                        for (int b = 0; b < dimensionsOfBlockRight_0_Above_1; b++)
                        {
                            retVal.Add(new rectPos()
                            {
                                startPos = new double[3] {
                                    (step-1)*item[1]*dimensionsOfBlockRight_0_Above_1+numHinStep*item[0]+b*item[1],
                                    a*item[0],
                                    0},
                                endPos = new double[3] 
                                {
                                     (step-1)*item[1]*dimensionsOfBlockRight_0_Above_1+numHinStep*item[0]+(b+1)*item[1],
                                    (a+1)*item[0],
                                    0},
                            
                            });
                        }
                    }
                    #endregion
                    #region Step items
                    for (int h = 0; h < numHinStep; h++)
                    {
                        for (int v = 0; v < numVinStep; v++)
                        {
                            retVal.Add(new rectPos()
                            {
                                startPos = new double[3] {
                                    step * item[1] * dimensionsOfBlockRight_0_Above_1+ h * item[0] ,
                                    indent * item[0]+ v *item[1],
                                    0 },
                                endPos = new double[3]
                                {
                                    step * item[1] * dimensionsOfBlockRight_0_Above_1+ (h+1) * item[0] ,
                                    indent * item[0]+ (v+1) *item[1],
                                    item[2] },
                            
                            });
                        }
                    }
                    #endregion
                    #region post step items
                    for (int a = 0; a < numRight_0_Above_1 - indent; a++)
                    {
                        for (int b = 0; b < dimensionsOfBlockRight_0_Above_1; b++)
                        {

                            retVal.Add(new rectPos()
                            {
                                startPos = new double[3] {
                                    step * item[1]* dimensionsOfBlockRight_0_Above_1 + b*item[1],
                                    indent * item[0] + numVinStep*item[1]+ a*item[0],
                                    0},
                                endPos = new double[3] {
                                    step * item[1]* dimensionsOfBlockRight_0_Above_1 + (b+1)*item[1],
                                    indent * item[0] + numVinStep*item[1]+ (a+1)*item[0],
                                    item[2]},
                            });

                        }
                    }
                    #endregion
                }


            }
            else if (overlapRotation == 1 && rotation == 1)
            {
                int indent = 0;
                for (int step = 0; step < numOfSteps; step++)
                {
                    int dimensionsOfBlockRight_0_Above_1 = (int)m.Ceiling(numHinStep * item[1] / item[0]);
                    #region incrementing indent 
                    if (step != 0)
                    {
                        int indentincrement = (int)(m.Ceiling((double)(numRight_0_Above_1 - indent) / (numOfSteps - step)));
                        indent += indentincrement;
                    }
                    #endregion
                    #region pre step items

                    for (int a = 0; a < indent; a++)
                    {
                        for (int b = 0; b < dimensionsOfBlockRight_0_Above_1; b++)
                        {
                            retVal.Add(new rectPos()
                            {
                                startPos = new double[3] {
                                    (step-1)*item[0]*dimensionsOfBlockRight_0_Above_1+numHinStep*item[1]+b*item[0],
                                    a*item[1],
                                    0},
                                endPos = new double[3]
                                {
                                     (step-1)*item[0]*dimensionsOfBlockRight_0_Above_1+numHinStep*item[1]+(b+1)*item[0],
                                    (a+1)*item[1],
                                    0},

                            });
                        }
                    }
                    #endregion
                    #region Step items
                    for (int h = 0; h < numHinStep; h++)
                    {
                        for (int v = 0; v < numVinStep; v++)
                        {
                            retVal.Add(new rectPos()
                            {
                                startPos = new double[3] {
                                    step * item[0] * dimensionsOfBlockRight_0_Above_1+ h * item[1] ,
                                    indent * item[1]+ v *item[0],
                                    0 },
                                endPos = new double[3]
                                {
                                    step * item[0] * dimensionsOfBlockRight_0_Above_1+ (h+1) * item[1] ,
                                    indent * item[1]+ (v+1) *item[0],
                                    item[2] },

                            });
                        }
                    }
                    #endregion
                    #region post step items
                    for (int a = 0; a < numRight_0_Above_1 - indent; a++)
                    {
                        for (int b = 0; b < dimensionsOfBlockRight_0_Above_1; b++)
                        {

                            retVal.Add(new rectPos()
                            {
                                startPos = new double[3] {
                                    step * item[0]* dimensionsOfBlockRight_0_Above_1 + b*item[0],
                                    indent * item[1] + numVinStep*item[0]+ a*item[1],
                                    0},
                                endPos = new double[3] {
                                    step * item[0]* dimensionsOfBlockRight_0_Above_1 + (b+1)*item[0],
                                    indent * item[1] + numVinStep*item[0]+ (a+1)*item[1],
                                    item[2]},
                            });

                        }
                    }
                    #endregion
                }

            }

            return retVal;
        }
        private static int[] MaxFillRectangle(double[] space, double[]item)
        {
            int[] retVal = new int[3] { (int)((space[0]) / item[0]), (int)(space[1] / item[1]), 0 };
            if ((int)((space[0]) / item[1])* (int)(space[1] / item[0]) > retVal[0] * retVal[1])
                retVal = new int[3] { (int)((space[0]) / item[1]), (int)(space[1] / item[0]), 1 };
            return retVal;
        }
        private static List<rectPos> MaxFillRectanglePositions(int[] fillRectangle, double[]bottomLeftPoint,double[]item)
        {
            List<rectPos> retVal = new List<rectPos>();
            for(int a=0;a<fillRectangle[0];a++)
            {
                for(int b=0;b<fillRectangle[1];b++)
                {
                    retVal.Add(new rectPos()
                    {
                        startPos= new double[3]
                        {
                            a*(fillRectangle[2] == 0 ? item[0]:item[1])+bottomLeftPoint[0],
                            b*(fillRectangle[2] == 0 ? item[1]:item[0])+bottomLeftPoint[1],
                            bottomLeftPoint[2]
                        },
                        endPos= new double[3]
                        {
                            (a+1)*(fillRectangle[2] == 0 ? item[0]:item[1])+bottomLeftPoint[0],
                            (b+1)*(fillRectangle[2] == 0 ? item[1]:item[0])+bottomLeftPoint[1],
                            bottomLeftPoint[2]+item[2]
                        }
                    });

                }
            }
            return retVal;
        }
        private static List<rectPos> LeftOverFillAlg(double[] space, double[] item, double[] taken)
        {
            // ____________________________________________________
            //|                               :                    | 
            //|      V2                       :        V3          | 
            //|                               :                    | 
            //|_______________________________:....................| 
            //|                               |                    | 
            //|                               |                    | 
            //|                               |                    | 
            //|                               |                    | 
            //|                               |       V1           | 
            //|        Stairs                 |                    | 
            //|                               |                    | 
            //|                               |                    | 
            //|                               |                    | 
            //|_______________________________|____________________| 

            List<int[]> clockwiseFit231 = new List<int[]>();
            clockwiseFit231.Add(MaxFillRectangle(new double[3] {taken[0] ,space[1]- taken[1] ,space[2] }, item));
            clockwiseFit231.Add(MaxFillRectangle(new double[3] {space[0]- clockwiseFit231[clockwiseFit231.Count - 1][0] * (clockwiseFit231[clockwiseFit231.Count - 1][2] == 0 ? item[0] : item[1]), space[1] - taken[1], space[2] }, item));
            clockwiseFit231.Add(MaxFillRectangle(new double[3] { space[0]- taken[0], space[1] - clockwiseFit231[clockwiseFit231.Count - 1][1] * (clockwiseFit231[clockwiseFit231.Count - 1][2] == 0 ? item[1] : item[0]), space[2] }, item));
            
            List<int[]> counterClockwiseFit132 = new List<int[]>();
            counterClockwiseFit132.Add(MaxFillRectangle(new double[3] { space[0] - taken[0], taken[1], space[2] }, item));
            counterClockwiseFit132.Add(MaxFillRectangle(new double[3] { space[0] - taken[0], space[1]- counterClockwiseFit132[counterClockwiseFit132.Count - 1][1] * (counterClockwiseFit132[counterClockwiseFit132.Count - 1][2] == 0 ? item[1] : item[0]), space[2] }, item));
            counterClockwiseFit132.Add(MaxFillRectangle(new double[3] { space[0] - counterClockwiseFit132[counterClockwiseFit132.Count - 1][0] * (counterClockwiseFit132[counterClockwiseFit132.Count - 1][2] == 0 ? item[0] : item[1]),space[1]- taken[1], space[2] }, item));

            List<rectPos> retVal = new List<rectPos>();
            if(clockwiseFit231[0][0]* clockwiseFit231[0][1] +
                clockwiseFit231[1][0] * clockwiseFit231[1][1] +
                clockwiseFit231[2][0] * clockwiseFit231[2][1] >
                counterClockwiseFit132[0][0] * counterClockwiseFit132[0][1] +
                counterClockwiseFit132[1][0] * counterClockwiseFit132[1][1] +
                counterClockwiseFit132[2][0] * counterClockwiseFit132[2][1])
            {
                //clockwise fill
                retVal.AddRange(MaxFillRectanglePositions(clockwiseFit231[0], new double[3] { 0, taken[1], 0 },item));
                retVal.AddRange(MaxFillRectanglePositions(clockwiseFit231[2], new double[3] { taken[0], 0, 0 }, item));
                retVal.AddRange(MaxFillRectanglePositions(clockwiseFit231[1], new double[3] { clockwiseFit231[0][0] * (clockwiseFit231[0][2] == 0 ? item[0] : item[1]), m.Max(taken[1], clockwiseFit231[2][1] * (clockwiseFit231[2][2] == 0 ? item[1] : item[0])), 0 }, item));
            }
            else
            {
                //counterclockwise fill
                retVal.AddRange(MaxFillRectanglePositions(counterClockwiseFit132[0], new double[3] {taken[0], 0, 0 }, item));
                retVal.AddRange(MaxFillRectanglePositions(counterClockwiseFit132[2], new double[3] {0, taken[1],  0 }, item));
                retVal.AddRange(MaxFillRectanglePositions(counterClockwiseFit132[1], new double[3] { m.Max(taken[0], counterClockwiseFit132[2][0] * (counterClockwiseFit132[2][2] == 0 ? item[0] : item[1]))   , counterClockwiseFit132[0][1] * (counterClockwiseFit132[0][2] == 0 ? item[1] : item[0]), 0 }, item));
            }
            return retVal;
        }
        private static List<rectPos> CentreLoad(List<rectPos> loadList,double[] space)
        {
            double biggestH = 0;
            double biggestV = 0;

            foreach(rectPos rP in loadList)
            {
                if (rP.endPos[0] > biggestH)
                    biggestH = rP.endPos[0];
                if (rP.endPos[1] > biggestV)
                    biggestV = rP.endPos[1];
            }

            double AdjustrmentH = (space[0] - biggestH)/2;
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
