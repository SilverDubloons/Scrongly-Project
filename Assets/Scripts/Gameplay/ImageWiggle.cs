using System.Collections;
using UnityEngine;

public class ImageWiggle : MonoBehaviour
{
    [SerializeField] private RectTransform rt;
    [SerializeField] private Vector3 originRotation;
    [SerializeField] private Vector3 rightRotation;
    [SerializeField] private Vector3 leftRotation;
    [SerializeField] private ButtonPlus lockButton;
    private float timeOfLastWiggle;
    [SerializeField] private float timeBetweenWiggles;
    private IEnumerator wiggleCoroutine;
    private bool wiggling = false;
    void Update()
    {
        if (!Preferences.instance.animateLockButton)
        {
            return;
        }
        if (!lockButton.GetButtonEnabled() || !lockButton.gameObject.activeInHierarchy)
        {
            if (wiggling)
            {
                StopCoroutine(wiggleCoroutine);
                rt.localRotation = Quaternion.identity;
                wiggling = false;
            }
            timeOfLastWiggle = 0;
            return;
        }
        if (Time.time - timeOfLastWiggle > timeBetweenWiggles)
        {
            timeOfLastWiggle = Time.time;
            if (wiggling)
            {
                StopCoroutine(wiggleCoroutine);
            }
            wiggleCoroutine = Wiggle();
            StartCoroutine(wiggleCoroutine);
        }
    }
    private IEnumerator Wiggle()
    {
        wiggling = true;
        float t = 0;
        float wiggleTime = 0.1f;
        while (t < wiggleTime)
        {
            t = Mathf.Clamp(t + Time.deltaTime, 0, wiggleTime);
            float normalizedTime = t / wiggleTime;
            rt.localEulerAngles = Vector3.Lerp(originRotation, rightRotation, normalizedTime);
            yield return null;
        }
        for (int i = 0; i < 2; i++)
        {
            t = 0;
            while (t < wiggleTime * 2f)
            {
                t = Mathf.Clamp(t + Time.deltaTime, 0, wiggleTime * 2);
                float normalizedTime = t / (wiggleTime * 2);
                rt.localEulerAngles = Vector3.Lerp(rightRotation, leftRotation, normalizedTime);
                yield return null;
            }
            t = 0;
            while (t < wiggleTime * 2f)
            {
                t = Mathf.Clamp(t + Time.deltaTime, 0, wiggleTime * 2);
                float normalizedTime = t / (wiggleTime * 2);
                rt.localEulerAngles = Vector3.Lerp(leftRotation, rightRotation, normalizedTime);
                yield return null;
            }
        }
        t = 0;
        while (t < wiggleTime)
        {
            t = Mathf.Clamp(t + Time.deltaTime, 0, wiggleTime);
            float normalizedTime = t / wiggleTime;
            rt.localEulerAngles = Vector3.Lerp(rightRotation, originRotation, normalizedTime);
            yield return null;
        }
        wiggling = false;
    }
}
