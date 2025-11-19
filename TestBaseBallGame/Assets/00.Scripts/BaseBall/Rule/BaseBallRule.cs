using UnityEngine;
[System.Serializable]
public class ScoreInfo
{
    public int runs;
    public int hits;
    public int errors;
}

public class BaseBallRule : MonoBehaviour
{

    
    
    private ScoreInfo[] _teamScores = new ScoreInfo[2];
    private int _strikes;
    private int _balls;
    private int _outs;
    
    public void AddStrike()
    {
        _strikes++;
        if (_strikes >= 3)
        {
            ProcessOut();
        }
    }
    
    public void AddBall()
    {
        _balls++;
        if (_balls >= 4)
        {
            ProcessWalk();
        }
    }
    
    public void ProcessOut()
    {
        _outs++;
        ResetCount();
        
        if (_outs >= 3)
        {
            // 이닝 종료
            GameManager.Instance.GetCompo<BaseBallGameManager>().StartNewInning();
        }
    }
    
    private void ProcessWalk()
    {
        // 4구 처리
        ResetCount();
        // 주자 진루
    }
    
    private void ResetCount()
    {
        _strikes = 0;
        _balls = 0;
    }
    
    public void AddRun(int teamIndex)
    {
        if (teamIndex >= 0 && teamIndex < 2)
        {
            _teamScores[teamIndex].runs++;
        }
    }
    
    public void AddHit(int teamIndex)
    {
        if (teamIndex >= 0 && teamIndex < 2)
        {
            _teamScores[teamIndex].hits++;
        }
    }
}
