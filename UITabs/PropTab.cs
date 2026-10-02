using ImGuiNET;

namespace BigDebug.UITabs;

public class PropTab : BigUITab
{
    public PropTab() { TabName = "Prop"; }
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
                    }

                    if (gourdProp == null || (prop.transform.position - pc.transform.position).sqrMagnitude < gourdSqrMagnitude)
                    {
                        gourdProp = prop;
                        gourdSqrMagnitude = (prop.transform.position - pc.transform.position).sqrMagnitude;
                    }
                }
                pc.hands.heldProp?.SetDropped(pc);
                gourdProp.SetHeld(pc);
            });
        }
    }
}