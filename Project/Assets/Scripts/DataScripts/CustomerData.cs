using UnityEngine;

[CreateAssetMenu(fileName = "Customer_", menuName = "Customer/CustomerData")]                 //손님SO 이름은 Customer_세자리 번호로 통일    ex: Customer_001, Customer_202
public class CustomerData : ScriptableObject
{
    public int customerID;                  //손님ID 에셋 이름 뒤 세자릿수 숫자에서 0을 제외하고 설정
    public string customerName;            //손님이름
    public Sprite Artwork;                  //손님이미지
    public PotionData potionOrder;          //원하는 포션
    public string orderText;                //주문할때 하는 대사
    public string orderBadText;             //원하는걸 못 받았을 때 대사
    public string orderGoodText;            //원하는걸 받았을 때 대사
    public float moveSpeed;                 //이동속도
}
