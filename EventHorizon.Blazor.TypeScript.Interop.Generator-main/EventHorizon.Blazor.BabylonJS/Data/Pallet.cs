using BABYLON;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using m = System.Math;
using System.IO;
using System.Reflection;
using System.Net;
using EventHorizon.Blazor.BabylonJS.Data.EBAFIT;


namespace EventHorizon.Blazor.BabylonJS.Data;

public class Pallet
{
    public decimal animH {get; set;}
    public decimal speed { get; set;}
    public decimal finalY { get; set;}
    public int index { get; set; }
    public decimal[] boxDimensions { get; set; }
    public decimal[] palletDimensions { get; set; }
    public bool packType { get; set; }
    public Scene scene  { get; set; }


    //Console.WriteLine('Dan Is GAy')

    /*
    static public List<Mesh> generateBoxList(decimal[] boxDimensions, decimal[] palletDimensions, bool packType, bool drawAll, Scene scene) 
    {
        //generate pallet
        var yellow = new Color4(1m, 1, 0, 0);
        var red = new Color4(0.658m, 0, 0, 0);
        var green = new Color4(0, 0.658m, 0, 0);

        double bWidth = Convert.ToDouble(boxDimensions[0]);
        double bHeight = Convert.ToDouble(boxDimensions[1]);
        double bLength = Convert.ToDouble(boxDimensions[2]);

        var dubPalX = Convert.ToDouble(palletDimensions[0]);
        var dubPalY = Convert.ToDouble(palletDimensions[1]);
        var dubPalZ = Convert.ToDouble(palletDimensions[2]);
        var palSelfY = palletDimensions[3];

        double[] item = { bWidth, bLength, bHeight };
        double[] space = { dubPalX, dubPalY, dubPalZ };
        double[] itemF = { bLength, bWidth, bHeight };

        List<rectPos> positions = new List<rectPos>();
        List<rectPos> positionsF = new List<rectPos>();

        if (packType == true)
        {
            positions = stairsFillAlg.Pack(item, space);
            positionsF = stairsFillAlg.Pack(itemF, space);
        }
        else
        {
            positions = standardFillAlg.Pack(item, space);
            positionsF = standardFillAlg.Pack(itemF, space);
        }
        List<Mesh> boxList = new List<Mesh>();

        decimal layerHeight = 0;
        var layers = m.Round(dubPalY / bHeight);
        bool flip = true;
        for (int i = 0; i < layers; i++)
        {
            if (flip == true)
            {
                flip = !flip;
                for (int k = 0; k < positions.Count; k++)
                {
                    double width = positions[k].endPos[0] - positions[k].startPos[0];
                    double height = positions[k].endPos[2] - positions[k].startPos[2];
                    double depth = positions[k].endPos[1] - positions[k].startPos[1];

                    boxList.Add(MeshBuilder.CreateBox($"box",
                                new
                                {
                                    width = width,
                                    height = height,
                                    depth = depth,
                                    faceColors = new[] { yellow, yellow, yellow, yellow, green, red }
                                }, scene));

                    boxList.Last().position = new Vector3(Convert.ToDecimal(positions[k].startPos[0] + (width / 2)),
                                                        Convert.ToDecimal(positions[k].startPos[2] + (height / 2)) + palSelfY + layerHeight,
                                                        Convert.ToDecimal(positions[k].startPos[1] + (depth / 2)));

                    boxList.Last().enableEdgesRendering();
                    boxList.Last().edgesWidth = 1.0m;
                    boxList.Last().edgesColor = new Color4(0, 0, 0, 1);
                    if (drawAll == true)
                    {
                        boxList.Last().setEnabled(true);
                    }
                    else { boxList.Last().setEnabled(false); }
                }
            }
            else
            {
                flip = !flip;
                for (int k = 0; k < positions.Count; k++)
                {
                    double width = positionsF[k].endPos[0] - positionsF[k].startPos[0];
                    double height = positionsF[k].endPos[2] - positionsF[k].startPos[2];
                    double depth = positionsF[k].endPos[1] - positionsF[k].startPos[1];

                    boxList.Add(MeshBuilder.CreateBox($"box",
                                new
                                {
                                    width = width,
                                    height = height,
                                    depth = depth,
                                    faceColors = new[] { yellow, yellow, yellow, yellow, green, red }
                                }, scene));

                    boxList.Last().position = new Vector3(Convert.ToDecimal(positionsF[k].startPos[0] + (width / 2)),
                                                        Convert.ToDecimal(positionsF[k].startPos[2] + (height / 2)) + .16m + layerHeight,
                                                        Convert.ToDecimal(positionsF[k].startPos[1] + (depth / 2)));


                    boxList.Last().enableEdgesRendering();
                    boxList.Last().edgesWidth = 1.0m;
                    boxList.Last().edgesColor = new Color4(0, 0, 0, 1);
                    if(drawAll == true)
                    {
                        boxList.Last().setEnabled(true);
                    }
                    else { boxList.Last().setEnabled(false); }
                    

                }
            }
            layerHeight += Convert.ToDecimal(bHeight);
        }
        return boxList;
    }
    */
    static public List<Mesh> generateMultiBoxList(List<inputItem> items, decimal[] palletDimensions, bool drawAll, Scene scene) 
    {
        List<Mesh> boxList = new List<Mesh>();
        //generate pallet
        var yellow = new Color4(1m, 1, 0, 1);
        var red = new Color4(0.658m, 0, 0, 0);
        var green = new Color4(0, 0.658m, 0, 0);
        var palSelfY = palletDimensions[3];

        List<rectPos> positions = new List<rectPos>();
     
  
        
        for (int k = 0; k < positions.Count; k++)
        {
            decimal width = positions[k].endPos[0] - positions[k].startPos[0];
            decimal height = positions[k].endPos[1] - positions[k].startPos[1];
            decimal depth = positions[k].endPos[2] - positions[k].startPos[2];

            boxList.Add(MeshBuilder.CreateBox($"box",
                        new
                        {
                            width = width,
                            height = height,
                            depth = depth,
                            faceColors = new[] { yellow, yellow, yellow, yellow, green, red }
                        }, scene));

            boxList.Last().position = new Vector3((positions[k].startPos[0] + (width / 2)),
                                                (positions[k].startPos[1] + (height / 2)) + palSelfY,
                                                (positions[k].startPos[2] + (depth / 2)));

            boxList.Last().enableEdgesRendering();
            boxList.Last().edgesWidth = 1.0m;
            boxList.Last().edgesColor = new Color4(0, 0, 0, 1);
            if (drawAll == true)
            {
                boxList.Last().setEnabled(true);
            }
            else { boxList.Last().setEnabled(false); }
        }

                    boxList.Last().enableEdgesRendering();
                    boxList.Last().edgesWidth = 1.0m;
                    boxList.Last().edgesColor = new Color4(0, 0, 0, 1);
                    if (drawAll == true)
                    {
                        boxList.Last().setEnabled(true);
                    }
                    else { boxList.Last().setEnabled(false); }

        return boxList;
    }
    


