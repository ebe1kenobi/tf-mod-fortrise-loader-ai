using Microsoft.Xna.Framework;
using System;
using TowerFall;

namespace TFModFortRiseLoaderAI
{
    public static class AI
  {
    public static bool isAgentReady = false;
    private static Agent[] agents = new Agent[8];
    public static PlayerInput[] AgentInputs = new PlayerInput[8];

    public static void CreateAgent()
    {
      if (isAgentReady) return;
      //detect first player slot free
      for (int i = 0; i < TFGame.Players.Length; i++) //todo use everywhere
      {
        // create an agent for each player
        AgentInputs[i] = new Input(i);
        agents[i] = new AIAgent(i, "AI", AgentInputs[i]);
        Logger.Info("Agent " + i + " Created");
        TFModFortRiseLoaderAIModule.nbPlayerType[i]++;
        if (null != TFGame.PlayerInputs[i]) continue;
        TFGame.PlayerInputs[i] = AgentInputs[i]; // default AI
        Logger.Info("PlayerInputs " + i + " set : " + TFGame.PlayerInputs[i].GetType().ToString());
        TFModFortRiseLoaderAIModule.currentPlayerType[i] = "AI";
      }
      ModExports.addAgent("AI", agents);
      isAgentReady = true;
    }
  }
}
