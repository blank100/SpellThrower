using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap;
using LeapInternal;

public enum EtatMain
{
    MAINFERMEE,
    MAINOUVERTE,
    INDEXONLY,
    ALLBUTTHUMB,
    SORTENCOURS,
    LANCEMENT,
    MAINSPARALLELESY,
    MAINSPARALLELESX,
    ANNULATION,
    PERDUE
}

public enum StarterSpell
{
    SORTINDEX,
    SORTPAUME,
    SORTDOSMAIN,
    ZERO
}

public class DetectionTestScript : MonoBehaviour
{
    [Header("Composants Main droite")]
    [SerializeField] private Transform paumeMainD;
    [SerializeField] private Transform pouceMainD;
    [SerializeField] private Transform indexMainD;
    [SerializeField] private Transform majeurMainD;
    [SerializeField] private Transform annulaireMainD;
    [SerializeField] private Transform auriculaireMainD;
    [SerializeField] private Rigidbody rigidbodyMainD;
    [SerializeField] private Transform viseeD;

    [Header ("Composants Main gauche")]
    [SerializeField] private Transform paumeMainG;
    [SerializeField] private Transform pouceMainG;
    [SerializeField] private Transform indexMainG;
    [SerializeField] private Transform majeurMainG;
    [SerializeField] private Transform annulaireMainG;
    [SerializeField] private Transform auriculaireMainG;
    [SerializeField] private Rigidbody rigidbodyMainG;
    [SerializeField] private Transform viseeG;

    [Header("Offsets Main droite")]
    [SerializeField] private Vector3 offsetPaumeD;
    [SerializeField] private Vector3 offsetIndexD;
    [SerializeField] private Vector3 offsetDosMainD;

    [Header ("Offsets Main gauche")]
    [SerializeField] private Vector3 offsetPaumeG;
    [SerializeField] private Vector3 offsetIndexG;
    [SerializeField] private Vector3 offsetDosMainG;

    [Header("Paramétrage des fourchettes")]
    [SerializeField] private float seuilPouce;
    [SerializeField] private float seuilIndex;
    [SerializeField] private float seuilMajeur;
    [SerializeField] private float seuilAnnulaire;
    [SerializeField] private float seuilAuriculaire;
    [SerializeField] private float seuilVitesseLancement;
    [SerializeField] private float seuilVitesseDepart;

    [Header("Références")]
    [SerializeField] private Pooling poolingManager;

    private Controller controller;

    private EtatMain currentStateMainD;
    private EtatMain currentStateMainG;
    private EtatMain previousStateMainD;
    private EtatMain previousStateMainG;

    private StarterSpell starterMainD = StarterSpell.ZERO;
    private StarterSpell starterMainG = StarterSpell.ZERO;

    private ExposerSort currentSpellMainD;
    private ExposerSort currentSpellMainG;

    private Vector3 currentPositionMainD;
    private Vector3 previousPositionMainD;

    private Vector3 currentPositionMainG;
    private Vector3 previousPositionMainG;

    private float velociteMainD;
    private float velociteMainG;
    private int nbMainsDetectees;

    private bool mainParalleles = false;
    private bool mainParallelesGauche = false;
    private bool bouleDeFeu = true;
    private bool bouleDeLaMort = true;
    private bool hadooken = true;
    private bool laser = true;

    // Start is called before the first frame update
    void Start()
    {
        controller = new Controller();

        currentStateMainD = EtatMain.MAINOUVERTE;
        previousStateMainD = EtatMain.MAINOUVERTE;

        currentStateMainG = EtatMain.MAINOUVERTE;
        previousStateMainG = EtatMain.MAINOUVERTE;

        currentPositionMainD = paumeMainD.transform.position;
        previousPositionMainD = currentPositionMainD;

        currentPositionMainG = paumeMainG.transform.position;
        previousPositionMainG = currentPositionMainG;
    }

