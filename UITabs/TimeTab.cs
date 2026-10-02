using System.Collections.Generic;
using BigDebug.ImGuiHelpers;
using ImGuiNET;

namespace BigDebug.UITabs;

public class TimeTab : BigUITab
{
    public TimeTab() { TabName = "Time"; }

    private float currentTime = 0f;
    private float timeToSet = 12f;
    private bool pauseTime = false;
    private readonly Dictionary<string, float> presetTimes = new()
    {
        { "Morning", 6f },
        { "Noon", 12f },
        { "Afternoon", 15f },
        { "Evening", 19f },
        { "Midnight", 0f },
        { "?", 2.784f },
    };
    public override void Layout()
    {
        UnityMainThreadDispatcher.Enqueue(() => { currentTime = SkyManager.GetCurrentTime(); });

        timeToSet = currentTime;
        if (ImGui.SliderFloat(" ", ref timeToSet, 0f, 24f, $"Current Time: {TimeFormat.Format(timeToSet)}", ImGuiSliderFlags.NoInput))
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