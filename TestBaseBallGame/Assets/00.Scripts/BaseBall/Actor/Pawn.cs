using UnityEngine;

public class Pawn : Actor
{
    protected Controller _controller;
    public Controller Controller => _controller;

    public void SetController(Controller controller)
    {
        _controller = controller;
    }




}
