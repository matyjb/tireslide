using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public enum GameState
{
    None,
    Playing,
    Paused,
    Finished,
    Starting
}

public class GameManager : MonoBehaviour
{
    [HideInInspector]
    public static GameManager instance;

    public Rigidbody player;

    public CanvasGroup pauseCanvas;

    private GameState _gameState;

    private Coroutine startingCoroutine;

    public Animator startAnim;

    private AudioSource countdownAudio;

    public GameState GameState
    {
        get => _gameState;
        set
        {
            if (_gameState != value)
            {
                // gamestate changed!
                switch (value)
                {
                    case GameState.Starting:
                        Time.timeScale = 1;
                        AudioListener.pause = false;
                        // freeze player
                        player.constraints = RigidbodyConstraints.FreezeAll;
                        // start 3..2..1..Go
                        if (startingCoroutine != null)
                        {
                            StopCoroutine(startingCoroutine);
                        }
                        startingCoroutine = StartCoroutine(ThreeTwoOneGo());
                        break;
                    case GameState.Playing:
                        Time.timeScale = 1;
                        AudioListener.pause = false;
                        // unfreeze player
                        player.constraints = RigidbodyConstraints.None;
                        break;
                    case GameState.Paused:
                        Time.timeScale = 0;
                        AudioListener.pause = true;
                        break;
                    case GameState.Finished:
                        Time.timeScale = 1;
                        AudioListener.pause = false;
                        // unfreeze player
                        player.constraints = RigidbodyConstraints.None;
                        break;
                }
                _gameState = value;
            }
        }
    }

    public void Pause_performed(InputAction.CallbackContext obj)
    {
        if (obj.started)
        {
            switch (GameState)
            {
                case GameState.Playing:
                    GameState = GameState.Paused;
                    break;
                case GameState.Paused:
                    GameState = GameState.Playing;
                    break;
                default:
                    break;
            }
        }
    }

    public void Reset_performed(InputAction.CallbackContext obj)
    {
        if (obj.started && GameState == GameState.Playing)
        {
            foreach (IResetable item in FindObjectsOfType<MonoBehaviour>().OfType<IResetable>())
            {
                item.ResetToInitial();
            }
            GameState = GameState.Starting;
        }
    }

    private void Start()
    {
        countdownAudio = GetComponent<AudioSource>();
        GameState = GameState.Starting;
    }

    void Awake()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        pauseCanvas.alpha = Mathf.Lerp(pauseCanvas.alpha, GameState == GameState.Paused ? 1 : 0, Time.unscaledDeltaTime / 0.05f);
    }

    private IEnumerator ThreeTwoOneGo()
    {
        startAnim.SetTrigger("Start");
        float time = 0.5f;
        countdownAudio.pitch = 1f;
        countdownAudio.PlayOneShot(countdownAudio.clip);
        yield return new WaitForSeconds(time);
        countdownAudio.PlayOneShot(countdownAudio.clip);
        yield return new WaitForSeconds(time);
        countdownAudio.PlayOneShot(countdownAudio.clip);
        yield return new WaitForSeconds(time);
        countdownAudio.pitch = 1.1f;
        countdownAudio.PlayOneShot(countdownAudio.clip);
        GameState = GameState.Playing;
        startingCoroutine = null;
    }
}
