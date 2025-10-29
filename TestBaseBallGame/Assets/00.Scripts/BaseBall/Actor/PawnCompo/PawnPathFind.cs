using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PawnPathFind : MonoBehaviour
{
    public Transform target;
    public Rigidbody ri;
    public float speed;
    Vector3 maxtmp = new Vector3(10, 1, 10);
    public LayerMask la;
    List<Vector3> dir = new List<Vector3>();
    List<float> dis = new List<float>();
    public CharacterController character;

    private void Start()
    {
        StartCoroutine(Pathfind(0.3f));
    }
    public IEnumerator Pathfind(float tick , float raidius =0.25f)
    {
        WaitForSeconds delay = new(tick);
        while (true)
        {

            if(Physics.SphereCast(transform.position, raidius, target.position-transform.position, out RaycastHit hit, 1f, la, QueryTriggerInteraction.Ignore))

            dir.Clear();
            dis.Clear();

            for (int i = -1; i <= 1; i++)
            {
                for (int i2 = -1; i2 <= 1; i2++)
                {
                    if (((i != 0 || i2 != 0) && (i * i2 == 0)))
                        if (!(Physics.SphereCast(transform.position, raidius, Vector3.forward * i2 + Vector3.right * i,out RaycastHit hit,1f, la, QueryTriggerInteraction.Ignore)))//(!Physics.SphereCast(transform.position, 0.3f, Vector3.forward * i + Vector3.right * i2, out RaycastHit ray, 0.05f, la))
                        {
                            dir.Add((Vector3.right * i + Vector3.forward * i2).normalized * 0.6f);
                        }
                }
            }
            for (int i = 0; i < dir.Count; i++)
            {
                dis.Add(Vector3.Distance(transform.position + dir[i], target.position));
                if (dir[i] == maxtmp * -1)
                {
                    dis[i] = 1024;
                }
            }
            maxtmp = dir[dis.IndexOf(dis.Min())];

            //ri.velocity = maxtmp.normalized * speed + Vector3.up * ri.velocity.y;
            //ri.AddForce(transform.up * -5, ForceMode.Impulse);
            yield return delay;
        }
    }
    void FixedUpdate()
    {

        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(-maxtmp, transform.up), 0.1f);
        character.SimpleMove(maxtmp.normalized * speed);

    }
}