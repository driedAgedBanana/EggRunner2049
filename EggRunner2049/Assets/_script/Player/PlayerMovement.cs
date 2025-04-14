using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 5f;
    public Rigidbody2D _rb2D;
    public Weapon weapon;

    private Vector2 _moveDirection;
    private Vector2 _mousePosition;

    private void Update()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.F))
        {
            weapon.Fire();
        }

        _moveDirection = new Vector2(moveX, moveY).normalized;
        _mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    private void FixedUpdate()
    {
        _rb2D.linearVelocity = new Vector2(_moveDirection.x * _moveSpeed, _moveDirection.y * _moveSpeed);

        Vector2 aimDirection = _mousePosition - _rb2D.position;
        float aimAngle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg - 90f;
        _rb2D.rotation = aimAngle;
    }
}
