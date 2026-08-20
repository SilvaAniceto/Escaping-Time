using System.Collections.Generic;

public class CharacterCommandBufferManager
{
    private const float MaxTimeForClearBuffer = 0.1f;
    private const int MaxBufferLength = 5;

    private Queue<ICharacterActionCommand> _jumpCommandBuffer = new Queue<ICharacterActionCommand>();
    private Queue<ICharacterActionCommand> _airJumpCommandCombo = new Queue<ICharacterActionCommand>();
    private Queue<ICharacterActionCommand> _dashCommandBuffer = new Queue<ICharacterActionCommand>();
    private Queue<ICharacterActionCommand> _wallMoveCommandBuffer = new Queue<ICharacterActionCommand>();
    private Queue<ICharacterActionCommand> _interactCommandBuffer = new Queue<ICharacterActionCommand>();
    private Queue<ICharacterActionCommand> _pauseCommandBuffer = new Queue<ICharacterActionCommand>();

    private float _jumpCommandBufferTimer;
    private float _dashCommandBufferTimer;
    private float _wallMoveCommandBufferTimer;
    private float _interactCommandBufferTimer;

    public Queue<ICharacterActionCommand> JumpCommandBuffer { get { return _jumpCommandBuffer; } }
    public Queue<ICharacterActionCommand> AirJumpCommandCombo { get { return _airJumpCommandCombo; } }
    public Queue<ICharacterActionCommand> DashCommandBuffer { get { return _dashCommandBuffer; } }
    public Queue<ICharacterActionCommand> WallMoveCommandBuffer { get { return _wallMoveCommandBuffer; } }
    public Queue<ICharacterActionCommand> InteractCommandBuffer { get { return _interactCommandBuffer; } }
    public Queue<ICharacterActionCommand> PauseCommandBuffer { get { return _pauseCommandBuffer; } }

    public float JumpCommandBufferTimer { get { return _jumpCommandBufferTimer; } set { _jumpCommandBufferTimer = value; } }
    public float DashCommandBufferTimer { get { return _dashCommandBufferTimer; } set { _dashCommandBufferTimer = value; } }
    public float WallMoveCommandBufferTimer { get { return _wallMoveCommandBufferTimer; } set { _wallMoveCommandBufferTimer = value; } }
    public float InteractCommandBufferTimer { get { return _interactCommandBufferTimer; } set { _interactCommandBufferTimer = value; } }

    public void ClearCommandBuffer(Queue<ICharacterActionCommand> commandBuffer)
    {
        commandBuffer.Clear();
    }

    public void ClearAirJumpCommandCombo()
    {
        _airJumpCommandCombo.Clear();
    }

    public void EnqueueJumpCommand(ICharacterActionCommand command)
    {
        if (_jumpCommandBuffer.Count >= MaxBufferLength)
        {
            ClearCommandBuffer(_jumpCommandBuffer);
            ClearCommandBuffer(_airJumpCommandCombo);
        }

        _jumpCommandBuffer.Enqueue(command);
        _airJumpCommandCombo.Enqueue(command);
    }

    public void EnqueueDashCommand(ICharacterActionCommand command)
    {
        if (_dashCommandBuffer.Count >= MaxBufferLength)
        {
            _dashCommandBuffer.Dequeue();
        }

        _dashCommandBuffer.Enqueue(command);
    }

    public void EnqueueWallMoveCommand(ICharacterActionCommand command)
    {
        if (_wallMoveCommandBuffer.Count >= MaxBufferLength)
        {
            ClearCommandBuffer(_wallMoveCommandBuffer);
        }

        _wallMoveCommandBuffer.Enqueue(command);
    }

    public void EnqueueInteractCommand(ICharacterActionCommand command)
    {
        if (_interactCommandBuffer.Count >= MaxBufferLength)
        {
            ClearCommandBuffer(_interactCommandBuffer);
        }

        _interactCommandBuffer.Enqueue(command);
    }

    public void ResetTimers()
    {
        _jumpCommandBufferTimer = 0f;
        _dashCommandBufferTimer = 0f;
        _wallMoveCommandBufferTimer = 0f;
        _interactCommandBufferTimer = 0f;
    }
}