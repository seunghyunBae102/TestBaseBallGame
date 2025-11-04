using System;
using UnityEngine;



public class Batter : Pawn, IBattingControl
{
    private Batting _battingSystem;
    private PawnMovement _movement;
    
    public Vector3 BatPosition;
    [SerializeField] private float _maxBattingPower = 100f;
    [SerializeField] private float _minBattingPower = 10f;
    
    private bool _isReadyToBat;
    private Vector2 _battingStartPos;
    private float _currentBattingPower;
    
    public bool CanBat => _isReadyToBat && _battingSystem != null;

    protected override void Awake()
    {
        base.Awake();
        _battingSystem = GetComponent<Batting>();
        _movement = GetComponent<PawnMovement>();
    }

    public void OnBattingStart(Vector2 position)
    {
        if (!CanBat) return;
        _battingStartPos = position;
        _currentBattingPower = _minBattingPower;
    }

    public void OnBattingDrag(Vector2 position)
    {
        if (!CanBat) return;
        float distance = Vector2.Distance(_battingStartPos, position);
        _currentBattingPower = Mathf.Lerp(_minBattingPower, _maxBattingPower, 
            distance / Screen.height);
    }

    public void OnBattingComplete(Vector2 position)
    {
        if (!CanBat) return;
        Vector3 targetPoint = CalculateHitTarget(position);
        ExecuteSwing(targetPoint, _currentBattingPower);
    }

    public void PrepareBat()
    {
        _isReadyToBat = true;
        _battingSystem?.OnBattingPrepare();
    }

    private void ExecuteSwing(Vector3 targetPoint, float power)
    {
        if (!_isReadyToBat) return;
        _battingSystem?.Swing(targetPoint,power);
        _isReadyToBat = false;
    }

    public void MoveToPosition(Vector3 position)
    {
        if (_movement == null) return;
        Vector2 moveDir = new Vector2(
            position.x - transform.position.x,
            position.z - transform.position.z).normalized;
        _movement.SetMovementInput(moveDir);
    }

    private Vector3 CalculateHitTarget(Vector2 screenPosition)
    {
        Ray ray = Camera.main.ScreenPointToRay(screenPosition);
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
        
        if (groundPlane.Raycast(ray, out float enter))
        {
            return ray.GetPoint(enter);
        }
        
        return transform.position + transform.forward * 10f;
    }

    public void RegisterEvents(Actor main)
    {
        throw new NotImplementedException();
    }

    public void UnregisterEvents(Actor main)
    {
        throw new NotImplementedException();
    }
}
