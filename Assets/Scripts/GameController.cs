using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] private GameObject enemy;
    private void Update()
    {
        if (Input.GetKeyDown("space"))
            Spawn();
    }

    private void Spawn()
    { 
        Instantiate(enemy, new Vector2(0, -4), Quaternion.identity); 
    }
}
