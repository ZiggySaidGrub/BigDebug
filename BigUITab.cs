using ImGuiNET;
using UnityEngine;

namespace BigDebug;

public abstract class BigUITab
{
    public virtual string TabName => "Default";
    public virtual bool Experimental => false;
    public PlayerCharacter pc;
    public Rigidbody rb;
    public BigTeleporter teleporter;
    public abstract void Layout();
    public virtual void Awake() {}
}