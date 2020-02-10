using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap;
using LeapInternal;

public class DetectionMainParalleles : MonoBehaviour
{
    [SerializeField] private DetectionTestScript detectionScript;
    [SerializeField] private GameObject mainD;
    [SerializeField] private GameObject mainG;

    private int nbMainsDetectees = 0;

    private void OnTriggerEnter(Collider other)
    {
        if (nbMainsDetectees < 2) return;

        if(transform.up.y > 0.8)
        {
            //si le collider est l'autre main alors les mains sont parallèles main gauche vers le bas
            if (other.gameObject.layer == 9 && other.transform.up.y > 0)
            {
                if(mainD.activeSelf && mainG.activeSelf)
                {
                    detectionScript.SetMainParallelesGauche(true);
                }
            }
        }
        else
        {
            //si le collider est l'autre main alors les mains sont parallèles mais droite vers le bas
            if (other.gameObject.layer == 9 && other.transform.up.y < -0.8)
            {
                if (mainD.activeSelf && mainG.activeSelf)
                {
                    detectionScript.SetMainParalleles(true);
                }
            }
        }   
    }

    private void OnTriggerExit(Collider other)
    {
        //si le collider est l'autre main alors les mains ne sont plus parallèles
        if (other.gameObject.layer == 9)
        {
            detectionScript.ResetMainsParalleles();
        }
    }

    private void FixedUpdate()
    {
        nbMainsDetectees = detectionScript.getNbMainsDetectees();
    }
}
