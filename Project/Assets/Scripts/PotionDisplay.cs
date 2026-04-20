using UnityEngine;
using UnityEngine.UI;

public class PotionDisplay : MonoBehaviour
{
    public PotionData potionInfo;               //포션 데이터 가져오기
    public Image potionImage;                        //포션 이미지
    public void SetupPotion(PotionData data)
    {
        potionInfo = data;

        //포션 이미지 설정
        potionImage.sprite = data.artwork;

    }
    void Start()
    {
        SetupPotion(potionInfo);
    }

    void Update()
    {

    }
}
