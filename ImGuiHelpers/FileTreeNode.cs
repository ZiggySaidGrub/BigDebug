using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using ImGuiNET;
using UnityEngine.UIElements;

namespace BigDebug.ImGuiHelpers;

public class FileTreeNode
{
    private static ImGuiTreeNodeFlags tree_node_flags_base = ImGuiTreeNodeFlags.None;
    
    public string Name;
    public string Path;
    public bool IsFolder;
    public BigWarpPoint WarpPoint = null;
    public List<FileTreeNode> Children = [];
    public void DisplayNode(Action<BigWarpPoint> onLeafClicked)
    {
        ImGui.TableNextColumn();

        var nodeFlags = tree_node_flags_base;
        if (IsFolder)
        {
            bool open = ImGui.TreeNodeEx(Name, nodeFlags);
            if (open)
            {
                foreach (FileTreeNode child in Children)
                {
                    child.DisplayNode(onLeafClicked);
                }
                ImGui.TreePop();
            }
        } else
        {
            ImGui.TreeNodeEx(Name, nodeFlags | ImGuiTreeNodeFlags.Leaf | ImGuiTreeNodeFlags.Bullet | ImGuiTreeNodeFlags.NoTreePushOnOpen);
            if (ImGui.IsItemClicked() && WarpPoint != null) onLeafClicked.Invoke(WarpPoint);
        }
    }
    public override string ToString()
    {

        if (IsFolder) return $"{Name}: [{string.Join(", ", Children)}]";
        return Name;
    }

    public static FileTreeNode BuildFromFileTree(string currentPath, FileTreeNode currentNode = null, string name = null)
    {
        DirectoryInfo dirInfo = new(currentPath);
        FileTreeNode node = currentNode;
        if (currentNode == null)
        {
            string nodeName = name;
            if (name == null) nodeName = dirInfo.Name;
            node = new FileTreeNode()
            {
                Name = nodeName,
                Path = dirInfo.FullName,
                IsFolder = true
            };
        }

        foreach (var dir in dirInfo.EnumerateDirectories())
        {
            FileTreeNode treeNode = new FileTreeNode()
            {
                Name = dir.Name,
                Path = dir.FullName,
                IsFolder = true
            };

            node.Children.Add(treeNode);

            BuildFromFileTree(dir.FullName, treeNode);
        }
        foreach (var file in dirInfo.EnumerateFiles())
        {
            if (file.Extension != ".walk") continue;

            string nodePath = file.FullName;
            string jsonString = File.ReadAllText(nodePath);
            BigWarpPoint warpPoint = JsonSerializer.Deserialize<BigWarpPoint>(jsonString);

            FileTreeNode treeNode = new()
            {
                Name = warpPoint.Name,
                Path = nodePath,
                WarpPoint = warpPoint,
                IsFolder = false
            };

            node.Children.Add(treeNode);
        }

        return node;
    }
}