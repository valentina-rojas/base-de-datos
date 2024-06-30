using UnityEngine;
using Supabase;
using System.Threading.Tasks;
using Supabase.Interfaces;
using Postgrest.Models;
using TMPro;
using System.Collections.Generic;
using System.Linq;

public class RankingManager : MonoBehaviour
{
    string supabaseUrl = "https://qyewiiivujjprrkornqr.supabase.co";
    string supabaseKey = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6InF5ZXdpaWl2dWpqcHJya29ybnFyIiwicm9sZSI6ImFub24iLCJpYXQiOjE3MTkwODkwODksImV4cCI6MjAzNDY2NTA4OX0.w28iWvwPbRAcDA7KoNsl4qISpwg3JJSBS71OxdlxNq8";
    public Supabase.Client clientSupabase;

    List<trivia> trivias = new List<trivia>();
    List<intentos> intentosList = new List<intentos>();
    List<usuarios> usuariosList = new List<usuarios>();
    [SerializeField] TMP_Dropdown _dropdown;
    [SerializeField] TMP_Text generalRankingText; // Referencia al objeto TextMeshPro para mostrar ranking general
    [SerializeField] TMP_Text categoryRankingText; // Referencia al objeto TextMeshPro para mostrar ranking por categoría

    public static int SelectedTriviaId { get; private set; }
    public static RankingManager Instance { get; private set; }

    public DatabaseManager databaseManager;

    async void Start()
    {
        Instance = this;
        clientSupabase = new Supabase.Client(supabaseUrl, supabaseKey);

        await SelectTrivias();
        PopulateDropdown();

        await LoadIntentosData();
        await LoadUsuariosData();

        // Suscribirse al evento OnValueChanged del Dropdown
        _dropdown.onValueChanged.AddListener(OnCategoryDropdownValueChanged);

        // Mostrar el ranking general al inicio
        ShowGeneralRanking();
    }

    async Task SelectTrivias()
    {
        var response = await clientSupabase
            .From<trivia>()
            .Select("*")
            .Get();

        if (response != null)
        {
            trivias = response.Models;
        }
    }

    async Task LoadIntentosData()
    {
        var response = await clientSupabase
            .From<intentos>()
            .Select("*")
            .Get();

        if (response != null)
        {
            intentosList = response.Models.ToList();
        }
    }

    async Task LoadUsuariosData()
    {
        var response = await clientSupabase
            .From<usuarios>()
            .Select("*")
            .Get();

        if (response != null)
        {
            usuariosList = response.Models.ToList();
        }
    }

    void PopulateDropdown()
    {
        _dropdown.ClearOptions();

        List<string> categories = new List<string>();

        foreach (var trivia in trivias)
        {
            categories.Add(trivia.category);
        }

        _dropdown.AddOptions(categories);
    }

    void ShowGeneralRanking()
    {
        var sortedUsers = intentosList.OrderByDescending(x => x.puntaje);

        // Construir el texto para mostrar en el ranking general
        string generalRankingText = "";
        foreach (var intento in sortedUsers)
        {
            var user = usuariosList.FirstOrDefault(u => u.id == intento.id_usuario);
            if (user != null)
            {
                generalRankingText += $"Usuario: {user.username}, Puntaje: {intento.puntaje}\n";
            }
        }

        // Mostrar el texto en el UI
        this.generalRankingText.text = generalRankingText;
    }

    void ShowCategoryRanking(string category)
    {
        // Obtener la categoría seleccionada del Dropdown
        var selectedCategory = trivias.FirstOrDefault(t => t.category == category);

        if (selectedCategory != null)
        {
            // Filtrar los intentos por la categoría seleccionada y ordenar por puntaje descendente
            var categoryUsers = intentosList.Where(x => x.id_category == selectedCategory.id).OrderByDescending(x => x.puntaje);

            // Construir el texto para mostrar en el ranking por categoría
            string categoryRankingText = "";
            foreach (var intento in categoryUsers)
            {
                var user = usuariosList.FirstOrDefault(u => u.id == intento.id_usuario);
                if (user != null)
                {
                    categoryRankingText += $"Usuario: {user.username}, Puntaje: {intento.puntaje} \n";
                }
            }

            // Mostrar el texto en el UI
            this.categoryRankingText.text = categoryRankingText;
        }
    }

    // Método llamado cuando se cambia la selección en el Dropdown
    void OnCategoryDropdownValueChanged(int index)
    {
        string selectedCategory = _dropdown.options[index].text;
        ShowCategoryRanking(selectedCategory);
    }
}
