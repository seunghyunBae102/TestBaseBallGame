using UnityEngine;

public interface IBattingControl
{
    void OnBattingStart(Vector2 position);
    void OnBattingDrag(Vector2 position);
    void OnBattingComplete(Vector2 position);
    bool CanBat { get; }
}