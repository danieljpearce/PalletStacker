using System;
using System.Collections.Generic;

namespace EventHorizon.Blazor.BabylonJS.Data
{

    public static class multiItem
    {
        //items takes tuples of 4 elements [X,Y,Z , Quantity] 
        //space takes one tuple for the area of the bounding box

        public static List<rectPos> Pack(List<item> items, decimal[] space)
        {
            List<rectPos> retValues = new List<rectPos>();
            List<decimal[]> corners = new List<decimal[]>() { new decimal[] {0,0,0},
                                                              new decimal[] {space[0], 0, 0},
                                                              new decimal[] {0, 0, space[2]},
                                                              new decimal[] {space[0], 0, space[2] } };
            decimal[] startPos = { 0, 0, 0 }; 
            for (int i = 0; i < items.Count; i++)//for each item
            {
                for (int k = 0; k < 6; k++)//for each orientation
                {
                    for (int j = 0; j < corners.Count; j++)//for each corner
                    {
                        if (fitsInCorner(corners,items[i]))
                        {
                            //need better system for box coordinates + way to check if existing boxes block a corner 
                            retValues.Add(new rectPos()
                            {
                                startPos = new decimal[] { startPos[0], startPos[1], startPos[2] },
                                endPos = new decimal[] { startPos[0] + items[i].X, startPos[1] + items[i].Y, startPos[2] + items[i].Z }
                            });
                            startPos[0] += items[i].X;
                        }
                    }
                }   
            }
            return retValues;

        }
        public static bool fitsInCorner(List<decimal[]> corners, item item)
        {
            //consider already placed boxes using their vertices 
            bool fits = false;
            return fits;
        }
    }

   
}

