using UnityEngine;

namespace BigDebug;

public class BigTeleporter : MonoBehaviour
{
    private Rigidbody rb;
    private PlayerCharacter pc;
    void Awake()
    {
        pc = GetComponent<PlayerCharacter>();
        rb = pc.rb;
    }
    public void Teleport(Vector3 position)
    {
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.position = position;
        pc.faller.ClearNextFall();
        pc.grease.Teleport(position, pc.transform.rotation, true);
        pc.transform.position = position;
        pc.mover.cachedKernalPos = position;
    }
}