using ImGuiNET;
using UnityEngine;

namespace BigDebug;

public class BigUITab
{
    public virtual string TabName { get { return "Default"; } }
    public virtual bool Experimental { get { return false; } }
    public PlayerCharacter pc;
    public Rigidbody rb;
    public BigTeleporter teleporter;
    public virtual void Layout()
    {
        ImGui.Text("This is the deafault tab content, please override this!");
    }
    public virtual void Awake() {}
}