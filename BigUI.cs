using System;
using System.Text.RegularExpressions;
using BepInEx.Configuration;
using BigDebug.ImGuiHelpers;
using ImGuiNET;
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
    void OnEnable()
    {
        DearImGuiInjection.DearImGuiInjection.Render += OnGUIRender;
    }
    void OnDisable()
    {
        DearImGuiInjection.DearImGuiInjection.Render -= OnGUIRender;
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

    public float rbPosX = 0f;
    public float rbPosY = 0f;
    public float rbPosZ = 0f;
    public float currentTime = 0f;
    public static ConfigEntry<bool> twelveHourClock;
    private void OnGUIRender()
    {
        if (!GUIOn) return;

        if (ImGui.Begin($"Big Debug {MyPluginInfo.PLUGIN_VERSION}"))
        {
            if (ImGui.BeginTabBar("MainTabBar", ImGuiTabBarFlags.AutoSelectNewTabs | ImGuiTabBarFlags.FittingPolicyScroll))
            {
                if (ImGui.BeginTabItem("Teleport"))
                {
                    UnityMainThreadDispatcher.Enqueue(() => { rbPosX = rb.position.x; rbPosY = rb.position.y; rbPosZ = rb.position.z; });
                    ImGui.Text($"Coords: {rbPosX}, {rbPosY}, {rbPosZ}");

                    if (ImGui.Button("Copy Coords to Clipboard"))
                    {
                        ImGui.SetClipboardText($"{rbPosX}, {rbPosY}, {rbPosZ}");
                    }
                    if (ImGui.Button("Teleport to Coords in Clipboard"))
                    {
                        UnityMainThreadDispatcher.Enqueue(() =>
                        {
                            string coords = ImGui.GetClipboardText();
                            coords = Regex.Replace(coords, @"\s+", "");
                            float[] splitCoords = Array.ConvertAll(coords.Split(","), Single.Parse);
                            Vector3 pos = new(splitCoords[0], splitCoords[1], splitCoords[2]);
                            Teleport(pos);
                        });
                    }

                    ImGui.EndTabItem();
                }

                if (ImGui.BeginTabItem("Time"))
                {
                    UnityMainThreadDispatcher.Enqueue(() => { currentTime = SkyManager.GetCurrentTime(); });
                    
                    string ampm = "AM";
                    int hour = (int) Math.Floor(currentTime);

                    float minuteDecimal = currentTime - hour;
                    int minute = (int) Math.Floor(minuteDecimal * 60);

                    if (twelveHourClock.Value)
                    {
                        if (hour >= 12) ampm = "PM";
                        hour %= 12;
                        if (hour == 0) hour = 12;
                    }


                    string formattedTime = $"{hour}:{minute:D2}{(twelveHourClock.Value ? $" {ampm}" : "")}";
                    
                    ImGui.Text($"Current Time: {formattedTime}");
                    
                    ImGui.EndTabItem();
                }


                if (ImGui.BeginTabItem("About"))
                {
                    MoreImGui.TextCentered("Big Debug");
                    MoreImGui.TextCentered("By Grub");

                    ImGui.EndTabItem();
                }

                ImGui.EndTabBar();
            }

            ImGui.End();
        }
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