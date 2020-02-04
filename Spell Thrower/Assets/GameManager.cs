using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public struct Level
{
    [SerializeField] public List<Transform> Spawners;
    [SerializeField] public Transform positionJoueur;
    [SerializeField] public bool bouleDeFeu;
    [SerializeField] public bool bouleDeLaMort;
    [SerializeField] public bool hadooken;
    [SerializeField] public bool laser;
    [SerializeField] public int nombreMonstres;
    public bool tousLesSorts;   
}

public class GameManager : MonoBehaviour
{
    [Header ("Références")]
    [SerializeField] private GameObject joueur;
    [SerializeField] private DetectionTestScript detector;

    [Header ("Liste des monstres")]
    [SerializeField] private List<GameObject> monstres;

    [Header("Levels")]
    [SerializeField] private Level niveau1;
    [SerializeField] private Level niveau2;

    private List<Level> listeNiveaux = new List<Level>();
    private List<GameObject> currentMonstresPossibles = new List<GameObject>();
    private Level currentLevel;
    private int pvJoueur = 3;

    private void Start()
    {
        listeNiveaux.Add(niveau1);
        //listeNiveaux.Add(niveau2);
    }

    //récupérer les pv du joueur
    public int GetPvJoueur()
    {
        return pvJoueur;
    }

    //lancer une partie infinie score et sorts dispo
    public void StartInfinite()
    {
        //choisir un niveau au hasard
        int i = UnityEngine.Random.Range(0, listeNiveaux.Count);

        currentLevel = listeNiveaux[i];
        currentLevel.tousLesSorts = true;

        //tant que le joueur a au moins 1 pv
        while(true && pvJoueur > 0)
        {
            //lancer un niveau

            //choisir un autre niveau
        }
    }

    //quand le joueur se prend un dégât
    public void Degat()
    {
        pvJoueur--;
    }

    //charger un niveau, envoyer les infos du niveau aux autres composantes
    public void ChargerNiveau(Level lv)
    {
        //placer le joueur à l'emplacement correspondant
        joueur.transform.position = lv.positionJoueur.position;
        joueur.transform.rotation = lv.positionJoueur.rotation;

        //si tous les sorts sont disponibles, on stock tous les monstres
        if(lv.tousLesSorts)
        {
            currentMonstresPossibles = monstres;
        }
        else
        {
            //sinon on ajoute que les monstres pour les sorts disponibles
            if(lv.bouleDeFeu)
            {
                //ajouter les monstres faibles à la boule de feu
                currentMonstresPossibles.Add(monstres[0]);
            }
            if(lv.bouleDeLaMort)
            {
                //ajouter les monstres faibles à la BDLM
                currentMonstresPossibles.Add(monstres[1]);
            }
            if(lv.hadooken)
            {
                //ajouter les monstres faibles au hadooken
                currentMonstresPossibles.Add(monstres[2]);
            }
            if(lv.laser)
            {
                //ajouter les monstres faibles au laser
            }
        }
        detector.LireConfigNiveau(currentLevel);
    }

    //lancer une partie histoire, pas de score et sorts prédéterminés
    public void StartStory()
    {
        //Lancer le premier niveau
        currentLevel = niveau1;
        ChargerNiveau(currentLevel);
    }
}
