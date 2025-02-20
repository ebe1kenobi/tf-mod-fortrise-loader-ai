﻿using System;
using MonoMod.ModInterop;
using TFModFortRiseLoaderAI;

namespace TFModFortRiseLoaderAI
{
  [ModImportName("com.fortrise.TFModFortRiseLoaderAI")]
  public static class LoaderAIImport
  {
    public static Func<String, Agent[], bool> addAgent;
    public static Func<String, int, bool> CurrentPlayerIs;
    public static Func<int, String> GetPlayerTypePlaying;
    public static Func<int, String> GetPlayerName;
    public static Func<int, TowerFall.Level,  bool> IsAgentPlaying;
    public static Func<bool> CanAddAgent;
  }
}