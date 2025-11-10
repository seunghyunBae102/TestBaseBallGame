using UnityEngine;

public class Batting : ActorComponent, IBallHitable
{
    private BatSkillSO _currentBatSkill;
    private Transform _batPoint;
    private Animator _animator;
    
    [SerializeField]
    private float _swingDuration = 0.5f;
    private bool _isSwinging;
    
    protected override void Awake()
    {
        base.Awake();
        _animator = GetComponent<Animator>();
        
    }
    
    public void ThrowBall(Vector3 start, Vector3 targetPos, BallSKillSO ballSkill)
    {
        //GetOrAddCompo<BattingBall>().InitBall(start, targetPos, ballSkill);
        //이거 컴포넌트가 아니라 액터여야할듯.
    }

    public void Swing(Vector3 pos, float power)
    {
        
    }

    public void OnBattingPrepare()
    {

        // 타격 준비 상태 설정
        // 애니메이션 재생 등
    }


    public void OnBallHit(Vector3 hitPoint, Vector3 hitNormal)
    {
        throw new System.NotImplementedException();
    }
}
