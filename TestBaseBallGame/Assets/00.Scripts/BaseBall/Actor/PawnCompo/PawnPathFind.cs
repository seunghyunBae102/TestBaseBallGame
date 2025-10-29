using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PawnPathFind : MonoBehaviour
{
    public Transform target;
    public Rigidbody ri;
    public float speed = 3f;
    public LayerMask la;                 // ��ֹ� ���̾�
    public CharacterController character;

    [Header("Sampling")]
    public bool use8Dirs = false;        // false=4����(�����¿�), true=8����(�밢 ����)
    public float step = 0.6f;            // �� ���� ���� �Ÿ�(�� �ڵ��� 0.6 ����)
    public float probe = 1.0f;           // ���� ����(��ֹ� ����)
    public float agentRadius = 0.3f;     // ���� �� ��� ������(0�̸� Raycast��, >0�̸� SphereCast)

    [Header("Rollout / Beam")]
    [Range(1, 10)] public int rolloutDepth = 5;  // n ���� �ùķ��̼� ����
    [Range(1, 8)] public int beamWidth = 4;      // �� ���̿��� ���� K ��� ����
    [Range(1, 8)] public int branchPerNode = 4;  // �� ��忡�� ���� B ���⸸ Ȯ��

    [Header("Scoring Weights")]
    public float wHeu = 1.0f;    // ��ǥ���� �Ÿ�(�������� ����)
    public float wTurn = 0.2f;   // ȸ��(���� ����) �г�Ƽ
    public float wBack = 0.7f;   // ������/�ǵ��ư� �г�Ƽ(��� ����)
    public float wStuck = 2.0f;  // �̵� ���� �г�Ƽ

    Vector3 lastMove = Vector3.zero;     // �ٷ� �� ������ �̵� ����(�ǵ��� ����)
    List<Vector3> dirs = new List<Vector3>(8);

    void Start()
    {
        //if (!target) target = player_move.plcam;
        BuildDirs();
        StartCoroutine(TickPlan());
        //InvokeRepeating(nameof(TickPlan), 0.2f, 0.1f);
    }

    void BuildDirs()
    {
        dirs.Clear();
        if (!use8Dirs)
        {
            // 4���� (�� ���� ����)
            dirs.Add(Vector3.forward);
            dirs.Add(Vector3.back);
            dirs.Add(Vector3.right);
            dirs.Add(Vector3.left);
        }
        else
        {
            // 8����
            dirs.Add(Vector3.forward);
            dirs.Add((Vector3.forward + Vector3.right).normalized);
            dirs.Add(Vector3.right);
            dirs.Add((Vector3.right + Vector3.back).normalized);
            dirs.Add(Vector3.back);
            dirs.Add((Vector3.back + Vector3.left).normalized);
            dirs.Add(Vector3.left);
            dirs.Add((Vector3.left + Vector3.forward).normalized);
        }
    }

    IEnumerator TickPlan()
    {
        //if (!target) return null;

        // 1) ���� ��ġ���� �ĺ� ����� �� ���̹� �� ĭ���� ������,
        //    �� �ĺ����� n���� �Ѿƿ��� ���� ���� ������ ���� ���� ù ���� ����
        while(true)
        {
            Vector3 bestStep = Vector3.zero;
            float bestScore = float.PositiveInfinity;

            // �ĺ� ������� ����ǥ�� ���൵�� �������� ����(��ǥ���� �켱)
            var ordered = OrderByGoalAlignment(dirs, (target.position - transform.position).normalized);

            // ���� branchPerNode ��ŭ�� 1���� �ĺ��� ���
            int firstCount = Mathf.Min(branchPerNode, ordered.Count);
            for (int i = 0; i < firstCount; ++i)
            {
                Vector3 d = ordered[i];
                if (!CanAdvance(transform.position, d, out Vector3 firstPos))
                {
                    // �� ĭ�� ������ ū �г�Ƽ
                    float bad = wStuck * 10f;
                    if (bad < bestScore)
                    {
                        bestScore = bad;
                        bestStep = Vector3.zero;
                    }
                    continue;
                }

                // 2) �Ѿƿ� ����(ù ������ d�� ����)
                float score = RolloutScore(firstPos, d, rolloutDepth - 1);

                // ù ���� ��ü�� ����� ������(ȸ��/������ ��)
                score += StepCost(transform.position, d, firstPos, lastMove);

                if (score < bestScore)
                {
                    bestScore = score;
                    bestStep = (firstPos - transform.position);
                }
            }

            // 3) �̹� ������ ���� �̵� ���� ����
            if (bestStep.sqrMagnitude > 1e-6f)
                lastMove = bestStep.normalized * step;
            else
                lastMove = Vector3.zero;

            // ���⼭�� ���� �̵��� ���� �ʰ� FixedUpdate���� �� ���� ����(���� ���� ���� ����)
            cachedNext = lastMove;
            yield return new WaitForSeconds(0.4f);
        }
        
    }

    // --- �Ѿƿ�: ���� n���� ���� �����ϸ� ���� ���� �ּ�ȭ ---
    float RolloutScore(Vector3 startPos, Vector3 startDir, int depthLeft)
    {
        // �� ��ġ: ���� ������ �ĺ� ���
        List<(Vector3 pos, Vector3 dir, float g, float score)> cur = new();
        cur.Add((startPos, startDir, 0f, Heuristic(startPos)));

        for (int depth = 0; depth < depthLeft; ++depth)
        {
            // ���� ���� �ĺ�
            List<(Vector3 pos, Vector3 dir, float g, float score)> nxt = new();

            foreach (var cand in cur)
            {
                // ��ǥ �þ� �����̸�: ��� �����ϴ� ���� �̵�(�޸���ƽ��)
                if (HasLineOfSight(cand.pos, target.position))
                {
                    float s = cand.g + wHeu * Heuristic(target.position);
                    nxt.Add((cand.pos, cand.dir, cand.g, s));
                    continue;
                }

                // cand���� Ȯ���� ����� ���� �� ���� branchPerNode��
                var ordered = OrderByGoalAlignment(dirs, (target.position - cand.pos).normalized);
                int take = Mathf.Min(branchPerNode, ordered.Count);

                for (int i = 0; i < take; ++i)
                {
                    Vector3 d = ordered[i];
                    if (!CanAdvance(cand.pos, d, out Vector3 np))
                    {
                        // �̵� ���д� ���� �г�Ƽ
                        float s = cand.g + wStuck * 5f + wHeu * Heuristic(cand.pos);
                        nxt.Add((cand.pos, cand.dir, cand.g, s));
                        continue;
                    }

                    float g = cand.g + StepCost(cand.pos, d, np, cand.dir * step); // g ����
                    float f = g + wHeu * Heuristic(np);
                    nxt.Add((np, d, g, f));
                }
            }

            if (nxt.Count == 0) break;

            // ��(Top-K) ����
            nxt.Sort((a, b) => a.score.CompareTo(b.score));
            if (nxt.Count > beamWidth) nxt.RemoveRange(beamWidth, nxt.Count - beamWidth);

            cur = nxt;
        }

        // ���� ���� �ĺ��� ���� ��ȯ
        float best = float.PositiveInfinity;
        for (int i = 0; i < cur.Count; ++i)
            if (cur[i].score < best) best = cur[i].score;

        return best;
    }

    // --- �� ���� �� �� �ִ°�? (Ray/SphereCast) ---
    bool CanAdvance(Vector3 from, Vector3 dir, out Vector3 nextPos)
    {
        dir = new Vector3(dir.x, 0f, dir.z).normalized;
        Vector3 dst = from + dir * step;

        bool blocked;
        if (agentRadius > 0f)
        {
            blocked = Physics.SphereCast(from + Vector3.up * 0.1f, agentRadius, dir, out _, probe, la, QueryTriggerInteraction.Ignore);
        }
        else
        {
            blocked = Physics.Raycast(from + Vector3.up * 0.1f, dir, probe, la, QueryTriggerInteraction.Ignore);
        }

        if (blocked)
        {
            nextPos = from;
            return false;
        }

        // �ٴ� ����(�־ �ǰ� ��� ��)
        if (Physics.Raycast(dst + Vector3.up * 0.5f, Vector3.down, out var hit, 2f, ~0, QueryTriggerInteraction.Ignore))
            dst.y = hit.point.y;
        nextPos = dst;
        return true;
    }

    bool HasLineOfSight(Vector3 a, Vector3 b)
    {
        Vector3 dir = (b - a);
        dir.y = 0f;
        float len = dir.magnitude;
        if (len < 1e-3f) return true;

        if (agentRadius > 0f)
            return !Physics.SphereCast(a + Vector3.up * 0.1f, agentRadius, dir.normalized, out _, len, la, QueryTriggerInteraction.Ignore);
        else
            return !Physics.Raycast(a + Vector3.up * 0.1f, dir.normalized, len, la, QueryTriggerInteraction.Ignore);
    }

    float Heuristic(Vector3 p)
    {
        // ��ǥ���� ���� �Ÿ�
        Vector3 a = transform.position; a.y = 0f;
        Vector3 b = target ? target.position : p; b.y = 0f;
        return Vector3.Distance(p.WithY(0f), b);
    }

    float StepCost(Vector3 from, Vector3 dir, Vector3 to, Vector3 prevMove)
    {
        // �̵� �Ÿ�
        float g = Vector3.Distance(from.WithY(0f), to.WithY(0f));

        // ȸ��(���� ����) �г�Ƽ
        Vector3 prevDir = prevMove.sqrMagnitude > 1e-6f ? prevMove.normalized : Vector3.zero;
        float turn = 0f;
        if (prevDir != Vector3.zero)
        {
            float dot = Mathf.Clamp(Vector3.Dot(prevDir, new Vector3(dir.x, 0f, dir.z).normalized), -1f, 1f);
            turn = Mathf.Acos(dot) * Mathf.Rad2Deg / 45f; // 45���� 1
        }

        // �ǵ��ư� �г�Ƽ
        float back = 0f;
        if (prevDir != Vector3.zero)
        {
            float d = Vector3.Dot(prevDir, new Vector3(dir.x, 0f, dir.z).normalized);
            if (d < -0.2f) back = 1f; // ���� �ݴ�� ������ ����
        }

        return g + wTurn * turn + wBack * back;
    }

    List<Vector3> OrderByGoalAlignment(List<Vector3> baseDirs, Vector3 toGoal)
    {
        toGoal.y = 0f;
        toGoal.Normalize();
        var list = new List<Vector3>(baseDirs);
        list.Sort((a, b) =>
        {
            float da = 1f - Vector3.Dot(new Vector3(a.x, 0, a.z).normalized, toGoal);
            float db = 1f - Vector3.Dot(new Vector3(b.x, 0, b.z).normalized, toGoal);
            return da.CompareTo(db);
        });
        return list;
    }

    Vector3 cachedNext;

    void FixedUpdate()
    {
        // �ü� ȸ��(����� "���� �̵�" �ݴ�� ���� �ִ� �� �ڵ� ����)
        if (cachedNext.sqrMagnitude > 1e-6f)
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(-cachedNext, Vector3.up), 0.15f);

        // ���� �̵�
        if (character)
            character.SimpleMove(cachedNext.normalized * speed);
        else if (ri)
            ri.linearVelocity = new Vector3(cachedNext.normalized.x * speed, ri.linearVelocity.y, cachedNext.normalized.z * speed);
        else
            transform.position += cachedNext.normalized * speed * Time.fixedDeltaTime;
    }
}

static class VecExt
{
    public static Vector3 WithY(this Vector3 v, float y) { v.y = y; return v; }
}
