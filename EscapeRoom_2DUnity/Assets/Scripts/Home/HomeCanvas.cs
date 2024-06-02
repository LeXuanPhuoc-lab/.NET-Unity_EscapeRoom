using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Home
{
    public class HomeCanvas : MonoBehaviour
    {
        [SerializeField] private TMP_Text errorMessage;
        [SerializeField] private List<GameObject> gameObjects;

        private void Start()
        {
            errorMessage.text = "";
            ShowObject("LoginForm");
        }


        public void ShowError(string message)
        {
            Debug.Log(message);
            errorMessage.text = message;
        }

        public void SetDefaultErrorMessage()
        {
            errorMessage.text = string.Empty;
        }
        
        public void ShowObject(string objectName)
        {
            errorMessage.text = "";
            //Observer pattern
            foreach (var go in gameObjects)
            {
                go.SetActive(objectName == go.name);
            }
        }
    }
}