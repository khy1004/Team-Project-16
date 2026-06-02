using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIAnchorMover : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private RectTransform panelRectTransform; // 이동할 패널의 RectTransform
    [SerializeField] private Button toggleButton;              // 클릭할 버튼

    [Header("Movement Settings")]
    [SerializeField] private float startX = 0f;       // 시작 X 좌표
    [SerializeField] private float endX = 500f;       // 목표 X 좌표
    [SerializeField] private float duration = 0.8f;   // 이동 시간 (0.8초)

    private bool isAtEnd = false;       // 현재 패널이 목표치(endX)에 가 있는지 여부
    private Coroutine moveCoroutine;    // 현재 실행 중인 코루틴 저장용

    private void Start()
    {
        if (toggleButton != null)
        {
            // 버튼에 클릭 이벤트 연결
            toggleButton.onClick.AddListener(OnButtonClick);
        }

        // 시작할 때 패널의 초기 위치를 startX로 설정 (선택 사항)
        if (panelRectTransform != null)
        {
            Vector2 pos = panelRectTransform.anchoredPosition;
            pos.x = startX;
            panelRectTransform.anchoredPosition = pos;
        }
    }

    private void OnButtonClick()
    {
        if (panelRectTransform == null) return;

        // 다음 목표 X 좌표 결정
        float targetX = isAtEnd ? startX : endX;

        // 상태 뒤집기 (토글)
        isAtEnd = !isAtEnd;

        // 이미 이동 중인 코루틴이 있다면 멈춰서 버벅임을 방지
        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
        }

        // 새로운 이동 코루틴 시작
        moveCoroutine = StartCoroutine(MovePanel(targetX));
    }

    private IEnumerator MovePanel(float targetX)
    {
        Vector2 startPosition = panelRectTransform.anchoredPosition;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            // 0에서 1까지의 진행률 계산
            float percent = Mathf.Clamp01(elapsed / duration);

            // 부드러운 움직임을 위해 가속/감속(SmoothStep) 적용
            // 만약 딱딱한 선형 이동을 원하시면 그냥 percent를 넣으세요.
            float smoothPercent = Mathf.SmoothStep(0f, 1f, percent);

            // X 좌표 변경 및 적용
            float currentX = Mathf.Lerp(startPosition.x, targetX, smoothPercent);
            panelRectTransform.anchoredPosition = new Vector2(currentX, startPosition.y);

            yield return null; // 다음 프레임까지 대기
        }

        // 소수점 오차를 잡기 위해 마지막으로 목표 위치 정확히 고정
        panelRectTransform.anchoredPosition = new Vector2(targetX, startPosition.y);
        moveCoroutine = null;
    }
}