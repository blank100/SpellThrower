using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pooling : MonoBehaviour
{
    [Header("Prefabs des sorts")]
    [SerializeField] private GameObject bouleDeFeu;
    [SerializeField] private GameObject bouleDeLaMort;
    [SerializeField] private GameObject hadooken;
    [SerializeField] private GameObject laser;

    [Header("Prefabs des monstres")]
    [SerializeField] private GameObject monstreFeu;
    [SerializeField] private GameObject monstreMort;
    [SerializeField] private GameObject monstreHadooken;
    [SerializeField] private GameObject monstreLaser;

    [Header("Paramètre des poolings")]
    [SerializeField] private int nbBouleDeFeu;
    [SerializeField] private int nbBouleDeLaMort;
    [SerializeField] private int nbHadooken;
    [SerializeField] private int nbLaser;
    [SerializeField] private int nbmonstres;

    [Header("Positions d'instantiation")]
    [SerializeField] private Vector3 positionInstantiate;
    [SerializeField] private Vector3 positionInstantiateMonstres;

    [Header("Références")]
    [SerializeField] private GameManager gameManager;

    //pooling des sorts
    private Queue<ExposerSort> boulesDeFeuDispo = new Queue<ExposerSort>();
    private List<ExposerSort> boulesDeFeuLancees = new List<ExposerSort>();

    private Queue<ExposerSort> boulesDeLaMortDispo = new Queue<ExposerSort>();
    private List<ExposerSort> boulesDeLaMortLancees = new List<ExposerSort>();

    private Queue<ExposerSort> hadookenDispo = new Queue<ExposerSort>();
    private List<ExposerSort> hadookenLances = new List<ExposerSort>();

    private Queue<ExposerSort> lasersDispo = new Queue<ExposerSort>();
    private List<ExposerSort> lasersLances = new List<ExposerSort>();

    //pooling des monstres
    private Queue<ExposerMonstre> monstresFeuDispo = new Queue<ExposerMonstre>();
    private List<ExposerMonstre> monstresFeuLances = new List<ExposerMonstre>();

    private Queue<ExposerMonstre> monstresMortDispo = new Queue<ExposerMonstre>();
    private List<ExposerMonstre> monstresMortLances = new List<ExposerMonstre>();

    private Queue<ExposerMonstre> monstresHadookenDispo = new Queue<ExposerMonstre>();
    private List<ExposerMonstre> monstresHadookenLances = new List<ExposerMonstre>();

    private Queue<ExposerMonstre> monstresLaserDispo = new Queue<ExposerMonstre>();
    private List<ExposerMonstre> monstresLaserLances = new List<ExposerMonstre>();

    //génération des pooling et positonnement des sorts à l'emplacement indiqué
    private void Awake()
    {
        GameObject s;

        //génération des pooling
        //pooling des boules de feu
        for(int i = 0; i < nbBouleDeFeu; i++)
        {
            s = Instantiate<GameObject>(bouleDeFeu);
            var exposer = s.GetComponent<ExposerSort>();
            s.transform.position = positionInstantiate;

            //désactiver le gameobject et le placer dans la queue
            exposer.DesactivationSort();
            boulesDeFeuDispo.Enqueue(exposer);
        }

        //pooling des boules de la mort
        for (int i = 0; i < nbBouleDeLaMort; i++)
        {
            s = Instantiate<GameObject>(bouleDeLaMort);
            var exposer = s.GetComponent<ExposerSort>();
            s.transform.position = positionInstantiate;

            //désactiver le gameobject et le placer dans la queue
            exposer.DesactivationSort();
            boulesDeLaMortDispo.Enqueue(exposer);
        }

        //pooling des hadooken
        for (int i = 0; i < nbHadooken; i++)
        {
            s = Instantiate<GameObject>(hadooken);
            var exposer = s.GetComponent<ExposerSort>();
            s.transform.position = positionInstantiate;

            //désactiver le gameobject et le placer dans la queue
            exposer.DesactivationSort();
            hadookenDispo.Enqueue(exposer);
        }

        //Pooling des lasers
        for (int i = 0; i < nbLaser; i++)
        {
            s = Instantiate<GameObject>(laser);
            var exposer = s.GetComponent<ExposerSort>();
            s.transform.position = positionInstantiate;

            //désactiver le gameobject et le placer dans la queue
            exposer.DesactivationSort();
            lasersDispo.Enqueue(exposer);
        }

        //pooling des monstres de feu
        for (int i = 0; i < nbmonstres; i++)
        {
            s = Instantiate<GameObject>(monstreFeu);
            var exposer = s.GetComponent<ExposerMonstre>();
            s.transform.position = positionInstantiateMonstres;
            exposer.SetGameManager(gameManager);

            //désactiver le gameobject et le placer dans la queue
            exposer.DesactivationMonstre();
            monstresFeuDispo.Enqueue(exposer);
        }

        //pooling des monstres de mort
        for (int i = 0; i < nbmonstres; i++)
        {
            s = Instantiate<GameObject>(monstreMort);
            var exposer = s.GetComponent<ExposerMonstre>();
            s.transform.position = positionInstantiateMonstres;
            exposer.SetGameManager(gameManager);

            //désactiver le gameobject et le placer dans la queue
            exposer.DesactivationMonstre();
            monstresMortDispo.Enqueue(exposer);
        }

        //pooling des monstres hadooken
        for (int i = 0; i < nbmonstres; i++)
        {
            s = Instantiate<GameObject>(monstreHadooken);
            var exposer = s.GetComponent<ExposerMonstre>();
            s.transform.position = positionInstantiateMonstres;
            exposer.SetGameManager(gameManager);

            //désactiver le gameobject et le placer dans la queue
            exposer.DesactivationMonstre();
            monstresHadookenDispo.Enqueue(exposer);
        }

        //pooling des monstres laser
        for (int i = 0; i < nbmonstres; i++)
        {
            s = Instantiate<GameObject>(monstreLaser);
            var exposer = s.GetComponent<ExposerMonstre>();
            s.transform.position = positionInstantiateMonstres;
            exposer.SetGameManager(gameManager);

            //désactiver le gameobject et le placer dans la queue
            exposer.DesactivationMonstre();
            monstresLaserDispo.Enqueue(exposer);
        }
    }

    //retirer une boule de feu du pooling
    public ExposerSort getBouleDeFeu()
    {
        if(boulesDeFeuDispo.Count > 0)
        {
            var s = boulesDeFeuDispo.Dequeue();
            s.ActivationSort();

            return s;
        }
        else return null;
    }

    //retirer une boule de la mort du pooling
    public ExposerSort getBouleDeLaMort()
    {
        if (boulesDeLaMortDispo.Count > 0)
        {
            var s = boulesDeLaMortDispo.Dequeue();
            s.ActivationSort();

            return s;
        }
        else return null;
    }

    //retirer un hadooken du pooling
    public ExposerSort getHadooken()
    {
        if (hadookenDispo.Count > 0)
        {
            var s = hadookenDispo.Dequeue();
            s.ActivationSort();

            return s;
        }
        else return null;
    }

    //retirer un laser du pooling
    public ExposerSort getLaser()
    {
        if (lasersDispo.Count > 0)
        {
            var s = lasersDispo.Dequeue();
            s.ActivationSort();

            return s;
        }
        else return null;
    }

    //retirer un monstre feu du pooling
    public ExposerMonstre getMonstreFeu(Vector3 cible, Vector3 positionDepart)
    {
        if (monstresFeuDispo.Count > 0)
        {
            var m = monstresFeuDispo.Dequeue();
            m.transform.position = positionDepart;
            monstresFeuLances.Add(m);
            m.ActivationMonstre();
            m.SetCible(cible);

            return m;
        }
        else return null;
    }

    //retirer un monstre mort du pooling
    public ExposerMonstre getMonstreMort(Vector3 cible, Vector3 positionDepart)
    {
        if (monstresMortDispo.Count > 0)
        {
            var m = monstresMortDispo.Dequeue();
            m.transform.position = positionDepart;
            monstresMortLances.Add(m);
            m.ActivationMonstre();
            m.SetCible(cible);

            return m;
        }
        else return null;
    }

    //retirer un monstre Hadooken du pooling
    public ExposerMonstre getMonstreHadooken(Vector3 cible, Vector3 positionDepart)
    {
        if (monstresHadookenDispo.Count > 0)
        {
            var m = monstresHadookenDispo.Dequeue();
            m.transform.position = positionDepart;
            monstresHadookenLances.Add(m);
            m.ActivationMonstre();
            m.SetCible(cible);

            return m;
        }
        else return null;
    }

    //retirer un monstre Laser du pooling
    public ExposerMonstre getMonstreLaser(Vector3 cible, Vector3 positionDepart)
    {
        if (monstresLaserDispo.Count > 0)
        {
            var m = monstresLaserDispo.Dequeue();
            m.transform.position = positionDepart;
            monstresLaserLances.Add(m);
            m.ActivationMonstre();
            m.SetCible(cible);

            return m;
        }
        else return null;
    }

    //remettre le sort dans le pooling correspondant
    public void ReleaseSort(ExposerSort s)
    {
        s.transform.position = positionInstantiate;
        s.transform.SetParent(null);
        s.DesactivationSort();

        if(s.bouleDeFeu)
        {
            boulesDeFeuDispo.Enqueue(s);
        }
        else
        {
            if(s.bouleDeLaMort)
            {
                boulesDeLaMortDispo.Enqueue(s);
            }
            else
            {
                if(s.hadooken)
                {
                    hadookenDispo.Enqueue(s);
                }
                else
                {
                    if(s.laser)
                    {
                        lasersDispo.Enqueue(s);
                    }
                }
            }
        }
    }

    //remettre le monstre dans le pooling correspondant
    public void ReleaseMonstre(ExposerMonstre m)
    {
        m.transform.position = positionInstantiateMonstres;
        m.transform.SetParent(null);
        m.DesactivationMonstre();

        if (m.GetFaiblesse() == Sort.BOULEDEFEU)
        {
            monstresFeuDispo.Enqueue(m);
        }
        else
        {
            if (m.GetFaiblesse() == Sort.BOULEDELAMORT)
            {
                monstresMortDispo.Enqueue(m);
            }
            else
            {
                if (m.GetFaiblesse() == Sort.HADOOKEN)
                {
                    monstresHadookenDispo.Enqueue(m);
                }
                else
                {
                    if (m.GetFaiblesse() == Sort.LASER)
                    {
                        monstresLaserDispo.Enqueue(m);
                    }
                }
            }
        }
    }

    //reset du pooling des monstres
    public void ResetPoolingMonstres()
    {
        for(int i = 0; i < monstresFeuLances.Count; i++)
        {
            if(monstresFeuLances[i].GetEtatMonstre() == EtatMonstre.ENMARCHE)
            {
                monstresFeuLances[i].SetEtatMonstre(EtatMonstre.MORT);
            }
        }

        for (int i = 0; i < monstresMortLances.Count; i++)
        {
            if (monstresMortLances[i].GetEtatMonstre() == EtatMonstre.ENMARCHE)
            {
                monstresMortLances[i].SetEtatMonstre(EtatMonstre.MORT);
            }
        }

        for (int i = 0; i < monstresHadookenLances.Count; i++)
        {
            if (monstresHadookenLances[i].GetEtatMonstre() == EtatMonstre.ENMARCHE)
            {
                monstresHadookenLances[i].SetEtatMonstre(EtatMonstre.MORT);
            }
        }

        for (int i = 0; i < monstresLaserLances.Count; i++)
        {
            if (monstresLaserLances[i].GetEtatMonstre() == EtatMonstre.ENMARCHE)
            {
                monstresLaserLances[i].SetEtatMonstre(EtatMonstre.MORT);
            }
        }
    }

    //Apparition du sort dans la main
    public ExposerSort Incantation(Sort s, Transform main, Vector3 offset)
    {
        ExposerSort sort;
        switch(s)
        {
            case Sort.BOULEDEFEU:
                sort = getBouleDeFeu();
                boulesDeFeuLancees.Add(sort);
                sort.transform.SetParent(main);
                sort.transform.localPosition = Vector3.zero + offset;
                sort.transform.localRotation = new Quaternion(0, 0, 0, 0);

                return sort;

            case Sort.BOULEDELAMORT:
                sort = getBouleDeLaMort();
                boulesDeLaMortLancees.Add(sort);
                sort.transform.SetParent(main);
                sort.transform.localPosition = Vector3.zero + offset;
                sort.transform.localRotation = new Quaternion(0, 0, 0, 0);

                return sort;

            case Sort.HADOOKEN:
                sort = getHadooken();
                hadookenLances.Add(sort);
                sort.transform.SetParent(main);
                sort.transform.localPosition = Vector3.zero + offset;
                sort.transform.localRotation = new Quaternion(0, 0, 0, 0);
                return sort;

            case Sort.LASER:
                sort = getLaser();
                lasersLances.Add(sort);
                sort.transform.SetParent(main);
                sort.transform.localPosition = Vector3.zero + offset;
                sort.transform.localRotation = Quaternion.Euler(0, -90, 0);
                return sort;

            default:
                return null;

        }
    }

    //contrôle du pooling
    private void FixedUpdate()
    {
        //pour chaque boule de feu active, on vérifie s'elle est arrivée en fin de vie
        for(int i = 0; i < boulesDeFeuLancees.Count; i++)
        {
            //si le sort est en état POOLING dans la liste des sorts lancés, on le remet dans le pooling
            if(boulesDeFeuLancees[i].GetEtat() == EtatSort.POOLING)
            {
                ExposerSort s = boulesDeFeuLancees[i];
                boulesDeFeuLancees.RemoveAt(i);
                ReleaseSort(s);
            }
        }

        //pour chaque boule de la mort active, on vérifie si elle est arrivée en fin de vie
        for (int i = 0; i < boulesDeLaMortLancees.Count; i++)
        {
            //si le sort est en état POOLING dans la liste des sorts lancés, on le remet dans le pooling
            if (boulesDeLaMortLancees[i].GetEtat() == EtatSort.POOLING)
            {
                ExposerSort s = boulesDeLaMortLancees[i];
                boulesDeLaMortLancees.RemoveAt(i);
                ReleaseSort(s);
            }
        }

        //pour chaque hadooken actif, on vérifie s'il est arrivé en fin de vie
        for (int i = 0; i < hadookenLances.Count; i++)
        {
            //si le sort est en état POOLING dans la liste des sorts lancés, on le remet dans le pooling
            if (hadookenLances[i].GetEtat() == EtatSort.POOLING)
            {
                ExposerSort s = hadookenLances[i];
                hadookenLances.RemoveAt(i);
                ReleaseSort(s);
            }
        }

        //pour chaque laser actif, on vérifie s'il est arrivé en fin de vie
        for (int i = 0; i < lasersLances.Count; i++)
        {
            //si le sort est en état POOLING dans la liste des sorts lancés, on le remet dans le pooling
            if (lasersLances[i].GetEtat() == EtatSort.POOLING)
            {
                ExposerSort s = lasersLances[i];
                lasersLances.RemoveAt(i);
                ReleaseSort(s);
            }
        }

        //pour chaque monstre de feu actif, on vérifie s'il est encore en vie
        for (int i = 0; i < monstresFeuLances.Count; i++)
        {
            //si le monstre est en état MORT dans la liste des monstres lancés, on le remet dans le pooling
            if (monstresFeuLances[i].GetEtatMonstre() == EtatMonstre.MORT)
            {
                ExposerMonstre m = monstresFeuLances[i];
                monstresFeuLances.RemoveAt(i);
                ReleaseMonstre(m);
            }
        }

        //pour chaque monstre de mort actif, on vérifie s'il est encore en vie
        for (int i = 0; i < monstresMortLances.Count; i++)
        {
            //si le monstre est en état MORT dans la liste des monstres lancés, on le remet dans le pooling
            if (monstresMortLances[i].GetEtatMonstre() == EtatMonstre.MORT)
            {
                ExposerMonstre m = monstresMortLances[i];
                monstresMortLances.RemoveAt(i);
                ReleaseMonstre(m);
            }
        }

        //pour chaque monstre hadooken actif, on vérifie s'il est encore en vie
        for (int i = 0; i < monstresHadookenLances.Count; i++)
        {
            //si le monstre est en état MORT dans la liste des monstres lancés, on le remet dans le pooling
            if (monstresHadookenLances[i].GetEtatMonstre() == EtatMonstre.MORT)
            {
                ExposerMonstre m = monstresHadookenLances[i];
                monstresHadookenLances.RemoveAt(i);
                ReleaseMonstre(m);
            }
        }

        //pour chaque monstre laser actif, on vérifie s'il est encore en vie
        for (int i = 0; i < monstresLaserLances.Count; i++)
        {
            //si le monstre est en état MORT dans la liste des monstres lancés, on le remet dans le pooling
            if (monstresLaserLances[i].GetEtatMonstre() == EtatMonstre.MORT)
            {
                ExposerMonstre m = monstresLaserLances[i];
                monstresLaserLances.RemoveAt(i);
                ReleaseMonstre(m);
            }
        }
    }
}
