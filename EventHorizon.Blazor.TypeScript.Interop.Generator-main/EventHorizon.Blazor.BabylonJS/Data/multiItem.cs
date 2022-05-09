using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using M = System.Math;

namespace EventHorizon.Blazor.BabylonJS.Data
{

    public static class multiItem
    {
        
        //item takes tuples of 4 elements [X,Y,Z , Quantity] 
        //space takes one tuple for the area of the bounding box
        public static List<rectPos> Pack(decimal[,] item, decimal[] space)
        {
            List<rectPos> retValue = new List<rectPos>();
            decimal[] startPos = {0,0,0};
            decimal[] endPos = {0,0,0};
            //1. Take Array of items and sort by priority (bigger item, higher priority) 

            //2. Create the largest block possible from items of the same type (without exceeding the space or running out of items)

            //for each item
            for (int i = 0; i < item.GetLength(0); i++)
            {
                //for each of the given quantity of the item
                for (int j = 0; j < item[i, 3]; j++)
                {
                   //check if theres space for the item

                    bool newBoxFitsSpaceX = (startPos[0] + item[i, 0] < space[0]);
                    bool newBoxFitsSpaceY = ((startPos[1] + item[i, 1] < space[1]) && newBoxFitsSpaceX == false);
                    bool newBoxFitsSpaceZ = ((startPos[2] + item[i, 2] < space[2]) && newBoxFitsSpaceY == false);

                    if (newBoxFitsSpaceZ == true)
                    {
                        endPos = new decimal[] { startPos[0], startPos[1], startPos[2] + item[i, 2] };
                    }
                    if(newBoxFitsSpaceY == true)
                    {
                        endPos = new decimal[] { startPos[0] , startPos[1] + item[i, 1], startPos[2] };
                    }
                    if (newBoxFitsSpaceX == true)
                    {
                        endPos = new decimal[] { startPos[0] + item[i, 0], startPos[1], startPos[2] };
                    }
                    retValue.Add(new rectPos()
                    {
                        startPos = startPos,
                        endPos = endPos
                    }); 
                    startPos = retValue.Last().endPos;
                }
            }
            
            //3. Fill the space with the largest possible block
            //4. Create a new set of cuboids from remaining space 
            //5. repeat previous 3 steps on largest remaining space 
            //6. repeat previous step until the remaining space is smaller than the smallest item 
            
            
          
            return retValue;
        }
    }
}


