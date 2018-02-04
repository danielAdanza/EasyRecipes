using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using ChartboostSDK;

public class MainMenu : MonoBehaviour {

    //variable initialization
    public GameObject settingsSection;
    public GameObject languajeSection;
    public GameObject moreAppsSection;
    //UI Texts
    public Text titleText;
    public Text changeLanguajeText;
    public Text moreAppsText;
    public Text ihaveneverText;
    //UI buttons
    public Text buttonYes;
    public Text buttonNo;

    public void Start ()
    {
        //if there is no languaje defined
        if (UnityEngine.PlayerPrefs.GetInt("languaje") == 0)
        {
            OpenLanguajeSection();
        }
        else 
        //if it is in Spanish we will change the text
        //the default languaje is English, if so we have to do nothing
        if (UnityEngine.PlayerPrefs.GetInt("languaje") == 1)
        {
            ChangeLanguajeToSpanish();
        }

        //SOUND
        //0 - WITH SOUND
        //1 - NO SOUND

        if (UnityEngine.PlayerPrefs.GetInt("sound") == 0)
        {
            buttonNo.text = "";
        }
        else
        {
            buttonYes.text = "";
        }

    }

	public void openUrl (string url)
    {
        Application.OpenURL(url);
    }

    public void goToScene ()
    {
        SceneManager.LoadScene("indexMenu");
    }

    public void OpenSettingsSection ()
    {
        if (settingsSection.activeSelf != true)
        {
            settingsSection.SetActive(true);
            languajeSection.SetActive(false);
            moreAppsSection.SetActive(false);
        }
        else
        {
            settingsSection.SetActive(false);
            languajeSection.SetActive(false);
            moreAppsSection.SetActive(false);
        }
    }

    public void OpenLanguajeSection ()
    {
        settingsSection.SetActive(false);
        languajeSection.SetActive(true);
    }

    public void OpenMoreAppsSection ()
    {
        settingsSection.SetActive(false);
        moreAppsSection.SetActive(true);
    }

    public void CloseLanguajeSection()
    {
        //the first time you use it you can not close it until you pick a languaje
        if (UnityEngine.PlayerPrefs.GetInt("languaje") != 0)
        {
            languajeSection.SetActive(false);
        }
    }

    public void CloseMoreAppsSection()
    {
        moreAppsSection.SetActive(false);
    }

    //Languaje Recipe:
    //LAN 1: spanish
    //LAN 2: english
    public void ChangeLanguaje(int languajeNumber)
    {
        if (UnityEngine.PlayerPrefs.GetInt("languaje") != languajeNumber)
        {
            UnityEngine.PlayerPrefs.SetInt("languaje", languajeNumber);

            //if it is in Spanish
            if (languajeNumber == 1)
            {
                ChangeLanguajeToSpanish();
            }
            else if (languajeNumber == 2)
            {
                ChangeLanguajeToEnglish();
            }

        }

        CloseLanguajeSection();
    }

    public void ChangeLanguajeToSpanish ()
    {
        titleText.text = "RECETAS \n FÁCILES \n PARA COCINAR";
        changeLanguajeText.text = "Cambiar Idioma";
        moreAppsText.text = "Más Apps";
        ihaveneverText.text = "Yo Nunca";
        if (buttonYes.text != "")
        { buttonYes.text = "Si"; }
    }

    public void ChangeLanguajeToEnglish()
    {
        titleText.text = "EASY \n RECIPES \n FOR COOKING";
        changeLanguajeText.text = "Change Languaje";
        moreAppsText.text = "More Apps";
        ihaveneverText.text = "I have never";
        if (buttonYes.text != "")
        { buttonYes.text = "Yes"; }
    }

    public void YesSoundClicked()
    {
        if (UnityEngine.PlayerPrefs.GetInt("sound") != 0)
        {
            UnityEngine.PlayerPrefs.SetInt("sound", 0);

            if (UnityEngine.PlayerPrefs.GetInt("languaje") == 1)
            {
                buttonYes.text = "Si";
            }
            else if (UnityEngine.PlayerPrefs.GetInt("languaje") == 2)
            {
                buttonYes.text = "Yes";
            }
            buttonNo.text = "";
        }
    }

    public void NoSoundClicked ()
    {
        if (UnityEngine.PlayerPrefs.GetInt("sound") != 1)
        {
            UnityEngine.PlayerPrefs.SetInt("sound", 1);
            buttonYes.text = "";
            buttonNo.text = "No";
        }
    }

    //it exits the application if that button has been clicked
    public void PowerOfClicked ()
    {
        Application.Quit();
    }

}
