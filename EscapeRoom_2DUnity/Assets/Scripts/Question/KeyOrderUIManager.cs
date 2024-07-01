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
                keyOrderText.text = "Trên một con đường quanh co, có những bậc thang dẫn lối. Mỗi bước bạn đi, số lượng bậc thang lại thêm một. Bạn đang bước trên con đường nào?";
            }
            else
            {
                keyOrderText.text = "Một dòng sông chảy êm đềm, nước từ thượng nguồn trôi xuống hạ lưu. Lượng nước giảm dần mỗi khi qua một trạm bơm. Bạn đang chứng kiến điều gì?";
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
