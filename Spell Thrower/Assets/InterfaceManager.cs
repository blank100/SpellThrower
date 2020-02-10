using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Text;
using System;

public enum StateInterface
{
    ONMAINMENU,
    ONCHOICEMENU,
    INGAME
}

public class InterfaceManager : MonoBehaviour
{
    [Header ("Les canvas des interfaces")]
    [SerializeField] Canvas menuPrincipal;
    [SerializeField] Canvas interfaceInGame;

    [Header("Composants actifs du menu principal")]
    [SerializeField] private Button boutonPlay;
    [SerializeField] private Button boutonQuitter;
    [SerializeField] private Button boutonStory;
    [SerializeField] private Button boutonInfinite;

    [Header("Composants actifs de l'interface ingame")]
    [SerializeField] private Image pv1;
    [SerializeField] private Image pv2;
    [SerializeField] private Image pv3;
    [SerializeField] private Button boutonQuitterInGame;
    [SerializeField] private Text score;

    [Header("Références")]
    [SerializeField] private DetectionTestScript detector;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private GameObject tuto;

    private StateInterface currentState = StateInterface.ONMAINMENU;
    private int pvjoueur = 3;

    //EVENTS
    public event Action clickJouer;
    public event Action clickQuitter;

    private void Start()
    {
        //désactivation du menu ingame et activation du menu principal
        interfaceInGame.gameObject.SetActive(false);
        menuPrincipal.gameObject.SetActive(true);

        boutonStory.gameObject.SetActive(false);
        boutonInfinite.gameObject.SetActive(false);

        boutonPlay.gameObject.SetActive(true);
        boutonQuitter.gameObject.SetActive(true);

        //désactivation du détecteur
        detector.gameObject.SetActive(false);
    }

    private void Update()
    {
        //si le joueur a perdu un pv
        if(pvjoueur > gameManager.GetPvJoueur())
        {
            pvjoueur = gameManager.GetPvJoueur();
            UpdateInterfacePV();
        }
    }

    //mettre à jour les PV du joueur sur l'interface
    private void UpdateInterfacePV()
    {
        switch(pvjoueur)
        {
            case 2:
                pv3.color = Color.white;
                break;
            case 1:
                pv2.color = Color.white;
                break;
            case 0:
                pv1.color = Color.white;
                RetourMenu();
                break;
        }
    }

    //afficher les boutons pour choisir le type de partie
    public void AfficherMenuChoix()
    {
        boutonPlay.gameObject.SetActive(false);
        boutonQuitter.gameObject.SetActive(false);

        boutonStory.gameObject.SetActive(true);
        boutonInfinite.gameObject.SetActive(true);
    }

    //reload du jeu au retour au menu
    public void RetourMenu()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
    }

    //fermer le jeu
    public void Quitter()
    {
        Application.Quit();
    }

    //lancer une partie sans score avec les sorts fixes selon les niveaux
    public void LancerPartieHistoire()
    {
        boutonInfinite.gameObject.SetActive(false);
        boutonStory.gameObject.SetActive(false);

        boutonPlay.gameObject.SetActive(true);
        boutonQuitter.gameObject.SetActive(true);

        menuPrincipal.gameObject.SetActive(false);
        score.gameObject.SetActive(false);
        interfaceInGame.gameObject.SetActive(true);

        detector.gameObject.SetActive(true);

        tuto.SetActive(false);

        gameManager.StartStory();
    }

    //lancer une partie avec score avec tous les sorts dès le début
    public void LancerPartieInfinie()
    {
        boutonInfinite.gameObject.SetActive(false);
        boutonStory.gameObject.SetActive(false);

        boutonPlay.gameObject.SetActive(true);
        boutonQuitter.gameObject.SetActive(true);

        menuPrincipal.gameObject.SetActive(false);
        score.gameObject.SetActive(true);
        interfaceInGame.gameObject.SetActive(true);

        detector.gameObject.SetActive(true);

        tuto.SetActive(false);

        gameManager.StartInfinite();
    }

    //mettre à jour le score
    public void UpdateScore(int s)
    {
        score.text = "Score : " + s;
    }
}
