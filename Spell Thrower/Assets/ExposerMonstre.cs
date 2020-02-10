using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum EtatMonstre
{
    POOLING,
    ENMARCHE,
    MORT
}

public class ExposerMonstre : MonoBehaviour
{
    [SerializeField] private Sort faiblesse;
    [SerializeField] private Transform monstreTransform;
    [SerializeField] private Rigidbody monstreRigidbody;
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private int points;

    private EtatMonstre etat = EtatMonstre.POOLING;
    [SerializeField] private GameManager gameManager;

    //quand le monstre arrive au contact d'un sort
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.layer == 8)
        {
            var exp = collision.gameObject.GetComponent<ExposerSort>();

            if(exp.bouleDeFeu && faiblesse == Sort.BOULEDEFEU)
            {
                //si le monstre est faible face à la boule de feu
                etat = EtatMonstre.MORT;
            }
            else
            {
                if(exp.bouleDeLaMort && faiblesse == Sort.BOULEDELAMORT)
                {
                    //si le monstre est faible face à la boule de la mort
                    etat = EtatMonstre.MORT;
                }
                else
                {
                    if(exp.hadooken && faiblesse == Sort.HADOOKEN)
                    {
                        //si le monstre est faible face au hadooken
                        etat = EtatMonstre.MORT;
                    }
                    else
                    {
                        if(exp.laser && faiblesse == Sort.LASER)
                        {
                            //si le monstre est faible face au laser
                            etat = EtatMonstre.MORT;
                        }
                    }
                }
            }
            exp.AnnulationSort();
            gameManager.MortMonstre();
            gameManager.Scoring(points);
        }
        else
        {
            //collision avec le joueur -> un PV en moins
            if(collision.gameObject.layer == 10)
            {
                gameManager.Degat();
                etat = EtatMonstre.MORT;
                gameManager.MortMonstre();
            }
        }
    }

    //définir le game manager du monstre
    public void SetGameManager(GameManager g)
    {
        gameManager = g;
    }

    //activation du monstre depuis le pooling
    public void ActivationMonstre()
    {
        gameObject.SetActive(true);
        etat = EtatMonstre.ENMARCHE;
    }

    //désactivation du monstre à sa remise dans le pooling
    public void DesactivationMonstre()
    {
        gameObject.SetActive(false);
    }

    //récupérer l'état du monstre
    public EtatMonstre GetEtatMonstre()
    {
        return etat;
    }

    public void SetEtatMonstre(EtatMonstre e)
    {
        etat = e;
    }

    //récupérer son point faible
    public Sort GetFaiblesse() 
    {
        return faiblesse;
    }

    //définir la position cible du monstre
    public void SetCible(Vector3 p)
    {
        agent.destination = p;
    }
}
