using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace AllieJoe.JuiceIt
{
    /// <summary>
    /// Hex grid system based on RedBlogGames post: https://www.redblobgames.com/grids/hexagon
    /// </summary>
    public class ProceduralEnviroment : MonoBehaviour
    {
        [SerializeField] private GridTileTuningSO _config;
        
        [Space]
        [SerializeField] private Transform _container;
        
        [Space]
        [SerializeField] private Camera _cam;
        [SerializeField] private Vector2 _safeZoneOffset;
        [SerializeField] private Vector2 _tweenZoneOffset;
        
        //private readonly List<Tile> _tiles = new();
        private readonly Dictionary<Vector2Int, Tile> _tiles = new();
        private Tile _lastSelected;

        private IObjectPool<Tile> _tilePool;

        private Vector2 _lastSafeZoneCheck = Vector2.zero;
        
        private const float VERTICAL_DISTANCE_MULTIPLIER = 0.75f; //0.75 = 3f/4f
        private float CACHED_SQRT_3 = 1.732f; //Cache sqrt(3) in awake method
        private float Height => 2 * _config.Size;
        private float Width => CACHED_SQRT_3 *  _config.Size;
        
        private void Awake()
        {
            CACHED_SQRT_3 = Mathf.Sqrt(3f);
        }

        private void Start()
        {
            if (_cam == null)
                _cam = Camera.main;

            _tilePool = new ObjectPool<Tile>(
                () => Instantiate(_config.TilePrefab, _container),
                tile => tile.gameObject.SetActive(true),
                tile => tile.gameObject.SetActive(false),
                tile => Destroy(tile.gameObject));
                
            GenerateInitGroup();
            
            GameManager.Instance.GameDelegates.OnConfigUpdated += OnConfigUpdated;
            GameManager.Instance.GameDelegates.AllConfigUpdated += OnResetLevel;
            GameManager.Instance.GameDelegates.OnResetLevel += OnResetLevel;
        }
        
        private void OnDestroy()
        {
            GameManager.Instance.GameDelegates.OnResetLevel -= OnResetLevel;
            GameManager.Instance.GameDelegates.OnConfigUpdated -= OnConfigUpdated;
            GameManager.Instance.GameDelegates.AllConfigUpdated -= OnResetLevel;
        }

        private void OnConfigUpdated(EConfigKey key)
        {
            if (key == EConfigKey.BackgroundSpawnTween)
                OnResetLevel();
        }
        
        private void OnResetLevel()
        {
            ClearTiles();
            GenerateInitGroup();
        }

        private void Update()
        {
            CheckSafeZone();
            // if (Input.GetMouseButtonDown(0))
            // {
            //     Vector2 mousePOs = _cam.ScreenToWorldPoint(Input.mousePosition);
            //     debugTr.position = mousePOs;
            //     
            //     (int q, int r) = PointToAxial(mousePOs);
            //
            //     if(_lastSelected != null)
            //         _lastSelected.SetSelected(false);
            //     
            //     foreach (var tile in _tiles)
            //     {
            //         if (tile.Q == q && tile.R == r)
            //         {
            //             _lastSelected = tile;
            //             tile.SetSelected(true);
            //             break;
            //         }
            //     }
            // }
        }

        #region HexCoordinates
        //Axial Coordinates
        private Vector2 AxialToPoint(int q, int r)
        {
            float x = Width * (q + r * 0.5f);
            // Multiply by -1 since Unity has Y-Axis inverted compared to RedBlogGames examples
            float y = -1 * Height * r * VERTICAL_DISTANCE_MULTIPLIER; 
            return new Vector2(x, y);
        }
        
        private (int column, int row) AxialToOddR(int q, int r)
        {
            int offset = (r % 2) == 0 ? 1 : 0;
            int col = q + (r + (offset)) / 2;
            int row = r;
            return (col, row);
        }

        private (int q, int r, int s) AxialToCube(int q, int r) => (q, r, -q - r);

        private (int q, int r) CubeToAxial(int q, int r, int s) => (q, r);

        private (int q, int r) PointToAxial(Vector2 position)
        {
            // Since Unity has Y-Axis inverted compared to RedBlogGames examples,
            //   the inverted result it's difference from the blog -> Need to take into account that -1 :)
            float r = -position.y / (Height * VERTICAL_DISTANCE_MULTIPLIER);
            float q = (position.x / Width) - r * 0.5f;

            return AxialRound(q, r);
        }

        private (int q, int r, int s) CubeRound(float fq, float fr, float fs)
        {
            int q = Mathf.RoundToInt(fq);
            int r = Mathf.RoundToInt(fr);
            int s = Mathf.RoundToInt(fs);

            var q_diff = Mathf.Abs(q - fq);
            var r_diff = Mathf.Abs(r - fr);
            var s_diff = Mathf.Abs(s - fs);

            if (q_diff > r_diff && q_diff > s_diff)
                q = -r - s;
            else if (r_diff > s_diff)
                r = -q - s;
            else
                s = -q - r;
            return (q, r, s);
        }

        private (int q, int r) AxialRound(float q, float r)
        {
            (int fq, int fr, int fs) = CubeRound(q, r, -q-r);
            return CubeToAxial(fq, fr, fs);
        }
        
        // OddR Coordinates
        private Vector2 OddRToPoint(int column, int row) 
        {
            int offset = (row % 2) == 0 ? 1 : 0;
            float x = Width * (column - 0.5f * offset);
            float y = Height * row * VERTICAL_DISTANCE_MULTIPLIER;
            return new Vector2(x, y);
        }
        
        private (int q, int r) OddRToAxial(int col, int row)
        {
            int offset = (row % 2) == 0 ? 1 : 0;
            var q = col - (row + offset) / 2;
            var r = row;
            return (q, r);
        }
#endregion

        private void GenerateInitGroup()
        {
            TileSpawnTuning spawnTuning = GameManager.Instance.GetConfigValue<TileSpawnTuning>(EConfigKey.BackgroundSpawnTween);
            float extraDelay = GameManager.Instance.JuiceConfig.InitExtraDelay;
            FillGrid(_cam.transform.position,  GetSafeZone() / 2f, spawnTuning, extraDelay);
        }

        private void ClearTiles()
        {
            foreach (var tile in _tiles.Values)
            {
                if(Application.isPlaying)
                    _tilePool.Release(tile);
                else
                    DestroyImmediate(tile.gameObject);
            }
            _tiles.Clear();

            if (!Application.isPlaying)
            {
                for (int i = _container.childCount - 1; i >= 0; i--)
                    DestroyImmediate(_container.GetChild(i).gameObject);
            }
        }

        private void CheckSafeZone()
        {
            Vector2 center = _cam.transform.position;
            if((center - _lastSafeZoneCheck).SqrMagnitude() < _config.Size * _config.Size)
                return;

            _lastSafeZoneCheck = _cam.transform.position;
            
            Vector2 safeZone = GetSafeZone() / 2f;
            List<Vector2Int> toRemove = new();
            foreach (var k in _tiles.Keys)
            {
                if (IsOutsideSafeZone(_tiles[k].transform.position, center, safeZone))
                    toRemove.Add(k);
            }

            foreach (var i in toRemove)
            {
                _tilePool.Release(_tiles[i]);
                _tiles.Remove(i);
            }
            toRemove.Clear();

            FillGrid(center, safeZone);
        }

        private void FillGrid(Vector2 center, Vector2 safeZone, TileSpawnTuning spawnTuning = null, float extraDelay = 0)
        {
            //Fill missing
            // Y-Axis is inverted in the Axial coordinates, so end/int are swapped
            (int initQ, int endR) = PointToAxial(center - safeZone);
            (int initX, int endY) = AxialToOddR(initQ, endR);
            
            (int endQ, int initR) = PointToAxial(center + safeZone);
            (int endX, int initY) = AxialToOddR(endQ, initR);

            for (int y = initY; y < endY; y++)
            {
                for (int x = initX; x < endX; x++)
                {
                    if (_tiles.ContainsKey(new Vector2Int(x, y)))
                        continue;
                    
                    Tile tile = GetNewTile(x, y);
                    _tiles.Add(new Vector2Int(x, y), tile);

                    if (spawnTuning != null)
                    {
                        float disNormalize = (center - (Vector2) tile.transform.position).sqrMagnitude / 100f; // "Random" distance for a "normalize" value and better felling
                        tile.PlayStartAnimation(spawnTuning, disNormalize + 1, extraDelay);
                    }
                }
            }
        }
        
        private Tile GetNewTile(int col, int row)
        {
            Tile tile = _tilePool.Get();
            
            // int r = currentX;
            // int q = currentY;
            // (int col, int row) = AxialToOddR(q, r);
            
            (int q, int r) = OddRToAxial(col, row);
            tile.SetData(q, r, col, row, _config.GetRandomTileSet() );

            tile.transform.localScale = Vector3.one * _config.Scale;
            tile.transform.localPosition = AxialToPoint(q, r);
            
            return tile;
        }

        private Vector2 GetSafeZone()
        {
            float worldHeight = _cam.orthographicSize * 2;
            float worldWidth = worldHeight * _cam.aspect;

            return new Vector2(worldWidth, worldHeight) + _safeZoneOffset;
        }

        private bool IsOutsideSafeZone(Vector2 pos, Vector2 center, Vector2 size)
        {
            return pos.x > center.x + size.x || pos.x < center.x - size.x ||
                   pos.y > center.y + size.y || pos.y < center.y - size.y;
        }
        

        private void OnDrawGizmosSelected()
        {
            if (_cam == null)
                return;

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(_cam.transform.position, GetSafeZone());
            
            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(_cam.transform.position, GetSafeZone() - _tweenZoneOffset);

        }
    }
}
