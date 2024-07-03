using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManagment : MonoBehaviour
{


    [SerializeField] TextMeshProUGUI _categoryText;
    [SerializeField] TextMeshProUGUI _questionText;
    public TextMeshProUGUI _timerText;
    public TextMeshProUGUI _pointsText;

    [SerializeField] GameObject _resultsPanel; // El panel de resultados
    public TextMeshProUGUI _scoreText;

    string _correctAnswer;

    public Button[] _buttons = new Button[3];

    [SerializeField] Button _backButton;
    [SerializeField] Button _nextButton;
    [SerializeField] Button _backMenuButton;

    private List<string> _answers = new List<string>();

    public bool queryCalled;
    public bool _timerActive;

    private Color _originalButtonColor;

    public static UIManagment Instance { get; private set; }


    void Awake()
    {
        // Configura la instancia
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Para mantener el objeto entre escenas
        }
        else
        {
            Destroy(gameObject);
        }

    }


    private void Start()
    {
        queryCalled = false;
        _timerActive = true;
        _originalButtonColor = _buttons[0].GetComponent<Image>().color;
        _nextButton.gameObject.SetActive(false); // Ocultar el botón "Siguiente" inicialmente

        int finalScore = GameManager.Instance.GetTotalPoints();
        _pointsText.text = "Puntos: " + GameManager.Instance.GetTotalPoints();
        _resultsPanel.SetActive(false);

    }

    void Update()
    {
        _categoryText.text = PlayerPrefs.GetString("SelectedTrivia");
        _questionText.text = GameManager.Instance.responseList[GameManager.Instance.randomQuestionIndex].QuestionText;

        updateTimer();

        GameManager.Instance.CategoryAndQuestionQuery(queryCalled);

    }


    public void updateTimer()
    {
        if (_timerActive)
        {
            GameManager.Instance.timer -= Time.deltaTime;
            if (GameManager.Instance.timer <= 0)
            {
                GameManager.Instance.timer = 0;
                OnTimeUp(); // Tiempo agotado, manejar la respuesta incorrecta
            }
           
           // _timerText.text = GameManager.Instance.timer.ToString("f0"); // Update timer text
_timerText.text = "Tiempo: " + GameManager.Instance.timer.ToString("f0");
        }
    }

    private void OnTimeUp()
    {
        Debug.Log("Tiempo agotado. Respuesta incorrecta.");
        _timerActive = false; // Detener el temporizador
        _nextButton.gameObject.SetActive(true); // Mostrar el botón "Siguiente"
        ChangeButtonColorToIncorrect();
        Invoke("RestoreButtonColor", 2f);
    }

    private void ChangeButtonColorToIncorrect()
    {
        foreach (Button button in _buttons)
        {
            Image buttonImage = button.GetComponent<Image>();
            buttonImage.color = Color.red;
        }
    }
    public void OnButtonClick(int buttonIndex)
    {

        string selectedAnswer = _buttons[buttonIndex].GetComponentInChildren<TextMeshProUGUI>().text;

        _correctAnswer = GameManager.Instance.responseList[GameManager.Instance.randomQuestionIndex].CorrectOption;

        _nextButton.gameObject.SetActive(true); // Mostrar el botón "Siguiente"


        // Deshabilitar todos los botones para evitar más interacción
        foreach (Button button in _buttons)
        {
            button.interactable = false;
        }

        if (selectedAnswer == _correctAnswer)
        {
            Debug.Log("�Respuesta correcta!");

            RespuestaCorrecta();

            ChangeButtonColor(buttonIndex, Color.green);
            // Invoke("RestoreButtonColor", 2f);
            GameManager.Instance._answers.Clear();
            // Invoke("NextAnswer", 2f);

        }
        else
        {
            Debug.Log("Respuesta incorrecta. Int�ntalo de nuevo.");
 _timerActive = false; // Detener el temporizador
            ChangeButtonColor(buttonIndex, Color.red);
            // Invoke("RestoreButtonColor", 2f);
        }
    }


    public void OnNextButtonClick()
    {
        RestoreButtonColor();
        _nextButton.gameObject.SetActive(false); // Ocultar el botón "Siguiente"
        //GameManager.Instance.CategoryAndQuestionQuery(false); // Cargar la siguiente pregunta
        Invoke("NextAnswer", 0f);
        foreach (Button button in _buttons)
        {
            button.interactable = true;
        }

    }


    private void ChangeButtonColor(int buttonIndex, Color color)
    {
        Image buttonImage = _buttons[buttonIndex].GetComponent<Image>();
        buttonImage.color = color;
    }

    private void RestoreButtonColor()
    {
        foreach (Button button in _buttons)
        {
            Image buttonImage = button.GetComponent<Image>();
            buttonImage.color = _originalButtonColor;
        }
    }

    private void NextAnswer()
    {
        queryCalled = false;
    }


    public void PreviousScene()
    {
        GameManager.Instance.ResetGame();

        Destroy(GameManager.Instance);
        Destroy(UIManagment.Instance);

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }


    public void ResultsScene()
    {
        // Muestra el panel de resultados
        _resultsPanel.SetActive(true);

        // Muestra el puntaje final
        _scoreText.text = "Puntaje final: " + GameManager.Instance.GetTotalPoints();

        // Guardar el intento en Supabase
        string username = SupabaseManager.CurrentUsername;
        int puntaje = GameManager.Instance.GetTotalPoints();

        if (!string.IsNullOrEmpty(username))
        {
        //GuardarIntentoEnSupabase(username, puntaje);
        }
        else
        {
            Debug.LogError("No se puede guardar el intento en Supabase: Nombre de usuario no válido.");
        }
    }


    public void RespuestaCorrecta()
    {
        // Detener el temporizador si está activo
        if (_timerActive)
        {
            GameManager.Instance.answerTime = Mathf.RoundToInt(10 - GameManager.Instance.timer); // Almacenar el tiempo de respuesta
            _timerActive = false; // Detener el temporizador

            // Calcular los puntos para esta respuesta
            GameManager.Instance._points = Mathf.RoundToInt(10 - GameManager.Instance.answerTime);

            Debug.Log("¡Respuesta correcta! Puntaje obtenido: " + GameManager.Instance._points);

            // Agregar puntos al total
            GameManager.Instance.AddPoints(GameManager.Instance._points);

            // Actualizar el texto de puntos en la interfaz de usuario
            _pointsText.text = "Puntos: " + GameManager.Instance.GetTotalPoints();
        }
        else
        {
            Debug.LogWarning("RespuestaCorrecta() llamada cuando el temporizador no está activo.");
        }
    }
}
