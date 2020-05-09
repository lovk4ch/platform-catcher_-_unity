using UnityEngine;
using UnityEngine.UI;

public class UIManager : Manager<UIManager>
{
    private int score => LevelManager.Instance.Blocks.Count - 1;
    private int highscore = 0;

    [SerializeField]
    private Text highscoreText = null;

    [SerializeField]
    private Text scoreText = null;

    [SerializeField]
    private Button restart = null;

    public bool RestartEnabled
    {
        set => restart.gameObject.SetActive(value);
    }

    private void Awake()
    {
        LevelManager.Instance.AddListener(UpdateUI);
        restart.onClick.AddListener(() => Restart());

        RestartEnabled = true;
    }

    private void UpdateUI()
    {
        scoreText.text = "Score: " + score;
    }

    private void Restart()
    {
        RestartEnabled = false;
        scoreText.text = "Score: " + 0;

        if (score > highscore)
        {
            highscore = score;
            highscoreText.text = "High Score: " + highscore;
        }

        LevelManager.Instance.Restart();
    }
}