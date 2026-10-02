namespace BigDebug.UITabs;

public class LobbyTab : BigUITab
{
    public LobbyTab() { TabName = "Lobby"; }
    private Il2CppSystem.Collections.Generic.List<PlayerCharacter> players;
    public override void Layout()
    {
        UnityMainThreadDispatcher.Enqueue(() =>
        {
            players = PlayerCharacter.allPlayerCharacters;
        });
    }
}