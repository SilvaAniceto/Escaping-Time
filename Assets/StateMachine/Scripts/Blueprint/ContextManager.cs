using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContextManager : MonoBehaviour
{
    private AbstractState _currentState;
    private StateFactory _stateFactory;

    public AbstractState CurrentState { get { return _currentState; } set { _currentState = value; } }

    int _cycleCount = 1;
    int _awakeCount = 0;
    int _onEnableCount = 0;
    int _startCount = 0;
    int _fixedUpdateCount = 0;
    int _updateCount = 0;
    int _lateUpdateCount = 0;
    int _onDisableCount = 0;
    int _onDestroyCount = 0;
 
    void Awake()
    {
        _stateFactory = new StateFactory(this);
        _currentState = _stateFactory.State();
        _awakeCount++;
    }

    void OnEnable()
    {
        _onEnableCount++;
    }

    void Start()
    {
        _currentState.EnterState();
        _startCount++;
    }

    void FixedUpdate()
    {
        _currentState.FixedUpdateState();
        _fixedUpdateCount++;
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

    void Update()
    {
        _currentState.UpdateStates();
        _updateCount++;
    }

    void LateUpdate()
    {
        _currentState.LateUpdateState();
        _cycleCount++;
        _lateUpdateCount++;
    }

    void OnGUI()
    {
        GUILayout.Label(_cycleCount + "º CICLO");
        GUILayout.Label(_awakeCount + " AWAKE");
        GUILayout.Label(_onEnableCount + " ON ENABLE");
        GUILayout.Label(_startCount + " START");
        GUILayout.Label(_fixedUpdateCount + " FIXED UPDATE");
        GUILayout.Label(_updateCount + " UPDATE");
        GUILayout.Label(_lateUpdateCount + " LATE UPDATE");
        GUILayout.Label(_onDisableCount + " ON DISABLE");
        GUILayout.Label(_onDestroyCount + " ON DESTROY");
        GUILayout.Label(Time.deltaTime + " DELTA TIME");
        GUILayout.Label(Time.fixedDeltaTime + " FIXED DELTA TIME");
        GUILayout.Label(Time.time + " TIME");
        GUILayout.Label(TimeSpan.MinValue + " TIME SPAN MINIMUN");
        GUILayout.Label(TimeSpan.MaxValue + " TIME SPAN MAXIMUN");
    }

    void OnDisable()
    {
        _onDisableCount++;
    }

    void OnDestroy()
    {
        _onDestroyCount++;
    }
}