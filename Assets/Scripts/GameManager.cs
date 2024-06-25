using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    //public TriviaManager triviaManager;


    public List<question> responseList; //lista donde guardo la respuesta de la query hecha en la pantalla de selecci�n de categor�a
    private List<int> usedQuestionIndices = new List<int>();  // Lista para mantener los índices de preguntas utilizadas

    public int currentTriviaIndex = 0;
    public int randomQuestionIndex = 0;

    public List<string> _answers = new List<string>();
    public bool queryCalled;

    public int _points;
    private static int _totalPoints = 0;
    //private int _maxAttempts = 10;
    public int _numQuestionAnswered = 0;

    public float initialTimerValue = 10f;
    public float timer;
    public float answerTime;
    private float _timeLeft;
    private bool _isAnswered;

    string _correctAnswer;

    public static GameManager Instance { get; private set; }


    // Variable para almacenar la cantidad total de preguntas
    public int TotalQuestions
    {
        get { return responseList != null ? responseList.Count : 0; }
    }

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


    void Start()
    {
        StartTrivia();

        queryCalled = false;
        timer = initialTimerValue;
        _points = 0; 
        _isAnswered = false;
    }

    void StartTrivia()
    {
        // Cargar la trivia desde la base de datos
        //triviaManager.LoadTrivia(currentTriviaIndex);
        //print(responseList.Count);
    }

  public void AddPoints(int pointsToAdd)
    {
        _totalPoints += pointsToAdd; // sumar puntos al total
    }

    public int GetTotalPoints()
    {
        return _totalPoints; // obtener los puntos totales acumulados
    }

    public void CategoryAndQuestionQuery(bool isCalled)
    {
        isCalled = UIManagment.Instance.queryCalled;

        if (!isCalled)
        {

           //si se han mostrado todas las preguntas, reiniciar el registro de índices
            if (usedQuestionIndices.Count >= responseList.Count)
            {
                usedQuestionIndices.Clear();
            }

            //obtener un índice aleatorio que no se haya utilizado
            do
            {
                randomQuestionIndex = Random.Range(0, responseList.Count);
            } while (usedQuestionIndices.Contains(randomQuestionIndex));

            //agregar el índice a la lista de utilizados
            usedQuestionIndices.Add(randomQuestionIndex);
 
            //_questionText.text = GameManager.Instance.responseList[randomQuestionIndex].QuestionText;
            _correctAnswer = GameManager.Instance.responseList[randomQuestionIndex].CorrectOption;


            //agrego a la lista de answers las 3 answers
            _answers.Add(GameManager.Instance.responseList[randomQuestionIndex].Answer1);
            _answers.Add(GameManager.Instance.responseList[randomQuestionIndex].Answer2);
            _answers.Add(GameManager.Instance.responseList[randomQuestionIndex].Answer3);

            // la mixeo con el m�todo Shuffle (ver script Shuffle List)
            _answers.Shuffle();

            // asigno estos elementos a los textos de los botones
            for (int i = 0; i < UIManagment.Instance._buttons.Length; i++)
            {
                UIManagment.Instance._buttons[i].GetComponentInChildren<TextMeshProUGUI>().text = _answers[i];

                int index = i; // Captura el valor actual de i en una variable local -- SINO NO FUNCA!

                UIManagment.Instance._buttons[i].onClick.AddListener(() => UIManagment.Instance.OnButtonClick(index));
            }


            UIManagment.Instance.queryCalled = true;


      // ajustar variable para la proxima pregunta
            timer = initialTimerValue;
            UIManagment.Instance._timerActive = true;
            _points = 0;
            answerTime = 0;
            _numQuestionAnswered++;

    //chequear si ya se mostraron todas las preguntas disponibles
           if (_numQuestionAnswered >=  TotalQuestions +1)
            {
                Debug.Log("¡Has respondido todas las preguntas!");
                EndGame();
            }
        }

    }


 public void EndGame()
    {
        Debug.Log("Juego terminado. Puntos totales: " + _totalPoints);

        // Reiniciar variables si es necesario para un nuevo juego
        _numQuestionAnswered = 0;
        _totalPoints = 0;

        // Aquí puedes manejar la transición a la pantalla de resultados o a otra escena
        // Por ejemplo:
       UIManagment.Instance.PreviousScene();
    }
}

