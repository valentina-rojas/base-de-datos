using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartScene : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

      public void OnPlayButtonStart()
    {
        // Aquí puedes cargar la escena del juego o realizar cualquier acción que desees
        SceneManager.LoadScene("LoginScene"); // Asegúrate de tener una escena llamada "GameScene"
    }

      public void OnPlayButtonRanking()
    {
        // Aquí puedes cargar la escena del juego o realizar cualquier acción que desees
        SceneManager.LoadScene("RankingScene"); // Asegúrate de tener una escena llamada "GameScene"
    }
}