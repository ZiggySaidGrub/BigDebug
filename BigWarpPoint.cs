using System;
using System.Text.Json.Serialization;
using UnityEngine;

namespace BigDebug;

public class BigWarpPoint
{
    [JsonInclude]
    public string Name { get; set; }
    [JsonInclude]
    public string Description { get; set; }
    [JsonInclude]
    public float[] Position { get; set; }
}