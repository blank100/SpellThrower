using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    private EtatMonstre etat = EtatMonstre.POOLING;

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
        }
    }

    public void ActivationMonstre()
    {
        gameObject.SetActive(true);
    }

    public void DesactivationMonstre()
    {
        gameObject.SetActive(false);
    }

    public EtatMonstre GetEtatMonstre()
    {
        return etat;
    }

    public Sort GetFaiblesse() 
    {
        return faiblesse;
    }

}
