using System;
using System.Collections.Generic;
using FortRise;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Monocle;
using MonoMod.Cil;
using MonoMod.Utils;
using TFModFortRiseProfiles;
using TowerFall;

namespace TFModFortRiseLoaderAI
{
  public class MyRollcallElement : IHookable
  {
    public static Dictionary<int, String> humanPlayerName = new Dictionary<int, String>(8);
    public static Dictionary<int, Image> upArrow = new Dictionary<int, Image>(8);
    public static Dictionary<int, Image> downArrow = new Dictionary<int, Image>(8);

    public static void Load(IHarmony harmony)
    {
      harmony.Patch(
          AccessTools.DeclaredConstructor(typeof(RollcallElement), [
                                                                        typeof(int),
                                                                    ]),
          postfix: new HarmonyMethod(ctor_patch)
      );

      harmony.Patch(
          AccessTools.DeclaredMethod(typeof(RollcallElement), nameof(RollcallElement.Render)),
          prefix: new HarmonyMethod(Render_patch),
          postfix: new HarmonyMethod(Keys_postfix)
      );
      harmony.Patch(
          AccessTools.DeclaredMethod(typeof(RollcallElement), "NotJoinedUpdate"),
          prefix: new HarmonyMethod(NotJoinedUpdate_patch)
      );
      harmony.Patch(
          AccessTools.DeclaredMethod(typeof(RollcallElement), "ForceStart"),
          prefix: new HarmonyMethod(ForceStart_patch)
      );
      harmony.Patch(
          AccessTools.DeclaredMethod(typeof(RollcallElement), "StartVersus"),
          prefix: new HarmonyMethod(StartVersus_patch)
      );
    }

    public static void ctor_patch(RollcallElement __instance, int playerIndex)
    {
      //typeof(EigthPlayerImport).ModInterop();
      //typeof(ProfilesImport).ModInterop();
      //var ProfilesModApi = TFModFortRiseLoaderAIModule.Instance.ProfilesModApi;
      //TFModFortRiseLoaderAIModule.Instance.ProfilesModApi = TFModFortRiseLoaderAIModule.Instance.Context.Interop.GetApi<IProfilesModApi>("TFModFortRiseProfiles");
      //if (widerSetApi is null)
      //{
      //  return 0;
      //}

      //return widerSetApi.IsWide ? 55 : 0;

      var dynData = DynamicData.For(__instance);

      if (TFModFortRiseLoaderAIModule.savedHumanPlayerInput.ContainsKey(playerIndex))
      {
        TFGame.PlayerInputs[playerIndex] = TFModFortRiseLoaderAIModule.savedHumanPlayerInput[playerIndex];
        dynData.Set("input", TFGame.PlayerInputs[playerIndex]);
      }
      if (!TFModFortRiseLoaderAIModule.GetPlayerTypePlaying(playerIndex).Equals("HUMAN"))
      {
        TFModFortRiseLoaderAIModule.Instance.ProfilesModApi.SetPlayerName(playerIndex, TFModFortRiseLoaderAIModule.GetAIPlayerName(playerIndex));
      }
      //if (TFModFortRiseLoaderAIModule.Instance.ProfilesModApi.GetPlayerName == null) {
      //  throw new Exception("ProfilesModApi.GetPlayerName is null");
      //}
      if (TFModFortRiseLoaderAIModule.GetPlayerTypePlaying(playerIndex).Equals("HUMAN"))
      {
        humanPlayerName[playerIndex] = TFModFortRiseLoaderAIModule.Instance.ProfilesModApi.GetPlayerName(playerIndex);
      }


      Color color = Color.White;
      upArrow[playerIndex] = new Image(TFGame.Atlas["versus/playerIndicator"]);
      upArrow[playerIndex].FlipY = true;
      upArrow[playerIndex].Visible = true;
      upArrow[playerIndex].Color = color;
      __instance.Add((Component)upArrow[playerIndex]);
      upArrow[playerIndex].X = -4;
      upArrow[playerIndex].Y = 0;

      downArrow[playerIndex] = new Image(TFGame.Atlas["versus/playerIndicator"]);
      downArrow[playerIndex].Visible = true;
      __instance.Add((Component)downArrow[playerIndex]);
      downArrow[playerIndex].X = -4;
      downArrow[playerIndex].Y = 0;
      downArrow[playerIndex].Color = color;

      dynData.Dispose();
    }

