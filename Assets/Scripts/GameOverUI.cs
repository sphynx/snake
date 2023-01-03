using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameOverUI : MonoBehaviour
{
    [SerializeField]
    private Score score;

    [SerializeField]
    private Score hiScore;

    [SerializeField]
    private TMP_Text scoreText;

    [SerializeField]
    private TMP_Text hiScoreText;

    [SerializeField]
    private TMP_Text newHiScoreText;

    void Start()
    {
        if (score.Value > hiScore.Value)
        {
            hiScore.Value = score.Value;
            newHiScoreText.enabled = true;
        }
        else
        {
            newHiScoreText.enabled = false;
        }

        scoreText.text = $"Your score: {score.Value}";
        hiScoreText.text = $"High score: {hiScore.Value}";
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene("Level");
        }
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }
}
