using UnityEngine;
using TMPro;

public class KeyOrderUIManager : MonoBehaviour
{
    [SerializeField] private GameObject keyOrderUI; // The UI object to show/hide
    [SerializeField] private TMP_Text keyOrderText; // The TextMeshProUGUI component
    private bool isPlayerNear = false;

    void Start()
    {
        keyOrderUI.SetActive(false); // Initially hide the UI
    }

    void Update()
    {
        if (isPlayerNear)
        {
            if (StaticData.KeyOder.Equals("Ascending"))
            {
                keyOrderText.text = "a<b<c<d";
            }
            else
            {
                keyOrderText.text = "a>b>c>d";
            }

            keyOrderUI.SetActive(true); // Show the UI
        }

        if (Input.GetKeyDown(KeyCode.Escape) && keyOrderUI.activeSelf)
        {
            keyOrderUI.SetActive(false); // Hide the UI when ESC is pressed
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = false;
            keyOrderUI.SetActive(false); // Hide the UI when player leaves the area
        }
    }
}
