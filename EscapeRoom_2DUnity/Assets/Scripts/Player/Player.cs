using Inputs;
using UnityEngine;

namespace Player
{
    public class Player : MonoBehaviour
    {
        private Rigidbody2D _rb;
        private bool _isFacingRight;
        private Animator _animator;
        private static readonly int IsWalking = Animator.StringToHash("isWalking");

        [SerializeField] private float moveSpeed = 50f;

        private void Start()
        {
            _rb = GetComponent<Rigidbody2D>();
            _animator = GetComponent<Animator>();
            _isFacingRight = true;
        }

        private void Update()
        {
            Move();
        }

        private void Move()
        {
            var moveHorizon = UserInput.Instance.moveInput.x;
            var moveVertical = UserInput.Instance.moveInput.y;

            _rb.velocity = new Vector2(moveHorizon * moveSpeed, moveVertical * moveSpeed);

            if (moveHorizon != 0)
            {
                TurnCheck(moveHorizon);
            }

            if (moveHorizon != 0 || moveVertical != 0)
            {
                _animator.SetBool(IsWalking, true);
            }
            else
            {
                _animator.SetBool(IsWalking, false);
            }
        }

        private void TurnCheck(float moveHorizon)
        {
            if (moveHorizon > 0 && !_isFacingRight)
            {
                Turn();
            }

            if (moveHorizon < 0 && _isFacingRight)
            {
                Turn();
            }
        }

        private void Turn()
        {
            var rotationYValue = _isFacingRight ? 180f : 0f;
            var rotator = new Vector3(transform.rotation.x, rotationYValue, transform.rotation.z);
            transform.rotation = Quaternion.Euler(rotator);
            _isFacingRight = !_isFacingRight;
        }
    }

}