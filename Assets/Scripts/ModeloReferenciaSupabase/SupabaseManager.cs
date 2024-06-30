using UnityEngine;
using Supabase;
using Supabase.Interfaces;
using System.Threading;
using Postgrest.Models;
using TMPro;
using UnityEngine.UI;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;

public class SupabaseManager : MonoBehaviour
{
    [Header("Campos de Interfaz")]
    [SerializeField] TMP_InputField _userIDInput;
    [SerializeField] TMP_InputField _userPassInput;
    [SerializeField] TextMeshProUGUI _stateText;

    [Header("Panel de Éxito")]
    [SerializeField] GameObject successPanel;
    [SerializeField] Button playButton;

    string supabaseUrl = "https://qyewiiivujjprrkornqr.supabase.co";
    string supabaseKey = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6InF5ZXdpaWl2dWpqcHJya29ybnFyIiwicm9sZSI6ImFub24iLCJpYXQiOjE3MTkwODkwODksImV4cCI6MjAzNDY2NTA4OX0.w28iWvwPbRAcDA7KoNsl4qISpwg3JJSBS71OxdlxNq8";

    Supabase.Client clientSupabase;

    private usuarios _usuarios = new usuarios();

    public static string CurrentUsername { get; private set; }
    public static int CurrentUserId { get; private set; }

    private void Start()
    {
        // Initialize the Supabase client once at the start
        clientSupabase = new Supabase.Client(supabaseUrl, supabaseKey);
        successPanel.SetActive(false);
    }

    public async void UserLogin()
    {
        if (clientSupabase == null)
        {
            Debug.LogError("Supabase client is not initialized.");
            return;
        }

        // prueba
        var test_response = await clientSupabase
            .From<usuarios>()
            .Select("*")
            .Get();
        Debug.Log(test_response.Content);

        // filtro según datos de login
        var login_password = await clientSupabase
            .From<usuarios>()
             .Select("id, password") // Seleccionar el ID y la contraseña
            .Where(usuarios => usuarios.username == _userIDInput.text)
            .Get();

        // Verificar si login_password tiene resultados
        if (login_password.Models.Count > 0)
        {
            var usuario = login_password.Models[0];
            if (usuario.password.Equals(_userPassInput.text))
            {
                print("LOGIN SUCCESSFUL");
                _stateText.text = "LOGIN SUCCESSFUL";
                _stateText.color = Color.green;
                SupabaseManager.CurrentUsername = _userIDInput.text; // Guardar el nombre de usuario actual

                SupabaseManager.CurrentUserId = usuario.id;  // Guardar el id del usuario actual

                ShowSuccessPanel();
            }
            else
            {
                print("WRONG PASSWORD");
                _stateText.text = "WRONG PASSWORD";
                _stateText.color = Color.red;
            }
        }
        else
        {
            print("USER NOT FOUND");
            _stateText.text = "USER NOT FOUND";
            _stateText.color = Color.red;
        }
    }

    public async void InsertarNuevoUsuario()
{
    if (clientSupabase == null)
    {
        Debug.LogError("Supabase client is not initialized.");
        return;
    }

    // Verificar si el nombre de usuario ya existe
    var usuarioExistente = await clientSupabase
        .From<usuarios>()
        .Select("username")
        .Where(usuarios => usuarios.username == _userIDInput.text)
        .Get();

    if (usuarioExistente.Models.Count > 0)
    {
        _stateText.text = "Nombre de usuario ya existe";
        _stateText.color = Color.red;
        return;
    }

    // Consultar el último id utilizado (ID = index)
    var ultimoId = await clientSupabase
        .From<usuarios>()
        .Select("id")
        .Order(usuarios => usuarios.id, Postgrest.Constants.Ordering.Descending) // Ordenar en orden descendente para obtener el último id
        .Get();

    int nuevoId = 1; // Valor predeterminado si la tabla está vacía

    if (ultimoId.Models.Count > 0)
    {
        nuevoId = ultimoId.Models[0].id + 1; // Incrementar el último id
    }

    // Crear el nuevo usuario con el nuevo id
    var nuevoUsuario = new usuarios
    {
        id = nuevoId,
        username = _userIDInput.text,
        age = Random.Range(0, 100), // luego creo el campo que falta en la UI
        password = _userPassInput.text,
    };

    // Insertar el nuevo usuario
    var resultado = await clientSupabase
        .From<usuarios>()
        .Insert(new[] { nuevoUsuario });

    // Verifico el estado de la inserción 
    if (resultado.ResponseMessage.IsSuccessStatusCode)
    {
        _stateText.text = "Usuario Correctamente Ingresado";
        _stateText.color = Color.green;
        SupabaseManager.CurrentUsername = _userIDInput.text; // Guardar el nombre de usuario actual
        SupabaseManager.CurrentUserId = nuevoId;  // Guardar el id del usuario actual
        ShowSuccessPanel();
    }
    else
    {
        _stateText.text = "Error en el registro de usuario";
        _stateText.text += "\n" + resultado.ResponseMessage.ToString();
        _stateText.color = Color.red;
    }
}



    private void ShowSuccessPanel()
    {
        successPanel.SetActive(true); // Mostrar el panel de éxito
    }

   public void OnPlayButtonClick()
    {
        // Aquí puedes cargar la escena del juego o realizar cualquier acción que desees
        SceneManager.LoadScene("TriviaSelectScene"); // Asegúrate de tener una escena llamada "GameScene"
    }

  public void ChangeScene(string name)
    {
        SceneManager.LoadScene(name);
    }

}

