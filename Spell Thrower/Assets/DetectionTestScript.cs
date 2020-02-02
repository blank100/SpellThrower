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
    SORTENCOURS,
    LANCEMENT,
    MAINSPARALLELESY,
    MAINSPARALLELESX,
    ANNULATION
}

public enum StarterSpell
{
    SORTINDEX,
    SORTPAUME,
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

    [Header ("Composants Main gauche")]
    [SerializeField] private Transform paumeMainG;
    [SerializeField] private Transform pouceMainG;
    [SerializeField] private Transform indexMainG;
    [SerializeField] private Transform majeurMainG;
    [SerializeField] private Transform annulaireMainG;
    [SerializeField] private Transform auriculaireMainG;
    [SerializeField] private Rigidbody rigidbodyMainG;

    [Header("Offsets Main droite")]
    [SerializeField] private Vector3 offsetPaumeD;
    [SerializeField] private Vector3 offsetIndexD;

    [Header ("Offsets Main gauche")]
    [SerializeField] private Vector3 offsetPaumeG;
    [SerializeField] private Vector3 offsetIndexG;

    [Header("Paramétrage des fourchettes")]
    [SerializeField] private float seuilPouce;
    [SerializeField] private float seuilIndex;
    [SerializeField] private float seuilMajeur;
    [SerializeField] private float seuilAnnulaire;
    [SerializeField] private float seuilAuriculaire;
    [SerializeField] private float seuilVitesseLancement;

    [Header("Sorts")]
    [SerializeField] List<GameObject> listeSorts;

    private Controller controller;

    private EtatMain currentStateMainD;
    private EtatMain currentStateMainG;
    private EtatMain previousStateMainD;
    private EtatMain previousStateMainG;

    private StarterSpell starterMainD = StarterSpell.ZERO;
    private StarterSpell starterMainG = StarterSpell.ZERO;

    private GameObject currentSpellMainD;
    private GameObject currentSpellMainG;

    private Vector3 currentPositionMainD;
    private Vector3 previousPositionMainD;

    private Vector3 currentPositionMainG;
    private Vector3 previousPositionMainG;

    private float velociteMainD;
    private float velociteMainG;

    private bool mainParalleles = false;
    private bool mainParallelesGauche = false;
    private bool bouleDeFeu = false;
    private bool bouleDeLaMort = false;
    private bool hadooken = false;

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

    private void FixedUpdate()
    {
        previousPositionMainD = currentPositionMainD;
        currentPositionMainD = paumeMainD.transform.position;

        previousPositionMainG = currentPositionMainG;
        currentPositionMainG = paumeMainD.transform.position;

        //velociteMainD = Vector3.Distance(previousPositionMainD, currentPositionMainD) * Time.deltaTime * 100000;
        //velociteMainG = Vector3.Distance(previousPositionMainG, currentPositionMainG) * Time.deltaTime * 100000;
        var v = currentPositionMainD - previousPositionMainD;
        v /= Time.deltaTime;
        velociteMainD = v.magnitude * 10;
    }

    // Update is called once per frame
    void Update()
    {
        Frame currentFrame = controller.Frame();

        //si on a au moins une main de détectée
        if (currentFrame.Hands.Count > 0)
        {
            foreach(var main in currentFrame.Hands)
            {
                if (main.IsLeft) DetermineEtatMain(false);
                else DetermineEtatMain(true);
            }

            Debug.Log(currentStateMainD + "---------" + previousStateMainD + "_________" + hadooken);
         
            //gestion des sorts en fonction des états déterminés précédemment
            GestionSort();
        }
    }

    //Appliquer les changements du niveau courant
    public void LireConfigNiveau(Level lv)
    {
        if(lv.tousLesSorts)
        {
            bouleDeFeu = true;
            bouleDeLaMort = true;
            hadooken = true;
        }
        else
        {
            bouleDeFeu = lv.bouleDeFeu;
            bouleDeLaMort = lv.bouleDeLaMort;
            hadooken = lv.hadooken;
        }      
    }

