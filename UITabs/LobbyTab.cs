namespace BigDebug.UITabs;

public class LobbyTab : BigUITab
{
    public override string TabName => "Lobby";
    public override bool Experimental => true;
    
    private Il2CppSystem.Collections.Generic.List<PlayerCharacter> players;
    public override void Layout()
    {
        UnityMainThreadDispatcher.Enqueue(() =>
        {
            players = PlayerCharacter.allPlayerCharacters;
        });
    }
}