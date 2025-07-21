using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuInicio : MonoBehaviour
{

    public void ModoAtacante()
    {
        Debug.Log("Atacando ....");
    }

    public void ModoDefensor()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);

    }

    public void salir()
    {
        Debug.Log("Saliendo ....");
        Application.Quit();
    }

}
