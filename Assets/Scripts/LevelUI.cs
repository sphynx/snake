using UnityEngine;
using TMPro;

public class LevelUI : MonoBehaviour
{
    [SerializeField]
    private TMP_Text scoreText;

    void Start()
    {
        SetScore(0);
    }

    public void SetScore(int score)
    {
        scoreText.text = $"Score: {score}";
    }
}