using UnityEngine;
using UnityEngine.UI;

public class MandelbrotCPU : MonoBehaviour
{
    public RawImage displayImage;
    public const int width = 640;
    public const int height = 360;
    public int maxIterations = 100;
    public float zoom = 1.0f;
    public float timeScaleFactor = 0.3f;
    public Vector2 panOffset;
	public float minDistSqFactor = 10f;
	public float minDistSqConstant = 0.1f;
	public bool useMouseInput;
	public Vector3 outsideConstant;
	public Vector3 outsideTrapGlowFactor;
	public Vector3 outsideDistanceFactor;
	public Vector3 outsideDZEffectFactor;
	public Vector3 outsodeVeinBoostPower;
	public Vector3 outsideTPower;
	public bool outsideTPowerInUseX;
	public bool outsideTPowerInUseY;
	public bool outsideTPowerInUseZ;
	
	public Vector3 insideConstant;
	public Vector3 insideTrapValueFactor;
	public Vector3 insideDZEffectFactor;
	public Vector3 insideDistanceFactor;

	public Vector2 juliaConstant;

    private Texture2D texture;

    void Start()
    {
        texture = new Texture2D(width, height);
        displayImage.texture = texture;
    }

    void Update()
    {
		float time = Time.time * timeScaleFactor;
		float sinwave = Mathf.Sin(Time.time * 4) / 45;
		juliaConstant.x = Mathf.PerlinNoise(time, sinwave) * 2f - 1f; // -1 to 1
		juliaConstant.y = Mathf.PerlinNoise(sinwave, time) * 2f - 1f;
		if(useMouseInput)
		{
			Vector2 normalizedMousePos = LocalInterface.instance.GetNormalizedMousePosition(); // -1 to 1
			juliaConstant += normalizedMousePos / 4;
		}
		GenerateJulia();
    }

	JuliaData CalculateJulia(Vector2 z, Vector2 c, int maxIterations)
	{
		int iterations = 0;
		float minDistSq = float.MaxValue;
		Vector2 dz = Vector2.one;
		while (iterations < maxIterations)
		{
			float xSquared = z.x * z.x;
			float ySquared = z.y * z.y;
			float twoXY = 2 * z.x * z.y;
			float distSq = z.x*z.x + z.y*z.y;
			dz = new Vector2(2 * (z.x * dz.x - z.y * dz.y), 2 * (z.x * dz.y + z.y * dz.x));
			if (distSq < minDistSq) 
			{
				minDistSq = distSq;
			}
			z.x = xSquared - ySquared + c.x;
			z.y = twoXY + c.y;

			if (xSquared + ySquared > 4) break;
			iterations++;
		}
		return new JuliaData(iterations, minDistSq, dz);;
	}
	
	public struct JuliaData
	{
		public int iterations;
		public float minDistSq;
		public Vector2 dz;
		public JuliaData(int iterations, float minDistSq, Vector2 dz)
		{
			this.iterations = iterations;
			this.minDistSq = minDistSq;
			this.dz = dz;
		}
	}
	
	void GenerateJulia()
	{
		for (int y = 0; y < height; y++)
		{
			for (int x = 0; x < width; x++)
			{
				float real = (x - width / 2f) / (width / 4f * zoom) + panOffset.x;
				float imaginary = (y - height / 2f) / (height / 4f * zoom) + panOffset.y;

				Vector2 z = new Vector2(real, imaginary);
				JuliaData juliaData = CalculateJulia(z, juliaConstant, maxIterations);
				texture.SetPixel(x, y, GetColor(juliaData.iterations, z, juliaConstant, juliaData.minDistSq, juliaData.dz));
			}
		}
		texture.Apply();
	}
	
	Color GetColor(int iterations, Vector2 z, Vector2 c, float minDistSq, Vector2 dz)
	{
		if (iterations == maxIterations)
		{
			float veinBoost = 1f + (1f - juliaConstant.magnitude); // 1x to 2x multiplier
			float trapValue = Mathf.Clamp01(1f / (minDistSq * 15f * veinBoost + 0.1f));
			float dzEffect = Mathf.Clamp01(dz.magnitude);
			float distance = z.magnitude;
			return new Color
			(
				insideConstant.x + trapValue * insideTrapValueFactor.x + dzEffect * insideDZEffectFactor.x + distance * insideDistanceFactor.x,
				insideConstant.y + trapValue * insideTrapValueFactor.y + dzEffect * insideDZEffectFactor.y + distance * insideDistanceFactor.y,
				insideConstant.z + trapValue * insideTrapValueFactor.z + dzEffect * insideDZEffectFactor.z + distance * insideDistanceFactor.z
			);
		}
		else
		{
			float t = (float)iterations / maxIterations;
			float trapGlow = 1f / (minDistSq * minDistSqFactor + minDistSqConstant); // Inverse of distance
			float distance = z.magnitude;
			float dzEffect = Mathf.Clamp01(dz.magnitude);
			float veinBoost = 1f + (1f - juliaConstant.magnitude);
			return new Color
			(
				outsideConstant.x + trapGlow * outsideTrapGlowFactor.x + distance * outsideDistanceFactor.x + dzEffect * outsideDZEffectFactor.x + (outsideTPowerInUseX ? Mathf.Pow(t, outsideTPower.x) : 0) + veinBoost * outsodeVeinBoostPower.x,
				outsideConstant.y + trapGlow * outsideTrapGlowFactor.y + distance * outsideDistanceFactor.y + dzEffect * outsideDZEffectFactor.y + (outsideTPowerInUseY ? Mathf.Pow(t, outsideTPower.y) : 0) + veinBoost * outsodeVeinBoostPower.y,
				outsideConstant.z + trapGlow * outsideTrapGlowFactor.z + distance * outsideDistanceFactor.z + dzEffect * outsideDZEffectFactor.z + (outsideTPowerInUseZ ? Mathf.Pow(t, outsideTPower.z) : 0) + veinBoost * outsodeVeinBoostPower.z
			);
		}
	}
}