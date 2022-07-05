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
using EventHorizon.Blazor.BabylonJS.Data.EBAFIT;
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
    using System.Reflection;

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

            decimal[] palDims = { palX, palY, palZ };
            List<Item> items = new List<Item>();
            Random r = new Random();

            double upperbound = 0.35;
            double lowerbound = 0.1;
           
            for (int i = 0; i < 5; i++)
            {
                decimal dim1 = Convert.ToDecimal(r.NextDouble() * (upperbound - lowerbound) + lowerbound);
                decimal dim2 = Convert.ToDecimal(r.NextDouble() * (upperbound - lowerbound) + lowerbound);
                decimal dim3 = Convert.ToDecimal(r.NextDouble() * (upperbound - lowerbound) + lowerbound);
                items.Add(new Item(i,dim1,dim2,dim3,r.Next(25)));
            }
 


            EBAFIT ebafit = new EBAFIT();
            Data.EBAFIT.Container container = new Data.EBAFIT.Container(0, palDims[0], palDims[1], palDims[2]);
            AlgorithmPackingResult packResult = ebafit.Pack(items,container);

            List<Mesh> boxList = Pallet.packFromEBAFIT(packResult, scene);

            

           // await formSubmit.Task; wait for form to be submitted


            decimal[] palletDim = { palX, palY, palZ, palSelfY };
            bool packType = boxDimensions.useStaircase;
            bool drawAll =  boxDimensions.drawAll;


            //List<Mesh> boxList = Pallet.packFromRectPos(positions, palletDim, scene);

           

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
                    //boxDim = new decimal[,]{{ boxDimensions.boxX / 2, boxDimensions.boxY/2, boxDimensions.boxZ/2, 6 },
                                           // { boxDimensions.boxX, boxDimensions.boxY, boxDimensions.boxZ, 10 }};
                    packType = boxDimensions.useStaircase;
                    boxList = await Pallet.regenPallet(boxList,items, palletDim, packType, drawAll, scene);
                }
            }
        }

       

        public static async Task<List<inputItem>> packFromFile(Scene scene)
        {
            List<inputItem> items = new List<inputItem>();
            System.Net.Http.HttpClient httpClient = new System.Net.Http.HttpClient();
            using (var s = await httpClient.GetStreamAsync("http://localhost:5000/assets/benchmarkData3.txt"))
            {
                string fullText = new StreamReader(s).ReadToEnd();
                var problem1 = fullText.Split(",");
                string[] lines = problem1[0].Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);

                for (int i = 0; i < lines.Length; i++)
                {
                    /*
                    string[] values = lines[i].Split(" ");
                    items.Add(new inputItem()
                    {
                        index = Convert.ToInt32(values[0]),
                        width = Convert.ToDecimal(values[1]),
                        height = Convert.ToDecimal(values[2]),
                        length = Convert.ToDecimal(values[3]),
                        quantity = Convert.ToInt32(values[4])
                    });
                    */
                }
            }
            return items;
        }
        public string ___guid { get; set; }
    }

}

