using UnityEngine;
using System.Collections;
using System;

[CreateAssetMenu()]
public class HeightMapSettings : UpdatableData {

	public NoiseSettings noiseSettings;
	public bool useFalloff;
	public float heightMultiplier;
	public AnimationCurve heightCurve;
	public BitMap bitmap;

	public float minHeight {
		get {
			return heightMultiplier * heightCurve.Evaluate (0);
		}
	}

	public float maxHeight {
		get {
			return heightMultiplier * heightCurve.Evaluate (1);
		}
	}
	public float EvaluateHeight(float heightValue)
    {
		return heightMultiplier * heightCurve.Evaluate(heightValue);
    }

	#if UNITY_EDITOR

	protected override void OnValidate() {
		noiseSettings.ValidateValues ();
		base.OnValidate ();
	}
	#endif

}
[Serializable]
public class BitMap
{
	public Texture2D bitmap;
    public int width = 50;
	public int height = 50;
	public Color[,] pixels;

    public BitMap(Texture2D bitmap)
    {
        this.bitmap = bitmap;
		InitBitMap();

    }
	public void InitBitMap()
	{
		this.width = bitmap.width;
		this.height = bitmap.height;

		pixels = new Color[width, height];

		for (int x = 0; x < width; x++)
			for (int y = 0; y < height; y++)
			{
				pixels[x, y] = bitmap.GetPixel(x, y);
			}
	}

}
