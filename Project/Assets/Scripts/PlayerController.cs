using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerController : MonoBehaviour
{
    private Vector2 moveInput;              //인풋 시스템 값 저장
    public float moveSpeed = 4f;            //플레이어 이동속도 계수
    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }
    void Start()
    {
        
    }


    void Update()
    {
        transform.Translate(moveInput * moveSpeed * Time.deltaTime);
    }
}
