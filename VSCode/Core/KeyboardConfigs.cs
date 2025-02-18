using Microsoft.Xna.Framework.Input;
using TowerFall;

namespace TFModFortRiseLoaderAI
{
  public static class KeyboardConfigs {

    public static KeyboardConfig[] Configs = new KeyboardConfig[8]; 

    static KeyboardConfigs() {
      for (int i = 0; i < Configs.Length; i++) {
        /* Towerfall defauly
        KeyboardConfig keyboardConfig = new KeyboardConfig();
        keyboardConfig.Left = new Keys[1] { Keys.Left };
        keyboardConfig.Right = new Keys[1] { Keys.Right };
        keyboardConfig.Up = new Keys[1] { Keys.Up };
        keyboardConfig.Down = new Keys[1] { Keys.Down };
        keyboardConfig.Jump = new Keys[1] { Keys.C };
        keyboardConfig.Shoot = new Keys[1] { Keys.X };
        keyboardConfig.AltShoot = new Keys[1] { Keys.Z };
        keyboardConfig.Dodge = new Keys[1] { Keys.LeftShift };
        keyboardConfig.Arrows = new Keys[1] { Keys.S };
        keyboardConfig.MenuAlt = new Keys[1] { Keys.Tab };
        keyboardConfig.Start = new Keys[1] { Keys.Enter };
        */
        Configs[i] = KeyboardConfig.GetDefault();
      }

      int j = 0;
      Configs[j].Down = new Keys[] { Keys.Q };
      Configs[j].Up = new Keys[] { Keys.A };
      Configs[j].Left = new Keys[] { Keys.O };
      Configs[j].Right = new Keys[] { Keys.L };
      Configs[j].Jump = new Keys[] { Keys.NumPad1 }; 
      Configs[j].Shoot = new Keys[] { Keys.F1 };
      Configs[j].Dodge = new Keys[] { Keys.F13 };

      j++;
      Configs[j].Down = new Keys[] { Keys.S };
      Configs[j].Up = new Keys[] { Keys.Z };
      Configs[j].Left = new Keys[] { Keys.P };
      Configs[j].Right = new Keys[] { Keys.M };
      Configs[j].Jump = new Keys[] { Keys.NumPad2 };
      Configs[j].Shoot = new Keys[] { Keys.F2 }; 
      Configs[j].Dodge = new Keys[] { Keys.F14 };

      j++;
      Configs[j].Down = new Keys[] { Keys.D };
      Configs[j].Up = new Keys[] { Keys.E };
      Configs[j].Left = new Keys[] { Keys.W };
      Configs[j].Right = new Keys[] { Keys.X };
      Configs[j].Jump = new Keys[] { Keys.NumPad3 };
      Configs[j].Shoot = new Keys[] { Keys.F3 };
      Configs[j].Dodge = new Keys[] { Keys.F15 };

      j++;
      Configs[j].Down = new Keys[] { Keys.F };
      Configs[j].Up = new Keys[] { Keys.R };
      Configs[j].Left = new Keys[] { Keys.C };
      Configs[j].Right = new Keys[] { Keys.V };
      Configs[j].Jump = new Keys[] { Keys.NumPad4 };
      Configs[j].Shoot = new Keys[] { Keys.F4 };
      Configs[j].Dodge = new Keys[] { Keys.F16 };

      j++;
      Configs[j].Down = new Keys[] { Keys.G };
      Configs[j].Up = new Keys[] { Keys.T };
      Configs[j].Left = new Keys[] { Keys.B };
      Configs[j].Right = new Keys[] { Keys.N };
      Configs[j].Jump = new Keys[] { Keys.NumPad5 }; 
      Configs[j].Shoot = new Keys[] { Keys.F5 };
      Configs[j].Dodge = new Keys[] { Keys.F17 };

      j++;
      Configs[j].Down = new Keys[] { Keys.H };
      Configs[j].Up = new Keys[] { Keys.Y };
      Configs[j].Left = new Keys[] { Keys.F9 };
      Configs[j].Right = new Keys[] { Keys.F10 };
      Configs[j].Jump = new Keys[] { Keys.NumPad6 };
      Configs[j].Shoot = new Keys[] { Keys.F6 };
      Configs[j].Dodge = new Keys[] { Keys.F18 };

      j++;
      Configs[j].Down = new Keys[] { Keys.J };
      Configs[j].Up = new Keys[] { Keys.U};
      Configs[j].Left = new Keys[] { Keys.F11};
      Configs[j].Right = new Keys[] { Keys.F12};
      Configs[j].Jump = new Keys[] { Keys.NumPad7 };
      Configs[j].Shoot = new Keys[] { Keys.F7 };
      Configs[j].Dodge = new Keys[] { Keys.F19 };

      j++;
      Configs[j].Down = new Keys[] { Keys.K };
      Configs[j].Up = new Keys[] { Keys.I };
      Configs[j].Left = new Keys[] { Keys.PageUp };
      Configs[j].Right = new Keys[] { Keys.PageDown };
      Configs[j].Jump = new Keys[] { Keys.NumPad8 };
      Configs[j].Shoot = new Keys[] { Keys.F8 };
      Configs[j].Dodge = new Keys[] { Keys.F20 };
    }
  }
}
