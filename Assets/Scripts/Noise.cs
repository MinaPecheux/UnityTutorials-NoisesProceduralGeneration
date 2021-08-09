using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface INoise
{
    public int Seed { get; set; }
}

public abstract class Noise : INoise
{
    public static Dictionary<string, Type> NOISE_TYPES = new Dictionary<string, Type>()
    {
        { "Perlin", typeof(PerlinNoise) },
        { "White", typeof(WhiteNoise) },
        { "Simplex", typeof(SimplexNoise) },
        { "Value", typeof(ValueNoise) },
        { "Worley", typeof(WorleyNoise) },
    };

    protected static int _seed;
    public virtual int Seed {
        get => _seed;
        set => _seed = value;
    }

    public abstract float GetNoiseMap(float x, float y, float scale = 1f);
}
