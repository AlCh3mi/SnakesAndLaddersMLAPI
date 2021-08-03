using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

public class Board : MonoBehaviour
{
    [SerializeField] private GameObject blockPrefab;
    [SerializeField] private Vector2Int boardDimensions;
    [SerializeField] private Color[] blockColours;

    public Dictionary<int, Block> positions;

    private void Start()
    {
        PopulateBoardPositionList();
    }

    private void PopulateBoardPositionList()
    {
        positions = new Dictionary<int, Block>();
        
        foreach (var block in GetComponentsInChildren<Block>())
        {
            positions.Add(block.Id, block);
        }
    }
    
    [ContextMenu("Clear Board")]
    public void ClearBoard()
    {
        foreach (var block in GetComponentsInChildren<Block>())
        {
            DestroyImmediate(block.gameObject);
        }
    }

#if UNITY_EDITOR
    [ContextMenu("Generate Board")]
    public void GenerateBoard()
    {
        for (int y = 0; y < boardDimensions.y; y++)
        {
            for (int x = 0; x < boardDimensions.x; x++)
            {
                int blockId = (y * boardDimensions.y) + x + 1;
                var prefab = PrefabUtility.InstantiatePrefab(blockPrefab, transform) as GameObject;
                
                if(y % 2 == 0)
                    prefab.transform.position = new Vector3(x, y);
                else
                    prefab.transform.position = new Vector3(boardDimensions.x - x - 1, y);
                
                prefab.GetComponent<Block>().Setup(blockId, blockColours[Random.Range(0, blockColours.Length)]);
                prefab.name = blockId.ToString();
            }
        }
    }
#endif
}
