using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChange : MonoBehaviour
{
    public void GoToStage()
    {
        SceneManager.LoadScene("Stage00");
    }
}