    // Update is called once per frame
    void Update()
    {
        previousPositionMainD = currentPositionMainD;
        currentPositionMainD = paumeMainD.transform.position;

        previousPositionMainG = currentPositionMainG;
        currentPositionMainG = paumeMainG.transform.position;

        var v = currentPositionMainD - previousPositionMainD;
        v /= Time.deltaTime;
        velociteMainD = v.magnitude * 10;

        var v2 = currentPositionMainG - previousPositionMainG;
        v2 /= Time.deltaTime;
        velociteMainG = v2.magnitude * 10;

        Frame currentFrame = controller.Frame();

        nbMainsDetectees = currentFrame.Hands.Count;

        if (currentFrame.Hands.Count < 2)
        {
            if(currentFrame.Hands.Count == 0)
            {
                currentStateMainG = EtatMain.PERDUE;
                previousStateMainG = EtatMain.PERDUE;
                if (currentSpellMainG != null) currentSpellMainG.AnnulationSort();
                currentSpellMainG = null;

                currentStateMainD = EtatMain.PERDUE;
                previousStateMainD = EtatMain.PERDUE;
                if (currentSpellMainD != null) currentSpellMainD.AnnulationSort();
                currentSpellMainD = null;
            }
            else
            {
                if (currentFrame.Hands[0].IsRight)
                {
                    currentStateMainG = EtatMain.PERDUE;
                    previousStateMainG = EtatMain.PERDUE;
                    if (currentSpellMainG != null) currentSpellMainG.AnnulationSort();
                    currentSpellMainG = null;
                }
                else
                {
                    currentStateMainD = EtatMain.PERDUE;
                    previousStateMainD = EtatMain.PERDUE;
                    if (currentSpellMainD != null) currentSpellMainD.AnnulationSort();
                    currentSpellMainD = null;
                }
            }
            
        }

        //si on a au moins une main de détectée
        if (currentFrame.Hands.Count > 0)
        {
            
            foreach(var main in currentFrame.Hands)
            {
                if (main.IsRight) DetermineEtatMain(true);
                else DetermineEtatMain(false);
            }

            //gestion des sorts en fonction des états déterminés précédemment
            GestionSort();
        }
    }

    //récupérer le nombre de mains actuellement détectées
    public int getNbMainsDetectees()
    {
        return nbMainsDetectees;
    }

    //Appliquer les changements du niveau courant
    public void LireConfigNiveau(Level lv)
    {
        if(lv.tousLesSorts)
        {
            bouleDeFeu = true;
            bouleDeLaMort = true;
            hadooken = true;
            laser = true;
        }
        else
        {
            bouleDeFeu = lv.bouleDeFeu;
            bouleDeLaMort = lv.bouleDeLaMort;
            hadooken = lv.hadooken;
            laser = lv.laser;
        }      
    }

    //fonction pour indiquer que les mains sont parallèles avec la main droite en bas
    public void SetMainParalleles(bool b)
    {
        mainParalleles = b;
        mainParallelesGauche = !b;
    }

    //fonction pour indiquer que les mains sont parallèles avec la main gauche en bas
    public void SetMainParallelesGauche(bool b)
    {
        mainParallelesGauche = b;
        mainParalleles = !b;
    }

    public void ResetMainsParalleles()
    {
        mainParalleles = false;
        mainParallelesGauche = false;
    }

