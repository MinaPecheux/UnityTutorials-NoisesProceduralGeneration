using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class NoiseManager : MonoBehaviour
{
    public RawImage noiseTextureImage;
    public Terrain noiseTerrain;
    
    public int width = 256;
    public int height = 256;

    private string[] _noiseTypes;
    private int _currentNoiseIndex, _lastNoiseIndex;
    private Noise _noise;
    private float _scale, _lastScale;
    private int _seed, _lastSeed;
    private float _amplitude, _lastAmplitude;
    private float _cameraRotation, _lastCameraRotation;

    private void Awake()
    {
        _noiseTypes = Noise.NOISE_TYPES.Keys.ToArray();

        _seed = 0;
        _scale = 0.1f;
        _amplitude = 0.4f;
        _cameraRotation = 0f;
        _currentNoiseIndex = 0;
        _UpdateNoise();
    }

    private void OnGUI()
    {
        GUI.BeginGroup(new Rect(20f, 20f, 220f, 300f));

        _currentNoiseIndex = GUI.SelectionGrid(
            new Rect(0f, 0f, 100f, 25f * _noiseTypes.Length),
            _currentNoiseIndex,
            _noiseTypes,
            1
        );

        GUI.color = Color.black;
        string _scaleStr = _scale.ToString("0.###");
        GUI.Label(new Rect(110f, 0f, 110f, 20f), $"Scale = {_scaleStr}");
        _scale = GUI.HorizontalSlider(new Rect(110f, 20f, 110f, 20f), _scale, 0.01f, 0.2f);
        GUI.Label(new Rect(110f, 40f, 110f, 20f), $"Seed = {_seed}");
        _seed = (int) GUI.HorizontalSlider(new Rect(110f, 60f, 110f, 20f), _seed, 0, 50);
        string _amplitudeStr = _amplitude.ToString("0.###");
        GUI.Label(new Rect(110f, 80f, 110f, 20f), $"Amplitude = {_amplitudeStr}");
        _amplitude = GUI.HorizontalSlider(new Rect(110f, 100f, 110f, 20f), _amplitude, 0, 1);
        string _camRotStr = _cameraRotation.ToString("0.#");
        GUI.Label(new Rect(110f, 120f, 110f, 20f), $"Cam Rot = {_camRotStr}");
        _cameraRotation = GUI.HorizontalSlider(new Rect(110f, 140f, 110f, 20f), _cameraRotation, 0, 359);
        GUI.EndGroup();

        if (GUI.changed)
            _UpdateNoise();
    }

    private void _UpdateNoise()
    {
        if (_cameraRotation != _lastCameraRotation)
            _SetCameraRotation();

        if (
            _currentNoiseIndex == _lastNoiseIndex &&
            _scale == _lastScale &&
            _seed == _lastSeed &&
            _amplitude == _lastAmplitude
        )
            return;

        _RecomputeNoise();

        _lastNoiseIndex = _currentNoiseIndex;
        _lastScale = _scale;
        _lastSeed = _seed;
    }

    private void _SetCameraRotation()
    {
        transform.eulerAngles = new Vector3(0f, _cameraRotation, 0f);
        _lastCameraRotation = _cameraRotation;
    }

    private void _RecomputeNoise()
    {
        System.Type NoiseClass = Noise.NOISE_TYPES[_noiseTypes[_currentNoiseIndex]];
        _noise = (Noise) System.Activator.CreateInstance(NoiseClass);

        _noise.Seed = _seed;

        float[,] noise = new float[width, height];
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                noise[y, x] = _noise.GetNoiseMap(x, y, _scale);
            }
        }

        _SetNoiseTexture(noise);
    }

    private void _SetNoiseTexture(float[,] noise)
    {
        Color[] pixels = new Color[width * height];
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                pixels[x + width * y] = Color.Lerp(Color.black, Color.white, noise[y, x]);
            }
        }
        Texture2D texture = new Texture2D(width, height);
        texture.SetPixels(pixels);
        texture.Apply();
        noiseTextureImage.texture = texture; for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                noise[y, x] *= _amplitude;
            }
        }
        noiseTerrain.terrainData.SetHeights(0, 0, noise);
    }
}
