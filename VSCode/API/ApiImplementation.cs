using System;
using Microsoft.Xna.Framework;
using TowerFall;

namespace TFModFortRiseLoaderAI;

public sealed class ApiImplementation : ILoaderAIModApi
{
  public ApiImplementation() {}

  public bool addAgent(String type, Agent[] agents)
  {
    String newNameType = type;
    int index = 1;
    while (TFModFortRiseLoaderAIModule.listAgentByType.ContainsKey(newNameType))
    {
      newNameType = type + "-" + index;
      index++;
    }
    TFModFortRiseLoaderAIModule.listAgentByType[newNameType] = agents;
    TFModFortRiseLoaderAIModule.listAgentType.Add(TFModFortRiseLoaderAIModule.listAgentType.Count, newNameType);

    for (var i = 0; i < agents.Length; i++)
    {
      TFModFortRiseLoaderAIModule.nbPlayerType[i]++;

      if (null != TFGame.PlayerInputs[i])
      {
        continue;
      }
      TFGame.PlayerInputs[i] = agents[i].getInput();
      TFModFortRiseLoaderAIModule.currentPlayerType[i] = newNameType;
    }
    return true;
  }

  public bool CurrentPlayerIs(String type, int playerIndex)
  {
    return TFModFortRiseLoaderAIModule.CurrentPlayerIs(type, playerIndex);
  }

  public String GetPlayerTypePlaying(int playerIndex)
  {
    return TFModFortRiseLoaderAIModule.GetPlayerTypePlaying(playerIndex);
  }

  public String GetPlayerName(int playerIndex)
  {
    //return TFModFortRiseLoaderAIModule.GetPlayerName(playerIndex);
    return TFModFortRiseLoaderAIModule.Instance.CustomNameModApi.GetPlayerName(playerIndex);
  }

  public bool IsAgentPlaying(int playerIndex, Level level)
  {
    return TFModFortRiseLoaderAIModule.IsAgentPlaying(playerIndex, level);
  }

  public bool CanAddAgent()
  {
    return TFModFortRiseLoaderAIModule.canAddAgent;
  }
}
