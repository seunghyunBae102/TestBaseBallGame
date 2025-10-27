
using System.Collections.Generic;
using UnityEngine;

public class HitedBall : Actor
{
    public Vector3 velocity;

    public SphereCollider ballCollider;

    public float mass = 1f;
    public Vector3 angularVelocity;

    public IHitedBallSKillable ballSkill;

    protected bool _isActive = false;

    [SerializeField]
    protected LayerMask _whatIsObstacle;
    [SerializeField] 
    SurfaceCatalogSO surfaces;
    protected bool _rolling = false;

    protected BaseBallFieldManager _fieldManager;
    public BaseBallFieldManager fieldManager
    {
        get
        {
            if (_fieldManager == null)
            {
                _fieldManager = GameManager.Instance.GetCompo<BaseBallGameManager>().GetCompo<BaseBallFieldManager>();
            }
            return _fieldManager;
        }
    }

    protected BaseBallRaycastManager _raycastManager;
    public BaseBallRaycastManager raycastManager
    {
        get
        {
            if (_raycastManager == null)
            {
                _raycastManager = GameManager.Instance.GetCompo<BaseBallGameManager>().GetCompo<BaseBallRaycastManager>();
            }
            return _raycastManager;
        }
    }

    protected BaseBallRunningManager _runningManager;
    public BaseBallRunningManager runningManager
    {
        get
        {
            if (_runningManager == null)
            {
                _runningManager = GameManager.Instance.GetCompo<BaseBallGameManager>().GetCompo<BaseBallRunningManager>();
            }
            return _runningManager;
        }
    }

    public float BallRadius
    {
        get
        {
            if (ballCollider != null)
            {
                return ballCollider.radius * Mathf.Max(transform.localScale.x, transform.localScale.y, transform.localScale.z);
            }
            return 0.5f;
        }
    }

    

    protected void FixedUpdate()
    {
        raycastManager.AddSpherecastSchedul(
        new SpherecastComboSet(
        new SpherecastCommand(Physics.defaultPhysicsScene, transform.position, BallRadius, velocity.normalized, new QueryParameters(_whatIsObstacle, false, QueryTriggerInteraction.UseGlobal, false), velocity.magnitude * Time.fixedDeltaTime),
        Move
        )
    );
        if (_rolling) RollingStep(Time.fixedDeltaTime);
    }

    public void InitBall(Vector3 startPos, Vector3 velocity,Vector3 angluerVelocity, IHitedBallSKillable ballSkill)
    {
        transform.position = startPos;
        this.velocity = velocity;
        this.angularVelocity = angluerVelocity;
        this.ballSkill = ballSkill;

        PostInitBall(velocity, angluerVelocity, ballSkill);
        _isActive = true;
    }

    public void PostInitBall(Vector3 velocity, Vector3 angluerVelocity, IHitedBallSKillable ballSkill)
    {
        IHitedBallSKillable ballHitable = ballSkill;

        this.ballSkill = new SimulateBallSkill();
        List<Vector3> simuatlatedposes;
        SimulateBall(out simuatlatedposes);

        runningManager.ChangeBallPos(simuatlatedposes);

        ballSkill = ballHitable;
    }


