using TMPro;
using UnityEngine;

namespace Home
{
    public class LoginForm : MonoBehaviour
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


        public void HandleLogin()
        {
            var usernameValue = username.text;
            var passwordValue = password.text;

            if (string.IsNullOrEmpty(usernameValue) || string.IsNullOrEmpty(passwordValue))
            {
                return;
            }


            HomeManager.Instance.Login(new LoginBody()
            {
                Username = usernameValue,
                Password = passwordValue,
            });
        }
    }
}
