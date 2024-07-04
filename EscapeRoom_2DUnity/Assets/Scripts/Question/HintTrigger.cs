using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class HintTrigger : MonoBehaviour
{
    [SerializeField] private GameObject hintUI;
    [SerializeField] private TMP_Text hintText;
    [SerializeField] private Image hintImage;


    void Start()
    {
        hintUI.SetActive(false);
    }

    void Update()
    {
        if (hintUI.activeSelf && Input.GetKeyDown(KeyCode.Escape))
        {
            HideHint();
        }
    }

    public void ShowHint(int keyDigit)
    {
        hintText.text = $"Key Digit mà bạn nhận được là {keyDigit}";
        hintUI.SetActive(true);
        Time.timeScale = 0;
    }

    public void HideHint()
    {
        hintUI.SetActive(false);
        Time.timeScale = 1;
    }
}