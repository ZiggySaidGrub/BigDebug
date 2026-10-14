using ImGuiNET;
using Mirror;

namespace BigDebug.UITabs;

public class PropTab : BigUITab
{
    public override string TabName => "Prop";
    public override bool Experimental => true;

    public override void Layout()
    {
        if (ImGui.Button("print list of props"))
        {
            UnityMainThreadDispatcher.Enqueue(() => {
                foreach (Prop prop in Prop.allProps)
                {
                    BigDebug.Log.LogInfo($"{prop.name}, {prop.saveablePropName}, {prop.savablePropGuid}");
                    
                }
            });
        }
        if (ImGui.Button("Grab nearest [object you get for completing puzzles]"))
        {
            UnityMainThreadDispatcher.Enqueue(() => {
                Prop gourdProp = null;
                float gourdSqrMagnitude = float.PositiveInfinity;
                foreach (Prop prop in Prop.allProps)
                {
                    if (prop.name != "GourdProp") continue;
                    if (prop.currentHome != null)
                    {
                        if (prop.currentHome.saveableHomeName.ToString().Contains("monoument"))
                        {
                            BigDebug.Log.LogInfo("skipping prop due to being in slot");
                            continue;
                        }
                        if (prop.savablePropGuid == "a804fda8-ecdf-4d8c-99c7-3862667f2ca7")
                        {
                            BigDebug.Log.LogInfo("skipping prop due to being glowing");
                            continue;
                        }
                    }

                    if (gourdProp == null || (prop.transform.position - pc.transform.position).sqrMagnitude < gourdSqrMagnitude)
                    {
                        gourdProp = prop;
                        gourdSqrMagnitude = (prop.transform.position - pc.transform.position).sqrMagnitude;
                    }
                }
                if (NetworkServer.active)
                {
                    gourdProp.ServerSetUnpinned();
                }
                pc.hands.heldProp?.SetDropped(pc);
                gourdProp.SetHeld(pc);
                pc.playerNetworking.CmdPickUp(new PlayerHeldInformation(gourdProp));
                BigDebug.Log.LogInfo($"{gourdProp.name}, {gourdProp.saveablePropName}, {gourdProp.savablePropGuid}");
            });
        }
    }
}