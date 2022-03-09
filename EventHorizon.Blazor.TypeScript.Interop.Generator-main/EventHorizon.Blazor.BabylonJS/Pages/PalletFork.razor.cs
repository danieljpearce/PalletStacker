using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BABYLON;
using BABYLON.GUI;
using EventHorizon.Blazor.BabylonJS.Model;
using EventHorizon.Blazor.Interop;
using EventHorizon.Blazor.Interop;
using EventHorizon.Blazor.Interop.Callbacks;

namespace EventHorizon.Blazor.BabylonJS.Pages
{
    using System.Threading;

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
            decimal boxX = 0.3m, boxZ = 0.5m, boxY = 0.2m; //Box dimensions - BOXES ONLY CURRENTLY WORK IF THEYRE DIVISIBLE BY PALLET DIMENSIONS
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
            var frameRate = 10;


            bool flip = false;
            //palY = boxY; //temporary
            engine.runRenderLoop(new ActionCallback(
                () => Task.Run(() => _scene.render(true, false))
            ));



            //make 3D array with dimensions derived from box size
            //Mesh[,,] boxArray = new Mesh[Convert.ToInt32(palX / boxX),Convert.ToInt32(palY / boxY),Convert.ToInt32(palZ / boxZ)]; 

            //A list is dynamic!
            List<Mesh> boxList = new List<Mesh>();

            int xIter = -1;
            int zIter = -1;
            int yIter = -1;
            int boxCount = -1;
            //nested loops to draw and position boxes 
            for (decimal y = palSelfY; y < palY; y += boxY)
            {
                yIter++;
                zIter = -1;
                for (decimal z = 0; z < palZ; z += boxZ)
                {
                    zIter++;
                    xIter = -1;
                    for (decimal x = 0; x < palX; x += boxX)
                    {
                        xIter++;
                        boxCount++;
                        //var alpha = flip ? 0.5m : 1m;     idk what this does , like why is alpha not just 1 value?
                        //flip = !flip;
                        var red = new Color4(1, 0, 0, 1);
                        var blue = new Color4(0, 0, 1, 1);
                        var green = new Color4(0, 1, 0, 1);
                        //fill 3D array
                        boxList.Add(MeshBuilder.CreateBox($"box{xIter}{yIter}{zIter}",
                            new
                            {
                                width = boxX,
                                height = boxY,
                                depth = boxZ,
                                faceColors = new[] { green, green, green, green, blue, red }
                            }, scene));
                        var box = boxList[boxCount];

                        //Add edges to box
                        box.enableEdgesRendering();
                        box.edgesWidth = 1.0m;
                        box.edgesColor = new Color4(0, 0, 0, 1);

                        //move box to its final position
                        box.position = new Vector3(x + (boxX / 2), y + (boxY / 2), z + (boxZ / 2));

                        //Make box invisible
                        box.setEnabled(false);


                    }
                }
            }
            var advancedTexture = AdvancedDynamicTexture.CreateFullscreenUI("UI");
            //var selection = new BABYLON.GUI.SelectionPanel("sp"); why doesnt this exist???????????

            Button nextLayer = Button.CreateSimpleButton("nextLayer", "Next Layer");
            nextLayer.top = "200px";
            nextLayer.width = "150px";
            nextLayer.height = "40px";
            nextLayer.color = "white";
            nextLayer.cornerRadius = 20;
            nextLayer.background = "green";

            Button button2 = Button.CreateSimpleButton("button2", "WIP");
            button2.top = "200px";
            button2.left = "200px";
            button2.width = "150px";
            button2.height = "40px";
            button2.color = "white";
            button2.cornerRadius = 20;
            button2.background = "green";

            Button nextBox = Button.CreateSimpleButton("nextBox", "Next Box");
            nextBox.top = "200px";
            nextBox.left = "-200px";
            nextBox.width = "150px";
            nextBox.height = "40px";
            nextBox.color = "white";
            nextBox.cornerRadius = 20;
            nextBox.background = "green";


            
            advancedTexture.addControl(nextLayer);

            advancedTexture.addControl(button2);

            advancedTexture.addControl(nextBox);

          
            decimal speed = 0.03m;
            decimal animH = (palY + palSelfY + 1.5m);
            int[] xyz = new int[3] { 0, 0, 0 };
            decimal finalY = boxList[0].position.y;
            bool skipToNextLayer = false;

            foreach (Mesh box in boxList)
            {
                TaskCompletionSource<bool> canDrawNextBox = null;
                TaskCompletionSource<bool> canDrawNextLayer = null;
                
                //On Click of 'Next Box'
                nextBox.onPointerClickObservable.add(async (Vector2WithInfo arg1, EventState state) =>
                {
                    skipToNextLayer = false;
                    canDrawNextLayer?.TrySetResult(true);
                    canDrawNextBox?.TrySetResult(true);
                });

                //On Click of 'Next Layer'
                nextLayer.onPointerClickObservable.add(async (Vector2WithInfo arg1, EventState state) =>
                {
                    skipToNextLayer = true;
                    canDrawNextLayer?.TrySetResult(true);
                    canDrawNextBox?.TrySetResult(true);
                });

                //check if this box belongs to a new layer
                if (box.position.y > finalY)
                {
                    canDrawNextLayer = new TaskCompletionSource<bool>();
                    await canDrawNextLayer.Task;
                }
                

                canDrawNextBox = new TaskCompletionSource<bool>();
                if (skipToNextLayer == true)
                {
                    canDrawNextBox?.TrySetResult(true);
                    speed = 0.08m;//speed up movement of boxes when skipping layers
                }
                else{
                    speed = 0.03m;
                }
                await canDrawNextBox.Task;
                if(skipToNextLayer == true) { speed = 0.08m; }//makes the first box of the layer skip also move faster
                finalY = box.position.y;

                box.position.y = animH;
                box.setEnabled(true);
                while ((box.position.y > finalY))
                {
                    box.position.y -= speed;
                    await Task.Delay(1);
                }
                box.position.y = finalY;
  
            }

            }
   
          

        public string ___guid { get; set; }
    }

}

