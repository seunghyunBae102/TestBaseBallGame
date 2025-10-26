using UnityEngine;

public class Pawn : Actor
{
    protected Controller _controller;
    public Controller Controller => _controller;

    [SerializeField]
    protected CapsuleCollider _bodyCollider;

    public CapsuleCollider BodyCollider => _bodyCollider;

    public void SetController(Controller controller)
    {
        _controller = controller;
    }




}
