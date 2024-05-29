using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class ExitButon : MonoBehaviour
{
    public void ReturnToTheMenu()
    {
        SceneManager.LoadScene(0);
    }
}