    //fonction pour remettre une main à l'état de départ quand celle-ci est perdue par le tracking
    public void ResetEtatMain(bool droit)
    {
        if(droit)
        {
            currentStateMainD = EtatMain.MAINOUVERTE;
            previousStateMainD = EtatMain.MAINOUVERTE;
        }
        else
        {
            currentStateMainG = EtatMain.MAINOUVERTE;
            previousStateMainG = EtatMain.MAINOUVERTE;
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
                    var rb = currentSpellMainD.GetComponent<Rigidbody>();
                    currentSpellMainD.transform.SetParent(null);
                    rb.isKinematic = false;
                    rb.AddForce((-paumeMainD.transform.up + new Vector3(0, 0, 0.25f)) * 100, ForceMode.Acceleration);
                    currentSpellMainD = null;
                    break;

                case StarterSpell.SORTINDEX:

                    var rbindex = currentSpellMainD.GetComponent<Rigidbody>();
                    currentSpellMainD.transform.SetParent(null);
                    rbindex.isKinematic = false;
                    rbindex.AddForce(indexMainD.transform.right * 100, ForceMode.Acceleration);
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
                    var rb = currentSpellMainG.GetComponent<Rigidbody>();
                    currentSpellMainG.transform.SetParent(null);
                    rb.isKinematic = false;
                    rb.AddForce((paumeMainG.transform.up + new Vector3(0, 0, 0.25f)) * 100, ForceMode.Acceleration);
                    currentSpellMainG = null;
                    break;

                case StarterSpell.SORTINDEX:

                    var rbindex = currentSpellMainG.GetComponent<Rigidbody>();
                    currentSpellMainG.transform.SetParent(null);
                    rbindex.isKinematic = false;
                    rbindex.AddForce(indexMainG.transform.up * 100, ForceMode.Acceleration);
                    currentSpellMainG = null;
                    break;
            }
        }
    }

    //fonction pour déterminer l'état de la main
    public void DetermineEtatMain(bool droit)
    {
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
                        if (currentStateMainD == EtatMain.ANNULATION && previousStateMainD == EtatMain.ANNULATION) return;

                        previousStateMainD = currentStateMainD;
                        currentStateMainD = EtatMain.INDEXONLY;
                        return;
                    }
                    else
                    {
                        if (listeDistance[0] < seuilPouce && listeDistance[1] < seuilIndex && listeDistance[2] < seuilMajeur && listeDistance[3] < seuilAnnulaire && listeDistance[4] < seuilAuriculaire)
                        {
                            //si la main est fermée suite à une annulation, l'état ne change pas
                            if (currentStateMainD == EtatMain.ANNULATION && previousStateMainD == EtatMain.ANNULATION) return;

                            previousStateMainD = currentStateMainD;
                            currentStateMainD = EtatMain.MAINFERMEE;
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
                if (velociteMainG < seuilVitesseLancement)
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
                        previousStateMainG = currentStateMainG;
                        currentStateMainG = EtatMain.INDEXONLY;
                        return;
                    }
                    else
                    {
                        if (listeDistance[0] < seuilPouce && listeDistance[1] < seuilIndex && listeDistance[2] < seuilMajeur && listeDistance[3] < seuilAnnulaire && listeDistance[4] < seuilAuriculaire)
                        {
                            if (currentStateMainG == EtatMain.ANNULATION && previousStateMainG == EtatMain.ANNULATION) return;

                            previousStateMainG = currentStateMainG;
                            currentStateMainG = EtatMain.MAINFERMEE;
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
                if (velociteMainG > 1f)
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
            if(currentSpellMainD != null) Destroy(currentSpellMainD);
            return;
        }

        if(currentStateMainG == EtatMain.ANNULATION)
        {
            if(currentSpellMainG != null) Destroy(currentSpellMainG);
            return;
        }

        //mains parallèles verticalement
        if(currentStateMainD == currentStateMainG && currentStateMainD == EtatMain.MAINSPARALLELESY)
        {
            if(mainParalleles && hadooken)
            {
                //instancier le sort dans la main droite
                var sort = Instantiate<GameObject>(listeSorts[2]);
                sort.transform.SetParent(paumeMainD);
                sort.transform.localPosition = Vector3.zero + offsetPaumeD;
                sort.transform.localRotation = new Quaternion(0, 0, 0, 0);
                currentSpellMainD = sort;
                starterMainD = StarterSpell.SORTPAUME;
                currentStateMainD = EtatMain.SORTENCOURS;
            }
            else
            {
                if(mainParallelesGauche && hadooken)
                {
                    //instancier le sort dans la main gauche
                    var sort = Instantiate<GameObject>(listeSorts[2]);
                    sort.transform.SetParent(paumeMainG);
                    sort.transform.localPosition = Vector3.zero + offsetPaumeG;
                    sort.transform.localRotation = new Quaternion(0, 0, 0, 0);
                    currentSpellMainG = sort;
                    starterMainG = StarterSpell.SORTPAUME;
                    currentStateMainG = EtatMain.SORTENCOURS;
                }
            }
            return;
        }

        //Main Droite
        if(currentStateMainD == EtatMain.MAINOUVERTE && previousStateMainD == EtatMain.MAINFERMEE && bouleDeFeu)
        {
            //instancier le sort dans la main droite
            currentStateMainD = EtatMain.SORTENCOURS;
            var sort = Instantiate<GameObject>(listeSorts[0]);
            sort.transform.SetParent(paumeMainD);
            sort.transform.localPosition = Vector3.zero + offsetPaumeD;
            sort.transform.localRotation = new Quaternion(0, 0, 0, 0);
            currentSpellMainD = sort;
            starterMainD = StarterSpell.SORTPAUME;
        }
        else
        {
            if(currentStateMainD == EtatMain.INDEXONLY && (previousStateMainD == EtatMain.MAINOUVERTE || previousStateMainD == EtatMain.MAINFERMEE) && bouleDeLaMort)
            {
                //instancier le sort dans la main droite
                currentStateMainD = EtatMain.SORTENCOURS;
                var sort = Instantiate<GameObject>(listeSorts[1]);
                sort.transform.SetParent(indexMainD);
                sort.transform.localPosition = Vector3.zero + offsetIndexD;
                sort.transform.localRotation = new Quaternion(0, 0, 0, 0);
                currentSpellMainD = sort;
                starterMainD = StarterSpell.SORTINDEX;
            }
        }

        //Main Gauche
        if(currentStateMainG == EtatMain.MAINOUVERTE && previousStateMainG == EtatMain.MAINFERMEE && bouleDeFeu)
        {
            //instancier le sort dans la main gauche
            currentStateMainG = EtatMain.SORTENCOURS;
            var sort = Instantiate<GameObject>(listeSorts[0]);
            sort.transform.SetParent(paumeMainG);
            sort.transform.localPosition = offsetPaumeG;
            currentSpellMainG = sort;
            starterMainG = StarterSpell.SORTPAUME;
        }
        else
        {
            if (currentStateMainG == EtatMain.INDEXONLY && (previousStateMainG == EtatMain.MAINOUVERTE || previousStateMainG == EtatMain.MAINFERMEE) && bouleDeLaMort)
            {
                //instancier le sort dans la main gauche
                currentStateMainG = EtatMain.SORTENCOURS;
                var sort = Instantiate<GameObject>(listeSorts[1]);
                sort.transform.SetParent(indexMainG);
                sort.transform.localPosition = Vector3.zero + offsetIndexG;
                sort.transform.localRotation = new Quaternion(0, 0, 0, 0);
                currentSpellMainG = sort;
                starterMainG = StarterSpell.SORTINDEX;
            }
        }
    }
}
