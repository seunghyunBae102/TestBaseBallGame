// Scripts/Game/Baseball/Match/Rules/RunnerAdvanceModule.cs
using UnityEngine;


    /// <summary>
    /// ���̽� ���� ���¸� �����ϰ� BatterAwardedBase�� ����
    /// ���� ���� + RunsScored�� ����ϴ� ���.
    /// ������ �ſ� �ܼ��� �������縸 �����ϰ�,
    /// ����/���� ����/���� ���� �ٷ��� �ʴ´�.
    /// </summary>
    public class RunnerAdvanceModule : Module
    {
        // [0]=1��, [1]=2��, [2]=3��
        private bool[] _bases = new bool[3];

        private TeamSide _offenseTeam;

        protected override void OnInit()
        {
            base.OnInit();
            Events?.Subscribe<BatterAwardedBase>(OnBatterAwardedBase);
            Events?.Subscribe<InningHalfStarted>(OnInningHalfStarted);
            Events?.Subscribe<InningHalfEnded>(OnInningHalfEnded);
        }

        protected override void OnShutdown()
        {
            Events?.Unsubscribe<BatterAwardedBase>(OnBatterAwardedBase);
            Events?.Unsubscribe<InningHalfStarted>(OnInningHalfStarted);
            Events?.Unsubscribe<InningHalfEnded>(OnInningHalfEnded);
            base.OnShutdown();
        }

        private void OnInningHalfStarted(InningHalfStarted e)
        {
            _offenseTeam = e.Offense;
            ClearBases();
        }

        private void OnInningHalfEnded(InningHalfEnded e)
        {
            ClearBases();
        }

        private void ClearBases()
        {
            _bases[0] = _bases[1] = _bases[2] = false;
        }

        private void OnBatterAwardedBase(BatterAwardedBase e)
        {
            int baseCount = Mathf.Clamp(e.BaseCount, 1, 4);

            int runs = 0;

            // 1) ���� ���� ���� ���� (3�� �� 2�� �� 1�� ����)
            for (int i = 2; i >= 0; i--)
            {
                if (!_bases[i]) continue;

                int destIndex = i + baseCount;
                if (destIndex >= 3)
                {
                    // 3�縦 �Ѿ� Ȩ���� ��
                    runs += 1;
                    _bases[i] = false;
                }
                else
                {
                    // �ش� ���̽��� �̵�
                    _bases[destIndex] = true;
                    _bases[i] = false;
                }
            }

            // 2) Ÿ�� ����
            if (baseCount >= 4)
            {
                // Ȩ��: Ÿ�ڵ� Ȩ ���
                runs += 1;
            }
            else
            {
                int batterDest = baseCount - 1; // 1��=0, 2��=1, 3��=2
                if (batterDest >= 0 && batterDest <= 2)
                {
                    // �̹� ���� �� �־, ���� ���� ��꿡�� ��������Ƿ� ���⼱ ��� ���� ��.
                    _bases[batterDest] = true;
                }
            }

            if (runs > 0)
            {
                Events?.Publish(new RunsScored
                {
                    Team = _offenseTeam,
                    RunCount = runs
                });
            }
        }

        // ����׿�: ���� ���̽� ���� ���
#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            // ������ �ð�ȭ�� ���� ����� �׷��� �� (���� ����)
        }
#endif
    }


