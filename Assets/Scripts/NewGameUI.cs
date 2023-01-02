using UnityEngine;
using UnityEngine.SceneManagement;

public class NewGameUI : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            SceneManager.LoadScene("Level");
        };
    }

}
