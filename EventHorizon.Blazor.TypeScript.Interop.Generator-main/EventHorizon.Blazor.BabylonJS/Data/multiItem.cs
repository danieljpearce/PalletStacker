using System;
using System.Collections.Generic;

namespace EventHorizon.Blazor.BabylonJS.Data
{

    public static class multiItem
    {
        //items takes tuples of 4 elements [X,Y,Z , Quantity] 
        //space takes one tuple for the area of the bounding box

        public static decimal[] origin = { 0, 0, 0 };
        public static decimal[] spaceArea = { 1.2m, 1, 1 };
        public static decimal[] bestCorner = origin;

        public static int[,] orientationMatrix = { { 0, 1, 2 },
                                                    { 0, 2, 1 },
                                                    { 1, 0, 2 },
                                                    { 1, 2, 0 },
                                                    { 2, 0, 1 },
                                                    { 2, 1, 0 }};

        static List<rectPos> retValues = new List<rectPos>();
        static List<decimal[]> corners = new List<decimal[]>() { new decimal[] {0,0,0},
                                                                new decimal[] {spaceArea[0], 0, 0},
                                                                new decimal[] {0, 0, spaceArea[2]},
                                                                new decimal[] {spaceArea[0], 0, spaceArea[2] } };


        public static List<rectPos> Pack(List<item> items, decimal[] space)
        {
            List<rectPos> retValues = new List<rectPos>();
         

            spaceArea = space;


            for (int i = 0; i < items.Count; i++)//for each item
            {

                for (int k = 0; k < 6; k++)//for each orientation
                {

                    decimal[,] orientations = getOrientations(items[i], k);
                    items[i].X = orientations[k, 0];
                    items[i].Y = orientations[k, 1];
                    items[i].Z = orientations[k, 2];

                    for (int j = 0; j < corners.Count; j++)//for each corner
                    {
                        if (fitsInCorner(corners, items[i])[0])
                        {
                            if (fitsInCorner(corners, items[i])[1])
                            {
                                retValues.Add(new rectPos()
                                {
                                    startPos = new decimal[] { bestCorner[0] - items[i].X, bestCorner[1] - items[i].Y, bestCorner[2] - items[i].Z },
                                    endPos = new decimal[] { bestCorner[0], bestCorner[1], bestCorner[2] }
                                });
                            }
                            else
                            {
                                retValues.Add(new rectPos()
                                {
                                    startPos = new decimal[] { bestCorner[0], bestCorner[1], bestCorner[2] },
                                    endPos = new decimal[] { bestCorner[0] + items[i].X, bestCorner[1] + items[i].Y, bestCorner[2] + items[i].Z }
                                });
                            }
                            //updateCorners(items[i]);
                            //update corner list
                            //need  way to check if existing boxes block a corner 
                        }
                    }
                }
            }

            return retValues;

        }

        public static bool[] fitsInCorner(List<decimal[]> corners, item item)
        {
            bool[] fits = { false, false };
            foreach (decimal[] corner in corners)
            {
                if (corner[0] + item.X < spaceArea[0] && corner[1] + item.Y < spaceArea[1] && corner[2] + item.Z < spaceArea[2])
                {
                    bestCorner = corner;
                    fits[0] = true;
                }
                else if (corner[0] - item.X > origin[0] && corner[1] + item.Y > origin[1] && item.Z + item.Z < origin[2])
                {
                    bestCorner = corner;
                    fits[0] = true;
                    fits[1] = true;
                }

            }
            //consider already placed boxes using their vertices 

            return fits;
        }
        public static void updateCorners(item item)
        {
            
            foreach (rectPos rectPos in retValues)
            {
                //check for corners with other boxes 
            }

            corners.Add(new decimal[] { bestCorner[0] + item.X, bestCorner[1], bestCorner[2] });
            corners.Add(new decimal[] { bestCorner[0], bestCorner[1], bestCorner[2] + item.Z });
            corners.Remove(bestCorner);

        }

        public static decimal[,] getOrientations(item item, int index)
        {
            decimal[,] orientations = new decimal[6, 3];

            for(int i = 0; i < 6; i++)
            {
                orientations[i, orientationMatrix[i, 0]] = item.X;
                orientations[i, orientationMatrix[i, 1]] = item.Y;
                orientations[i, orientationMatrix[i, 2]] = item.Z;
            }

            return orientations;
        }
    }

}
