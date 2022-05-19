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


            Pallet Pallet = new Pallet();

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

            List<item> items = new List<item>();

            items.Add(new item() { X = 0.1m, Y = 0.12m, Z = 0.13m});
            items.Add(new item() { X = 0.2m, Y = 0.12m, Z = 0.15m });

            decimal[,] boxDim = {{ 0.34m,  0.12m, 0.23m, 40 },
                                 { 0.15m, 0.23m, 0.05m, 6 },
                                 { 0.17m, 0.09m, 0.12m, 20 }};

            decimal[] palletDim = { palX, palY, palZ, palSelfY };
            bool packType = boxDimensions.useStaircase;
            bool drawAll =  boxDimensions.drawAll;


            List<Mesh> boxList = Pallet.generateMultiBoxList(items, palletDim,drawAll, scene);

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
            resetButton.top = "200px";
            resetButton.left = "200px";
            resetButton.width = "150px";
            resetButton.height = "40px";
            resetButton.color = "white";
            resetButton.cornerRadius = 20;
            resetButton.background = "green";

            Button fillPallet = Button.CreateSimpleButton("fillPallet", "Fill Pallet");
            fillPallet.top = "250px";
            fillPallet.left = "200px";
            fillPallet.width = "150px";
            fillPallet.height = "40px";
            fillPallet.color = "white";
            fillPallet.cornerRadius = 20;
            fillPallet.background = "green";


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
            advancedTexture.addControl(fillPallet);

            Pallet.speed = 0.08m;
            Pallet.animH = (palY + palSelfY + 1.5m);
            Pallet.finalY = boxList[0].position.y;

            for (Pallet.index = 0; Pallet.index < boxList.Count; Pallet.index++)
            {
                buttonClick = new TaskCompletionSource<bool>();
                //On Click of 'Next Box'
                nextBox.onPointerClickObservable.add(async (Vector2WithInfo arg1, EventState state) =>
                {
                    boxList = await Pallet.addNextBox(boxList);
                });

                //On Click of 'Next Layer'
                nextLayer.onPointerClickObservable.add(async (Vector2WithInfo arg1, EventState state) =>
                {
                    boxList = await Pallet.addNextLayer(boxList);
                });

                //On Click of 'resetButton'
                resetButton.onPointerClickObservable.add(async (Vector2WithInfo arg1, EventState state) =>
                {
                    boxList = await Pallet.regenPallet(boxList, items, palletDim, packType, drawAll,scene);
                });

                //On Click of 'lastBox'
                lastBox.onPointerClickObservable.add(async (Vector2WithInfo arg1, EventState state) =>
                {
                    boxList = await Pallet.removeLastBox(boxList);
                });

                //On Click of 'lastLayer'
                lastLayer.onPointerClickObservable.add(async (Vector2WithInfo arg1, EventState state) =>
                {
                    boxList = await Pallet.removeLastLayer(boxList);
                });

                fillPallet.onPointerClickObservable.add(async (Vector2WithInfo arg1, EventState state) =>
                {
                    boxList = await Pallet.fillPallet(boxList);
                });

                await buttonClick?.Task;

                if(regen == true)
                {
                    boxDim = new decimal[,]{{ boxDimensions.boxX / 2, boxDimensions.boxY/2, boxDimensions.boxZ/2, 6 },
                                            { boxDimensions.boxX, boxDimensions.boxY, boxDimensions.boxZ, 10 }};
                    packType = boxDimensions.useStaircase;
                    boxList = await Pallet.regenPallet(boxList,items, palletDim, packType, drawAll, scene);
                }
            }
        }

   

        public string ___guid { get; set; }
    }

}