    public async Task<List<Mesh>> regenPallet(List<Mesh> boxList, List<inputItem> items, decimal[] palletDimensions, bool packType,bool drawAll, Scene scene)
    {
        foreach (Mesh box in boxList) { box.dispose(); }
        boxList = generateMultiBoxList(items, palletDimensions,drawAll, scene);
        index = 0;
        return boxList;
    }
    
    private bool checkForNewLayer(List<Mesh> boxList)
    {
        if (index <= 1)
        {
            return false;
        }
        if (boxList[index].position.y > boxList[index - 1].position.y)
        {
            return true;
        }
        return false;
    }


    internal async Task<List<Mesh>> addNextBox(List<Mesh> boxList)
    {
        //animate
        finalY = boxList[index].position.y;
        boxList[index].position.y = animH;
        boxList[index].setEnabled(true);
        while ((boxList[index].position.y > finalY))
        {
            boxList[index].position.y -= speed;
            await Task.Delay(1);
        }
        boxList[index].position.y = finalY;
        index++;
        return boxList;
    }

    internal async Task<List<Mesh>> addNextLayer(List<Mesh> boxList)
    {
        speed = 0.16m;
        if(checkForNewLayer(boxList) == true)
        {
            boxList = await addNextBox(boxList);
            while (checkForNewLayer(boxList) == false)
            {
                boxList = await addNextBox(boxList);
            }
        }
        else
        {
            while(checkForNewLayer(boxList) == false)
            {
                boxList = await addNextBox(boxList);
            }
        }
        speed = 0.08m;
        return boxList;
    }

