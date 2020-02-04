using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EtatSort
{
    DANSLAMAIN,
    LANCE,
    POOLING,
    ANNULE
}

public enum Sort
{
    BOULEDEFEU,
    BOULEDELAMORT,
    HADOOKEN,
    LASER
}

public class ExposerSort : MonoBehaviour
{
    [Header ("Composantes du sort")]
    [SerializeField] private float dureeDeVie;
    [SerializeField] private Transform sortTransform;
    [SerializeField] private Rigidbody sortRigidbody;
    [SerializeField] private Collider sortCollider;
    [SerializeField] private float vitesse;

    [Header ("Différenciation")]
    [SerializeField] public bool bouleDeFeu;
    [SerializeField] public bool bouleDeLaMort;
    [SerializeField] public bool hadooken;
    [SerializeField] public bool laser;

    private float popTime = 0.0f;
    private EtatSort encours = EtatSort.POOLING;

    public float GetVitesse()
    {
        return vitesse;
    }

    public void ActivationSort()
    {
        gameObject.SetActive(true);

    }

    public void DesactivationSort()
    {
        gameObject.SetActive(false);
    }

    public EtatSort GetEtat()
    {
        return encours;
    }

    public void AnnulationSort()
    {
        encours = EtatSort.POOLING;
    }

    public Rigidbody getRigidbody()
    {
        return sortRigidbody;
    }

    void Update()
    {
        //si le sort est lancé, on le désactive une fois sa durée de vie dépassée
        if(encours == EtatSort.LANCE)
        {
            if (Time.time - popTime > dureeDeVie)
            {
                encours = EtatSort.POOLING;
            }
        }
    }

    public void Incantation()
    {
        encours = EtatSort.DANSLAMAIN;
        sortRigidbody.isKinematic = true;
    }

    //quand le sort est lancé, on lance le compte à rebours de la durée de vie
    public void LancementSort()
    {
        sortRigidbody.isKinematic = false;
        encours = EtatSort.LANCE;
        popTime = Time.time;
    }
}
