using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StructureHelper : MonoBehaviour
{
    public HouseType[] houseTypes;
    public GameObject[] naturePrefab;
    public bool randomNaturePlacement = false;
    [Range(0, 1)]
    public float randomNaturePlacementThreshold = 0.25f;
    public Dictionary<Vector3Int, GameObject> structureDictionary = new Dictionary<Vector3Int, GameObject>();
    public Dictionary<Vector3Int, GameObject> natureDictionary = new Dictionary<Vector3Int, GameObject>();

    public void PlaceStructuresAroundRoad(List<Vector3Int> roadPositions)
    {
        Dictionary<Vector3Int, Direction> freeEstateSpots = FindFreeSpacesAroundRoad(roadPositions);
        List<Vector3Int> blockedPositions = new List<Vector3Int>();

        foreach (var freeSpot in freeEstateSpots)
        {
            if (blockedPositions.Contains(freeSpot.Key))
            {
                continue;
            }
            
            var rotation = Quaternion.identity;
            switch (freeSpot.Value)
            {
                case Direction.Up:
                   rotation = Quaternion.Euler(0, 90, 0);
                    break;
                case Direction.Down:
                    rotation = Quaternion.Euler(0, -90, 0);
                    break;
                case Direction.Right:
                    rotation = Quaternion.Euler(0, 180, 0);
                    break;
                default:
                    break;
            }

            for (int i = 0; i < houseTypes.Length; i++)
            {
                if (houseTypes[i].quantity == -1)
                {
                    if (randomNaturePlacement)
                    {
                        var random = UnityEngine.Random.value;
                        if(random < randomNaturePlacementThreshold)
                        {
                            var nature = SpawnPrefab(naturePrefab[UnityEngine.Random.Range(0, naturePrefab.Length)], freeSpot.Key, rotation);
                            natureDictionary.Add(freeSpot.Key, nature);
                            break;
                        }
                    }
                    var building = SpawnPrefab(houseTypes[i].GetPrefab(), freeSpot.Key, rotation);
                    structureDictionary.Add(freeSpot.Key, building);
                    break;
                }
                if (houseTypes[i].IsBuildingAvailable())
                {
                    if (houseTypes[i].sizeRequired > 1)
                    {
                        var halfSize = Mathf.FloorToInt(houseTypes[i].sizeRequired / 2.0f);
                        List<Vector3Int> tempPositionsBlocked = new List<Vector3Int>();
                        if (VerifyIfBuildingFits(halfSize, freeEstateSpots, freeSpot, blockedPositions, ref tempPositionsBlocked))
                        {
                            blockedPositions.AddRange(tempPositionsBlocked);
                            var building = SpawnPrefab(houseTypes[i].GetPrefab(), freeSpot.Key, rotation);
                            structureDictionary.Add(freeSpot.Key, building);
                            foreach (var pos in tempPositionsBlocked)
                            {
                                structureDictionary.Add(pos, building);
                            }
                            break;
                        }


                    } else
                    {
                        var building = SpawnPrefab(houseTypes[i].GetPrefab(), freeSpot.Key, rotation);
                        structureDictionary.Add(freeSpot.Key, building);
                    }
                    break;
                }
            }

            
        }
    }

    private bool VerifyIfBuildingFits(int halfSize, Dictionary<Vector3Int, Direction> freeEstateSpots, KeyValuePair<Vector3Int, Direction> freeSpot, List<Vector3Int> blockedPositions, ref List<Vector3Int> tempPositionsBlocked)
    {
        Vector3Int direction = Vector3Int.zero;
        if (freeSpot.Value == Direction.Down || freeSpot.Value == Direction.Up)
        {
            direction = Vector3Int.right;
        } else
        {
            direction = new Vector3Int(0, 0, 1);
        }
        for (int i = 1; i <= halfSize; i++)
        {
            var pos1 = freeSpot.Key + direction * i;
            var pos2 = freeSpot.Key - direction * i;

            if(!freeEstateSpots.ContainsKey(pos1) || !freeEstateSpots.ContainsKey(pos2)
                || blockedPositions.Contains(pos1) || blockedPositions.Contains(pos2))
            {
                return false;
            }
            tempPositionsBlocked.Add(pos1);
            tempPositionsBlocked.Add(pos2);
        }
        return true;
    }

    private GameObject SpawnPrefab(GameObject prefab, Vector3Int position, Quaternion rotation)
    {
        var newStructure = Instantiate(prefab, position, rotation, transform);
        return newStructure;
    }

    private Dictionary<Vector3Int, Direction> FindFreeSpacesAroundRoad(List<Vector3Int> roadPositions)
    {
        Dictionary<Vector3Int, Direction> freeSpaces = new Dictionary<Vector3Int, Direction>();
        foreach (var position in roadPositions)
        {
            var neighbourDirections = PlacementHelper.FindNeighbour(position, roadPositions);
            foreach (Direction direction in Enum.GetValues(typeof(Direction))){
                if (neighbourDirections.Contains(direction) == false)
                {
                    var newPosition = position + PlacementHelper.GetOffsetFromDirection(direction);
                    if(freeSpaces.ContainsKey(newPosition))
                    {
                        continue;
                    }
                    freeSpaces.Add(newPosition, PlacementHelper.GetReserveDirection(direction));
                }
            }
        }
        return freeSpaces;
    }
}
