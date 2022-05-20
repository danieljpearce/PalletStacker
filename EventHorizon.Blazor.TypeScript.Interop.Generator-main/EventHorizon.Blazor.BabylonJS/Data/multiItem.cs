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

                    items[i].X = items[i].orientations[k, 0];
                    items[i].Y = items[i].orientations[k, 1];
                    items[i].Z = items[i].orientations[k, 2];
                    
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
    }

}
