using UnityEngine;

public class CharacterConveyorEffector : MonoBehaviour
{
    private CharacterContextManager _characterContextManager;

    private void Awake()
    {
        _characterContextManager = GetComponentInParent<CharacterContextManager>();

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
        if (!collision.GetComponent<ConveyorElement>())
        {
            return;
        }

        if (_characterContextManager.transform.position.y > collision.bounds.max.y)
        {
            collision.TryGetComponent(out Rigidbody2D rigidbody);

            if (rigidbody)
            {
                _characterContextManager.FixedJointConnectedBody = rigidbody;

                _characterContextManager.EnableFixedJoint2D();
            }
        }

    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.GetComponent<ConveyorElement>())
        {
            return;
        }

        _characterContextManager.FixedJointConnectedBody = null;

        _characterContextManager.DisableFixedJoint2D();
    }
}
