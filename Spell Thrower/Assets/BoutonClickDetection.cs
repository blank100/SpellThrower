using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TypeBouton
{
    PLAY,
    QUITTER,
    STORY,
    INFINITE
}

public class BoutonClickDetection : MonoBehaviour
{
    [SerializeField] private InterfaceManager manager;
    [SerializeField] private TypeBouton typeBouton;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Selecteur")
        {
            switch(typeBouton)
            {
                case TypeBouton.PLAY:

                    //afficher le menu pour choisir le type de partie
                    manager.AfficherMenuChoix();
                    break;

                case TypeBouton.QUITTER:

                    //fermer le jeu
                    manager.Quitter();
                    break;

                case TypeBouton.STORY:

                    //lancer partie en mode histoire
                    manager.LancerPartieHistoire();
                    break;

                case TypeBouton.INFINITE:

                    //lancer partie en mode infini
                    manager.LancerPartieInfinie();
                    break;

                default:
                    break;
            }
        }
    }
}
