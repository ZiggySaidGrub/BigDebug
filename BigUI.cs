using System;
using System.Collections.Generic;
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
    public BigTeleporter teleporter;
    public bool GUIOn = false;
    void Awake()
    {
        pc = GetComponent<PlayerCharacter>();
        rb = pc.rb;
        teleporter = GetComponent<BigTeleporter>();

        rbPosX = rb.position.x; rbPosY = rb.position.y; rbPosZ = rb.position.z;
    }
    void OnEnable()
    {
        DearImGuiInjection.DearImGuiInjection.Render += OnGUIRender;
    }
    void OnDisable()
    {
        DearImGuiInjection.DearImGuiInjection.Render -= OnGUIRender;
    }

    void Update()
    {
        if (Input.GetKeyDown(BigDebug.BigConfig.menuKey.Value))
        {
            GUIOn = !GUIOn;
        }
        if (Input.GetKeyDown(BigDebug.BigConfig.unlockCursorKey.Value))
        {
            if      (Cursor.lockState == CursorLockMode.Locked) Cursor.lockState = CursorLockMode.None;
            else if (Cursor.lockState == CursorLockMode.None)   Cursor.lockState = CursorLockMode.Locked;
        }
    }

    private void OnGUIRender()
    {
        if (!GUIOn) return;

        ImGui.SetNextWindowSize(new(350f, 400f), ImGuiCond.FirstUseEver);
        if (ImGui.Begin($"Big Debug {MyPluginInfo.PLUGIN_VERSION}"))
        {
            if (ImGui.BeginTabBar("MainTabBar", ImGuiTabBarFlags.AutoSelectNewTabs | ImGuiTabBarFlags.FittingPolicyScroll))
            {
                if (ImGui.BeginTabItem("Teleport"))
                {
                    TeleportLayout();

                    ImGui.EndTabItem();
                }
                if (ImGui.BeginTabItem("Time"))
                {
                    TimeLayout();

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

    public float rbPosX = 0f;
    public float rbPosY = 0f;
    public float rbPosZ = 0f;
    private void TeleportLayout()
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
                teleporter.Teleport(pos);
            });
        }
    }

    public float currentTime = 0f;
    public float timeToSet = 12f;
    public bool pauseTime = false;
    private readonly Dictionary<string, float> presetTimes = new()
    {
        { "Morning", 6f },
        { "Noon", 12f },
        { "Afternoon", 15f },
        { "Evening", 19f },
        { "Midnight", 0f },
        { "?", 2.784f },
    };
    private void TimeLayout()
    {
        UnityMainThreadDispatcher.Enqueue(() => { currentTime = SkyManager.GetCurrentTime(); });

        timeToSet = currentTime;
        if (ImGui.SliderFloat(" ", ref timeToSet, 0f, 24f, $"Current Time: {TimeFormat.Format(timeToSet)}"))
        {
            UnityMainThreadDispatcher.Enqueue(() => {
                SkyManager.SetFixedTime(timeToSet);
                if (!pauseTime) SkyManager.ClearFixedTime();
            });
        }
        ImGui.SameLine();
        if (ImGui.Checkbox("Pause Time", ref pauseTime))
        {
            if (pauseTime)
            {
                UnityMainThreadDispatcher.Enqueue(() => {
                    SkyManager.SetFixedTime(timeToSet);
                });
            } else
            {
                UnityMainThreadDispatcher.Enqueue(() => {
                    SkyManager.ClearFixedTime();
                });
            }
        }

        MoreImGui.TextCentered("Time Set");

        if (ImGui.BeginTable("PresetTimes", 2))
        {
            foreach (var time in presetTimes)
            {
                ImGui.TableNextColumn();
                if (ImGui.Button(time.Key, new(-float.Epsilon, 0f)))
                {
                    UnityMainThreadDispatcher.Enqueue(() => {
                        timeToSet = time.Value;
                        SkyManager.SetFixedTime(timeToSet);
                        if (!pauseTime) SkyManager.ClearFixedTime();
                    });
                }
            }

            ImGui.EndTable();
        }
    }
}