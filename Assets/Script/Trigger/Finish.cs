using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Finish : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
            if (other.gameObject.CompareTag("Player"))
            {
                SceneManager.LoadScene(2);
            }
    }
}
