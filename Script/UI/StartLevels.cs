using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class StartLevels : MonoBehaviour
{
    public void SetLevels()
    {
        string nameButten = EventSystem.current.currentSelectedGameObject.name;
        SceneManager.LoadScene(nameButten);
    }
}
