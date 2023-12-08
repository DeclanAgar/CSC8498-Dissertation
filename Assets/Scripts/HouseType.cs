using System;
using UnityEngine;

[Serializable]
public class HouseType
{
        [SerializeField]
        private GameObject[] prefabs;
        public int sizeRequired;
        public int quantity;
        public int quantityPlaced;

        public GameObject GetPrefab()
        {
            quantityPlaced++;
            if (prefabs.Length > 1)
            {
                var random = UnityEngine.Random.Range(0, prefabs.Length);
                return prefabs[random];
            }
            return prefabs[0];
        }

    public bool IsBuildingAvailable()
    {
        return quantityPlaced < quantity;
    }

    public void Reset()
    {
        quantityPlaced = 0;
    }
}