    //fonction de tir du sort
    public void LancerSort(bool droit)
    {
        //lancer avec la main droite
        if(droit)
        {
            switch(starterMainD)
            {
                case StarterSpell.SORTPAUME:
                    var rb = currentSpellMainD.getRigidbody();
                    currentSpellMainD.transform.SetParent(null);
                    rb.isKinematic = false;
                    rb.AddForce(viseeD.right * currentSpellMainD.GetVitesse(), ForceMode.Acceleration);
                    currentSpellMainD.LancementSort();
                    currentSpellMainD = null;
                    break;

                case StarterSpell.SORTINDEX:

                    var rbindex = currentSpellMainD.getRigidbody();
                    currentSpellMainD.transform.SetParent(null);
                    rbindex.isKinematic = false;
                    rbindex.AddForce(indexMainD.transform.right * currentSpellMainD.GetVitesse(), ForceMode.Acceleration);
                    currentSpellMainD.LancementSort();
                    currentSpellMainD = null;
                    break;

                case StarterSpell.SORTDOSMAIN:

                    var rbdos = currentSpellMainD.getRigidbody();
                    currentSpellMainD.transform.SetParent(null);
                    rbdos.isKinematic = false;
                    rbdos.AddForce(paumeMainD.transform.right * currentSpellMainD.GetVitesse(), ForceMode.Acceleration);
                    currentSpellMainD.LancementSort();
                    currentSpellMainD = null;
                    break;
            }
        }
        else
        {
            //lancer avec la main gauche
            switch(starterMainG)
            {
                case StarterSpell.SORTPAUME:
                    var rb = currentSpellMainG.getRigidbody();
                    currentSpellMainG.transform.SetParent(null);
                    rb.isKinematic = false;
                    rb.AddForce(-viseeG.right * currentSpellMainG.GetVitesse(), ForceMode.Acceleration);
                    currentSpellMainG.LancementSort();
                    currentSpellMainG = null;
                    break;

                case StarterSpell.SORTINDEX:

                    var rbindex = currentSpellMainG.getRigidbody();
                    currentSpellMainG.transform.SetParent(null);
                    rbindex.isKinematic = false;
                    rbindex.AddForce(-indexMainG.transform.right * currentSpellMainG.GetVitesse(), ForceMode.Acceleration);
                    currentSpellMainG.LancementSort();
                    currentSpellMainG = null;
                    break;

                case StarterSpell.SORTDOSMAIN:

                    var rbdos = currentSpellMainG.getRigidbody();
                    currentSpellMainG.transform.SetParent(null);
                    rbdos.isKinematic = false;
                    rbdos.AddForce(-paumeMainG.transform.right * currentSpellMainG.GetVitesse(), ForceMode.Acceleration);
                    currentSpellMainG.LancementSort();
                    currentSpellMainG = null;
                    break;
            }
        }
    }

