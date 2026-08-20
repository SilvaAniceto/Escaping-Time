using UnityEngine;

public class CharacterPhysicsHandler
{
    private CharacterContextManager _contextManager;
    private Joint2D _fixedJoint2D;

    public CharacterPhysicsHandler(CharacterContextManager contextManager, Joint2D fixedJoint2D)
    {
        _contextManager = contextManager;
        _fixedJoint2D = fixedJoint2D;
    }

    public void EnableFixedJoint2D()
    {
        if (_fixedJoint2D.enabled || _contextManager.FixedJointConnectedBody == null)
        {
            return;
        }

        if (_contextManager.CurrentState.CurrentSubState != _contextManager.CurrentState.CharacterStateFactory.IdleState())
        {
            return;
        }

        if (_contextManager.CurrentState != _contextManager.CurrentState.CharacterStateFactory.GroundedState())
        {
            return;
        }

        _fixedJoint2D.connectedBody = _contextManager.FixedJointConnectedBody;
        _fixedJoint2D.enableCollision = true;
        _fixedJoint2D.enabled = true;
    }

    public void DisableFixedJoint2D()
    {
        _fixedJoint2D.enabled = false;
        _fixedJoint2D.connectedBody = null;
    }
}