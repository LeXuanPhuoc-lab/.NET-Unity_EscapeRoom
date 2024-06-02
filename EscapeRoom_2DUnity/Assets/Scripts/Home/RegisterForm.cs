using TMPro;
using UnityEngine;

namespace Home
{
    public class RegisterForm : MonoBehaviour
    {
        [SerializeField] private TMP_InputField username;
        [SerializeField] private TMP_InputField password;

        public void ResetInputs()
        {
            username.text = string.Empty;
            password.text = string.Empty;
        }

        private void OnEnable()
        {
            ResetInputs();
        }

        public void HandleRegister()
        {
            var usernameValue = username.text;
            var passwordValue = password.text;

            if (string.IsNullOrEmpty(usernameValue) || string.IsNullOrEmpty(passwordValue))
            {
                return;
            }


            HomeManager.Instance.Register(new LoginBody()
            {
                Username = usernameValue,
                Password = passwordValue,
            });
        }
    }
}