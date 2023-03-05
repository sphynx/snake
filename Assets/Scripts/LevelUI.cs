using UnityEngine;
using TMPro;

public class LevelUI : MonoBehaviour
{
    [SerializeField]
    private TMP_Text scoreText;

    [SerializeField]
    private TMP_Text controlText;

    [SerializeField]
    private BoolVar aiMode;

    void Start()
    {
        SetScore(0);
    }

    public void SetScore(int score)
    {
        scoreText.text = $"Score: {score}";
    }

    void Update()
    {
        if (aiMode.Value)
        {
            controlText.text = "AI";
        }
        else
        {
            controlText.text = "Human";
        }
    }
}