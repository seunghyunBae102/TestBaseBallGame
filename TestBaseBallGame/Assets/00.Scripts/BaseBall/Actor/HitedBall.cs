using UnityEngine;

public class HitedBall : Actor
{
    public Vector3 velocity;

    [SerializeField]
    protected LayerMask _whatIsObstacle;

    public void SimulateBall()
    {
        Vector3 simvelocity = velocity;

        while (true)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, simvelocity.normalized, out hit, simvelocity.magnitude * Time.fixedDeltaTime, _whatIsObstacle, QueryTriggerInteraction.UseGlobal))
            {
                transform.position = hit.point;
                break;
                //Collide whit AnyThing!;
            }
            transform.position += simvelocity * Time.fixedDeltaTime;
            simvelocity = Vector3.Lerp(simvelocity, Vector3.zero, GameManager.Instance.GetCompo<BaseBallGameManager>().GetCompo<BaseBallFieldManager>().FieldInfo.AirFriction * Time.fixedDeltaTime);
            simvelocity += GameManager.Instance.GetCompo<BaseBallGameManager>().GetCompo<BaseBallFieldManager>().FieldInfo.Gravity * Time.fixedDeltaTime;
            if (simvelocity.magnitude < 0.1f)
                break;
        }
    }

    protected void FixedUpdate()
    {
        GameManager.Instance.GetCompo<BaseBallGameManager>().GetCompo<BaseBallRaycastManager>().AddRaycastSchedul(
            new RaycastComboSet(
                new RaycastCommand(transform.position, velocity.normalized,  new QueryParameters(_whatIsObstacle,false,QueryTriggerInteraction.UseGlobal,false), velocity.magnitude * Time.fixedDeltaTime),
                Move
                )
            );
    }

    public void Move(RaycastHit hit,float DeltaTime) //ccd
    {
        if(hit.collider != null)
        {
            transform.position = hit.point;
            return;
            //COllide whit AnyThing!;
        }

        transform.position += velocity * DeltaTime;

        velocity = Vector3.Lerp(velocity, Vector3.zero, GameManager.Instance.GetCompo<BaseBallGameManager>().GetCompo<BaseBallFieldManager>().FieldInfo.AirFriction * DeltaTime);

        velocity += GameManager.Instance.GetCompo<BaseBallGameManager>().GetCompo<BaseBallFieldManager>().FieldInfo.Gravity * DeltaTime;

    }


}
