using Comfort.Common;
using EFT;
using EFT.InventoryLogic;
using HarmonyLib;
using System;
using UnityEngine;
using static EFT.Player;

namespace HandsAreNotBusy;

internal class HANB_Component : MonoBehaviour
{
    private LocalPlayer _player;

    protected void Awake()
    {
        _player = (LocalPlayer)Singleton<GameWorld>.Instance.MainPlayer;

        if (_player == null)
        {
            HANB_Plugin.HANB_Logger.LogError("Unable to find LocalPlayer, destroying module.");
            Destroy(this);
        }

        if (!_player.IsYourPlayer)
        {
            HANB_Plugin.HANB_Logger.LogError("MainPlayer is not your player, destroying module");
            Destroy(this);
        }
    }

    protected void Update()
    {
        if (!Singleton<GameWorld>.Instantiated)
        {
            return;
        }

        if (_player == null)
        {
            return;
        }

        if (HANB_Plugin.ResetKey.Value.IsDown())
        {
            FixHandsController(_player);
        }
    }

    private void FixHandsController(Player player)
    {
        InventoryController inventoryController = player.InventoryController;
        if (inventoryController != null)
        {
            int length = inventoryController.List_0.Count;
            if (length > 0)
            {
                GEventArgs1[] args = new GEventArgs1[length];
                inventoryController.List_0.CopyTo(args);
                foreach (GEventArgs1 queuedEvent in args)
                {
                    inventoryController.RemoveActiveEvent(queuedEvent);
                }
                HANB_Plugin.HANB_Logger.LogInfo($"Cleared {length} stuck inventory operations.");
            }

            AbstractHandsController handsController = player.HandsController;

            if (handsController is FirearmController currentFirearmController)
            {
                player.MovementContext.OnStateChanged -= currentFirearmController.method_17;
                player.Physical.OnSprintStateChangedEvent -= currentFirearmController.method_16;
                currentFirearmController.RemoveBallisticCalculator();
            }

            try
            {
                player.SpawnController(player.method_162());
            }
            catch (Exception ex)
            {
                HANB_Plugin.HANB_Logger.LogWarning("Stopped exception when spawning controller. InnerException: " + ex.InnerException);
            }

            if (player.LastEquippedWeaponOrKnifeItem != null)
            {
                InteractionsHandlerClass.Discard(player.LastEquippedWeaponOrKnifeItem, inventoryController, true);

                player.ProcessStatus = EProcessStatus.None;
                player.TrySetLastEquippedWeapon();
            }
            else
            {
                player.ProcessStatus = EProcessStatus.None;
                player.SetFirstAvailableItem((result) => { });
            }

            player.SetInventoryOpened(false);
            handsController?.Destroy();

            if (handsController != null)
            {
                Destroy(handsController);
            }

            // This fixes a null ref error
            if (player.HandsController is FirearmController firearmController && firearmController.Weapon != null)
            {
                Traverse.Create(player.ProceduralWeaponAnimation)
                    .Field("_firearmAnimationData")
                    .SetValue(firearmController);
            }
        }
        else
        {
            HANB_Plugin.HANB_Logger.LogError("FixHandsController: could not find '_inventoryController' field!");
        }
    }
}
