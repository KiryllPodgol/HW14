using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Playables; 

public class GameManager : MonoBehaviour
{
    public GameObject controlPanel;
    public GameObject startPanel; 
    public GameObject player;
    public PlayableDirector cutsceneDirector; 
    public Button startButton; 

    private bool isCutscenePlaying = false;

    void Awake()
    {
        cutsceneDirector.Stop();
        controlPanel.SetActive(true);
        startPanel.SetActive(true);
        startButton.onClick.AddListener(StartGame);
        cutsceneDirector.stopped += OnCutsceneEnd;
    }

    void Update()
    {
        if (isCutscenePlaying)
        {
            DisablePlayerControls();
        }
    }

    public void StartGame()
    {
        controlPanel.SetActive(false);
        startPanel.SetActive(false);
        PlayCutscene();
    }

    void PlayCutscene()
    {
        isCutscenePlaying = true;
        cutsceneDirector.Play();
    }

    void OnCutsceneEnd(PlayableDirector director)
    {
        if (director == cutsceneDirector)
        {
            isCutscenePlaying = false;
            EnablePlayerControls();
        }
    }

    void DisablePlayerControls()
    {
        // Пример: выключение управления игроком
        if (player != null)
        {
            var playerController = player.GetComponent<CharacterController>();
            if (playerController != null)
            {
                playerController.enabled = false;
            }
        }
    }

    void EnablePlayerControls()
    {
        // Пример: включение управления игроком
        if (player != null)
        {
            var playerController = player.GetComponent<CharacterController>();
            if (playerController != null)
            {
                playerController.enabled = true;
            }
        }
    }
}
