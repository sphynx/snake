using UnityEngine;
using UnityEngine.SceneManagement;

public class NewGameUI : MonoBehaviour
{
    [SerializeField]
    BoolVar aiMode;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            aiMode.Value = false;
            SceneManager.LoadScene("Level");
        }
        else if (Input.GetKeyDown(KeyCode.I))
        {
            aiMode.Value = true;
            SceneManager.LoadScene("Level");
        }
    }

}
