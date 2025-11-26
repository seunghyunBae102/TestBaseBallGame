using UnityEngine;

/// <summary>
/// 스킬/룰/상태 등을 정의하는 ScriptableObject 기본형.
/// </summary>
public abstract class ScriptableConfig : ScriptableObject
{
    [SerializeField] private string id;
    public string Id => id;
}
