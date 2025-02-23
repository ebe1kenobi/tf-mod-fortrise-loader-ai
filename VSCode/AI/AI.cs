using Microsoft.Xna.Framework;
using System;
using TowerFall;

namespace TFModFortRiseLoaderAI
{
    public static class AI
  {
    public static bool isAgentReady = false;
    private static Agent[] agents;
    public static PlayerInput[] AgentInputs;

    public static void CreateAgent()
    {
      if (isAgentReady) return;
      int max = TFModFortRiseLoaderAIModule.EightPlayerMod ? 8 : 4;
      agents = new Agent[max];
      AgentInputs = new PlayerInput[max];
      //detect first player slot free
      for (int i = 0; i < max; i++) //todo use everywhere
      {
        // create an agent for each player
        AgentInputs[i] = new Input(i);
        agents[i] = new AIAgent(i, "AI", AgentInputs[i]);
        //Logger.Info("Agent " + i + " Created");
      }
      ModExports.addAgent("AI", agents, false);
      isAgentReady = true;
    }
  }
}
