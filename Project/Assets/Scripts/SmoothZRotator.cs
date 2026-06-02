using System.Collections;
using UnityEngine;

public class InfiniteAutoZRotator : MonoBehaviour
{
    // 오브젝트별 개별 설정을 위한 구조체
    [System.Serializable]
    public struct RotationConfig
    {
        public Transform targetObject;   // 회전할 오브젝트
        public float rotationAngle;     // 회전할 Z축 각도 (예: 2 설정 시 +2도와 -2도를 왕복)
        public float delayTime;         // 각 회전 상태에서 머무를 대기 시간 (요청하신 1초 등)
        public bool startPositive;      // true면 +각도부터 시작, false면 -각도부터 시작 (방향 다르게)
    }

    [Header("오브젝트별 개별 설정")]
    [SerializeField] private RotationConfig[] rotationConfigs;

    private void Start()
    {
        if (rotationConfigs == null || rotationConfigs.Length == 0) return;

        // 게임 시작과 동시에 모든 오브젝트의 회전 루프를 각각 독립된 코루틴으로 실행
        foreach (var config in rotationConfigs)
        {
            if (config.targetObject != null)
            {
                StartCoroutine(InfiniteRotateRoutine(config));
            }
        }
    }

    private IEnumerator InfiniteRotateRoutine(RotationConfig config)
    {
        Transform target = config.targetObject;
        Vector3 originRot = target.localEulerAngles;

        // 시작 방향에 따라 첫 번째, 두 번째 목표 각도 설정
        float firstAngle = config.startPositive ? config.rotationAngle : -config.rotationAngle;
        float secondAngle = config.startPositive ? -config.rotationAngle : config.rotationAngle;

        // while(true)를 통해 게임이 켜져 있는 동안 무한 반복
        while (true)
        {
            // 1. 첫 번째 방향으로 회전
            target.localRotation = Quaternion.Euler(originRot.x, originRot.y, firstAngle);
            // 지정된 시간(1초) 동안 대기
            yield return new WaitForSeconds(config.delayTime);

            // 2. 반대 방향으로 회전
            target.localRotation = Quaternion.Euler(originRot.x, originRot.y, secondAngle);
            // 지정된 시간(1초) 동안 대기
            yield return new WaitForSeconds(config.delayTime);
        }
    }
}