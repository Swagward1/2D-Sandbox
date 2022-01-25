using UnityEngine;
using UnityEngine.SceneManagement;

/*Links:
https://www.youtube.com/watch?v=05OfmBIf5os
Unity video demonstrating how button OnClick functions work.*/

public class SceneController : MonoBehaviour
{
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}