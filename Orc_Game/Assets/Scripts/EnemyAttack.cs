using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemyAttack : MonoBehaviour
{
    EnemyAI_Grunt gruntAI;

    void Start()
    {
        gruntAI = transform.parent.GetComponent<EnemyAI_Grunt>();
    }

    // Hurt player if touches the grunt's weapon
    void OnTriggerEnter2D (Collider2D col) 
    {
        // PlayerCharacter was given tag "Player" in editor
        if (col.CompareTag("Player")) 
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}
