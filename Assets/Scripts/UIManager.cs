using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [Header("Game Over Screen")]
    [SerializeField] GameObject gameOverPanel;

    void Start()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
        else
        {
            return;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (gameOverPanel == null || GameManager.Instance == null)
        {
            return;
        }

        if (GameManager.Instance.isGameOver && gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }
    }

    public void Play()
    {
        SceneManager.LoadScene("Main");
    }

    public void Quit()
    {
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
#else
        Application.Quit();
#endif
    }
    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene("Menu");
    }
}