    internal async Task<List<Mesh>> reset(List<Mesh> boxList)
    {
        foreach (Mesh box in boxList) { box.setEnabled(false); }
        index = 0;
        return boxList;
    }
    //Console.WriteLine('Dan Is super GAy')
    internal async Task<List<Mesh>> removeLastBox(List<Mesh> boxList)
    {
        boxList[index-1].setEnabled(false);
        index--;
        return boxList; 
    }

    internal async Task<List<Mesh>> removeLastLayer(List<Mesh> boxList)
    {
        if(checkForNewLayer(boxList) == true)
        {
            boxList = await removeLastBox(boxList);
            while (checkForNewLayer(boxList) == false)
            {
                boxList = await removeLastBox(boxList);
            }
        }
        else 
        {
            while (checkForNewLayer(boxList) == false)
            {
                boxList = await removeLastBox(boxList);
            }
        }
      
        return boxList;
    }

    internal async Task<List<Mesh>> fillPallet(List<Mesh> boxList)
    {
        foreach (Mesh box in boxList) { box.setEnabled(true); }
        index = boxList.Count;
        return boxList;
    }



    static public List<Mesh> packFromRectPos(List<rectPos> positions, decimal[] palletDimensions, Scene scene)
    {
        List<Mesh> boxList = new List<Mesh>();
        //generate pallet
        var yellow = new Color4(0, 0, 1m, 0);
        var red = new Color4(1m, 0, 0, 0);
        var green = new Color4(0, 1m, 0, 0);
        var palSelfY = palletDimensions[3];


        for (int k = 0; k < positions.Count; k++)
        {
            decimal width = positions[k].endPos[0] - positions[k].startPos[0];
            decimal height = positions[k].endPos[1] - positions[k].startPos[1];
            decimal depth = positions[k].endPos[2] - positions[k].startPos[2];

            boxList.Add(MeshBuilder.CreateBox($"box",
                        new
                        {
                            width = width,
                            height = height,
                            depth = depth,
                            faceColors = new[] { yellow, yellow, yellow, yellow, green, red }
                        }, scene));

            boxList.Last().position = new Vector3((positions[k].startPos[0] + (width / 2)),
                                                (positions[k].startPos[1] + (height / 2)) + palSelfY,
                                                (positions[k].startPos[2] + (depth / 2)));

            boxList.Last().enableEdgesRendering();
            boxList.Last().edgesWidth = 25.0m;
            boxList.Last().edgesColor = new Color4(0, 0, 0, 1);
        
        }


        return boxList;
    }


    static public List<Mesh> packFromEBAFIT(AlgorithmPackingResult packResult, Scene scene)
    {
        List<Mesh> boxList = new List<Mesh>();
        var palSelfY = 0.16m;
        var yellow = new Color4(0, 0, 1m, 0);
        var red = new Color4(1m, 0, 0, 0);
        var green = new Color4(0, 1m, 0, 0);

        foreach (Item item in packResult.PackedItems)
        {
            boxList.Add(MeshBuilder.CreateBox($"box",
                 new
                 {
                     width = item.PackDimX,
                     height = item.PackDimY,
                     depth = item.PackDimZ,
                     faceColors = new[] { yellow, yellow, yellow, yellow, green, red }
                 }, scene));

            boxList.Last().position = new Vector3(item.CoordZ,
                                                (item.CoordY + palSelfY),
                                                item.CoordX);

            boxList.Last().enableEdgesRendering();
            boxList.Last().edgesWidth = 1.0m;
            boxList.Last().edgesColor = new Color4(0, 0, 0, 1);

        }
        return boxList;
    }

    public List<inputItem> debugPack()
    {
        List<inputItem> inputItems = new List<inputItem>();

      /*
       inputItems.Add(new inputItem(0, 0.12m, 0.09m , 0.06m, 12));
       inputItems.Add(new inputItem(0, 0.15m, 0.03m , 0.03m, 12));
       inputItems.Add(new inputItem(0, 0.12m, 0.14m , 0.16m, 12));
       inputItems.Add(new inputItem(0, 0.04m, 0.09m , 0.05m, 12));
       inputItems.Add(new inputItem(0, 0.10m, 0.09m , 0.06m, 12));
        */

        return inputItems;
    }

}
