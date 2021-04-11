using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static bool isPaused = true;
    public float timeRemaining = 90.9f;

    public int goblinScore = 0;
    public int playerScore = 0;
    public int dwarvesRemaining = 1;

    //UI references
    public GameObject pauseMenu;
    public GameObject HUD;
    public GameObject GameOverScreen;

    public Text timer;
    public Text goblinScoreText;
    public Text playerScoreText;
    public Text dwarvesRemainingText;
    
    // Start is called before the first frame update
    void Start(){
        Time.timeScale = 0;
    }

    // Update is called once per frame
    void Update(){
        if(timeRemaining <= 0 || dwarvesRemaining <= 0){
            GameOver();
        }

        if(Input.GetKeyDown(KeyCode.Escape)){
            isPaused = !isPaused;
            PauseGame();
        }        
        timeRemaining -= Time.deltaTime;
        timer.text = Mathf.FloorToInt(timeRemaining).ToString();
        goblinScoreText.text = goblinScore.ToString();
        playerScoreText.text = playerScore.ToString();
        dwarvesRemainingText.text = dwarvesRemaining.ToString();
    }

    void PauseGame(){
        if(isPaused){
            Time.timeScale = 0;
            pauseMenu.SetActive(true);
            HUD.SetActive(false);
        }
        else{
            Time.timeScale = 1;
            pauseMenu.SetActive(false);
            HUD.SetActive(true);
        }
    }

    void GameOver(){
        Time.timeScale = 0;
        pauseMenu.SetActive(false);
        HUD.SetActive(false);
        GameOverScreen.SetActive(true);
    }
}
