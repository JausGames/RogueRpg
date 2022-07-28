using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class HeightMapGenerator {

	public static HeightMap GenerateHeightMap(int width, int height, HeightMapSettings settings, Vector2 sampleCentre) {
		float[,] values = Noise.GenerateNoiseMap (width, height, settings.noiseSettings, sampleCentre);

		AnimationCurve heightCurve_threadsafe = new AnimationCurve (settings.heightCurve.keys);

		float minValue = float.MaxValue;
		float maxValue = float.MinValue;

		
		var falloffMap = FalloffGenerator.GenerateFalloffMap(width);
		for (int i = 0; i < width; i++)
		{
			for (int j = 0; j < height; j++)
			{
				if (settings.useFalloff)
				{
					values[i, j] = Mathf.Clamp01(values[i, j] - falloffMap[i, j]);
				}
				values[i, j] *= heightCurve_threadsafe.Evaluate(values[i, j]) * settings.heightMultiplier;
				if (values[i, j] > maxValue)
				{
					maxValue = values[i, j];
				}
				if (values[i, j] < minValue)
				{
					minValue = values[i, j];
				}
			}
		}
		

		return new HeightMap (values, minValue, maxValue);
	}

	public static HeightMap GenerateHeightMapFromBitmap(int width, int height, HeightMapSettings settings, Vector2 sampleCentre)
	{
		float[,] values = GenerateHeightMapFromBitmap(width, height, settings.bitmap, sampleCentre);

		AnimationCurve heightCurve_threadsafe = new AnimationCurve(settings.heightCurve.keys);

		float minValue = float.MaxValue;
		float maxValue = float.MinValue;


		var falloffMap = FalloffGenerator.GenerateFalloffMap(width);
		for (int i = 0; i < width; i++)
		{
			for (int j = 0; j < height; j++)
			{
				if (settings.useFalloff)
				{
					values[i, j] = Mathf.Clamp01(values[i, j] - falloffMap[i, j]);
				}
				values[i, j] *= heightCurve_threadsafe.Evaluate(values[i, j]) * settings.heightMultiplier;
				if (values[i, j] > maxValue)
				{
					maxValue = values[i, j];
				}
				if (values[i, j] < minValue)
				{
					minValue = values[i, j];
				}
			}
		}


		return new HeightMap(values, minValue, maxValue);
	}
	public static float[,] GenerateHeightMapFromBitmap(int mapWidth, int mapHeight, BitMap bitmap, Vector2 sampleCentre)
	{
		float[,] noiseMap = new float[mapWidth, mapHeight];

		float maxLocalNoiseHeight = float.MinValue;
		float minLocalNoiseHeight = float.MaxValue;

		float halfWidth = mapWidth / 2f;
		float halfHeight = mapHeight / 2f;

		Debug.Log("GenerateNoiseMap, mapWidth = " + mapWidth);
		Debug.Log("GenerateNoiseMap, mapHeight = " + mapHeight);


		for (int y = 0; y < mapHeight; y++)
		{
			for (int x = 0; x < mapWidth; x++)
			{
				//var bitXRatio = mapWidth / bitmap.width;
				//var bitYRatio = mapHeight / bitmap.height;


				//Debug.Log("GenerateNoiseMap, bitXIndex = " + x / bitXRatio + ", bitUIndex = " + y / bitYRatio);

				var pixelcolor = bitmap.pixels[x, y];
				var colorDarkness = (pixelcolor.r + pixelcolor.g + pixelcolor.b) / 3f;
				noiseMap[x, y] = colorDarkness < .8 ? 1 : 0;

				/*if (noiseHeight > maxLocalNoiseHeight)
				{
					maxLocalNoiseHeight = noiseHeight;
				}
				if (noiseHeight < minLocalNoiseHeight)
				{
					minLocalNoiseHeight = noiseHeight;
				}*/

				/*if (settings.normalizeMode == NormalizeMode.Global) {
					float normalizedHeight = (noiseMap [x, y] + 1) / (maxPossibleHeight / 0.9f);
					noiseMap [x, y] = Mathf.Clamp (normalizedHeight, 0, int.MaxValue);
				}*/
			}
		}

		var zoom = 10;
		float[,] zoomedValues = new float[mapWidth * zoom, mapHeight * zoom];

		for (int y = 0; y < mapHeight; y++)
		{
			for (int x = 0; x < mapWidth; x++)
			{
				for (int i = 0; i < zoom; i++)
				{
					for (int j = 0; j < zoom; j++)
					{
						zoomedValues[zoom * x + i, zoom * y + j] = noiseMap[x, y];
					}
				}
			}
		}


				/*if (settings.normalizeMode == NormalizeMode.Local) {
					for (int y = 0; y < mapHeight; y++) {
						for (int x = 0; x < mapWidth; x++) {
							noiseMap [x, y] = Mathf.InverseLerp (minLocalNoiseHeight, maxLocalNoiseHeight, noiseMap [x, y]);
						}
					}*/


				return zoomedValues;
	}
}


public struct HeightMap {
	public readonly float[,] values;
	public readonly float minValue;
	public readonly float maxValue;

	public HeightMap (float[,] values, float minValue, float maxValue)
	{
		this.values = values;
		this.minValue = minValue;
		this.maxValue = maxValue;
	}
}

