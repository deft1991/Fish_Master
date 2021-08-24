using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishSpawner : MonoBehaviour
{
    [SerializeField] private Fish fishPrefab;
    [SerializeField] private Fish.FishType[] fishTypes;
    
    // Start is called before the first frame update
    void Awake()
    {
        for (int i = 0; i < fishTypes.Length; i++) // create all fish types
        {
            int num = 0;
            while (num < fishTypes[i].fishCount) // create needed count of this fish types
            {
                Fish fish = UnityEngine.Object.Instantiate<Fish>(fishPrefab);
                fish.Type = fishTypes[i];
                fish.ResetFish();
                num++;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
