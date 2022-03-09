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



            //initialize 3D array with dimensions derived from box size
            Mesh[,,] boxArray = new Mesh[Convert.ToInt32(palX / boxX),Convert.ToInt32(palY / boxY),Convert.ToInt32(palZ / boxZ)];
            int xIter = -1;
            int zIter = -1;
            int yIter = -1;
            //nested loops to draw and position boxes 
            for (decimal y = palSelfY; y < palY; y += boxY)//3
            {
            yIter++;
                zIter = -1;
                for (decimal z = 0; z < palZ; z += boxZ)//5
                {
                zIter++;
                    xIter = -1;
                    for (decimal x = 0; x < palX; x += boxX)//12
                        {
                        xIter++;
                        //var alpha = flip ? 0.5m : 1m;     idk what this does , like why is alpha not just 1 value?
                        //flip = !flip;
                        var red = new Color4(1, 0, 0, 1);
                        var blue = new Color4(0, 0, 1, 1);
                        var green = new Color4(0, 1, 0, 1);  
                        //fill 3D array
                        boxArray[xIter, yIter, zIter] = MeshBuilder.CreateBox($"box{xIter}{yIter}{zIter}",
                            new
                            {
                                width = boxX,
                                height = boxY,
                                depth = boxZ,
                                faceColors = new[] { green, green, green, green, blue, red }
                            }, scene);
                        var box = boxArray[xIter,yIter,zIter];
                       
                        //Add edges to box
                        box.enableEdgesRendering();
                        box.edgesWidth = 1.0m;
                        box.edgesColor = new Color4(0, 0, 0, 1);

                        //move box to its final position
                        box.position = new Vector3(x + (boxX / 2), y +(boxY/2), z + (boxZ / 2));

                        //Make box invisible
                        box.setEnabled(false);

                        //Task.WaitAny(animate);

                    }
                }
            }
            var advancedTexture = AdvancedDynamicTexture.CreateFullscreenUI("UI");
            //var selection = new BABYLON.GUI.SelectionPanel("sp"); why doesnt this exist???????????

            Button button = Button.CreateSimpleButton("button","Start");
            button.top = "200px";
            button.width = "150px";
            button.height = "40px";
            button.color = "white";
            button.cornerRadius = 20;
            button.background = "green";

            Button button2 = Button.CreateSimpleButton("button2", "Stop");
            button2.top = "200px";
            button2.left = "200px";
            button2.width = "150px";
            button2.height = "40px";
            button2.color = "white";
            button2.cornerRadius = 20;
            button2.background = "green";

            Button button3 = Button.CreateSimpleButton("button3", "Reset");
            button3.top = "200px";
            button3.left = "-200px";
            button3.width = "150px";
            button3.height = "40px";
            button3.color = "white";
            button3.cornerRadius = 20;
            button3.background = "green";




            const decimal speed = 0.03m;
            decimal animH = (palY + palSelfY + 1.5m);
            int[] xyz = new int[3] {0,0,0};
            bool running = false;
         

            async Task animate()
            {
  
                for (int y = xyz[1]; y < boxArray.GetLength(1); y++)
                {  
                    for (int z = xyz[2]; z < boxArray.GetLength(2); z++)
                    {
                        for (int x = xyz[0]; x < boxArray.GetLength(0); x++)
                        {                     
                            button2.onPointerClickObservable.add(async (Vector2WithInfo arg1, EventState state) =>
                            {
                                xyz[0] = x;
                                xyz[1] = y;
                                xyz[2] = z;
                                z = boxArray.GetLength(2);
                                y = boxArray.GetLength(1);
                                x = boxArray.GetLength(0);
                            });
                            var box = boxArray[x, y, z];
                            var finalY = box.position.y;
                            box.position.y = animH;
                            box.setEnabled(true);
                            while (box.position.y > finalY)
                            {
                                box.position.y -= speed;
                                await Task.Delay(1);
                            }
                            box.position.y = finalY;
                        }
              
                        
                    }
                  
                }
                running = false;

            }

         
            button.onPointerClickObservable.add(async (Vector2WithInfo arg1, EventState state) =>
            {
                if(running == false)
                {
                    running = true;
                    await animate();
                    Console.WriteLine(string.Join("\n", xyz));
                }          
            });

            button3.onPointerClickObservable.add(async (Vector2WithInfo arg1, EventState state) =>
            {
            //todo
            });

            advancedTexture.addControl(button);

            advancedTexture.addControl(button2);

            advancedTexture.addControl(button3);

            /*
            scene.registerBeforeRender(new ActionCallback(async () => await Task.Run(() =>
            {
                if (animH <= (y + (boxY / 2)))
                {
                    box.position.y = y + (boxY / 2);
                    return Task.CompletedTask;
                }
                animH -= speed;
                box.position.y = animH;
                return null;
            })));
            await Task.Delay(200);
            */
        }


        /*
         Animation ySlide = new Animation("ySlide", "AnimH", frameRate,
         BABYLON.Animation.ANIMATIONTYPE_FLOAT, BABYLON.Animation.ANIMATIONLOOPMODE_CYCLE);

        IAnimationKey[] test = new IAnimationKey[3];
        test[0] = new IAnimationKeyCachedEntity{frame = 0, value = new CachedEntity{___guid = animH.ToString()}};
        test[1] = new IAnimationKeyCachedEntity{frame = 0, value = new CachedEntity{___guid = (animH/2).ToString()}};
        test[2] = new IAnimationKeyCachedEntity{frame = 0, value = new CachedEntity{___guid = (palSelfY).ToString()}};
        ((IList)box.animations).Add(ySlide);
        */


        /*var heptagonalPrism = news
                {
                    name = "Heptagonal Prism",
                    category = new[] {"Prism"},
                    vertex = new decimal[][]
                    {
                        new decimal[] {0, 0, 1.090071m}, new decimal[] {0.796065m, 0, 0.7446715m},
                        new decimal[] {-0.1498633m, 0.7818315m, 0.7446715m},
                        new decimal[] {-0.7396399m, -0.2943675m, 0.7446715m},
                        new decimal[] {0.6462017m, 0.7818315m, 0.3992718m},
                        new decimal[] {1.049102m, -0.2943675m, -0.03143449m},
                        new decimal[] {-0.8895032m, 0.487464m, 0.3992718m},
                        new decimal[] {-0.8658909m, -0.6614378m, -0.03143449m},
                        new decimal[] {0.8992386m, 0.487464m, -0.3768342m},
                        new decimal[] {0.5685687m, -0.6614378m, -0.6538232m},
                        new decimal[] {-1.015754m, 0.1203937m, -0.3768342m},
                        new decimal[] {-0.2836832m, -0.8247995m, -0.6538232m},
                        new decimal[] {0.4187054m, 0.1203937m, -0.9992228m},
                        new decimal[] {-0.4335465m, -0.042968m, -0.9992228m}
                    },
                    face = new decimal[][]
                    {
                        new decimal[] {0, 1, 4, 2}, new decimal[] {0, 2, 6, 3}, new decimal[] {1, 5, 8, 4},
                        new decimal[] {3, 6, 10, 7}, new decimal[] {5, 9, 12, 8}, new decimal[] {7, 10, 13, 11},
                        new decimal[] {9, 11, 13, 12}, new decimal[] {0, 3, 7, 11, 9, 5, 1},
                        new decimal[] {2, 4, 8, 12, 13, 10, 6}
                    }
                }
                ;
            Random r = new Random();
            var faceColors = new List<Color3>();
            for (var i = 0; i < 9; i++)
            {
                faceColors.Add(new Color3(r.Next(), r.Next(), r.Next()));
            }

            var heptPrism = MeshBuilder.CreatePolyhedron("h",
                new {custom = heptagonalPrism, faceColors = faceColors.ToArray()}, scene);
            heptPrism.position = new Vector3(3, 3, 3);
            */


        public string ___guid { get; set; }
    }

}

