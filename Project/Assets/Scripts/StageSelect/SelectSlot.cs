using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ScrollViewToggleController : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private RectTransform scrollRectTransform; // 움직일 스크롤뷰의 RectTransform
    [SerializeField] private Button clickButton;               // 클릭할 버튼

    [Header("Animation Settings")]
    [SerializeField] private float delayTime = 2f;      // 2초 대기 시간
    [SerializeField] private float duration = 1f;       // 이동하는 데 걸리는 시간 (1초)
    [SerializeField] private float inXPosition = 0f;     // 화면 안으로 들어왔을 때의 X 좌표
    [SerializeField] private float outXPosition = -1200f; // 화면 밖으로 나갔을 때의 X 좌표

    private bool isInScreen = false; // 현재 스크롤뷰가 화면 안에 있는지 여부
    private Coroutine moveCoroutine; // 현재 실행 중인 코루틴을 기억하기 위한 변수

    private void Start()
    {
        if (clickButton != null)
        {
            clickButton.onClick.AddListener(OnButtonClick);
        }

        // 시작할 때 스크롤뷰의 위치를 화면 밖 위치로 강제 초기화합니다.
        if (scrollRectTransform != null)
        {
            scrollRectTransform.anchoredPosition = new Vector2(outXPosition, scrollRectTransform.anchoredPosition.y);
        }
    }

    private void OnButtonClick()
    {
        // 연출이 진행되는 도중 버튼을 연속으로 막 누르는 것을 방지하기 위해 버튼을 잠시 끕니다.
        clickButton.interactable = false;

        // 혹시 기존에 이미 실행 중이던 이동 코루틴이 있다면 강제로 종료합니다. (버그 방지)
        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
        }

        // 상태를 반전시킵니다 (밖에 있었으면 안으로, 안에 있었으면 밖으로)
        isInScreen = !isInScreen;

        // 목적지 설정: isInScreen이 true면 안쪽 좌표(inXPosition), false면 바깥 좌표(outXPosition)
        float targetX = isInScreen ? inXPosition : outXPosition;

        // 코루틴 시작 및 변수에 저장
        moveCoroutine = StartCoroutine(MoveScrollViewRoutine(targetX));
    }

    private IEnumerator MoveScrollViewRoutine(float targetXPosition)
    {
        // 1. 지정된 시간(2초) 동안 대기
        yield return new WaitForSeconds(delayTime);

        float startX = scrollRectTransform.anchoredPosition.x;
        float elapsedTime = 0f;

        // 2. 부드럽게 이동하는 연출
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;

            // 부드러운 감속 효과 (Ease-Out) 수학 공식
            t = Mathf.Sin(t * Mathf.PI * 0.5f);

            float currentX = Mathf.Lerp(startX, targetXPosition, t);
            scrollRectTransform.anchoredPosition = new Vector2(currentX, scrollRectTransform.anchoredPosition.y);

            yield return null;
        }

        // 3. 목적지 좌표 최종 고정
        scrollRectTransform.anchoredPosition = new Vector2(targetXPosition, scrollRectTransform.anchoredPosition.y);

        // 이동이 완전히 끝났으므로 버튼을 다시 클릭할 수 있게 활성화합니다.
        clickButton.interactable = true;
        moveCoroutine = null;
    }
}