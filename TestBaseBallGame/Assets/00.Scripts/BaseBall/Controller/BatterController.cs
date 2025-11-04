using UnityEngine;

public class BatterController : Controller, IBattingControl
{
    protected Batter _batter; //다 컴포넌트로 받아와야디 ㅎㅎ
    public bool CanBat => _batter != null && _batter.CanBat;
    
    public void OnBattingStart(Vector2 position)
    {
        if (!CanBat) return;
        _batter.OnBattingStart(position);
    }

    public void OnBattingDrag(Vector2 position)
    {
        if (!CanBat) return;
        _batter.OnBattingDrag(position);
    }

    public void OnBattingComplete(Vector2 position)
    {
        if (!CanBat) return;
        _batter.OnBattingComplete(position);
    }

    public void PrepareBat()
    {
        _batter?.PrepareBat();
    }

    public void MoveToHomeBase(Vector3 position)
    {
        _batter?.MoveToPosition(position);
    }
}
