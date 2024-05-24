using UnityEngine;

namespace Inputs
{
    public class UserInput : MonoBehaviour
    {
        public static UserInput Instance;

        public Controls Controls;

        [HideInInspector] public Vector2 moveInput;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }

            Controls = new Controls();
            Controls.Movement.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        }

        private void OnEnable()
        {
            Controls.Enable();
        }

        private void OnDisable()
        {
            Controls.Disable();
        }
    }

}
