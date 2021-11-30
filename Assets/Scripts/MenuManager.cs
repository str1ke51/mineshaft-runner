using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public List<GameObject> possibleMenuPanels = new List<GameObject>();
    public TextMeshProUGUI highScoreText;

    private void Start()
    {
        int overall = PlayerPrefs.GetInt(PlayerPrefsKeys.BestScore);
        int best1 = PlayerPrefs.GetInt(PlayerPrefsKeys.BestLevelScore(1));
        int best2 = PlayerPrefs.GetInt(PlayerPrefsKeys.BestLevelScore(2));
        int best3 = PlayerPrefs.GetInt(PlayerPrefsKeys.BestLevelScore(3));
        int scored = PlayerPrefs.GetInt(PlayerPrefsKeys.OverallPoints);

        string text = $"Overall high score: {overall}\n\n" +
                      $"High score level 1: {best1}\n" +
                      $"High score level 2: {best2}\n" +
                      $"High score level 3: {best3}\n\n" +
                      $"Points scored: {scored}";

        highScoreText.text = text;
    }

    public void MM_NavigateToPanel(GameObject panel)
    {
        StartCoroutine("NavigateToPanel", panel);
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
        foreach (var scene in possibleMenuPanels)
            scene.SetActive(false);

        yield return new WaitForSeconds(0.1f);

        panelToActivate.SetActive(true);
    }
}
