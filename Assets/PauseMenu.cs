using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class PauseMenu : MonoBehaviour
{
   public static bool isGamePaused = false;
   public GameObject pauseMenu;
   void Update() {
    if(Input.GetKeyDown(KeyCode.Escape)){
        if(isGamePaused){
            resumeGame();
        }else{
            pauseGame();
        }
    }
   }

   public void resumeGame(){
    pauseMenu.gameObject.SetActive(false);
    Time.timeScale = 1f;
    isGamePaused = false;
   }
   public void pauseGame(){
    pauseMenu.gameObject.SetActive(true);
    isGamePaused = true;
    Time.timeScale = 0f;
   }
   public void returnToMainMenu(){
    Time.timeScale = 1f;
    SceneManager.LoadScene("Menu");
   }
   public void Quit(){
    Application.Quit();
   }
}
