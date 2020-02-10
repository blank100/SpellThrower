using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.AI;

[Serializable]
public struct Level
{
    [SerializeField] public List<Transform> Spawners;
    [SerializeField] public Transform positionJoueur;
    [SerializeField] public bool bouleDeFeu;
    [SerializeField] public bool bouleDeLaMort;
    [SerializeField] public bool hadooken;
    [SerializeField] public bool laser;
    public bool tousLesSorts;

    [SerializeField] public int nombreMonstres;

    [SerializeField] public Material skybox;
    [SerializeField] public PostProcessProfile profilPP;
    [SerializeField] public GameObject decor;
    [SerializeField] public Color couleur;
}

public class GameManager : MonoBehaviour
{
    [Header ("Références")]
    [SerializeField] private GameObject joueur;
    [SerializeField] private DetectionTestScript detector;
    [SerializeField] private Pooling pooling;
    [SerializeField] private InterfaceManager interfaceManager;
    [SerializeField] private Light lumiere;

    [Header ("Levels")]
    [SerializeField] private Level niveau1;
    [SerializeField] private Level niveau2;
    [SerializeField] private Level niveau3;

    [Header("PostProcessing")]
    [SerializeField] private PostProcessVolume postProcess;
    [SerializeField] private Skybox skybox;

    private List<Level> listeNiveaux = new List<Level>();
    private Level currentLevel;
    private int pvJoueur = 3;
    private int currentMonstresEnVie = 0;
    private bool[] monstresPossibles = new bool[4];
    private bool niveauTermine = false;
    private bool story;
    private int progression = 0;
    private int score = 0;

    private void Start()
    {
        listeNiveaux.Add(niveau1);
        listeNiveaux.Add(niveau2);
        listeNiveaux.Add(niveau3);
    }

    public void Scoring(int s)
    {
        score += s;
        interfaceManager.UpdateScore(score);
    }

    //récupérer les pv du joueur
    public int GetPvJoueur()
    {
        return pvJoueur;
    }

    //lancer une partie infinie score et sorts dispo
    public void StartInfinite()
    {
        story = false;

        //choisir un niveau au hasard et le lancer
        int i = UnityEngine.Random.Range(0, listeNiveaux.Count);

        currentLevel = listeNiveaux[i];
        currentLevel.tousLesSorts = true;
        currentLevel.nombreMonstres = UnityEngine.Random.Range(5, 15);

        ChargerNiveau(currentLevel);
        StartCoroutine(JouerNiveau());
    }

    //quand le joueur se prend un dégât
    public void Degat()
    {
        pvJoueur--;
    }

    //charger un niveau, envoyer les infos du niveau aux autres composantes
    public void ChargerNiveau(Level lv)
    {
        lv.decor.SetActive(true);

        //placer le joueur à l'emplacement correspondant
        joueur.transform.position = lv.positionJoueur.position;
        joueur.transform.rotation = lv.positionJoueur.rotation;

        //changer la skybox et le post processing
        skybox.material = lv.skybox;
        postProcess.profile = lv.profilPP;
        lumiere.color = lv.couleur;

        //si le niveau autorise tous les sorts
        if(lv.tousLesSorts)
        {
            for (int i = 0; i < monstresPossibles.Length; i++) monstresPossibles[i] = true;
        }
        else
        {
            //si les boules de feu sont autorisées
            if (lv.bouleDeFeu) monstresPossibles[0] = true;
            else monstresPossibles[0] = false;

            //si les boules de la mort sont autorisées
            if (lv.bouleDeLaMort) monstresPossibles[1] = true;
            else monstresPossibles[1] = false;

            //si les hadooken sont autorisés
            if (lv.hadooken) monstresPossibles[2] = true;
            else monstresPossibles[2] = false;

            //si les lasers sont autorisés
            if (lv.laser) monstresPossibles[3] = true;
            else monstresPossibles[3] = false;
        }
        detector.LireConfigNiveau(currentLevel);
    }

