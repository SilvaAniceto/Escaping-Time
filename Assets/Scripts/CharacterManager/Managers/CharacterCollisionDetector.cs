using UnityEngine;

public class CharacterCollisionDetector
{
    private Transform _transform;
    private Transform _wallCheckerPoint;
    private LayerMask _groundLayerTarget;
    private LayerMask _wallLayerTarget;

    private bool _isGrounded;
    private bool _isTouchingWall;

    public bool IsGrounded
    {
        get { return _isGrounded; }
    }

    public bool IsTouchingWall
    {
        get { return _isTouchingWall; }
    }

    public CharacterCollisionDetector(Transform transform, Transform wallCheckerPoint, LayerMask groundLayerTarget, LayerMask wallLayerTarget)
    {
        _transform = transform;
        _wallCheckerPoint = wallCheckerPoint;
        _groundLayerTarget = groundLayerTarget;
        _wallLayerTarget = wallLayerTarget;
    }

    public void UpdateCollisions()
    {
        UpdateGrounded();
        UpdateWallCollision();
    }

    private void UpdateGrounded()
    {
        _isGrounded = Physics2D.OverlapBox(_transform.position, new Vector2(0.40f, 0.2f), 0.00f, _groundLayerTarget);
    }

    private void UpdateWallCollision()
    {
        if (_wallCheckerPoint == null)
        {
            _isTouchingWall = false;
            return;
        }

        Collider2D collider = Physics2D.OverlapBox(_wallCheckerPoint.position, new Vector2(0.06f, 0.15f), 0.00f, _wallLayerTarget);

        if (collider != null)
        {
            if (collider.gameObject.CompareTag("Ground"))
            {
                _isTouchingWall = true;
                return;
            }
        }

        _isTouchingWall = false;
    }
}