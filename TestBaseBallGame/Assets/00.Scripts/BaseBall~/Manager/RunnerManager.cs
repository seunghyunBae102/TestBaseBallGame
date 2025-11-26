// Scripts/Game/Baseball/Match/Runner/RunnerManager.cs
using System.Collections.Generic;


    /// <summary>
    /// ���̽� �� ���� ���¸� ����.
    /// - BatterAwardedBase: Ÿ��/���� �⺻ ����
    /// - RunsScored: ���� ���� �̺�Ʈ ����
    /// - RunnerAdvanceRequest: �߰� ����(����/���� �� �о�� ��) ó�� (�⺻ ���)
    /// </summary>
    public class RunnerManager : Module
    {
        // 1~3�� (index: 1~3 ���, 0�� Ȩ���̽���, 4�� ���� ó���� ����)
        private RunnerState?[] _bases = new RunnerState?[4];

        public IReadOnlyList<RunnerState?> GetBaseStates() => _bases;

        protected override void OnInit()
        {
            base.OnInit();
            Events?.Subscribe<BatterAwardedBase>(OnBatterAwardedBase);
            Events?.Subscribe<RunnerAdvanceRequest>(OnRunnerAdvanceRequest);
        }

        protected override void OnShutdown()
        {
            Events?.Unsubscribe<BatterAwardedBase>(OnBatterAwardedBase);
            Events?.Unsubscribe<RunnerAdvanceRequest>(OnRunnerAdvanceRequest);
            base.OnShutdown();
        }

        private void OnBatterAwardedBase(BatterAwardedBase e)
        {
            // 1) ���� ���� ���� ����
            ForceAdvanceAllRunners(e.BaseCount);

            // 2) Ÿ�� ���ڸ� Ȩ(0)���� e.BaseCount��ŭ ����
            var batterRunner = new RunnerState
            {
                id = new RunnerId { team = TeamSide.Away, batterIndex = 0, uniqueIndex = UnityEngine.Random.Range(0, 1000000) }, // TODO: ���� Ÿ�� Id�� ��ü
                baseIndex = 0,
                phase = RunnerPhase.OnBase
            };

            MoveRunner(ref batterRunner, e.BaseCount);
        }

        private void ForceAdvanceAllRunners(int baseCount)
        {
            // 3����� 1�� ������ ó���ؾ� ����� �� ����
            for (int baseIdx = 3; baseIdx >= 1; baseIdx--)
            {
                if (_bases[baseIdx].HasValue)
                {
                    var runner = _bases[baseIdx].Value;
                    MoveRunner(ref runner, baseCount);
                }
            }
        }

        private void MoveRunner(ref RunnerState runner, int baseCount)
        {
            int startBase = runner.baseIndex;
            int target = startBase + baseCount;

            // ���� ó��
            if (target > 3)
            {
                int runCount = 1;
                runner.phase = RunnerPhase.Scored;
                runner.baseIndex = 4;

                Events?.Publish(new RunsScored
                {
                    Team = runner.id.team,
                    RunCount = runCount
                });

                // ���� ��ġ ����
                if (startBase >= 1 && startBase <= 3)
                    _bases[startBase] = null;

                return;
            }

            // Ÿ�� ���̽��� ��������� �̵�
            if (!_bases[target].HasValue)
            {
                if (startBase >= 1 && startBase <= 3)
                    _bases[startBase] = null;

                runner.baseIndex = target;
                runner.phase = RunnerPhase.OnBase;
                _bases[target] = runner;
            }
            else
            {
                // TODO: �ַ��/�±� �ƿ�/���� �ƿ� �� Ȯ�� ����
                // ������ ������ ����� ���� �������� �ƹ��͵� �� ��.
            }
        }

        private void OnRunnerAdvanceRequest(RunnerAdvanceRequest req)
        {
            // TODO: ���� RunnerId�� ������� �ش� RunnerState�� ã�Ƽ�,
            // MoveRunner(ref runner, req.extraBaseCount) ȣ��.
            // ������ ��ݸ� ����� ��.
        }
    }


