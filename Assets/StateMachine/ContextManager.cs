using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContextManager : MonoBehaviour
{
    private AbstractState _currentState;
    private StateFactory _stateFactory;

    public AbstractState CurrentState { get { return _currentState; } set { _currentState = value; } }


    void Awake()
    {
        _stateFactory = new StateFactory(this);
        _currentState = _stateFactory.State();
    }

    void Start()
    {
        _currentState.EnterState();
    }

    void Update()
    {
        _currentState.UpdateStates();
    }

    void FixedUpdate()
    {
        _currentState.FixedUpdateState();
    }

    void LateUpdate()
    {
        
    }

    void OnEnable()
    {
        
    }

    void OnDisable()
    {
        
    }

    void OnDestroy()
    {
        
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        
    }

    void OnCollisionStay(Collision collision)
    {
        
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        
    }
}

