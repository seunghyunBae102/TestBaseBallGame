// Scripts/Game/Baseball/Match/Runner/RunnerAI.cs
using UnityEngine;


    /// <summary>
    /// �ſ� �ܼ��� �ַ� AI.
    /// ������ HitBallResult �������� "��� �� ���̽� �� ���� �;���" ������ ����.
    /// ���� �ƿ�/������ ������ RunnerManager �Ǵ� ���� Rule ��⿡�� �ϰ� �Ѵ�.
    /// </summary>
    public class RunnerAI : Module
    {
        [SerializeField] private RunnerManager _runnerManager;

        protected override void OnInit()
        {
            base.OnInit();
            Events?.Subscribe<HitBallResult>(OnHitBallResult);
        }

        protected override void OnShutdown()
        {
            Events?.Unsubscribe<HitBallResult>(OnHitBallResult);
            base.OnShutdown();
        }

        private void OnHitBallResult(HitBallResult r)
        {
            // ���� �ƿ��̸�(�ö��� �ƿ�) �ַ��� �� ����.
            if (r.IsCaughtInAir) return;
            if (!r.IsFair) return;

            // �⺻ ����: ��� ���ڰ� �� ���̽� �� ���� �;��Ѵ�.
            var bases = _runnerManager.GetBaseStates();
            for (int i = 0; i < bases.Count; i++)
            {
                if (!bases[i].HasValue) continue;
                var runner = bases[i].Value;

                // ������ RunnerAdvanceRequest ����
                Events?.Publish(new RunnerAdvanceRequest
                {
                    runnerId = runner.id,
                    extraBaseCount = 1
                });
            }

            // Ÿ�� ���ε� �߰� ���縦 �õ��ϰ� �ʹ١� �� �� ���߿�.
        }
    }


