using System;

namespace TFModFortRiseCustomName;

public partial interface ICustomNameModApi
{
    void SetPlayerName(int playerIndex, String playerName);
    String GetPlayerName(int playerIndex);
}
