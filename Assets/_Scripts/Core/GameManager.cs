using Sirenix.OdinInspector;
using UltEvents;
using UnityEngine;

namespace SunsetSystems.Core
{
    public class GameManager : SerializedMonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [SerializeField]
        private GameState _state;
        public GameState State
        {
            get
            {
                return _state;
            }
            set
            {
                _state = value;
                OnGameStateChanged?.InvokeSafe(value);
            }
        }

        public UltEvent<GameState> OnGameStateChanged = new();

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);
        }
    }
}
