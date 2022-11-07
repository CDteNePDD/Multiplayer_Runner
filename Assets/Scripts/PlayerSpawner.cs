using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MultiplayerRunner
{
    public class PlayerSpawner : MonoBehaviour
    {
        public static PlayerSpawner Instance;

        [SerializeField]
        private float m_SpawnRadius = 2;

        private Transform[] spawnPoints;

        public Vector3 GetRandomPoints()
        {
            int index = Random.Range(0, spawnPoints.Length);

            if(TryFindRunnerInLocation(spawnPoints[index].position))
            {
                index = GetIndexFreeLocation();
            }

            return spawnPoints[index].position;
        }

        private void Awake()
        {
            if(Instance != null)
            {
                Destroy(gameObject);
            }

            Instance = this;

            spawnPoints = GetComponentsInChildren<Transform>();
        }

        private bool TryFindRunnerInLocation(Vector3 location)
        {
            Collider[] colliders = Physics.OverlapSphere(location, m_SpawnRadius);
            if (colliders.Length != 0)
            {
                foreach (var collider in colliders)
                {
                    if (collider is CharacterController)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private int GetIndexFreeLocation()
        {
            for (int i = 0; i < spawnPoints.Length; i++)
            {
                if (TryFindRunnerInLocation(spawnPoints[i].position) == false)
                {
                    return i;
                }
            }

            return Random.Range(0, spawnPoints.Length);
        }
    }
}
