using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    CustomerData customerData;
    PotionData potionData;
    public PotionData[] craftablePotion;                         //제작 가능한 포션 배열
    public int nextCustomer = 0;                           //다음에 들어올 손님 번호
    public int estimateGuage = 0;                       //민심 게이지
    public int targetOrderCount = 0;                    //성공적으로 완료한 주문 개수

    public float[] minigameOptions = new float[3];              //0: 타임오버 게이지 줄어드는 속도, 1: 미니게임 인디케이터 속도, 2: 미니게임 인디케이더 왕복거리
    public Image timeOverGauge;                    //타임오버 게이지 이미지
    public RectTransform indicatorPosition;         //인디케이터 렉트 트랜스폼
    public RectTransform indicatorStartPos;             //인디케이터 왕복 시작 위치
    public RectTransform indicatorEndPos;               //인디케이터 왕복 끝 위치
    public float indicatorSpeed = 0.1f;             //인디케이터 왕복 속도;
    private float pingPongTime;                     //인디케이터 왕복 타이머
    public RectTransform[] perfectZone;             //퍼펙트존 구성요소는 씬, 인스펙터 참고
    string[] perfectZoneIndex = new string[5];                   //성공하여 수집한 퍼펙트존 배열
    public Image[] perfectZoneIndexImage = new Image[5];        //수집한 퍼펙트존 이미지 배열
    private int attemptCount = 0;                       //퍼펙트존 시도 횟수
    private int[] collectedIngredients = new int[5];    //수집한 재료 종류별 갯수 저장 배열(빨강 : 0, 노랑 : 1, 핑크 : 2, 초록 : 3, 파랑 : 4)
    public RectTransform resultBox;                    //포션 결과물이 나오는 상자UI
    public GameObject potionPrefab;                     //포션 프리팹

    public GameObject customerPrefabs;                  //손님 프리팹
    public GameObject customerDialog;                   //손님 대화UI
    public TextMeshProUGUI customerDialogText;              //손님 대화UI 텍스트
    public Transform joinPosition;                          //손님 입장 위치
    public Transform orderPosition;                         //손님이 주문하는 위치
    GameObject currentCustomer;                     //생성한 손님 저장
    public bool customerIsEmpty;                    //가게에 손님이 없는 상태


    public bool isOrdering = false;
    bool acceptOrder = false;
    bool isOnMiniGame = false;
    bool isAbleToGive = false;
    bool isOrderAccomplished;                       //주문이 완료되었는가?

    public List<CustomerData> customerOrder = new List<CustomerData>();             //손님 정보

    private static ShopManager instance;
    public static ShopManager Instance
    {
        get
        {
            if (instance == null) instance = new ShopManager();
            return instance;
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {
        //변수 초기화
        minigameOptions[0] = 0.08f;
        minigameOptions[1] = 150f;
        minigameOptions[2] = 200f;

        ShuffleCustomer();                      //등장하는 손님 목록 섞기
        CustomerJoin();                         //손님 입장
    }

    public void ShuffleCustomer()
    {
        List<CustomerData> tempCustomer = new List<CustomerData>(customerOrder);            //임시 복사
        customerOrder.Clear();                      //원본 리스트 비우기

        while (tempCustomer.Count > 0)              //임시 리스트가 빌때까지 반복
        {
            int randIndex = Random.Range(0, tempCustomer.Count);                //0~tempCustomer-1 범위에서 랜덤으로 숫자 뽑기
            customerOrder.Add(tempCustomer[randIndex]);                         //랜덤으로 뽑은 값 기반 진짜 리스트에 넣고
            tempCustomer.RemoveAt(randIndex);                                   //임시 리스트에서는 삭제
        }
    }

    public void CustomerJoin()                  //손님 입장
    {
        if (currentCustomer != null)
        {
            Destroy(currentCustomer);
        }
        Debug.Log("손놈생성");
        Debug.Log(customerPrefabs);
        int randNumber = Random.Range(0, customerOrder.Count);              //0에서 등장하는 손님 목록 범위 안에서 숫자 하나 뽑고
        customerData = customerOrder[randNumber];                        //나온 숫자 번호의 손님 생성
        PotionEstimate.Instance.currentCustomerData = customerData;     //가져온 손님 정보를 포션 평가기계에 전달
        //가져온 정보를 바탕으로 손님 프리팹을 입장위치에 회전값 없이 생성
        currentCustomer = Instantiate(customerPrefabs, joinPosition.position, Quaternion.identity);
    }
    public void OnOrderConfirmButtonClicked()               //말풍선 버튼을 누르면
    {
        if(acceptOrder)                         //주문 받기가 가능한 상태이면
        {
            StartPotionMaking();                //포션제작을 시작
        }
        else if(isOrderAccomplished == false)        //주문이 미완료면
        {
            customerDialog.SetActive(false);
            isOnMiniGame = false;
            CustomerController.Instance.isQuitting = true;
        }
        else if(isOrderAccomplished == true)        //주문을 성공적으로 완료했으면
        {
            customerDialog.SetActive(false);
            isOnMiniGame = false;
            CustomerController.Instance.isQuitting = true;
        }

        
    }
    public void StartPotionMaking()
    {
        acceptOrder = false;                //bool 값 재설정
        isOrdering = false;
        isOnMiniGame = true;                //미니게임 활성화
        customerDialog.SetActive(false);    //주문 UI 비활성화        
    }

    public string PerfectZoneDetect()
    {
        float indicatorXPos = indicatorPosition.anchoredPosition.x;         //인디케이터 앵커 포지션 저장                        
        foreach (RectTransform eachPerfectZone in perfectZone)               //퍼펙트존 배열에 있는거 다 꺼내서 하나씩 돌리기
        {
            float pzXPos = eachPerfectZone.anchoredPosition.x;              //꺼낸 퍼펙트존의 x 앵커포지션 저장
            float pzWidth = eachPerfectZone.rect.width;                       //꺼낸 퍼펙트존의 넓이 저장 (왼쪽 끝, 오른쪽 끝)
            //인디케이터가 퍼펙트 존의 오른쪽 끝, 왼쪽 끝 사이에 있으면
            if (indicatorXPos >= pzXPos - pzWidth / 2 && indicatorXPos <= pzXPos + pzWidth / 2)
            {
                return eachPerfectZone.name;
            }
        }
        return "miss";
    }

    public void UpdatePZIndexUI()           //수집한 퍼펙트존 UI표시 함수
    {
        if (perfectZoneIndex[attemptCount] == "PerfectZoneRed")
        {
            perfectZoneIndexImage[attemptCount].color = new Color(255f / 255f, 0f, 0f, 255f / 255f);                  //수집한 퍼펙트존 이미지 변경 
            collectedIngredients[0]++;
        }
        else if (perfectZoneIndex[attemptCount] == "PerfectZoneYellow")
        {
            perfectZoneIndexImage[attemptCount].color = new Color(255f / 255f, 255f / 255f, 0f, 255f / 255f);
            collectedIngredients[1]++;
        }
        else if (perfectZoneIndex[attemptCount] == "PerfectZonePink")
        {
            perfectZoneIndexImage[attemptCount].color = new Color(255f / 255f, 0f, 208f / 255f, 255f / 255f);
            collectedIngredients[2]++;
        }
        else if (perfectZoneIndex[attemptCount] == "PerfectZoneGreen")
        {
            perfectZoneIndexImage[attemptCount].color = new Color(0f, 255f / 255f, 0f, 255f / 255f);
            collectedIngredients[3]++;
        }
        else if (perfectZoneIndex[attemptCount] == "PerfectZoneBlue")
        {
            perfectZoneIndexImage[attemptCount].color = new Color(0f, 255f / 255f, 255f / 255f, 255f / 255f);
            collectedIngredients[4]++;
        }
    }

    public void PotionResult()
    {
        for (int i = 0; i < craftablePotion.Length; i++)             //제작 가능한 모든 포션 개수만큼 반복
        {
            if (craftablePotion[i].potionRecipe[i] == collectedIngredients[i])            //일치하는 레시피를 찾으면 해당 포션 제작
            {
                Debug.Log("포션을 만들어라!");
                potionData = craftablePotion[i];                        //만들어야 할 포션 정보를 가져오고
                PotionEstimate.Instance.craftedPotionData = potionData;             //포션 평가계에 만든 포션 정보 집어넣기
                Debug.Log(craftablePotion[i]);
                GameObject resultPotion = Instantiate(potionPrefab, resultBox, false);       //결과 박스에 포션을 회전값 없이 생성
                resultPotion.transform.SetAsFirstSibling();
                PotionDisplay potionDisplay = resultPotion.GetComponent<PotionDisplay>();   //포션 정보 설정
                if (potionDisplay != null)
                {
                    potionDisplay.SetupPotion(potionData);
                }
                resultPotion.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            }
        }
    }

    public void GoodEstimated()
    {
        targetOrderCount++;         //성공한 주문 수 증가
        customerDialog.SetActive(true);             //손님 대화박스 활성화 후
        customerDialogText.text = $"{customerData.orderGoodText}";           //긍정적인 대사 출력
    }

    public void BadEstimated()
    {
        estimateGuage++;                //민심 게이지 증가
        isOrderAccomplished = false;    //주문 미완료 처리
        customerDialog.SetActive(true);
        customerDialogText.text = $"{customerData.orderBadText}";
        
    }

    public void MinigameInitialize()                    //미니게임 초기화 함수
    {
        attemptCount = 0;
        isOnMiniGame = false;
        isOrdering = false;
        isAbleToGive = false;
        perfectZoneIndex = new string[5];
        collectedIngredients = new int[5];
        timeOverGauge.fillAmount = 1f;
        indicatorPosition = indicatorStartPos;
    }
    public void WaitNextCustomer()
    {
        StartCoroutine(WaitSecods());
    }
    IEnumerator WaitSecods()
    {
        yield return new WaitForSeconds(3.0f);
        CustomerJoin();
    }

    void Update()
    {
        if(isOrdering)              //주문이 활성화면
        {
            customerDialog.SetActive(true);             //손님 대화 UI 활성화
            customerDialogText.text = $"{customerData.orderText}";       //손님 주문 텍스트 설정
            acceptOrder = true;             //주문 받기가 가능할 때
        }

        if(acceptOrder)             //주문 받기가 가능할 때
        {
            if(Input.GetKeyDown(KeyCode.Space))     //스페이스 바를 누르면
            {
                StartPotionMaking();                //포션제작을 시작(버튼을 눌러도 시작)
            }
        }

        if(isOnMiniGame)            //미니게임 중이면
        {

            timeOverGauge.fillAmount -= minigameOptions[0] * Time.deltaTime;            //타임오버 게이지 감소
            pingPongTime += Time.deltaTime * indicatorSpeed;                            //시간이 지날수록 일정하게 타이머 증가
            
            float indicatorXPos = Mathf.Lerp(indicatorStartPos.anchoredPosition.x, indicatorEndPos.anchoredPosition.x, Mathf.PingPong(pingPongTime, 1));
            indicatorPosition.anchoredPosition = new Vector2(indicatorXPos, indicatorPosition.anchoredPosition.y);
            if (Input.GetKeyDown(KeyCode.Space))         //스페이스 바 누르면
            {
                string detectResult = PerfectZoneDetect();          //퍼펙트존 검사 함수를 실행하여 이름 string 반환
                if(detectResult != "miss")              //퍼펙트존을 성공하면
                {                  
                    perfectZoneIndex[attemptCount] = detectResult;              //검사 함수 반환값을 인덱스에 저장
                    UpdatePZIndexUI();              //수집한 퍼펙트존 UI표시 함수 실행
                    attemptCount++;                                             //시도 횟수 증가
                    Debug.Log($"{perfectZoneIndex[0]},{perfectZoneIndex[1]},{perfectZoneIndex[2]},{perfectZoneIndex[3]},{perfectZoneIndex[4]}");
                    Debug.Log($"{collectedIngredients[0]},{collectedIngredients[1]},{collectedIngredients[2]},{collectedIngredients[3]},{collectedIngredients[4]} ");
                }
            }
        }

        if(attemptCount >= 5)                   //시도 횟수가 5이상이면
        {
            PotionResult();                     //포션 제작 함수 실행
            attemptCount = 0;
            isOnMiniGame = false;
            isOrdering = false;
            isAbleToGive = true;                //포션을 제공할 수 있는 상태
        }
        else if(timeOverGauge.fillAmount <= 0f)     //혹은 시간이 전부 지나갔을 때
        {
            attemptCount = 0;
            isOnMiniGame = false;
            isOrdering = false;
            BadEstimated();
        }

        if (isAbleToGive == true && Input.GetKeyDown(KeyCode.E))             //포션을 제공할 수 있는 상태에서 E키를 누르면
        {
            if (PotionEstimate.Instance.EstimateStart())                    //포션 평가 함수를 실행하여 true 혹은 false 반환받기
            {   //true를 받으면 
                Debug.Log("정답");
                isOrderAccomplished = true;
                GoodEstimated();                //좋은 평가 함수 실행
            }
            else
            {   //false를 받으면
                isOrderAccomplished = false;    //주문 미완료 처리
                BadEstimated();                 //나쁜 평가 함수 실행
            }
        }
    }
}
