﻿using System;
using MonoMod.ModInterop;

namespace TFModFortRiseLoaderAI
{
  [ModImportName("com.fortrise.TFModFortRiseCustomName")]
  public static class CustomNameImport
  {
    public static Action<int, String> SetPlayerName;
    public static Func<int, String> GetPlayerName;
  }
}
