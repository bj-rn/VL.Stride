using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using VL.Stride.Core;
using VL.Stride.Games;
using Stride.Engine;
using Stride.Graphics;

namespace VL.Stride
{
    // TODO: VL script should talk to one IVLNode so we can also feed it data properly from Stride as well as exposing its pins to Stride editor
    public class VLScript : AsyncScript
    {
        /// <summary>
        /// Recommended default doc while developing.
        /// </summary>
        public static string MainVLDocSrc => Path.Combine(Application.StartupPath,"..", "..", "..", "vl", "Main.vl");

        private readonly Action VLUpdate;
        readonly VLContext FContext;
        readonly bool FGoFullscreen;

        // Declared public member fields and properties will show in the game studio
        public VLScript(VLContext context, VLGame game, bool goFullscreen)
        {
            FContext = context;
            FGoFullscreen = goFullscreen;
            Game = game;
            VLGame.GameInstance = game;

            if (FContext != null)   
                VLUpdate = FContext.Update;
        }

        public new VLGame Game { get; }

        public override async Task Execute()
        {
            VLGame.GameInstance = Game;

            if (FGoFullscreen)
            {
                //if (FContext.Runtime is RuntimeHost rth)
                //    rth.Mode = Lang.Symbols.RunMode.Paused;

                var gfxOutput = GraphicsAdapterFactory.Adapters[0].Outputs;
                var displayMode = gfxOutput[0].CurrentDisplayMode;

                var screenWidth = Math.Min(displayMode.Width, 1920);
                var maxHeight = displayMode.AspectRatio < 1.7f ? 1200 : 1080;
                var screenHeight = Math.Min(displayMode.Height, maxHeight);

                Game.GraphicsDeviceManager.PreferredBackBufferWidth = screenWidth;
                Game.GraphicsDeviceManager.PreferredBackBufferHeight = screenHeight;
                Game.GraphicsDeviceManager.IsFullScreen = true;
                Game.GraphicsDeviceManager.ApplyChanges();
            }

            if (Game is VLGame vlGame)
                vlGame.AddLayerRenderFeature();

            while (true)
            {
                await Script.NextFrame();
                // Update all VL root nodes
                //await Task.Run(VLUpdate);
                FContext?.Update();
            }
        }
    }
}