    //fonction pour déterminer l'état de la main
    public void DetermineEtatMain(bool droit)
    {
        //déterminer si les mains sont parallèles et ouvertes
        if((mainParalleles || mainParallelesGauche) && currentStateMainD == EtatMain.MAINOUVERTE && currentStateMainG == EtatMain.MAINOUVERTE)
        {
            previousStateMainD = currentStateMainD;
            currentStateMainD = EtatMain.MAINSPARALLELESY;

            previousStateMainG = currentStateMainG;
            currentStateMainG = EtatMain.MAINSPARALLELESY;

            return;
        }

        if(droit)
        {
            if(currentStateMainD == EtatMain.LANCEMENT)
            {
                if (velociteMainD < seuilVitesseLancement)
                {
                    LancerSort(true);
                    previousStateMainD = currentStateMainD;
                    currentStateMainD = EtatMain.ANNULATION;
                    return;
                }
                return;
            }

            //s'il n'y a aucun sort dans la main
            if (currentStateMainD != EtatMain.SORTENCOURS)
            {
                //récupération des distances entre les doigts et le centre de la paume
                List<float> listeDistance = new List<float>();
                listeDistance.Add(Vector3.Distance(paumeMainD.transform.position, pouceMainD.position));
                listeDistance.Add(Vector3.Distance(paumeMainD.transform.position, indexMainD.position));
                listeDistance.Add(Vector3.Distance(paumeMainD.transform.position, majeurMainD.position));
                listeDistance.Add(Vector3.Distance(paumeMainD.transform.position, annulaireMainD.position));
                listeDistance.Add(Vector3.Distance(paumeMainD.transform.position, auriculaireMainD.position));

                if (listeDistance[0] > seuilPouce && listeDistance[1] > seuilIndex && listeDistance[2] > seuilMajeur && listeDistance[3] > seuilAnnulaire && listeDistance[4] > seuilAuriculaire)
                {
                    previousStateMainD = currentStateMainD;
                    currentStateMainD = EtatMain.MAINOUVERTE;
                    return;
                }
                else
                {
                    if (listeDistance[0] < seuilPouce && listeDistance[1] > seuilIndex && listeDistance[2] < seuilMajeur && listeDistance[3] < seuilAnnulaire && listeDistance[4] < seuilAuriculaire)
                    {
                        //si la main est fermée suite à une annulation, l'état ne change pas
                        if (currentStateMainD == EtatMain.ANNULATION && (previousStateMainD == EtatMain.ANNULATION || previousStateMainD == EtatMain.SORTENCOURS)) return;

                        previousStateMainD = currentStateMainD;
                        currentStateMainD = EtatMain.INDEXONLY;
                        return;
                    }
                    else
                    {
                        if (listeDistance[0] < seuilPouce && listeDistance[1] < seuilIndex && listeDistance[2] < seuilMajeur && listeDistance[3] < seuilAnnulaire && listeDistance[4] < seuilAuriculaire)
                        {
                            //si la main est fermée suite à une annulation, l'état ne change pas
                            if (currentStateMainD == EtatMain.ANNULATION && (previousStateMainD == EtatMain.ANNULATION || previousStateMainD == EtatMain.SORTENCOURS)) return;

                            previousStateMainD = currentStateMainD;
                            currentStateMainD = EtatMain.MAINFERMEE;
                            return;
                        }
                        else
                        {
                            if(listeDistance[0] < seuilPouce && listeDistance[1] > seuilIndex && listeDistance[2] > seuilMajeur && listeDistance[3] > seuilAnnulaire && listeDistance[4] > seuilAuriculaire)
                            {
                                if (currentStateMainD == EtatMain.ANNULATION && (previousStateMainD == EtatMain.ANNULATION || previousStateMainD == EtatMain.SORTENCOURS)) return;

                                previousStateMainD = currentStateMainD;
                                currentStateMainD = EtatMain.ALLBUTTHUMB;
                                return;
                            }
                            else
                            {
                                //par défaut la main est ouverte
                                previousStateMainD = currentStateMainD;
                                currentStateMainD = EtatMain.MAINOUVERTE;
                                return;
                            }
                        }
                    }
                }
            }
            else
            {
                //s'il y a un sort dans la main droite
                //récupération des distances entre les doigts et le centre de la paume
                List<float> listeDistance = new List<float>();
                listeDistance.Add(Vector3.Distance(paumeMainD.transform.position, pouceMainD.position));
                listeDistance.Add(Vector3.Distance(paumeMainD.transform.position, indexMainD.position));
                listeDistance.Add(Vector3.Distance(paumeMainD.transform.position, majeurMainD.position));
                listeDistance.Add(Vector3.Distance(paumeMainD.transform.position, annulaireMainD.position));
                listeDistance.Add(Vector3.Distance(paumeMainD.transform.position, auriculaireMainD.position));

                if (listeDistance[0] < seuilPouce && listeDistance[1] < seuilIndex && listeDistance[2] < seuilMajeur && listeDistance[3] < seuilAnnulaire && listeDistance[4] < seuilAuriculaire)
                {
                    previousStateMainD = EtatMain.ANNULATION;
                    currentStateMainD = EtatMain.ANNULATION;
                    return;
                }

                //si on dépasse une certaine vélocité, on passe en lancement
                if(velociteMainD > seuilVitesseLancement)
                {
                    previousStateMainD = currentStateMainD;
                    currentStateMainD = EtatMain.LANCEMENT;
                }
            }
        }
        else
        {
            if (currentStateMainG == EtatMain.LANCEMENT)
            {
                if (velociteMainG < seuilVitesseDepart)
                {
                    LancerSort(false);
                    previousStateMainG = currentStateMainG;
                    currentStateMainG = EtatMain.MAINOUVERTE;
                    return;
                }
                return;
            }

            //s'il n'y a aucun sort dans la main
            if (currentStateMainG != EtatMain.SORTENCOURS)
            {
                //récupération des distances entre les doigts et le centre de la paume
                List<float> listeDistance = new List<float>();
                listeDistance.Add(Vector3.Distance(paumeMainG.transform.position, pouceMainG.position));
                listeDistance.Add(Vector3.Distance(paumeMainG.transform.position, indexMainG.position));
                listeDistance.Add(Vector3.Distance(paumeMainG.transform.position, majeurMainG.position));
                listeDistance.Add(Vector3.Distance(paumeMainG.transform.position, annulaireMainG.position));
                listeDistance.Add(Vector3.Distance(paumeMainG.transform.position, auriculaireMainG.position));

                //analyse des résultats
                if (listeDistance[0] > seuilPouce && listeDistance[1] > seuilIndex && listeDistance[2] > seuilMajeur && listeDistance[3] > seuilAnnulaire && listeDistance[4] > seuilAuriculaire)
                {
                    previousStateMainG = currentStateMainG;
                    currentStateMainG = EtatMain.MAINOUVERTE;
                    return;
                }
                else
                {
                    if (listeDistance[0] < seuilPouce && listeDistance[1] > seuilIndex && listeDistance[2] < seuilMajeur && listeDistance[3] < seuilAnnulaire && listeDistance[4] < seuilAuriculaire)
                    {
                        if (currentStateMainG == EtatMain.ANNULATION && (previousStateMainG == EtatMain.ANNULATION || previousStateMainG == EtatMain.SORTENCOURS)) return;

                        previousStateMainG = currentStateMainG;
                        currentStateMainG = EtatMain.INDEXONLY;
                        return;
                    }
                    else
                    {
                        if (listeDistance[0] < seuilPouce && listeDistance[1] < seuilIndex && listeDistance[2] < seuilMajeur && listeDistance[3] < seuilAnnulaire && listeDistance[4] < seuilAuriculaire)
                        {
                            if (currentStateMainG == EtatMain.ANNULATION && (previousStateMainG == EtatMain.ANNULATION || previousStateMainG == EtatMain.SORTENCOURS)) return;

                            previousStateMainG = currentStateMainG;
                            currentStateMainG = EtatMain.MAINFERMEE;
                            return;
                        }
                        else
                        {
                            if(listeDistance[0] < seuilPouce && listeDistance[1] > seuilIndex && listeDistance[2] > seuilMajeur && listeDistance[3] > seuilAnnulaire && listeDistance[4] > seuilAuriculaire)
                            {
                                if (currentStateMainG == EtatMain.ANNULATION && (previousStateMainG == EtatMain.ANNULATION || previousStateMainG == EtatMain.SORTENCOURS)) return;

                                previousStateMainG = currentStateMainG;
                                currentStateMainG = EtatMain.ALLBUTTHUMB;
                                return;
                            }

                            else
                            {
                                //par défaut la main est ouverte
                                previousStateMainG = currentStateMainG;
                                currentStateMainG = EtatMain.MAINOUVERTE;
                                return;
                            }
                        }
                    }
                }
            }
            else
            {
                //s'il y a un sort dans la main gauche
                //récupération des distances entre les doigts et le centre de la paume
                List<float> listeDistance = new List<float>();
                listeDistance.Add(Vector3.Distance(paumeMainG.transform.position, pouceMainG.position));
                listeDistance.Add(Vector3.Distance(paumeMainG.transform.position, indexMainG.position));
                listeDistance.Add(Vector3.Distance(paumeMainG.transform.position, majeurMainG.position));
                listeDistance.Add(Vector3.Distance(paumeMainG.transform.position, annulaireMainG.position));
                listeDistance.Add(Vector3.Distance(paumeMainG.transform.position, auriculaireMainG.position));

                if (listeDistance[0] < seuilPouce && listeDistance[1] < seuilIndex && listeDistance[2] < seuilMajeur && listeDistance[3] < seuilAnnulaire && listeDistance[4] < seuilAuriculaire)
                {
                    previousStateMainG = EtatMain.ANNULATION;
                    currentStateMainG = EtatMain.ANNULATION;
                    return;
                }

                //si on dépasse une certaine vélocité, on passe en lancement
                if (velociteMainG > seuilVitesseLancement)
                {
                    previousStateMainG = currentStateMainG;
                    currentStateMainG = EtatMain.LANCEMENT;
                }
            }
        }
    }

