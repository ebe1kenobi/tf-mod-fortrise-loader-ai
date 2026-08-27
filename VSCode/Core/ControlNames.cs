using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Input;
using TowerFall;

namespace TFModFortRiseLoaderAI
{
  /// <summary>
  /// Nomme les touches et les boutons reellement assignes a une entree.
  ///
  /// <b>Pourquoi ce fichier existe</b> : sur l'ecran de selection, les deux triangles
  /// annoncent qu'on peut monter et descendre pour changer d'IA, mais pas AVEC QUOI.
  /// Un emplacement tenu par une IA n'a le plus souvent aucune manette : son entree est
  /// le clavier de secours de <see cref="KeyboardConfigs"/>, ou le haut et le bas du
  /// joueur 1 sont A et Q - pas les fleches que les triangles laissent croire.
  ///
  /// <b>On LIT l'entree de l'element, on ne devine pas depuis l'index du joueur.</b>
  /// C'est cette instance-la que <c>NotJoinedUpdate</c> interroge : selon qu'une manette
  /// humaine a ete mise de cote ou non, ce sera son mappage ou celui du clavier de
  /// secours. Fabriquer le nom a partir de l'index afficherait la bonne touche dans un
  /// cas sur deux, et rien ne le signalerait.
  /// </summary>
  internal static class ControlNames
  {
    /// <summary>Nom de la touche qui fait monter dans la liste des IA.</summary>
    public static string Up(PlayerInput input)
    {
      if (input is XGamepadInput pad)
      {
        return First(pad.Config?.Up);
      }

      if (input is KeyboardInput keyboard)
      {
        return First(keyboard.Config?.Up);
      }

      return null;
    }

    /// <summary>Nom de la touche qui fait descendre dans la liste des IA.</summary>
    public static string Down(PlayerInput input)
    {
      if (input is XGamepadInput pad)
      {
        return First(pad.Config?.Down);
      }

      if (input is KeyboardInput keyboard)
      {
        return First(keyboard.Config?.Down);
      }

      return null;
    }

    /// <summary>
    /// La PREMIERE assignation, et elle seule. Une direction peut en compter
    /// plusieurs - le pave numerique double les chiffres, la gachette double la
    /// tranche - et les afficher toutes remplirait le portrait pour dire deux fois la
    /// meme chose. On montre par quoi commencer, pas l'inventaire.
    /// </summary>
    private static string First(Keys[] keys)
    {
      return keys != null && keys.Length > 0 ? Safe(NameOf(keys[0])) : null;
    }

    private static string First(Buttons[] buttons)
    {
      return buttons != null && buttons.Length > 0 ? Safe(NameOf(buttons[0])) : null;
    }

    // ------------------------------------------------------------------
    // Noms lisibles
    // ------------------------------------------------------------------

    /// <summary>
    /// Le nom d'enumeration de XNA n'est pas fait pour etre lu : <c>D1</c> pour le
    /// chiffre 1, <c>OemComma</c> pour la virgule. On ne traduit que les familles qui
    /// mentent vraiment, le reste passe tel quel en majuscules - une liste exhaustive
    /// serait a tenir a jour pour un gain nul sur les lettres.
    /// </summary>
    private static string NameOf(Keys key)
    {
      string name = key.ToString();

      // D1..D0 : les chiffres de la rangee du haut.
      if (name.Length == 2 && name[0] == 'D' && name[1] >= '0' && name[1] <= '9')
      {
        return name.Substring(1);
      }

      if (name.StartsWith("NumPad"))
      {
        return "NUM" + name.Substring("NumPad".Length);
      }

      if (name.StartsWith("Oem"))
      {
        return name.Substring("Oem".Length).ToUpperInvariant();
      }

      switch (key)
      {
        case Keys.LeftShift: return "LSHIFT";
        case Keys.RightShift: return "RSHIFT";
        case Keys.LeftControl: return "LCTRL";
        case Keys.RightControl: return "RCTRL";
        case Keys.LeftAlt: return "LALT";
        case Keys.RightAlt: return "RALT";
        case Keys.PageUp: return "PGUP";
        case Keys.PageDown: return "PGDN";
        case Keys.Escape: return "ESC";
        case Keys.Space: return "SPACE";
        case Keys.Back: return "BKSP";
        case Keys.Enter: return "ENTER";
        default: return name.ToUpperInvariant();
      }
    }

    private static string NameOf(Buttons button)
    {
      switch (button)
      {
        case Buttons.DPadUp: return "DPAD UP";
        case Buttons.DPadDown: return "DPAD DN";
        case Buttons.DPadLeft: return "DPAD LF";
        case Buttons.DPadRight: return "DPAD RT";
        case Buttons.LeftShoulder: return "LB";
        case Buttons.RightShoulder: return "RB";
        case Buttons.LeftTrigger: return "LT";
        case Buttons.RightTrigger: return "RT";
        case Buttons.LeftStick: return "LSTICK";
        case Buttons.RightStick: return "RSTICK";
        case Buttons.LeftThumbstickUp: return "STICK UP";
        case Buttons.LeftThumbstickDown: return "STICK DN";
        case Buttons.LeftThumbstickLeft: return "STICK LF";
        case Buttons.LeftThumbstickRight: return "STICK RT";
        default: return button.ToString().ToUpperInvariant();
      }
    }

    // ------------------------------------------------------------------
    // Filtre de police
    // ------------------------------------------------------------------

    private static HashSet<char> known;

    /// <summary>
    /// Retire les caracteres que la police du jeu ne sait pas dessiner.
    ///
    /// <c>MeasureString</c> ne les ignore pas : il leve, et comme la mesure a lieu
    /// pendant le rendu, c'est tout le jeu qui tombe. Les noms de touches d'un clavier
    /// non anglais peuvent en contenir.
    /// </summary>
    private static string Safe(string text)
    {
      if (string.IsNullOrEmpty(text))
      {
        return text;
      }

      if (known == null)
      {
        var characters = TFGame.Font?.Characters;
        if (characters == null)
        {
          return text;
        }

        known = new HashSet<char>(characters);
      }

      bool clean = true;
      foreach (char c in text)
      {
        if (!known.Contains(c))
        {
          clean = false;
          break;
        }
      }

      if (clean)
      {
        return text;
      }

      var builder = new StringBuilder(text.Length);
      foreach (char c in text)
      {
        if (known.Contains(c))
        {
          builder.Append(c);
        }
      }

      return builder.ToString();
    }
  }
}
