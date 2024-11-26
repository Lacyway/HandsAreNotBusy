using Comfort.Common;
using EFT;
using SPT.Reflection.Patching;
using System.Reflection;

namespace HandsAreNotBusy
{
	internal class HANB_Patch : ModulePatch
	{
		protected override MethodBase GetTargetMethod()
		{
			return typeof(GameWorld).GetMethod(nameof(GameWorld.RegisterPlayer));
		}

		[PatchPostfix]
		public static void PostFix(IPlayer iPlayer)
		{
			if (iPlayer == null)
			{
				HANB_Plugin.HANB_Logger.LogError("Could not add component, player was null!");
				return;
			}

			if (!iPlayer.IsYourPlayer)
			{
				return;
			}

			Singleton<GameWorld>.Instance.MainPlayer.gameObject.AddComponent<HANB_Component>();
			HANB_Plugin.HANB_Logger.LogInfo("Added HANB Component to player: " + Singleton<GameWorld>.Instance.MainPlayer.Profile.Nickname);
		}
	}
}
