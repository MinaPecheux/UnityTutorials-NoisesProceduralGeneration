/* Adapted from:
 * https://github.com/Scrawk/Procedural-Noise/blob/master/Assets/ProceduralNoise/Noise/WorleyNoise.cs */
using UnityEngine;

public class WorleyNoise : Noise
{
    private static readonly float[] OFFSET_F = new float[] { -0.5f, 0.5f, 1.5f };
    private const float K = 0.142857142857f; // 1/7
    private const float Ko = 0.428571428571f; // 3/7
    private float _jitter = 1.0f; // Less gives more regular pattern

    private const int PERM_MAX = 255;
    private const int PERM_SIZE = 256;
    private const int PERM_WRAP = PERM_SIZE - 1;
    private static int[] _Perm;

    public override int Seed
    {
        get => _seed;
        set
        {
            _Perm = new int[PERM_SIZE];
            System.Random random = new System.Random(value);
            for (int i = 0; i < PERM_SIZE; i++)
            {
                _Perm[i] = random.Next();
            }

            _seed = value;
        }
    }

    public override float GetNoiseMap(float x, float y, float scale = 1f)
    {
        x *= scale;
        y *= scale;

        int Pi0 = (int)Mathf.Floor(x);
        int Pi1 = (int)Mathf.Floor(y);

        float Pf0 = _Frac(x);
        float Pf1 = _Frac(y);

        Vector3 pX = new Vector3();
        pX[0] = _PermAt(Pi0 - 1);
        pX[1] = _PermAt(Pi0);
        pX[2] = _PermAt(Pi0 + 1);

        float d0, d1, d2;
        float F0 = float.PositiveInfinity;
        float F1 = float.PositiveInfinity;
        float F2 = float.PositiveInfinity;

        int px, py, pz;
        float oxx, oxy, oxz;
        float oyx, oyy, oyz;

        for (int i = 0; i < 3; i++)
        {
            px = _PermAt((int)pX[i], Pi1 - 1);
            py = _PermAt((int)pX[i], Pi1);
            pz = _PermAt((int)pX[i], Pi1 + 1);

            oxx = _Frac(px * K) - Ko;
            oxy = _Frac(py * K) - Ko;
            oxz = _Frac(pz * K) - Ko;

            oyx = _Mod(Mathf.Floor(px * K), 7.0f) * K - Ko;
            oyy = _Mod(Mathf.Floor(py * K), 7.0f) * K - Ko;
            oyz = _Mod(Mathf.Floor(pz * K), 7.0f) * K - Ko;

            d0 = _Distance2(Pf0, Pf1, OFFSET_F[i] + _jitter * oxx, -0.5f + _jitter * oyx);
            d1 = _Distance2(Pf0, Pf1, OFFSET_F[i] + _jitter * oxy, 0.5f + _jitter * oyy);
            d2 = _Distance2(Pf0, Pf1, OFFSET_F[i] + _jitter * oxz, 1.5f + _jitter * oyz);

            if (d0 < F0) { F2 = F1; F1 = F0; F0 = d0; }
            else if (d0 < F1) { F2 = F1; F1 = d0; }
            else if (d0 < F2) { F2 = d0; }

            if (d1 < F0) { F2 = F1; F1 = F0; F0 = d1; }
            else if (d1 < F1) { F2 = F1; F1 = d1; }
            else if (d1 < F2) { F2 = d1; }

            if (d2 < F0) { F2 = F1; F1 = F0; F0 = d2; }
            else if (d2 < F1) { F2 = F1; F1 = d2; }
            else if (d2 < F2) { F2 = d2; }

        }

        return Combine(F0, F1, F2);
    }

    private float _Mod(float x, float y)
    {
        return x - y * Mathf.Floor(x / y);
    }

    private float _Frac(float v)
    {
        return v - Mathf.Floor(v);
    }

    private float _Distance2(float p1x, float p1y, float p2x, float p2y)
    {
        return (p1x - p2x) * (p1x - p2x) + (p1y - p2y) * (p1y - p2y);
    }

    private float Combine(float f0, float f1, float f2)
    {
        return f1 - f0;
    }

    private int _PermAt(int i)
    {
        return _Perm[i & PERM_WRAP] & PERM_MAX;
    }

    private int _PermAt(int i, int j)
    {
        return _Perm[(j + _Perm[i & PERM_WRAP]) & PERM_WRAP] & PERM_MAX;
    }
}
