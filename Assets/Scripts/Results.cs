using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class Results : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _scoreText;

    public static Results Instance { get; private set; }

  private void Start()
    {
        if (GameManager.Instance != null)
        {
            DisplayResults();
        }
        else
        {
            Debug.LogError("GameManager.Instance is null in Results.Start().");
        }
    }
    public void DisplayResults()
    {
        int finalScore = GameManager.Instance.GetTotalPoints();
        _scoreText.text = "Puntaje final: " + finalScore.ToString();
    }

    public void OnMenuButtonClicked()
    {
        Debug.Log("RestartGame called");
        GameManager.Instance.ResetGame();
      
    //    SceneManager.LoadScene("TriviaSelectScene"); // Asegúrate de tener la escena principal del juego cargada con el nombre correcto
     SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 2);
    }
}
