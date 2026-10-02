using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.RegularExpressions;
using BigDebug.ImGuiHelpers;
using BigDebug.UITabs;
using ImGuiNET;
using UnityEngine;

namespace BigDebug;

public class BigUI : MonoBehaviour
{
    public PlayerCharacter pc;
    public Rigidbody rb;
    public BigTeleporter teleporter;
    public bool GUIOn = false;
    public List<BigUITab> Tabs =
    [
        new TeleportTab(),
        new TimeTab(),
        new LobbyTab(),
        new PropTab(),
    ];
    void Awake()
    {
        pc = GetComponent<PlayerCharacter>();
        rb = pc.rb;
        teleporter = GetComponent<BigTeleporter>();

        foreach (BigUITab tab in Tabs)
        {
            tab.pc = pc;
            tab.rb = rb;
            tab.teleporter = teleporter;

            tab.Awake();
        }
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

    public static bool WantsKeyboard { get; private set; } = false;
    private void OnGUIRender()
    {
        WantsKeyboard = ImGui.GetIO().WantCaptureKeyboard;
        
        if (!GUIOn) return;

        ImGui.SetNextWindowSize(new(465f, 210f), ImGuiCond.FirstUseEver);
        if (ImGui.Begin($"Big Debug {MyPluginInfo.PLUGIN_VERSION}"))
        {
            if (ImGui.BeginTabBar("MainTabBar", ImGuiTabBarFlags.AutoSelectNewTabs | ImGuiTabBarFlags.FittingPolicyScroll))
            {
                foreach (BigUITab tab in Tabs)
                {
                    if (ImGui.BeginTabItem(tab.TabName))
                    {
                        tab.Layout();

                        ImGui.EndTabItem();
                    }
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
}