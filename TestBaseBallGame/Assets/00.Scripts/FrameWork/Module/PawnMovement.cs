//using System;
//using UnityEngine;
//using Bash.Framework.Core;
//using Bash.Framework.Node;

//[Serializable]
//public struct MovementStat
//{
//    public float MaxSpeed;
//    public float Accelation;
//    public float JumpForce;
//    public float SlideTime;

//    public MovementStat(float maxSpeed, float accelation, float jumpForce, float slideTime)
//    {
//        MaxSpeed = maxSpeed;
//        Accelation = accelation;
//        JumpForce = jumpForce;
//        SlideTime = slideTime;
//    }
//}

//public class PawnMovement : Module
//{
//    protected Rigidbody _rigidCompo;

//    protected MovementStat _movementStat;

//    // Input And State
//    protected Vector3 _moveDir;
//    protected bool _isCanJump;
//    protected bool _isGround;

//    [SerializeField]
//    protected float _airAccelationModify = 0.1f;
//    [SerializeField]
//    protected float _defaultAccelationMod = 70f;

//    [SerializeField]
//    protected LayerMask _whatIsGround;

//    //public Pawn MyPawn;
//    public ActorNode MyPawn;

//    //public override void Init(Actor mom)
//    //{
//    //    base.Init(mom);
//    //    MyPawn = mom as Pawn; // cashing to be Smaller Casting
//    //}

//    protected virtual void Awake()
//    {
//        _rigidCompo = GetComponent<Rigidbody>();
//    }

//    void FixedUpdate()
//    {
//        MoveTick();
//    }

    

//    protected void MoveTick(float deltaTime = 0.02f)
//    {
//         _isCanJump = false;
//         _isGround = false;
//        float accelModify = _defaultAccelationMod;

//        Vector3 input = BashUtils.V2toV3(_moveDir);

//        if (Physics.SphereCast(transform.position, MyPawn.BodyCollider.radius, -transform.up, out RaycastHit groundCheck, MyPawn.BodyCollider.height/1.9f, _whatIsGround))
//        {
//            _isGround = true;
//            _isCanJump = true;
//            //OnGorund
//            MoveOnGorund(ref input,groundCheck.normal);
//        }
//        else
//        {
//            //OnAir

//            accelModify = _airAccelationModify;

//        }


//       input = (input * Mathf.Lerp(1, 0, (Vector3.Dot(_rigidCompo.linearVelocity.normalized, input) * _rigidCompo.linearVelocity.magnitude) / _movementStat.MaxSpeed) * _movementStat.Accelation)*deltaTime;

//        _rigidCompo.AddForce(input, ForceMode.Acceleration);


//        InputRefresh();
//    }

//    public void Jump()
//    {
//        if (_isCanJump)
//        {
//            _rigidCompo.AddForce(Vector3.up * _movementStat.JumpForce, ForceMode.Impulse);

//        }
//    }

//    //protected void CheckGround()
//    //{
//    //    return 
//    //}

//    public void SetMovementInput(Vector2 dir)
//    {
//        _moveDir = BashUtils.V2toV3(dir);
//    }
//    public void SetMovementInput(Vector3 dir)
//    {
//        _moveDir = dir;
//    }

//    protected void InputRefresh()
//    {
//        _moveDir = Vector3.zero;
//    }

//    protected void MoveOnGorund(ref Vector3 input,Vector3 normal)
//    {
//        Vector3 horizontalSpeed = BashUtils.V3X0Z(_rigidCompo.linearVelocity);

//        input = Vector3.ProjectOnPlane(input, normal);
//    }
//}
