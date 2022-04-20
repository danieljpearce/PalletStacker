using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BABYLON;
using BABYLON.GUI;
using EventHorizon.Blazor.BabylonJS.Model;
using EventHorizon.Blazor.BabylonJS.Data;
using EventHorizon.Blazor.Interop;
using EventHorizon.Blazor.Interop;
using EventHorizon.Blazor.Interop.Callbacks;
using System.Text.Json;
using m = System.Math;


namespace EventHorizon.Blazor.BabylonJS.Pages
{

    using System.Threading;
    using System.Net.Http.Json;
    using System.Net.Http;
    using System.Runtime.Serialization.Formatters.Binary;
    using Microsoft.AspNetCore.Components.Forms;

    public partial class PalletFork : IDisposable
    {
        private Engine _engine;
        private Scene _scene;



        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await CreateSceneAsync();
            }
        }

        public void Dispose()
        {
            _engine?.dispose();
        }

        public async Task CreateSceneAsync()
        {
            var canvas = Canvas.GetElementById(
                "game-window"
            );
            var engine = new Engine(
                canvas,
                true
            );

            // This creates a basic Babylon Scene object (non-mesh)
            var scene = new Scene(engine);

            scene.clearColor = new Color4(
                1,
                1,
                1,
                1
            );

            //Load the pallet 

            var palletModel = (await SceneLoader.ImportMeshAsync(
                "",
                "http://localhost:5000/assets/",
                "ukpallet_one.glb",
                scene
            )).ToEntity<SceneLoaderImportMeshEntity>();
            palletModel.meshes[0].name = "pallet";

            decimal palX = 1.2m, palZ = 1m, palY = 1m, palSelfY = .16m;//Pallet dimensions
            //add an arcRotateCamera to the scene
            var camera = new ArcRotateCamera(
                "camera",
                Tools.ToRadians(60), //maypole arc
                Tools.ToRadians(60), //birdseye-to-horizon arc
                10,
                new Vector3(palX / 2, palZ / 2, palY / 2),
                scene
            );
            camera.lowerRadiusLimit = 2;
            camera.upperRadiusLimit = 10;
            camera.inertia = 0.5m;
            camera.inertialRadiusOffset *= 2;
            camera.wheelPrecision = 50;
            camera.wheelDeltaPercentage = 0.1m;

            // This attaches the camera to the canvas
            camera.attachControl(true);
            _scene = scene;
            _scene.activeCamera = camera;
            _engine = engine;
            var light = new HemisphericLight("ambientLight", new Vector3(0, 10, 0), scene);

            //palY = boxY; //temporary
            engine.runRenderLoop(new ActionCallback(
                () => Task.Run(() => _scene.render(true, false))
            ));

            await formSubmit.Task;

            decimal[] boxDim = { boxDimensions.boxX, boxDimensions.boxZ, boxDimensions.boxY };
            decimal[] palletDim = { palX, palY, palZ, palSelfY };
            bool packType = boxDimensions.useStaircase;

            List<Mesh> boxList = generatePallet.generateBoxList(boxDim, palletDim, packType, scene);





            var advancedTexture = AdvancedDynamicTexture.CreateFullscreenUI("UI");
            //var selection = new BABYLON.GUI.SelectionPanel("sp"); why doesnt this exist???????????

            Button nextLayer = Button.CreateSimpleButton("nextLayer", "Next Layer");
            nextLayer.top = "200px";
            nextLayer.width = "150px";
            nextLayer.height = "40px";
            nextLayer.color = "white";
            nextLayer.cornerRadius = 20;
            nextLayer.background = "green";

            Button resetButton = Button.CreateSimpleButton("resetButton", "Reset");
            resetButton.top = "225px";
            resetButton.left = "200px";
            resetButton.width = "150px";
            resetButton.height = "40px";
            resetButton.color = "white";
            resetButton.cornerRadius = 20;
            resetButton.background = "green";

            Button nextBox = Button.CreateSimpleButton("nextBox", "Next Box");
            nextBox.top = "200px";
            nextBox.left = "-200px";
            nextBox.width = "150px";
            nextBox.height = "40px";
            nextBox.color = "white";
            nextBox.cornerRadius = 20;
            nextBox.background = "green";

            Button lastBox = Button.CreateSimpleButton("lastBox", "Last Box");
            lastBox.top = "250px";
            lastBox.left = "-200px";
            lastBox.width = "150px";
            lastBox.height = "40px";
            lastBox.color = "white";
            lastBox.cornerRadius = 20;
            lastBox.background = "green";

            Button lastLayer = Button.CreateSimpleButton("lastLayer", "Last Layer");
            lastLayer.top = "250px";
            lastLayer.width = "150px";
            lastLayer.height = "40px";
            lastLayer.color = "white";
            lastLayer.cornerRadius = 20;
            lastLayer.background = "green";

            advancedTexture.addControl(nextLayer);
            advancedTexture.addControl(resetButton);
            advancedTexture.addControl(nextBox);
            advancedTexture.addControl(lastBox);
            advancedTexture.addControl(lastLayer);

            decimal speed = 0.06m;
            decimal animH = (palY + palSelfY + 1.5m);
            decimal finalY = boxList[0].position.y;

            bool drawNextBox = false;
            bool nextLayerButtonPressed = false;
            bool reset = false;
            bool deleteCurrentBox = false;
            bool deleteCurrentLayer = false;

            for (int i = 0; i < boxList.Count; i++)
            {
                buttonClick = new TaskCompletionSource<bool>();
                //On Click of 'Next Box'
                nextBox.onPointerClickObservable.add(async (Vector2WithInfo arg1, EventState state) =>
                {
                    drawNextBox = true;
                    buttonClick?.TrySetResult(true);
                });

                //On Click of 'Next Layer'
                nextLayer.onPointerClickObservable.add(async (Vector2WithInfo arg1, EventState state) =>
                {
                    drawNextBox = true;
                    nextLayerButtonPressed = true;
                    buttonClick?.TrySetResult(true);
                });

                //On Click of 'resetButton'
                resetButton.onPointerClickObservable.add(async (Vector2WithInfo arg1, EventState state) =>
                {
                    reset = true;
                    buttonClick?.TrySetResult(true);
                });

                //On Click of 'lastBox'
                lastBox.onPointerClickObservable.add(async (Vector2WithInfo arg1, EventState state) =>
                {
                    deleteCurrentBox = true;
                    buttonClick?.TrySetResult(true);
                });

                //On Click of 'lastLayer'
                lastLayer.onPointerClickObservable.add(async (Vector2WithInfo arg1, EventState state) =>
                {
                    deleteCurrentLayer = true;
                    buttonClick?.TrySetResult(true);
                });

                //detect new layer 
                if (boxList[i].position.y != finalY)
                {
                    deleteCurrentBox = false;
                    deleteCurrentLayer = false;
                    nextLayerButtonPressed = false;
                    drawNextBox = false;
                    finalY = boxList[i].position.y;
                }

                //if flag is true continue drawing rest of layer
                if (nextLayerButtonPressed == true)
                {
                    drawNextBox = true;
                    buttonClick?.TrySetResult(true);
                }

                //if flag is true continue deleting rest of layer
                if (deleteCurrentLayer == true)
                {
                    drawNextBox = false;
                    nextLayerButtonPressed = false;
                    buttonClick?.TrySetResult(true);
                }

                await buttonClick.Task;//Wait for button pressed flag to be true

                if(regen == true)
                {
                    boxDim = new[] { boxDimensions.boxX, boxDimensions.boxZ, boxDimensions.boxY };
                    packType = boxDimensions.useStaircase;
                    boxList = generatePallet.regenPallet(boxList,boxDim, palletDim, packType, scene);
                }

                if (nextLayerButtonPressed == true) { speed = 0.10m; }
                else { speed = 0.06m; }

                //reset if flag is true
                if (reset == true)
                {
                    nextLayerButtonPressed = false;
                    foreach (Mesh box in boxList) { box.setEnabled(false); }
                    i = -1;
                    finalY = boxList[0].position.y;
                    reset = false;
                    continue;
                }

                //delete most recent box if flag is true
                if (deleteCurrentBox == true | deleteCurrentLayer == true)
                {
                    if (i - 1 < 0)
                    {
                        i = 0;
                        continue;
                    }
                    else
                    {
                        boxList[i - 1].setEnabled(false);
                        i -= 2;
                        if (i < -1) { i = -1; }//makes sure removing boxes cant set i to a negative value
                        deleteCurrentBox = false;
                        continue;
                    }
                }

                //draw next box if flag is true
                if (drawNextBox == true)
                {
                    drawNextBox = false;
                    //animate
                    boxList[i].position.y = animH;
                    boxList[i].setEnabled(true);
                    while ((boxList[i].position.y > finalY))
                    {
                        boxList[i].position.y -= speed;
                        await Task.Delay(1);
                    }
                    boxList[i].position.y = finalY;
                }
            }
        }

   

        public string ___guid { get; set; }
    }

}

