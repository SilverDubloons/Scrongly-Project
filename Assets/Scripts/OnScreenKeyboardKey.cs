using UnityEngine;

public class OnScreenKeyboardKey : MonoBehaviour
{
	public ButtonPlus buttonPlus;
	
	public char baseChar;	// lowercase, or non-shift
	public char altChar;	// uppercase or shift, null for keys like tab, backspace
	
	public bool affectedByCapsLock;	// non letters do not get affected by caps lock
	
    public void ButtonPressed()
	{
		OnScreenKeyboard.instance.KeyboardButtonPressed(baseChar, altChar, affectedByCapsLock);
	}
}
