using System;
using System.IO;
using System.Text.Json;
using System.Text.RegularExpressions;
using BigDebug.ImGuiHelpers;
using ImGuiNET;
using UnityEngine;

namespace BigDebug.UITabs;

public class TeleportTab : BigUITab
{
    public TeleportTab() { TabName = "Teleport"; }
    
    private float rbPosX = 0f;
    private float rbPosY = 0f;
    private float rbPosZ = 0f;
    private string warpPointName = "";
    private string warpPointDescription = "";
    private FileTreeNode warpTree;
    private bool loadedTree = false;
    public override void Awake()
    {
        rbPosX = rb.position.x; rbPosY = rb.position.y; rbPosZ = rb.position.z;
        ReloadWarps();
    }
    public override void Layout()
    {
        UnityMainThreadDispatcher.Enqueue(() => { rbPosX = rb.position.x; rbPosY = rb.position.y; rbPosZ = rb.position.z; });
        ImGui.Text($"Coords: {rbPosX}, {rbPosY}, {rbPosZ}");

        if (ImGui.Button("Copy Coords to Clipboard"))
        {
            ImGui.SetClipboardText($"{rbPosX}, {rbPosY}, {rbPosZ}");
        }
        ImGui.SameLine();
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

        ImGui.SeparatorText("Save Warp Point");

        ImGui.InputText("Name", ref warpPointName, 24);
        ImGui.InputTextMultiline("Description", ref warpPointDescription, 240, new(0f, 50f));
        if (ImGui.Button("Save Location as Warp Point"))
        {
            BigWarpPoint warpPoint = new BigWarpPoint
            {
                Name = warpPointName,
                Description = warpPointDescription,
                Position = [rbPosX, rbPosY, rbPosZ]
            };
            string jsonString = JsonSerializer.Serialize(warpPoint);
            BigDebug.Log.LogInfo(jsonString);

            string invalidChars = Regex.Escape( new string(Path.GetInvalidFileNameChars()) );
            string invalidRegStr = string.Format( @"([{0}]*\.+$)|([{0}]+)", invalidChars );
            string fileName = $"{Regex.Replace(warpPointName, invalidRegStr, "-")}.walk";

            string filePath = Path.Join(BigDebug.BigConfig.WarpsFolder, fileName);
            File.WriteAllText(filePath, jsonString);

            loadedTree = false;
        }

        ImGui.SeparatorText("Load Warp Point");

        if (ImGui.Button("Reload Warps")) loadedTree = false;
        
        if (!loadedTree) ReloadWarps();
        ImGuiTableFlags table_flags = ImGuiTableFlags.BordersV | ImGuiTableFlags.BordersOuterH | ImGuiTableFlags.Resizable | ImGuiTableFlags.RowBg | ImGuiTableFlags.NoBordersInBody;
        if (ImGui.BeginTable("WarpsTable", 1, table_flags))
        {
            warpTree.DisplayNode(teleporter.Teleport);
            ImGui.EndTable();
        }
    }
    private void ReloadWarps()
    {
        warpTree = FileTreeNode.BuildFromFileTree(BigDebug.BigConfig.WarpsFolder, null, "Warps");
        loadedTree = true;
        BigDebug.Log.LogInfo(warpTree);
    }
}