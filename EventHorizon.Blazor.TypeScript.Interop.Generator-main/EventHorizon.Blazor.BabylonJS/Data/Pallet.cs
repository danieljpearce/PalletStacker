using BABYLON;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using m = System.Math;

namespace EventHorizon.Blazor.BabylonJS.Data;

public class Pallet
{
    public decimal animH {get; set;}
    public decimal speed { get; set;}
    public decimal finalY { get; set;}
    public int index { get; set; }

    static public List<Mesh> generateBoxList(decimal[] boxDimensions, decimal[] palletDimensions, bool packType, Scene scene) 
    {
        //generate pallet
        var red = new Color4(1, 0, 0, 1);
        var blue = new Color4(0, 0, 1, 1);
        var green = new Color4(0, 1, 0, 1);

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
                                    faceColors = new[] { green, green, green, green, blue, red }
                                }, scene));

                    boxList.Last().position = new Vector3(Convert.ToDecimal(positions[k].startPos[0] + (width / 2)),
                                                        Convert.ToDecimal(positions[k].startPos[2] + (height / 2)) + palSelfY + layerHeight,
                                                        Convert.ToDecimal(positions[k].startPos[1] + (depth / 2)));


                    boxList.Last().enableEdgesRendering();
                    boxList.Last().edgesWidth = 1.0m;
                    boxList.Last().edgesColor = new Color4(0, 0, 0, 1);
                    boxList.Last().setEnabled(false);

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
                                    faceColors = new[] { green, green, green, green, blue, red }
                                }, scene));

                    boxList.Last().position = new Vector3(Convert.ToDecimal(positionsF[k].startPos[0] + (width / 2)),
                                                        Convert.ToDecimal(positionsF[k].startPos[2] + (height / 2)) + .16m + layerHeight,
                                                        Convert.ToDecimal(positionsF[k].startPos[1] + (depth / 2)));


                    boxList.Last().enableEdgesRendering();
                    boxList.Last().edgesWidth = 1.0m;
                    boxList.Last().edgesColor = new Color4(0, 0, 0, 1);
                    boxList.Last().setEnabled(false);

                }
            }
            layerHeight += Convert.ToDecimal(bHeight);
        }
        return boxList;
    }

    static public List<Mesh> regenPallet(List<Mesh> boxList, decimal[] boxDimensions, decimal[] palletDimensions, bool packType, Scene scene)
    {
        foreach (Mesh box in boxList) { box.dispose(); }
        boxList = generateBoxList(boxDimensions, palletDimensions, packType, scene);
        return boxList;
    }

    private bool checkForNewLayer(List<Mesh> boxList)
    {
        bool newLayer = false;
        if(boxList[index].position.y != finalY)
        {
            newLayer = true;
        }
        return newLayer;
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
        while (checkForNewLayer(boxList) == false)
        {
            boxList = await addNextBox(boxList);
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

    internal async Task<List<Mesh>> removeLastBox(List<Mesh> boxList)
    {
        boxList[index-1].setEnabled(false);
        index--;
        return boxList; 
    }

    internal async Task<List<Mesh>> removeLastLayer(List<Mesh> boxList)
    {
        while (checkForNewLayer(boxList) == false)
        {
            boxList = await removeLastBox(boxList);
        }
        return boxList;
    }
}
