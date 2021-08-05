using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Board : MonoBehaviour
{
    [SerializeField] private GameObject blockPrefab;
    public GameObject startPosition;
    [SerializeField] private Vector2Int boardDimensions;
    
    public Dictionary<int, Block> positions;

    private void Start()
    {
        positions = new Dictionary<int, Block> {{0, startPosition.GetComponent<Block>()}};
        PopulateBoardPositionList();
    }

    private void PopulateBoardPositionList()
    {
        positions = new Dictionary<int, Block>();
        
        foreach (var block in GetComponentsInChildren<Block>())
        {
            if(block.Id == 0)
                continue;
            positions.Add(block.Id, block);
        }
    }

    public Vector2 NextPosition(int currentPosition)
    {
        if (currentPosition == 100) 
            return positions[currentPosition].transform.position;
        
        return positions[currentPosition + 1].transform.position;
    }
    
    [ContextMenu("Clear Board")]
    public void ClearBoard()
    {
        foreach (var block in GetComponentsInChildren<Block>())
        {
            if(block.Id == 0)
                continue;
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
                
                prefab.GetComponent<Block>().Setup(blockId);
                prefab.name = blockId.ToString();
            }
        }
    }
#endif
}
