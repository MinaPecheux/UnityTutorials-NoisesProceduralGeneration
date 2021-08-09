/* Adapted from:
 * https://www.scratchapixel.com/lessons/procedural-generation-virtual-worlds/procedural-patterns-noise-part-1/creating-simple-2D-noise */
using UnityEngine;

public class ValueNoise : Noise
{
    private const int K_MAX_TABLE_SIZE = 256;
    private const int K_MAX_TABLE_SIZE_MASK = K_MAX_TABLE_SIZE - 1;

    private float[] r = new float[K_MAX_TABLE_SIZE * K_MAX_TABLE_SIZE];

    public override int Seed
    {
        get => _seed;
        set
        {
            Random.InitState(value);
            for (int k = 0; k < K_MAX_TABLE_SIZE * K_MAX_TABLE_SIZE; k++)
                r[k] = Random.Range(0f, 1f);

            _seed = value;
        }
    }

    public override float GetNoiseMap(float x, float y, float scale = 1f)
    {
        x *= scale;
        y *= scale;

        int xi = Mathf.FloorToInt(x);
        int yi = Mathf.FloorToInt(y);

        float tx = x - xi;
        float ty = y - yi;

        int rx0 = xi & K_MAX_TABLE_SIZE_MASK;
        int rx1 = (rx0 + 1) & K_MAX_TABLE_SIZE_MASK;
        int ry0 = yi & K_MAX_TABLE_SIZE_MASK;
        int ry1 = (ry0 + 1) & K_MAX_TABLE_SIZE_MASK;

        // random values at the corners of the cell using permutation table
        float c00 = r[ry0 * K_MAX_TABLE_SIZE_MASK + rx0];
        float c10 = r[ry0 * K_MAX_TABLE_SIZE_MASK + rx1];
        float c01 = r[ry1 * K_MAX_TABLE_SIZE_MASK + rx0];
        float c11 = r[ry1 * K_MAX_TABLE_SIZE_MASK + rx1];

        // remapping of tx and ty using the Smoothstep function 
        float sx = _Smoothstep(tx);
        float sy = _Smoothstep(ty);

        // linearly interpolate values along the x axis
        float nx0 = Mathf.Lerp(c00, c10, sx);
        float nx1 = Mathf.Lerp(c01, c11, sx);

        // linearly interpolate the nx0/nx1 along they y axis
        return Mathf.Lerp(nx0, nx1, sy);
    }

    private float _Smoothstep (float t) {
        return t * t * (3 - 2 * t);
    }
}
