using UnityEngine;

[CreateAssetMenu(fileName = "Potion_", menuName = "Potion/Potion Data")]                //포션SO 이름은 Potion_세자리 번호로 통일    ex: Potion_001, Potion_201
public class PotionData : ScriptableObject
{
    public int potionID;              //포션ID 에셋 이름 뒤 세자릿수 숫자에서 0을 제외하고 설정
    public string potionName;           //포션 이름
    public Sprite artwork;              //포션 이미지
    public int[] potionRecipe = new int[5];          //포션 레시피(0: 빨강, 1: 노랑, 2: 핑크, 3: 초록, 4: 하늘) 
}
