using System.Collections.Generic;
using UnityEngine;

public class DoorPassage : MonoBehaviour
{
    [SerializeField] private List<DoorSwitch> _doorSwitches = new List<DoorSwitch>();

    private Collider2D _collider;
    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _collider = GetComponent<Collider2D>();
    }

    private void Start()
    {
        _collider.enabled = false;
    }

    private void Update()
    {
        if (_doorSwitches.TrueForAll(x => x.Activated == true))
        {
            _animator.Play("Opening");
            _collider.enabled = true;
        }
    }
}
