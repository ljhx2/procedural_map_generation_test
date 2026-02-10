using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapGenerator : MonoBehaviour
{
    [Header("설정 값")]
    [SerializeField] private GameObject _floorPrefab; //바닥 프리팹
    [SerializeField] private GameObject _wallPrefab; //벽 프리팹
    [SerializeField] private int _walkLength = 50; //이동 거리
    [SerializeField] private Vector2Int _startPos;
    [SerializeField] private Transform _mapParent;

    [Header("시드 설정")]
    [SerializeField] private bool _useRandomSeed = true;
    [SerializeField] private string _seed = "";

    [Header("UI")]
    [SerializeField] private Button _button;

    private HashSet<Vector2Int> _floorPositions = new HashSet<Vector2Int>(); //중복 위치 방지
    private System.Random _prang;


    private void Start()
    {
        _button.onClick.AddListener(OnGenerateButtonClick);
    }

    public void OnGenerateButtonClick()
    {
        DeleteOldMap();

        GenerateSeed();
        GenerateFloor();
        SpawnFloor();
        SpawnWall();
    }

    private void DeleteOldMap()
    {
        if (_mapParent != null)
        {
            foreach (Transform child in _mapParent)
            {
                Destroy(child.gameObject);
            }
        }
        _floorPositions.Clear();
    }

    private void GenerateSeed()
    {
        if (_useRandomSeed)
        {
            _seed = Time.time.ToString(); //현재시간 기반
        }
        _prang = new System.Random(_seed.GetHashCode());
        Debug.Log("사용된 시드값 : " + _seed);
    }

    private void GenerateFloor()
    {
        Vector2Int currentPos = _startPos;

        for (int i = 0; i < _walkLength; i++)
        {
            Vector2Int direction = GetRandomDir();
            currentPos += direction;
            if (!_floorPositions.Add(currentPos))
            {
                Debug.LogWarning("중복");
            }
        }
    }

    private Vector2Int GetRandomDir()
    {
        Vector2Int[] directions = { Vector2Int.down, Vector2Int.up, Vector2Int.left, Vector2Int.right };
        int index = _prang.Next(0, directions.Length);
        return directions[index];
    }

    private void SpawnFloor()
    {
        foreach (Vector2Int pos in _floorPositions)
        {
            Vector3 worldPos = new Vector3(pos.x, 0f, pos.y);
            Instantiate(_floorPrefab, worldPos, Quaternion.identity, _mapParent);
        }
    }

    private void SpawnWall()
    {
        HashSet<Vector2Int> wallPoss = new HashSet<Vector2Int>();

        foreach (Vector2Int floorPos in _floorPositions)
        {
            foreach (Vector2Int dir in Dir2D.EIGHT_DIRS)
            {
                Vector2Int neighborPos = floorPos + dir;
                if (!_floorPositions.Contains(neighborPos))
                {
                    wallPoss.Add(neighborPos);
                }
            }
        }

        foreach (Vector2Int wallPos in wallPoss)
        {
            Vector3 worldPos = new Vector3(wallPos.x, 0f, wallPos.y);
            Instantiate(_wallPrefab, worldPos, Quaternion.identity, _mapParent);
        }
    }
}
