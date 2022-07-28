using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

[CreateAssetMenu()]
public class InstanciableData : UpdatableData {

	public Layer[] layers;

	float savedMinHeight;
	float savedMaxHeight;



	[System.Serializable]
	public class Layer {
		public GameObject visual;
		[Range(0,1)]
		public float startHeight;
		[Range(0, 1)]
		public float endHeight;
		[Range(0,1)]
		public float blendStrength;
		public NoiseSettings noiseSettings;
		public AnimationCurve densityCurve;
	}
		
	 
}
