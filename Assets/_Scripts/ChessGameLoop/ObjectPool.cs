using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace ChessMainLoop
{
    /// <summary>
    /// Contains pools for objects and methods to put objects and recieve objects to and from pools.
    /// </summary>
    public  class ObjectPool : Singleton<ObjectPool>
    {
        [SerializeField] private List<PathPiece> _prefabs;
        private Dictionary<PathPieceType, Queue<GameObject>> _poolDictionary;
        private Queue<Piece> _pieces;

        private void Start()
        {
            _poolDictionary = new Dictionary<PathPieceType, Queue<GameObject>>();
            _pieces = new Queue<Piece>();

            Queue<GameObject> queue;

            foreach (PathPiece prefab in _prefabs)
            {
                queue = new Queue<GameObject>();
                _poolDictionary.Add(prefab.PathPieceType, queue);
            }
        }

        /// <summary>
        /// Returns number of path objects indexed by name equal to quantity parameter. Gets objects from pool or instantiates new ones if quantity in pool isnt enough.
        /// </summary>
        /// <returns>List of path objects quantity long</returns>
        public GameObject[] GetHighlightPaths(int quantity, PathPieceType pathPieceType)
        {
            GameObject[] paths = new GameObject[quantity];

            for(int i = 0; i < quantity; i++)
            {
                if (_poolDictionary[pathPieceType].Count > 0)
                {
                    paths[i] = _poolDictionary[pathPieceType].Dequeue();
                    paths[i].SetActive(true);
                }
                else
                {
                    paths[i]= Instantiate(_prefabs.Where(piece => piece.PathPieceType == pathPieceType)
                        .SingleOrDefault().gameObject, transform.parent);
                }
            }

            return paths;
        }

        /// <summary>
        /// Returns a singular path object indexed by type
        /// </summary>
        /// <returns>Path object of type</returns>
        public GameObject GetHighlightPath(PathPieceType pathPieceType)
        {
            if (_poolDictionary.TryGetValue(pathPieceType, out var pool) && pool.Count > 0)
            {
                var path = pool.Dequeue();
                path.SetActive(true);
                return path;
            }
            else
            {
                var prefab = _prefabs.SingleOrDefault(piece => piece.PathPieceType == pathPieceType);
                if (prefab != null)
                {
                    var newPath = Instantiate(prefab.gameObject, transform.parent);
                    return newPath;
                }
            }
            return null;
        }

        /// <summary>
        /// Disables a path object and puts it back into pool
        /// </summary>
        public void RemoveHighlightPath(PathPiece path)
        {
            path.gameObject.SetActive(false);
            if (_poolDictionary.TryGetValue(path.PathPieceType, out var pool))
            {
                pool.Enqueue(path.gameObject);
            }
        }

        public void AddPiece(Piece piece)
        {
            _pieces.Enqueue(piece);
            piece.gameObject.SetActive(false);
        }

        public void ResetPieces()
        {
            while (_pieces.Count > 0)
            {
                _pieces.Dequeue().gameObject.SetActive(true);
            }
        }
    }
}
