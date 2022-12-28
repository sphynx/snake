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

    void Start()
    {
        //bool record = false;
        if (score.Value > hiScore.Value)
        {
            //record = true;
            hiScore.Value = score.Value;
        }

        //string newBest = record ? "- NEW BEST!" : "";

        scoreText.text = $"Your score: {score.Value}";
        hiScoreText.text = $"High score: {hiScore.Value}";
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            Restart();
        };
    }

    void Restart()
    {
        SceneManager.LoadScene("Level");
    }
}