    public void SimulateBall(out List<Vector3> simuatlatedposes, float cnt = 20f)
    {
        Vector3 simvelocity = velocity;

        simuatlatedposes = new List<Vector3>();

        while (true)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, simvelocity.normalized, out hit, simvelocity.magnitude * Time.fixedDeltaTime * 2, _whatIsObstacle, QueryTriggerInteraction.UseGlobal))
            {
                Move(hit, Time.fixedDeltaTime * 2);
            }

            simuatlatedposes.Add(transform.position);

            cnt -= Time.fixedDeltaTime * 2;
            if (simvelocity.magnitude < 0.1f || cnt <=0)
                break;
        }
    }


    public void Move(RaycastHit hit,float DeltaTime) //ccd
    {
        velocity += fieldManager.FieldInfo.Gravity * DeltaTime;

        if (hit.collider != null)
        {
            transform.position = hit.point;
            
            Vector3 pos = transform.position;
            ResolveCollision(hit, ref pos, ref velocity, ref angularVelocity, DeltaTime);
            transform.position = pos;

            ballSkill?.OnHit(this, hit);

            return;
        }
        
        transform.position += velocity * DeltaTime;

        velocity = Vector3.Lerp(velocity, Vector3.zero, fieldManager.FieldInfo.AirFriction * DeltaTime);

    }

    protected void ResolveCollision(RaycastHit hit, ref Vector3 pos, ref Vector3 v, ref Vector3 w, float dt)
    {
        var n = hit.normal.normalized;
        var sp = surfaces ? surfaces.GetFor(hit.collider) : null;
        float e = sp?.restitution ?? 0.35f;    // 탄성
        float mu = sp?.friction ?? 0.6f;      // 접선 마찰

        // 속도 분해
        float vnMag = Vector3.Dot(v, n);
        var vn = vnMag * n;
        var vt = v - vn;

        // 들어가고 있는 충돌만 처리
        if (vnMag > 0f) { pos += n * 0.001f; return; }

        // 법선 반사(탄성계수)
        var vnOut = -e * vn;

        // Coulomb 마찰 임펄스 근사: vt' = vt - clamp(μ*(1+e)*|vn|, |vt|) * vt_hat
        var vtMag = vt.magnitude;
        Vector3 vtOut = vt;
        if (vtMag > 1e-4f)
        {
            float slipDecel = mu * (1f + e) * Mathf.Abs(vnMag);
            float reduce = Mathf.Min(slipDecel, vtMag);
            vtOut = vt - (reduce / vtMag) * vt;
        }

        v = vnOut + vtOut;

        // 간단한 스핀 변화: 접선 변화가 있을 때 접촉 축으로 스핀 부여/감쇠
        // (정확한 강체임펄스는 I=(2/5)mR^2로 계산하지만 여기선 경량 근사)
        if (vtMag > 1e-4f)
        {
            var axis = Vector3.Cross(n, vt.normalized); // 접촉선 수직축
            float spinGain = 10f;   // 튜닝(배트 충돌은 더 크게)
            w += axis * (spinGain * (vtMag - vtOut.magnitude) / BallRadius);
        }

        // 바닥면(법선이 위쪽)이고 속도 낮으면 rolling 진입
        if (Vector3.Dot(n, Vector3.up) > 0.5f && v.magnitude < 12f) // 임계속도 튜닝
            EnterRolling(n, sp);
    }

    protected void EnterRolling(Vector3 groundNormal, SurfaceProps sp)
    {
        _rolling = true;
        // 순수 구름 조건: 접점에서 상대속도≈0 → ω ≈ (n × v)/R
        if (velocity.sqrMagnitude > 1e-4f)
            angularVelocity = Vector3.Cross(groundNormal, velocity) / Mathf.Max(1e-4f, BallRadius);

        // 잔디에서 첫 접촉 시 속도 약간 감쇠
        if (sp != null) velocity *= 1f - Mathf.Clamp01(sp.friction * 0.15f);
    }

    bool NearGround(out RaycastHit groundHit)
    {
        return Physics.SphereCast(transform.position, BallRadius-0.01f, Vector3.down, out groundHit, 0.02f, _whatIsObstacle);
    }


    void RollingStep(float dt)
    {
        // 지면이 없으면 롤링 해제
        if (!NearGround(out var groundHit)) { _rolling = false; return; }

        var sp = surfaces ? surfaces.GetFor(groundHit.collider) : null;
        var n = groundHit.normal.normalized;
        var v = velocity;

        // 순수 구름 제약(느슨하게): 접점에서 상대속도 거의 0
        // v_t = v - (v·n)n;     ω_target = (n × v)/R
        var vt = v - Vector3.Dot(v, n) * n;
        angularVelocity = Vector3.Lerp(angularVelocity, Vector3.Cross(n, v) / Mathf.Max(1e-4f, BallRadius), 0.5f);

        // 구름 저항(속도 방향 반대 감속): a = - μ_r g * v_hat
        float mu_r = sp?.rollingResistance ?? 0.04f;
        if (vt.sqrMagnitude > 1e-6f)
        {
            var vhat = vt.normalized;
            var a = -(mu_r * Mathf.Abs(fieldManager.FieldInfo.Gravity.y)) * vhat; // m/s^2
            v += a * dt;
        }

        // 스핀 감쇠(잔디 거칠기)
        float spinDamp = sp?.spinDamping ?? 0.1f;
        angularVelocity *= Mathf.Clamp01(1f - spinDamp * dt);

        // 거의 멈췄으면 종료
        if (v.magnitude < 0.2f) { v = Vector3.zero; _rolling = false; }

        velocity = v;
    }



}


public class SimulateBallSkill : IHitedBallSKillable
{
    public void OnMove()
    {

    }

    public void OnCollide(HitedBall ballm, RaycastHit hit)
    {

    }

    public void OnHit(HitedBall ballm, RaycastHit hit)
    {

    }
}