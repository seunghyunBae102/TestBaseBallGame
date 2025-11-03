using UnityEngine;

public class RunnerController : Controller
{
    private Pawn _runner;
    private PawnMovement _movement;
    private PawnPathFind _pathFinder;
    
    [SerializeField]
    private float _baseCheckRadius = 1.5f;
    
    private Vector3 _targetBase;
    private bool _isRunning;
    
    protected virtual void Awake()
    {
        _runner = GetComponent<Pawn>();
        _movement = GetComponent<PawnMovement>();
        _pathFinder = GetComponent<PawnPathFind>();
    }

    public void RunToNextBase(Vector3 basePosition)
    {
        _targetBase = basePosition;
        _isRunning = true;
        
        if (_pathFinder != null)
        {
            _pathFinder.SetDestination(basePosition);
        }
    }

    private void Update()
    {
        if (!_isRunning) return;

        if (_pathFinder != null && _pathFinder.HasPath)
        {
            Vector3 moveDir = _pathFinder.GetNextPathDirection();
            _movement.SetMovementInput(new Vector2(moveDir.x, moveDir.z));
        }
        
        // 베이스 도달 체크
        if (Vector3.Distance(transform.position, _targetBase) < _baseCheckRadius)
        {
            _isRunning = false;
            _movement.SetMovementInput(Vector2.zero);
            // 베이스 도달 이벤트 발생
        }
    }
}
