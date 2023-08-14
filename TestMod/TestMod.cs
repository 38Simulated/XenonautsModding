using System;
using System.Reflection;
using HarmonyLib;
using UnityEngine;
using Xenonauts;

namespace TestMod
{
    //For this project to build correctly you will need to copy the xenonauts Assembly-CSharp.dll
    //and Assembly-CSharp-firstpass.dll in to the debug folder
    //These dlls can be found in the [Install location]\Xenonauts2_Data\Managed folder
    public class TestMod : Xenonauts.IMod
    {
        public static TestMod Instance { get; private set; }

        private Harmony _harmony;

        public void Initialise()
        {
            if (Instance is null)
            {
                Instance = this;
                Logger.Log("TestMod initialised!");
            }
            else
            {
                Logger.Log("TestMod.Initialise was called but TestMod is already initialised!");
            }
            try
            {
                _harmony = new Harmony("mods.TestMod");
                _harmony.PatchAll(Assembly.GetExecutingAssembly());
                Logger.Log("No patching exceptions");
            }
            catch (Exception e)
            {
                Logger.Log($"exception during patching: {e.GetType()} | {e.Message} | {e.InnerException}");
            }
        }
    }
}
