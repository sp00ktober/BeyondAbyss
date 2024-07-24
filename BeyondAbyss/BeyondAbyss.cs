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
            WinchCore.Log.Info("Initializing BeyondAbyss...");

            new Harmony("sp00ktober.BeyondAbyss").PatchAll(Assembly.GetExecutingAssembly());

            WinchCore.Log.Info("Done :D");
        }
    }
}
