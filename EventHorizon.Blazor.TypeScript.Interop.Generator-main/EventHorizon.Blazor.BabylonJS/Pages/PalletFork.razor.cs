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
            decimal palW = 1.2m, palD = 1m, palH = 1m, palSelfH = .16m;

            //add an arcRotateCamera to the scene
            var camera = new ArcRotateCamera(
                "camera",
                Tools.ToRadians(60), //maypole arc
                Tools.ToRadians(60), //birdseye-to-horizon arc
                10,
                new Vector3(palW / 2, palD / 2, palH / 2),
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

            decimal boxW = 0.1m, boxD = 0.2m, boxH = 0.3m; //box dimensions
            bool flip = false;
            //palH = boxH; //temporary
            engine.runRenderLoop(new ActionCallback(
                () => Task.Run(() => _scene.render(true, false))
            ));
            
            

            //nested loops to draw and position boxes 

            for (decimal y = palSelfH; y < palH; y += boxH)
            {
                for (decimal z = 0; z < palD; z += boxD)
                {
                    for (decimal x = 0; x < palW; x += boxW)
                    {
                        //var alpha = flip ? 0.5m : 1m;     idk what this does , like why is alpha not just 1 value?
                        //flip = !flip;
                        var red = new Color4(1, 0, 0, 1);
                        var blue = new Color4(0, 0, 1, 1);
                        var green = new Color4(0, 1, 0, 1);
                        Mesh box = MeshBuilder.CreateBox("box",
                            new
                            {
                                width = boxW,
                                height = boxH,
                                depth = boxD,
                                faceColors = new[] {green, green, green, green, blue, red}
                            }, scene);
                        decimal animH = palH + palSelfH + 1.5m;
                        box.enableEdgesRendering();
                        box.edgesWidth = 1.0m;
                        box.edgesColor = new Color4(0, 0, 0, 1);
                        box.position = new Vector3(x + (boxW / 2), animH, z + (boxD / 2));

                        
                        Task animate = new Task(() =>
                        {
                            const decimal speed = 0.02m;
                            scene.registerBeforeRender(new ActionCallback(() => 
                            {
                                if (animH <= (palSelfH + (boxH / 2)))
                                {
                                    box.position.y = palSelfH + (boxH/2);
                                    return Task.CompletedTask;
                                }

                                animH -= speed;
                                box.position.y = animH;
                                return Task.CompletedTask;
                            }));
                        });
                        animate.Start();
                        while (animH > palSelfH + (boxH / 2))
                        { 
                            Task.WaitAny(animate);
                        }
                        
                    }
                }
            }
            
        }
        
        
    /*
     Animation ySlide = new Animation("ySlide", "AnimH", frameRate,
     BABYLON.Animation.ANIMATIONTYPE_FLOAT, BABYLON.Animation.ANIMATIONLOOPMODE_CYCLE);
         
    IAnimationKey[] test = new IAnimationKey[3];
    test[0] = new IAnimationKeyCachedEntity{frame = 0, value = new CachedEntity{___guid = animH.ToString()}};
    test[1] = new IAnimationKeyCachedEntity{frame = 0, value = new CachedEntity{___guid = (animH/2).ToString()}};
    test[2] = new IAnimationKeyCachedEntity{frame = 0, value = new CachedEntity{___guid = (palSelfH).ToString()}};
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

