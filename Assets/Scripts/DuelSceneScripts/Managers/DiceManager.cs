using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using Random = UnityEngine.Random;

public class DiceManager : MonoBehaviour
{
    public static DiceManager instance;
    [SerializeField] private PhotonView view;
    
    [SerializeField] private DiceListScriptable diceListDeck;
    
    [SerializeField] private List<DiceScriptable> diceDeck = new List<DiceScriptable>();
    [SerializeField] private DiceScriptable[] dicesNotDisponible = new DiceScriptable[6];

    [SerializeField] private GameObject[] diceObjet = new GameObject[3];
    [SerializeField] private GameObject[] diceGaugeObjet = new GameObject[3];
    
    [SerializeField] private DiceListScriptable.enumRessources[] diceChoosen = new DiceListScriptable.enumRessources[3];
    [SerializeField] private DiceListScriptable.enumRessources[] diceGauge = new DiceListScriptable.enumRessources[3];

    [SerializeField] private Vector3 spawner;
    private int random;

    public DiceListScriptable DiceListScriptable
    {
        get => diceListDeck;
    }

    public PhotonView View
    {
        get => view;
    }
    
    public GameObject[] DiceObjects
    {
        get => diceObjet;
    }

    public GameObject[] DiceGaugeObjet
    {
        get => diceGaugeObjet;
    }
    public DiceListScriptable.enumRessources[] p_diceGauge
    {
        get => diceGauge;
        set
        {
            diceGauge = value;
        }
    }
    