    //après avoir déterminer les états des mains, on gère les sorts
    public void GestionSort()
    {
        if(currentStateMainD == EtatMain.ANNULATION)
        {
            if(currentSpellMainD != null) currentSpellMainD.AnnulationSort();
            return;
        }

        if(currentStateMainG == EtatMain.ANNULATION)
        {
            if(currentSpellMainG != null) currentSpellMainG.AnnulationSort();
            return;
        }

        //mains parallèles verticalement
        if(currentStateMainD == currentStateMainG && currentStateMainD == EtatMain.MAINSPARALLELESY)
        {
            if(mainParalleles && hadooken)
            {
                //instancier le sort dans la main droite
                currentSpellMainD = poolingManager.Incantation(Sort.HADOOKEN, paumeMainD, offsetPaumeD);
                currentSpellMainD.Incantation();
                starterMainD = StarterSpell.SORTPAUME;
                currentStateMainD = EtatMain.SORTENCOURS;
            }
            else
            {
                if(mainParallelesGauche && hadooken)
                {
                    //instancier le sort dans la main gauche
                    currentSpellMainG = poolingManager.Incantation(Sort.HADOOKEN, paumeMainG, offsetPaumeG);
                    currentSpellMainG.Incantation();
                    starterMainG = StarterSpell.SORTPAUME;
                    currentStateMainG = EtatMain.SORTENCOURS;
                }
            }
            mainParalleles = false;
            mainParallelesGauche = false;
            return;
        }

        //Main Droite
        if(currentStateMainD == EtatMain.MAINOUVERTE && previousStateMainD == EtatMain.MAINFERMEE && bouleDeFeu)
        {
            //instancier le sort dans la main droite
            currentSpellMainD = poolingManager.Incantation(Sort.BOULEDEFEU, paumeMainD, offsetPaumeD);
            currentSpellMainD.Incantation();
            currentStateMainD = EtatMain.SORTENCOURS;
            starterMainD = StarterSpell.SORTPAUME;
        }
        else
        {
            if(currentStateMainD == EtatMain.INDEXONLY && (previousStateMainD == EtatMain.MAINOUVERTE || previousStateMainD == EtatMain.MAINFERMEE) && bouleDeLaMort)
            {
                //instancier le sort dans la main droite
                currentSpellMainD = poolingManager.Incantation(Sort.BOULEDELAMORT, indexMainD, offsetIndexD);
                currentSpellMainD.Incantation();
                currentStateMainD = EtatMain.SORTENCOURS;
                starterMainD = StarterSpell.SORTINDEX;
            }
            else
            {
                if(currentStateMainD == EtatMain.ALLBUTTHUMB && (previousStateMainD == EtatMain.MAINOUVERTE || previousStateMainD == EtatMain.MAINFERMEE) && laser)
                {
                    currentSpellMainD = poolingManager.Incantation(Sort.LASER, paumeMainD, offsetDosMainD);
                    currentSpellMainD.Incantation();
                    currentStateMainD = EtatMain.SORTENCOURS;
                    starterMainD = StarterSpell.SORTDOSMAIN;
                }
            }
        }

        //Main Gauche
        if(currentStateMainG == EtatMain.MAINOUVERTE && previousStateMainG == EtatMain.MAINFERMEE && bouleDeFeu)
        {
            //instancier le sort dans la main gauche
            currentSpellMainG = poolingManager.Incantation(Sort.BOULEDEFEU, paumeMainG, offsetPaumeG);
            currentSpellMainG.Incantation();
            currentStateMainG = EtatMain.SORTENCOURS;
            starterMainG = StarterSpell.SORTPAUME;
        }
        else
        {
            if (currentStateMainG == EtatMain.INDEXONLY && (previousStateMainG == EtatMain.MAINOUVERTE || previousStateMainG == EtatMain.MAINFERMEE) && bouleDeLaMort)
            {
                //instancier le sort dans la main gauche
                currentSpellMainG = poolingManager.Incantation(Sort.BOULEDELAMORT, indexMainG, offsetIndexG);
                currentSpellMainG.Incantation();
                currentStateMainG = EtatMain.SORTENCOURS;
                starterMainG = StarterSpell.SORTINDEX;
            }
            else
            {
                if (currentStateMainG == EtatMain.ALLBUTTHUMB && (previousStateMainG == EtatMain.MAINOUVERTE || previousStateMainG == EtatMain.MAINFERMEE) && laser)
                {
                    currentSpellMainG = poolingManager.Incantation(Sort.LASER, paumeMainG, offsetDosMainG);
                    currentSpellMainG.Incantation();
                    currentStateMainG = EtatMain.SORTENCOURS;
                    starterMainG = StarterSpell.SORTDOSMAIN;
                }
            }
        }
    }
}