    public static void SetAllPLayerInput()
    {
      for (var i = 0; i < TFGame.Players.Length; i++)
      {
        if (TFGame.Players[i])
        {
          TFGame.PlayerInputs[i] = TFModFortRiseLoaderAIModule.GetCurrentPlayerInput(i);
        }
      }
    }
    public static void ForceStart_patch(Level __instance)
    {
      SetAllPLayerInput();
    }
    public static void StartVersus_patch(Level __instance)
    {
      SetAllPLayerInput();
    }


    public static void Render_patch(Level __instance)
    {
      var dynData = DynamicData.For(__instance);
      int playerIndex = (int)dynData.Get("playerIndex");
      //SetPlayerName(playerIndex);
      // **Les fleches haut/bas s'affichent AUSSI sur une case sans humain.**
      //
      // Elles etaient conditionnees a la fleche droite du jeu, celle qui sert a changer
      // d'archer : cette fleche n'apparait qu'une fois la case rejointe par un humain,
      // si bien qu'un emplacement tenu par une IA - ou vide - n'annoncait rien. Le geste
      // marchait pourtant : NotJoinedUpdate lit deja MenuUp et MenuDown dans cet etat.
      // On ne montrait simplement pas qu'il existait.
      //
      // La seule condition qui compte est donc "y a-t-il autre chose a choisir", et
      // c'est celle qui reste.
      if (TFModFortRiseLoaderAIModule.IsThereOtherPlayerType(playerIndex))
      {
        if ("NONE".Equals(TFModFortRiseLoaderAIModule.PreviousPlayerTypeExist(playerIndex)))
        {
          upArrow[playerIndex].Visible = false;
        }
        else
        {
          upArrow[playerIndex].Visible = true;
        }

        if ("NONE".Equals(TFModFortRiseLoaderAIModule.NextPlayerTypeExist(playerIndex)))
        {
          downArrow[playerIndex].Visible = false;
        }
        else
        {
          downArrow[playerIndex].Visible = true;
        }

        var arrowSine = DynamicData.For(dynData.Get("arrowSine"));
        var rightArrowWiggle = (bool)dynData.Get("rightArrowWiggle");
        var arrowWiggle = DynamicData.For(dynData.Get("arrowWiggle"));
        float arrowSineValue = (float)arrowSine.Get("Value");
        float arrowWiggleValue = (float)arrowWiggle.Get("Value");

        int upY = -62;
        int downY = -46;
        if (TFGame.Players.Length > 4) {
          //if (EigthPlayerImport.LaunchedEightPlayer())
          //{
          //  upY = -53;
          //  downY = -37;
          //}
        } 
        upArrow[playerIndex].Y = (float)(upY + arrowSineValue * 3.0 + 5.0 * (rightArrowWiggle ? arrowWiggleValue : 0.0));
        downArrow[playerIndex].Y = (float)(downY - arrowSineValue * 3.0 + 5.0 * (!rightArrowWiggle ? arrowWiggleValue : 0.0));
        arrowSine.Dispose();
        arrowWiggle.Dispose();
      }
      else
      {
        upArrow[playerIndex].Visible = false;
        downArrow[playerIndex].Visible = false;
      }

      dynData.Dispose();

    }
    /// <summary>Hauteur des deux lignes, de part et d'autre du centre du portrait.</summary>
    private const float KeysOffsetY = 7f;

