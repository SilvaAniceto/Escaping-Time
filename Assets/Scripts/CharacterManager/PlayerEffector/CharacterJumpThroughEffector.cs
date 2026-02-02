using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class CharacterJumpThroughEffector : MonoBehaviour
{
    private Collider2D _characterCollider;

    private void Awake()
    {
        _characterCollider = GetComponentInParent<CharacterContextManager>().GetComponent<Collider2D>();

        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("JumpThrough"), LayerMask.NameToLayer("Default"));
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("JumpThrough"), LayerMask.NameToLayer("TransparentFX"));
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("JumpThrough"), LayerMask.NameToLayer("Ignore Raycast"));
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("JumpThrough"), LayerMask.NameToLayer("Player"));
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("JumpThrough"), LayerMask.NameToLayer("Water"));
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("JumpThrough"), LayerMask.NameToLayer("UI"));
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("JumpThrough"), LayerMask.NameToLayer("Camera Objects"));
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("JumpThrough"), LayerMask.NameToLayer("Interaction Trigger"));
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("JumpThrough"), LayerMask.NameToLayer("Ceiling"));
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("JumpThrough"), LayerMask.NameToLayer("WallChecker"));
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("JumpThrough"), LayerMask.NameToLayer("JumpThrough"));
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.GetComponent<JumpThroughElement>())
        {
            return;
        }

        if (_characterCollider.transform.position.y < collision.bounds.max.y)
        {
            Physics2D.IgnoreCollision(_characterCollider, collision, true);
        }

    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.GetComponent<JumpThroughElement>())
        {
            return;
        }

        Physics2D.IgnoreCollision(_characterCollider, collision, false);
    }
}
