using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class BeginGame : MonoBehaviour
{
    public void ClickAgain()
    {
        SceneManager.LoadScene("Game");
    }
    public void ClickMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

}
