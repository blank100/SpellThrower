using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectionMainParalleles : MonoBehaviour
{
    [SerializeField] private DetectionTestScript detectionScript;

    private void OnTriggerEnter(Collider other)
    {
        if(transform.up.y > 0)
        {
            
            //si le collider est l'autre main alors les mains sont parallèles main gauche vers le bas
            if (other.gameObject.layer == 9 && other.transform.up.y > 0)
            {
                Debug.Log("MAIN DROITE EN HAUT");
                detectionScript.SetMainParallelesGauche(true);
            }
        }
        else
        {
            
            //si le collider est l'autre main alors les mains sont parallèles mais droite vers le bas
            if (other.gameObject.layer == 9 && other.transform.up.y < 0)
            {
                Debug.Log("MAIN DROITE EN BAS");
                detectionScript.SetMainParalleles(true);
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
}
