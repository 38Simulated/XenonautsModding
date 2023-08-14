using HarmonyLib;
using Xenonauts;


namespace TestMod.Patches
{
    [HarmonyPatch(typeof(XenonautsMain), nameof(XenonautsMain.Start))]
    internal class XenonautsMainStartPatch
    {
        private static void Postfix()
        {
            Logger.Log("XenonautsMain.Start called");
        }
    }
}
