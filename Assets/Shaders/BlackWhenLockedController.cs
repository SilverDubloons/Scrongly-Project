using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class BlackWhenLockedController : MonoBehaviour
{
    private Material _materialInstance;
    private Image _image;
    private bool _initialized = false;

    // Initialize manually since Awake might happen before sprite assignment
    public void Initialize()
    {
        if (_initialized) return;
        
        _image = GetComponent<Image>();
        
        // Only proceed if we have a sprite
        if (_image.sprite == null)
        {
            Debug.LogWarning("Image sprite not assigned yet - will initialize when sprite is set");
            return;
        }
        
        Shader shader = Shader.Find("UI/BlackWhenLocked");
        if (shader == null)
        {
            Debug.LogError("Shader not found! Make sure it's in a Resources folder");
            return;
        }
        
        _materialInstance = new Material(shader);
        _materialInstance.hideFlags = HideFlags.DontSave;
        _materialInstance.SetTexture("_MainTex", _image.sprite.texture);
        _materialInstance.SetColor("_Color", _image.color);
        _image.material = _materialInstance;
        
        _initialized = true;
    }

    public void SetLocked(bool locked)
    {
        // Lazy initialization if not done yet
        if (!_initialized) Initialize();
        
        if (_materialInstance != null)
        {
            _materialInstance.SetFloat("_Locked", locked ? 1 : 0);
        }
    }

    void OnDestroy()
    {
        if (_materialInstance != null)
        {
            DestroyImmediate(_materialInstance);
        }
    }
}