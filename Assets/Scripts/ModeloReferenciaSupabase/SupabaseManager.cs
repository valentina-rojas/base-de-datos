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

    [SerializeField] TMP_InputField _userIDInput;
    [SerializeField] TMP_InputField _userPassInput;
    [SerializeField] TextMeshProUGUI _stateText;

    [SerializeField] GameObject successPanel;
    [SerializeField] Button playButton;

    string supabaseUrl = "https://qyewiiivujjprrkornqr.supabase.co";
    string supabaseKey = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6InF5ZXdpaWl2dWpqcHJya29ybnFyIiwicm9sZSI6ImFub24iLCJpYXQiOjE3MTkwODkwODksImV4cCI6MjAzNDY2NTA4OX0.w28iWvwPbRAcDA7KoNsl4qISpwg3JJSBS71OxdlxNq8";

    Supabase.Client clientSupabase;

    private usuarios _usuarios = new usuarios();

    public static string CurrentUsername { get; private set; } // para guardar el nombre del usuario actual
    public static int CurrentUserId { get; private set; } // para guardar el id del usuario actual

    private void Start()
    {
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
             .Select("id, password") // selecciona el ID y la contraseña
            .Where(usuarios => usuarios.username == _userIDInput.text)
            .Get();

        // verifica si login_password tiene resultados
        if (login_password.Models.Count > 0)
        {
            var usuario = login_password.Models[0];
            if (usuario.password.Equals(_userPassInput.text))
            {
                print("LOGIN SUCCESSFUL");
                _stateText.text = "LOGIN SUCCESSFUL";
                _stateText.color = Color.green;
                SupabaseManager.CurrentUsername = _userIDInput.text; // guarda el nombre de usuario actual

                SupabaseManager.CurrentUserId = usuario.id;  // guardar el id del usuario actual

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

    // verifica si el nombre de usuario ya existe
    var usuarioExistente = await clientSupabase
        .From<usuarios>()
        .Select("username")
        .Where(usuarios => usuarios.username == _userIDInput.text)
        .Get();

    if (usuarioExistente.Models.Count > 0)
    {
        _stateText.text = "USERNAME ALREADY EXISTS";
        _stateText.color = Color.red;
        return;
    }

    // consultar el último id utilizado (ID = index)
    var ultimoId = await clientSupabase
        .From<usuarios>()
        .Select("id")
        .Order(usuarios => usuarios.id, Postgrest.Constants.Ordering.Descending) // Ordenar en orden descendente para obtener el último id
        .Get();

    int nuevoId = 1; // Valor predeterminado si la tabla está vacía

    if (ultimoId.Models.Count > 0)
    {
        nuevoId = ultimoId.Models[0].id + 1; // incrementar el último id
    }

    // crear el nuevo usuario con el nuevo id
    var nuevoUsuario = new usuarios
    {
        id = nuevoId,
        username = _userIDInput.text,
        age = Random.Range(0, 100), // luego creo el campo que falta en la UI
        password = _userPassInput.text,
    };

    // inserta el nuevo usuario
    var resultado = await clientSupabase
        .From<usuarios>()
        .Insert(new[] { nuevoUsuario });

    // verifico el estado de la inserción 
    if (resultado.ResponseMessage.IsSuccessStatusCode)
    {
        _stateText.text = "SIGN UP SUCCESSFUL";
        _stateText.color = Color.green;
        SupabaseManager.CurrentUsername = _userIDInput.text; // guarda el nombre de usuario actual
        SupabaseManager.CurrentUserId = nuevoId;  // guarda el id del usuario actual
        ShowSuccessPanel();
    }
    else
    {
        _stateText.text = "ERROR TRY AGAIN";
        _stateText.text += "\n" + resultado.ResponseMessage.ToString();
        _stateText.color = Color.red;
    }
}



    private void ShowSuccessPanel()
    {
        successPanel.SetActive(true); // muestra el panel de éxito
    }

   public void OnPlayButtonClick()
    {
        SceneManager.LoadScene("TriviaSelectScene"); 
    }

  public void ChangeScene(string name)
    {
        SceneManager.LoadScene(name);
    }

}

