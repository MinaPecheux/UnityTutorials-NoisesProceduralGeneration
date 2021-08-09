using UnityEngine;

public class PerlinNoise : Noise
{
    private const float C = 1000;

    public override float GetNoiseMap(float x, float y, float scale = 1f)
    {
        x = (x + _seed * C) * scale;
        y = (y + _seed * C) * scale;
        return Mathf.Clamp01(Mathf.PerlinNoise(x, y));
    }
}
