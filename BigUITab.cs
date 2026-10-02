using ImGuiNET;
using UnityEngine;

namespace BigDebug;

public class BigUITab
{
    public string TabName { get; protected set; } = "Default";
    public PlayerCharacter pc;
    public Rigidbody rb;
    public BigTeleporter teleporter;
    public virtual void Layout()
    {
        ImGui.Text("This is the deafault tab content, please override this!");
    }
    public virtual void Awake() {}
}