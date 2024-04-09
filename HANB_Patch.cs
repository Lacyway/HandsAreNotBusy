using Aki.Reflection.Patching;
using Comfort.Common;
using EFT;
using System.Reflection;

namespace HandsAreNotBusy
{
    internal class HANB_Patch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return typeof(BaseLocalGame<GamePlayerOwner>).GetMethod(nameof(BaseLocalGame<GamePlayerOwner>.vmethod_1));
        }

        [PatchPostfix]
        public static void PostFix()
        {
            if (Singleton<GameWorld>.Instance.MainPlayer == null)
            {
                HANB_Plugin.HANB_Logger.LogError("Could not add component, player was null!");
                return;
            }

            Singleton<GameWorld>.Instance.MainPlayer.gameObject.AddComponent<HANB_Component>();
            HANB_Plugin.HANB_Logger.LogInfo("Added HANB Component to player: " + Singleton<GameWorld>.Instance.MainPlayer.Profile.Nickname);
        }
    }
}
