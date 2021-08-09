using UnityEngine;

public class WhiteNoise : Noise
{
    public override float GetNoiseMap(float x, float y, float scale = 1f)
    {
        return Random.Range(0f, 1f);
    }
}
