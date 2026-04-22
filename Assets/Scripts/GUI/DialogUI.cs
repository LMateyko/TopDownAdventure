using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogUI : MonoBehaviour
{
    [SerializeField] private TMP_Text m_dialogText;
    [SerializeField] private Image m_backgroundImage;
    [SerializeField] private Image m_backgroundPanel;

    public void SetDialogText(string text)
    {
        if(string.IsNullOrEmpty(text))
        {
            m_dialogText.text = "";
            m_backgroundImage.gameObject.SetActive(false);
            m_backgroundPanel.gameObject.SetActive(false);
        }
        else
        {
            m_dialogText.text = text;
            m_backgroundImage.gameObject.SetActive(true);
            m_backgroundPanel.gameObject.SetActive(true);
        }
    }

    private void Awake()
    {
        // Default the dialog to off
        SetDialogText("");
    }
}
