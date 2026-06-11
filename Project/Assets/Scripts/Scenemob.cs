using UnityEngine;
using UnityEngine.SceneManagement; // 씬 전환을 위해 반드시 필요한 네임스페이스

public class SceneChanger : MonoBehaviour
{
    // 버튼을 누르면 호출될 함수
    public void ChangeToStageSelect()
    {
        SceneManager.LoadScene("StageSelect");
    }
}