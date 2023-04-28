using HarmonyLib;
using System;
using System.Reflection;
using Winch.Core;

namespace BeyondAbyss
{
    public class BeyondAbyss
    {
        public static void Init()
        {
            //Winch.Core.API.DredgeEvent.ModAssetsLoaded += Patch;
            WinchCore.Log.Info("Initializing BeyondAbyss...");

            var harmony = new Harmony("com.dredge.BeyondAbyss");
            harmony.PatchAll(Assembly.GetExecutingAssembly());

            WinchCore.Log.Info("Done :D");
        }

        public static void Patch(object sender, EventArgs e)
        {
            WinchCore.Log.Info("Initializing BeyondAbyss...");

            var harmony = new Harmony("com.dredge.BeyondAbyss");
            harmony.PatchAll(Assembly.GetExecutingAssembly());

            WinchCore.Log.Info("Done :D");
        }
    }
}
