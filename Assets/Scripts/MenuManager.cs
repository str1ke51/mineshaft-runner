using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public GameObject mainMenuPanel;
    public GameObject selectLevelPanel;

    public void MM_NavigateToSelectLevel()
    {
        Debug.Log("Navigating to SL");
        StartCoroutine("NavigateToPanel", selectLevelPanel);
        Debug.Log("Navigated to SL");
    }

    public void MM_NavigateToMainMenu()
    {
        StartCoroutine("NavigateToPanel", mainMenuPanel);
    }

    public void MM_LoadLevel(int index)
    {
        SceneManager.LoadScene(index);
    }

    public void MM_QuitApp()
    {
        Application.Quit();
    }

    private IEnumerator NavigateToPanel(GameObject panelToActivate)
    {
        mainMenuPanel.SetActive(false);
        selectLevelPanel.SetActive(false);

        yield return new WaitForSeconds(0.1f);

        panelToActivate.SetActive(true);
    }
}
