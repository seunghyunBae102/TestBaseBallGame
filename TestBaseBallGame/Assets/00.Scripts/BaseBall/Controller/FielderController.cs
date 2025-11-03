using UnityEngine;

public class FielderController : Controller
{
    private Pawn _controlledFielder;
    private PawnMovement _pawnMovement;
    private PawnPathFind _pathFinder;
    
    [SerializeField]
    private float _catchRadius = 1.5f;
    
    private Vector3 _targetPosition;
    private bool _isChasing = false;

    private void Awake()
    {
        _controlledFielder = GetComponent<Pawn>();
        _pawnMovement = GetComponent<PawnMovement>();
        _pathFinder = GetComponent<PawnPathFind>();
    }

    public void SetTargetPosition(Vector3 position)
    {
        _targetPosition = position;
        _isChasing = true;
    }

    public void StopChasing()
    {
        _isChasing = false;
        _pawnMovement.SetMovementInput(Vector2.zero);
    }

    private void Update()
    {
        if (!_isChasing) return;

        Vector3 directionToTarget = (_targetPosition - transform.position);
        directionToTarget.y = 0;

        if (directionToTarget.magnitude <= _catchRadius)
        {
            StopChasing();
            return;
        }

        Vector2 moveDirection = new Vector2(directionToTarget.x, directionToTarget.z).normalized;
        _pawnMovement.SetMovementInput(moveDirection);
    }

    public bool IsInCatchRange(Vector3 position)
    {
        Vector3 difference = position - transform.position;
        difference.y = 0;
        return difference.magnitude <= _catchRadius;
    }
}
