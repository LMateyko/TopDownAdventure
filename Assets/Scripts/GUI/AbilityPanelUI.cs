using UnityEngine;
using UnityEngine.UI;

public class AbilityPanelUI : MonoBehaviour
{
    [SerializeField] Image m_controllerInputDisplay;
    [SerializeField] Image m_keyboardInputDisplay;

    public void SetControlDisplay(bool isKeyboard)
    {
        m_controllerInputDisplay.enabled = !isKeyboard;
        m_keyboardInputDisplay.enabled = isKeyboard;
    }
}
