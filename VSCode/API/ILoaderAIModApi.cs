using System;
using System.Collections.Generic;
using TFModFortRiseAI.Abstractions;
using TowerFall;

namespace TFModFortRiseLoaderAI;

public partial interface ILoaderAIModApi
{
  bool RegisterAgent(List<IAgentLogic> logic);
  bool CanAddAgent();

}
//namespace TFModFortRiseLoaderAI;

//public partial interface ILoaderAIModApi
//{
//  bool addAgent(String type, Agent[] agents);
//  bool CurrentPlayerIs(String type, int playerIndex);
//  String GetPlayerTypePlaying(int playerIndex);
//  String GetPlayerName(int playerIndex);
//  bool IsAgentPlaying(int playerIndex, Level level);
//  bool CanAddAgent();
//}
