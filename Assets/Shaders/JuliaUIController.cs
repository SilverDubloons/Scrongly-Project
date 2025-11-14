using UnityEngine;
using UnityEngine.UI;

public class JuliaUIController : MonoBehaviour
{
    public RawImage juliaImage;
    public float timeScaleFactor = 0.3f;
    public bool useMouseInput;

    void Update()
    {
        float time = Time.time * timeScaleFactor;
        float sinwave = Mathf.Sin(Time.time * 4) / 45;
        Vector2 juliaConstant = new Vector2
		(
            Mathf.PerlinNoise(time, sinwave) * 2f - 1f,
            Mathf.PerlinNoise(sinwave, time) * 2f - 1f
        );

        if(useMouseInput && LocalInterface.instance != null)
        {
            Vector2 mousePos = LocalInterface.instance.GetNormalizedMousePosition() / 4f;
            juliaConstant += mousePos;
        }

        juliaImage.material.SetVector("_JuliaConstant", juliaConstant);
    }
}