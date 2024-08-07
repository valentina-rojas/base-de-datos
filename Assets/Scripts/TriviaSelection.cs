using UnityEngine;
using Supabase;
using Supabase.Interfaces;
using System.Threading.Tasks;
using System.Collections.Generic;
using Postgrest.Models;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TriviaSelection : MonoBehaviour
{
    string supabaseUrl = "https://qyewiiivujjprrkornqr.supabase.co"; 
    string supabaseKey = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6InF5ZXdpaWl2dWpqcHJya29ybnFyIiwicm9sZSI6ImFub24iLCJpYXQiOjE3MTkwODkwODksImV4cCI6MjAzNDY2NTA4OX0.w28iWvwPbRAcDA7KoNsl4qISpwg3JJSBS71OxdlxNq8"; 
    public Supabase.Client clientSupabase;

    List<trivia> trivias = new List<trivia>();
    [SerializeField] TMP_Dropdown _dropdown;

    public static int SelectedTriviaId { get; private set; } // para almacenar el id de la trivia seleccionada

    public static TriviaSelection Instance { get; private set; } 

    public DatabaseManager databaseManager;

    async void Start()
    {
        Instance = this; 
        clientSupabase = new Supabase.Client(supabaseUrl, supabaseKey);

        await SelectTrivias();
        PopulateDropdown();
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

    public void OnStartButtonClicked()
    {
        int selectedIndex = _dropdown.value;

        SelectedTriviaId = trivias[selectedIndex].id; // obtiene el id de la trivia seleccionada

        string selectedTrivia = _dropdown.options[selectedIndex].text;

        PlayerPrefs.SetInt("SelectedIndex", selectedIndex+1);
        PlayerPrefs.SetString("SelectedTrivia", selectedTrivia);

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

}