    /// <summary>
    /// Ecrit au milieu du portrait le NOM des deux touches qui changent d'IA.
    ///
    /// Les triangles disent qu'on peut monter et descendre ; ils ne disent pas avec
    /// quoi, et c'est precisement ce qui manquait. Un emplacement tenu par une IA n'a
    /// le plus souvent pas de manette : son entree est le clavier de secours, ou le
    /// haut et le bas du joueur 1 sont A et Q. Les fleches du dessin laissaient croire
    /// aux fleches du clavier.
    ///
    /// **Pose par-dessus, sans rien retirer.** Le dessin des triangles reste ce qu'il
    /// est ; ces deux lignes ne font que le completer, le temps de voir a l'usage ce
    /// qui merite de remplacer quoi.
    ///
    /// Seulement sur les emplacements tenus par une IA, et seulement s'il y a
    /// effectivement autre chose a choisir : ailleurs ce serait du texte sur un visage
    /// pour annoncer un geste sans effet.
    /// </summary>
    public static void Keys_postfix(RollcallElement __instance)
    {
      DynamicData dynData = null;

      try
      {
        dynData = DynamicData.For(__instance);
        int playerIndex = (int)dynData.Get("playerIndex");

        if (!TFModFortRiseLoaderAIModule.IsThereOtherPlayerType(playerIndex))
        {
          return;
        }

        if (!TFModFortRiseLoaderAIModule.currentPlayerType.TryGetValue(playerIndex, out string type)
            || "HUMAN".Equals(type) || "NONE".Equals(type))
        {
          return;
        }

        var input = dynData.Get("input") as PlayerInput;
        if (input == null)
        {
          return;
        }

        string up = ControlNames.Up(input);
        string down = ControlNames.Down(input);

        // **Taille PLEINE, jamais reduite.** La police du jeu est faite de pixels
        // dessines un par un : la reduire ne rapetisse pas les lettres, elle en efface
        // des traits. A 0,6 il ne restait que des points blancs, illisibles au point
        // qu'on ne reconnaissait meme pas du texte. Mieux vaut deux lignes courtes bien
        // lisibles qu'une longue en miettes - c'est pour cela que le nom de la touche
        // est seul sur sa ligne, sous une fleche qui dit le sens.
        if (!string.IsNullOrEmpty(up))
        {
          Draw.OutlineTextCentered(TFGame.Font, ControlNames.Safe("^ " + up),
              __instance.Position + new Vector2(0f, -KeysOffsetY),
              Color.White, Color.Black, 1f);
        }

        if (!string.IsNullOrEmpty(down))
        {
          Draw.OutlineTextCentered(TFGame.Font, ControlNames.Safe("v " + down),
              __instance.Position + new Vector2(0f, KeysOffsetY),
              Color.White, Color.Black, 1f);
        }
      }
      catch (Exception e)
      {
        // L'ecran de selection doit rester utilisable : sans lui on ne lance plus rien.
        Logger.Error($"[Rollcall] nom des touches non affiche : {e.Message}");
      }
      finally
      {
        dynData?.Dispose();
      }
    }

    public static void NotJoinedUpdate_patch(Level __instance)
    {
      var dynData = DynamicData.For(__instance);
      int playerIndex = (int)dynData.Get("playerIndex");
      if (dynData.Get("input") == null)
        return;
      var input = DynamicData.For(dynData.Get("input"));
      if (input == null)
        return;

      var ProfilesModApi = TFModFortRiseLoaderAIModule.Instance.ProfilesModApi;

      var MenuUp = (bool)input.Get("MenuUp");
      var MenuDown = (bool)input.Get("MenuDown");

      if (TFModFortRiseLoaderAIModule.IsThereOtherPlayerType(playerIndex))
      { //at leat 2 player type
        // Move up 


        String previousPlayerType = TFModFortRiseLoaderAIModule.PreviousPlayerTypeExist(playerIndex);
        String nextPlayerType = TFModFortRiseLoaderAIModule.NextPlayerTypeExist(playerIndex);

        if (MenuUp
            && !"NONE".Equals(previousPlayerType))
        {
          if (TFModFortRiseLoaderAIModule.currentPlayerType[playerIndex].Equals("HUMAN"))
          {
            humanPlayerName[playerIndex] = ProfilesModApi.GetPlayerName(playerIndex);
          }

          TFModFortRiseLoaderAIModule.currentPlayerType[playerIndex] = previousPlayerType;

          if (!previousPlayerType.Equals("HUMAN"))
          {
            ProfilesModApi.SetPlayerName(playerIndex, TFModFortRiseLoaderAIModule.GetAIPlayerName(playerIndex));
          }
          if (previousPlayerType.Equals("HUMAN"))
          {
            ProfilesModApi.SetPlayerName(playerIndex, humanPlayerName[playerIndex]);
          }
        }

        // Move down
        if (MenuDown
            && !"NONE".Equals(nextPlayerType))
        {
          if (TFModFortRiseLoaderAIModule.currentPlayerType[playerIndex].Equals("HUMAN"))
          {
            humanPlayerName[playerIndex] = ProfilesModApi.GetPlayerName(playerIndex);
          }

          TFModFortRiseLoaderAIModule.currentPlayerType[playerIndex] = nextPlayerType;

          if (!nextPlayerType.Equals("HUMAN"))
          {
            ProfilesModApi.SetPlayerName(playerIndex, TFModFortRiseLoaderAIModule.GetAIPlayerName(playerIndex));
          }
          if (nextPlayerType.Equals("HUMAN"))
          {
            ProfilesModApi.SetPlayerName(playerIndex, humanPlayerName[playerIndex]);
          }
        }
      }
      dynData.Dispose();
    }
  }
}