    public DiceListScriptable.enumRessources[] p_diceChoosen
    {
        get => diceChoosen;
        set
        {
            diceChoosen = value;
        }
    }
    
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }else
        {
            Destroy(gameObject);
        }
    }
    
    private void Start()
    {
        InitDeck();
    }

    private void InitDeck()
    {
        for(int i = 0; i < FireBaseManager.instance.User.currentDiceDeck.Length; i++)
        {
            diceDeck.Add(diceListDeck.diceDeck[FireBaseManager.instance.User.currentDiceDeck[i]]);
        }
    }

    public void InitDiceMesh()
    {
        for (int i = 0; i < diceChoosen.Length; i++)
        {
            spawner = PlayerSetup.instance.GetSpawner()[i].position;
            diceObjet[i] = PhotonNetwork.Instantiate("Dice", spawner, PlayerSetup.instance.transform.rotation * Quaternion.Euler(-90,PlayerSetup.instance.GetSpawner()[i].eulerAngles.y,0),0);
        }
    }
    
    public void InitGaugeDiceMesh()
    {
        for (int i = 0; i < diceChoosen.Length; i++)
        {
            spawner = PlayerSetup.instance.GetGaugeSpawner()[i].position;
            diceGaugeObjet[i] = PhotonNetwork.Instantiate("GaugeDice", spawner,PlayerSetup.instance.transform.rotation* Quaternion.Euler(-90,0,0),0);
        }
    }

    public void ChooseDice()
    {
        for (int i = 0; i < diceChoosen.Length; i++)
        {
            random = Random.Range(0, diceDeck.Count);
            diceChoosen[i] = PickDice(random).faces[Random.Range(0, 6)];

            dicesNotDisponible[i] = PickDice(random);
            diceDeck.Remove(PickDice(random));

            Debug.Log(diceChoosen[i]);
            diceObjet[i].GetComponent<MeshRenderer>().material.mainTexture =
                ChooseTextureDice(diceChoosen[i]);
            diceObjet[i].GetComponent<MeshRenderer>().enabled = true;
        }
        
        DeckManager.instance.CheckUnitWithRessources();
        RoundManager.instance.p_roundState = RoundManager.enumRoundState.DrawPhase;
    }

    Texture2D ChooseTextureDice(DiceListScriptable.enumRessources resource)
    {
        switch (resource)
        {
            case DiceListScriptable.enumRessources.Red:
                return diceListDeck.textureList[1];
            case DiceListScriptable.enumRessources.Purple:
                return diceListDeck.textureList[2];
            case DiceListScriptable.enumRessources.Blue:
                return diceListDeck.textureList[3];
            case DiceListScriptable.enumRessources.Neutral:
                return diceListDeck.textureList[4];
            case DiceListScriptable.enumRessources.Milk:
                return diceListDeck.textureList[5];
        }

        return null;
    }
    
    public bool CheckPositionDiceGauge(GameObject diceTarget)
    {
        for (var i = 0; i < diceGaugeObjet.Length; i++)
        {
            if (diceTarget.transform.position.x < diceGaugeObjet[i].transform.position.x + 0.5 &&
                diceTarget.transform.position.x > diceGaugeObjet[i].transform.position.x - 0.5)
            {
                if (diceTarget.transform.position.z < diceGaugeObjet[i].transform.position.z + 0.5 &&
                    diceTarget.transform.position.z > diceGaugeObjet[i].transform.position.z - 0.5)
                {
                    diceTarget.GetComponent<MeshRenderer>().enabled = false;
                    PutInGauge(i, diceTarget);
                    diceTarget = null;
                    return true;
                }
            }
        }

        return false;
    }

    private void PutInGauge(int index, GameObject dice)
    {
        for (var i = 0; i < diceObjet.Length; i++)
        {
            if (diceObjet[i].Equals(dice))
            {
                for (int j = 0; j < diceGauge.Length; j++)
                {
                    if (diceGauge[j] == 0)
                    {
                        view.RPC("RPC_SynchGaugeDice",RpcTarget.All, diceGaugeObjet[j].GetComponent<PhotonView>().ViewID, true, diceChoosen[i]);
                        diceGauge[j] = diceChoosen[i];
                        dicesNotDisponible[diceGauge.Length + j] = dicesNotDisponible[i];
                        dicesNotDisponible[i] = null;
                        diceChoosen[i] = DiceListScriptable.enumRessources.Nothing;
                        DeckManager.instance.CheckUnitWithRessources();
                        return;
                    }
                }
                
                view.RPC("RPC_SynchGaugeDice",RpcTarget.All, diceGaugeObjet[index].GetComponent<PhotonView>().ViewID, true, diceChoosen[i]);
                diceGauge[index] = diceChoosen[i];
                diceDeck.Add(dicesNotDisponible[diceGauge.Length + index]);
                dicesNotDisponible[diceGauge.Length + index] = dicesNotDisponible[i];
                dicesNotDisponible[i] = null;
                diceChoosen[i] = DiceListScriptable.enumRessources.Nothing;
                DeckManager.instance.CheckUnitWithRessources();
                EffectManager.instance.CheckAllHaveAMilkInGauge();
                return;
            }
        }
    }

    public void DeleteAllResources(List<DiceListScriptable.enumRessources> resource)
    {
        for (int i = 0; i < resource.Count; i++)
        {
            DeleteResource(resource[i]);
        }

        EffectManager.instance.CheckAllHaveAMilkInGauge();
        DeckManager.instance.CheckUnitWithRessources();
    }
    
    public void DeleteAllResources(DiceListScriptable.enumRessources[] resource)
    {
        for (int i = 0; i < resource.Length; i++)
        {
            DeleteResource(resource[i]);
        }

        DeckManager.instance.CheckUnitWithRessources();
    }

    void DeleteResource(DiceListScriptable.enumRessources i)
    {
        for (int j = 0; j < diceChoosen.Length + diceGauge.Length; j++)
        {
            if (i.Equals(4))
            {
                if (j < diceChoosen.Length)
                {
                    if (diceChoosen[j].Equals(i))
                    {
                        diceChoosen[j] = DiceListScriptable.enumRessources.Nothing;
                        if (dicesNotDisponible[j] != null)
                        {
                            diceDeck.Add(dicesNotDisponible[j]);
                        }
                        dicesNotDisponible[j] = null;
                        diceObjet[j].GetComponent<MeshRenderer>().enabled = false;
                        return;
                    }
                }
                else
                {
                    if (i.Equals(diceGauge[j - diceGauge.Length]))
                    {
                        if (dicesNotDisponible[j] != null)
                        {
                            diceDeck.Add(dicesNotDisponible[j]);
                        }

                        diceGauge[j - diceGauge.Length] = DiceListScriptable.enumRessources.Nothing;
                        dicesNotDisponible[j] = null;
                        view.RPC("RPC_SynchGaugeDice", RpcTarget.All,
                            diceGaugeObjet[j - diceGauge.Length].GetComponent<PhotonView>().ViewID, false, 0);
                        return;
                    }
                }
            }
        }

        for (int j = 0; j < diceChoosen.Length + diceGauge.Length; j++)
        {
            if (j < diceChoosen.Length)
            {
                if (i.Equals(diceChoosen[j]) || (i.Equals(4) && diceChoosen[j]!=0))
                {
                    diceChoosen[j] = DiceListScriptable.enumRessources.Nothing;
                    if (dicesNotDisponible[j] != null)
                    {
                        diceDeck.Add(dicesNotDisponible[j]);
                    }

                    dicesNotDisponible[j] = null;
                    diceObjet[j].GetComponent<MeshRenderer>().enabled = false;
                    return;
                }
            }
            else
            {
                if (i.Equals(diceGauge[j - diceGauge.Length]) || (i.Equals(4) && diceGauge[j - diceGauge.Length]!=0))
                {
                    diceGauge[j - diceGauge.Length] = DiceListScriptable.enumRessources.Nothing;
                    if (dicesNotDisponible[j] != null)
                    {
                        diceDeck.Add(dicesNotDisponible[j]);
                    }

                    
                    dicesNotDisponible[j] = null;
                    view.RPC("RPC_SynchGaugeDice", RpcTarget.All,
                        diceGaugeObjet[j - diceGauge.Length].GetComponent<PhotonView>().ViewID, false, 0);
                    return;
                }
            }
        }

    }

    private DiceScriptable PickDice(int random)
    {
        return diceDeck[random];
    }
    
    [PunRPC]
    private void RPC_SynchGaugeDice(int id, bool b, DiceListScriptable.enumRessources index)
    {
        PhotonView.Find(id).GetComponent<MeshRenderer>().material.mainTexture = ChooseTextureDice(index);
        PhotonView.Find(id).GetComponent<MeshRenderer>().enabled = b;
    }
    
}