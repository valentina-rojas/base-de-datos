using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    //public TriviaManager triviaManager;

    public List<question> responseList; //lista donde guardo la respuesta de la query hecha en la pantalla de selección de categoría
    private List<int> usedQuestionIndices = new List<int>();  // lista para mantener los índices de preguntas utilizadas

    public int currentTriviaIndex = 0;
    public int randomQuestionIndex = 0;

    public List<string> _answers = new List<string>();
    public bool queryCalled;

    public int _points;
    private static int _totalPoints = 0;
    public int _numQuestionAnswered = 0;
    public int respuestasCorrectas;
    public int respuestasIncorrectas;


    public float initialTimerValue = 10f;
    public float timer;
    public float answerTime;
    private float _timeLeft;
    public float tiempoJuego;

    string _correctAnswer;

    Supabase.Client clientSupabase;

    public static GameManager Instance { get; private set; }

    string supabaseUrl = "https://qyewiiivujjprrkornqr.supabase.co";
    string supabaseKey = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6InF5ZXdpaWl2dWpqcHJya29ybnFyIiwicm9sZSI6ImFub24iLCJpYXQiOjE3MTkwODkwODksImV4cCI6MjAzNDY2NTA4OX0.w28iWvwPbRAcDA7KoNsl4qISpwg3JJSBS71OxdlxNq8";

    // Variable para almacenar la cantidad total de preguntas
    public int TotalQuestions
    {
        get { return responseList != null ? responseList.Count : 0; }
    }


    // Awake para inicialización
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        // Inicialización de clientSupabase
        clientSupabase = new Supabase.Client(supabaseUrl, supabaseKey);
    }


    void Start()
    {
        StartTrivia();

        queryCalled = false;
        timer = initialTimerValue;
        _points = 0;
        respuestasCorrectas = 0;
        respuestasIncorrectas = 0;
        tiempoJuego = 0f;
    }

    void StartTrivia()
    {

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
        
             _answers.Clear();

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

            // la mixeo con el método Shuffle (ver script Shuffle List)
            _answers.Shuffle();

            // asigno estos elementos a los textos de los botones
            for (int i = 0; i < UIManagment.Instance._buttons.Length; i++)
            {
                UIManagment.Instance._buttons[i].GetComponentInChildren<TextMeshProUGUI>().text = _answers[i];

                int index = i; // Captura el valor actual de i en una variable local -- SINO NO FUNCA!

                UIManagment.Instance._buttons[i].onClick.AddListener(() => UIManagment.Instance.OnButtonClick(index));
            }

            UIManagment.Instance.queryCalled = true;

            // reinicia variables para la próxima pregunta
            timer = initialTimerValue;
            UIManagment.Instance._timerActive = true;
            _points = 0;
            answerTime = 0;
            _numQuestionAnswered++;

            //chequear si ya se mostraron todas las preguntas disponibles
            if (_numQuestionAnswered >= TotalQuestions + 1)
            {
                Debug.Log("¡Has respondido todas las preguntas!");
                EndGame();
            }
        }
    }

    public void EndGame()
    {
        Debug.Log("Juego terminado. Puntos totales: " + _totalPoints);
         
         // Convertir tiempoJuego a entero
        int tiempoJuegoFinal = Mathf.RoundToInt(tiempoJuego);

        Debug.Log("Tiempo total de juego: " + tiempoJuegoFinal + " segundos.");

        // obtener el id del usuario actualmente logueado desde SupabaseManager
        int userId = SupabaseManager.CurrentUserId;

        // obtener el id de la categoría de trivia actual 
        int categoryId = TriviaSelection.SelectedTriviaId;

        // obtener el puntaje final
        int puntajeFinal = _totalPoints;

        int correctas = respuestasCorrectas;

        int incorrectas = respuestasIncorrectas;

         // llama al método para guardar el intento en Supabase
        GuardarIntentoEnSupabase(userId, categoryId, puntajeFinal, tiempoJuegoFinal, correctas, incorrectas);

        UIManagment.Instance.ResultsScene();
    }

    public void ResetGame()
    {
        _numQuestionAnswered = 0;
        _totalPoints = 0;
    }


     public async void GuardarIntentoEnSupabase(int userId, int categoryId, int puntajeFinal, int tiempoJuegoFinal, int correctas, int incorrectas)
    {
        // consulta el último id utilizado (ID = index)
        var ultimoId = await clientSupabase
            .From<intentos>()
            .Select("id")
            .Order(intentos => intentos.id, Postgrest.Constants.Ordering.Descending) // ordena en orden descendente para obtener el último id
            .Get();

        int nuevoId = 1; // valor predeterminado si la tabla está vacía

        if (ultimoId.Models.Count > 0)
        {
            nuevoId = ultimoId.Models[0].id + 1; // incrementar el último id
        }

        // crear un nuevo intento con los datos obtenidos
        var nuevoIntento = new intentos
        {
            id = nuevoId,
            id_usuario = userId,
            id_category = categoryId,
            puntaje = puntajeFinal,
            tiempoJuego = tiempoJuegoFinal,
            respCorrectas = correctas,
            respIncorrectas = incorrectas
        };

        // insertar el nuevo intento en Supabase
        var resultado = await clientSupabase
            .From<intentos>()
            .Insert(new[] { nuevoIntento });

        if (resultado.ResponseMessage.IsSuccessStatusCode)
        {
            Debug.Log("Intento guardado correctamente en Supabase.");
        }
        else
        {
            Debug.LogError("Error al guardar el intento en Supabase: " + resultado.ResponseMessage);
        }
    }
}