    //lancer une partie histoire, pas de score et sorts prédéterminés
    public void StartStory()
    {
        story = true;

        niveauTermine = false;

        //Lancer le premier niveau
        progression++;
        currentLevel = niveau1;
        ChargerNiveau(currentLevel);

        StartCoroutine(JouerNiveau());

    }

    //passer au niveau suivant
    private void GoNextLevel(Level lv)
    {
        progression++;
        currentLevel = lv;
        ChargerNiveau(lv);
        StartCoroutine(JouerNiveau());
    }

    //jouer un niveau
    private IEnumerator JouerNiveau()
    {
        currentMonstresEnVie = 0;
        
        //faire spawn le nombre de monstre indiqué dans la configuration du niveau
        for(int i = 0; i < currentLevel.nombreMonstres; i++)
        {
            //choisir un type de monstre à faire spawn dans la liste des monstres possibles dans le niveau
            int choix;
            do
            {
                choix = UnityEngine.Random.Range(0, 4);
            }
            while (monstresPossibles[choix] != true);

            ExposerMonstre m;

            if(choix == 0)
            {
                m = pooling.getMonstreFeu(currentLevel.positionJoueur.position, currentLevel.Spawners[UnityEngine.Random.Range(0, currentLevel.Spawners.Count)].position);
            }
            else
            {
                if (choix == 1)
                {
                    m = pooling.getMonstreMort(currentLevel.positionJoueur.position, currentLevel.Spawners[UnityEngine.Random.Range(0, currentLevel.Spawners.Count)].position);
                }
                else
                {
                    if (choix == 2)
                    {
                        m = pooling.getMonstreHadooken(currentLevel.positionJoueur.position, currentLevel.Spawners[UnityEngine.Random.Range(0, currentLevel.Spawners.Count)].position);
                    }
                    else
                    {
                        if (choix == 3)
                        {
                            m = pooling.getMonstreLaser(currentLevel.positionJoueur.position, currentLevel.Spawners[UnityEngine.Random.Range(0, currentLevel.Spawners.Count)].position);
                        }
                    }
                }
            }

            

            //faire spawn le monstre et incrémenter le nombre de monstres en vie
            NewMonstre();

            //attendre un temps random avant le prochain spawn
            yield return new WaitForSeconds(UnityEngine.Random.Range(2, 6));
        }
        while (currentMonstresEnVie > 0)
        {
            yield return new WaitForSeconds(0.5f);
        }

        //fin du niveau
        niveauTermine = true;
    }

    //baisser le nombre de monstre en vie
    public void MortMonstre()
    {
        currentMonstresEnVie--;
    }

    //un nouveau monstre vient de spawn
    private void NewMonstre()
    {
        currentMonstresEnVie++;
    }

    private void Update()
    {
        //si le niveau est fini, on passe au suivant
        if(niveauTermine)
        {
            //si on est en mode histoire
            if(story)
            {
                if(pvJoueur > 0)
                {
                    niveauTermine = false;
                    currentLevel.decor.SetActive(false);

                    switch (progression)
                    {
                        case 1:
                            //passer au lv 2
                            GoNextLevel(niveau2);
                            break;

                        case 2:
                            //passer au lv 3
                            GoNextLevel(niveau3);
                            break;
                    }
                }
            }
            else
            {
                //si on est en partie infinie;
                if(pvJoueur > 0)
                {
                    //en mode infini, on rechoisi un niveau au hasard et on le lance
                    niveauTermine = false;

                    currentLevel.decor.SetActive(false);

                    int i = UnityEngine.Random.Range(0, listeNiveaux.Count);
                    pooling.ResetPoolingMonstres();
                    currentLevel = listeNiveaux[i];
                    currentLevel.tousLesSorts = true;
                    currentLevel.nombreMonstres = UnityEngine.Random.Range(5, 15);
                    GoNextLevel(currentLevel);
                }
            }
        }
    }
}
