using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class SceneController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
   
        //Set screen size for Standalone
        /*
        #if UNITY_STANDALONE
        Screen.SetResolution(1920, 1200, false);
        Screen.fullScreen = false;
#endif
*/
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void loadMainGameScene()
    {
        SceneManager.LoadScene("Main");
    }

    public void quitGame()
    {
        Application.Quit();
    }

    public void restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);

    }
}
