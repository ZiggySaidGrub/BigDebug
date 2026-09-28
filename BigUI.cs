using System;
using System.Text.RegularExpressions;
using BepInEx.Configuration;
using UnityEngine;

namespace BigDebug;

public class BigUI : MonoBehaviour
{
    public PlayerCharacter pc;
    public Rigidbody rb;
    public bool GUIOn = false;
    void Awake()
    {
        pc = GetComponent<PlayerCharacter>();
        rb = pc.rb;
    }

    public static ConfigEntry<KeyCode> menuKey;
    public static ConfigEntry<KeyCode> unlockCursorKey;
    void Update()
    {
        if (Input.GetKeyDown(menuKey.Value))
        {
            GUIOn = !GUIOn;
        }
        if (Input.GetKeyDown(unlockCursorKey.Value))
        {
            if      (Cursor.lockState == CursorLockMode.Locked) Cursor.lockState = CursorLockMode.None;
            else if (Cursor.lockState == CursorLockMode.None)   Cursor.lockState = CursorLockMode.Locked;
        }
    }

    // void OnGUI()
    // {
    //     if (!GUIOn) return;

    //     GUI.Box(new Rect(10, 10, 250, 90), $"Big Debug {MyPluginInfo.PLUGIN_VERSION}");
    //     GUI.Label(new(15, 25, 1000, 90), $"Coords: {rb.position}");
    //     if (GUI.Button(new(15, 45, 240, 20), "Copy Coords to Clipboard"))
    //     {
    //         GUIUtility.systemCopyBuffer = $"{rb.position.x}, {rb.position.y}, {rb.position.z}";
    //     }
    //     if (GUI.Button(new(15, 70, 240, 20), "Teleport to Coords in Clipboard"))
    //     {
    //         string coords = GUIUtility.systemCopyBuffer;
    //         coords = Regex.Replace(coords, @"\s+", "");
    //         float[] splitCoords = Array.ConvertAll(coords.Split(","), Single.Parse);
    //         Vector3 pos = new(splitCoords[0], splitCoords[1], splitCoords[2]);
    //         Teleport(pos);
    //     }
    // }

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