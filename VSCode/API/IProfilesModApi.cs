using System;

namespace TFModFortRiseProfiles;

public partial interface IProfilesModApi
{
    void SetPlayerName(int playerIndex, String playerName);
    String GetPlayerName(int playerIndex);
}
