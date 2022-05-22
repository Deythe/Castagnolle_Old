using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using Random = UnityEngine.Random;

public class DiceManager : MonoBehaviour
{
    public static DiceManager instance;
    [SerializeField] private PhotonView view;
    
    [SerializeField] private List<DiceScriptable> diceDeck = new List<DiceScriptable>();
    [SerializeField] private DiceListScriptable diceListDeck;
    
    [SerializeField] private GameObject[] diceObjet = new GameObject[3];
    [SerializeField] private GameObject[] diceGaugeObjet = new GameObject[3];

    [SerializeField] private DiceScriptable[] dicesNotDisponible = new DiceScriptable[6];
        
    [SerializeField] private int[] diceChoosen = new int[3];
    [SerializeField] private int[] diceGauge = new int[3];

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

    public GameObject[] DiceGaugeObjet
    {
        get => diceGaugeObjet;
    }
    public int[] Gauge
    {
        get => diceGauge;
        set
        {
            diceGauge = value;
        }
    }
    
    public int[] DiceChoosen
    {
        get => diceChoosen;
        set
        {
            diceChoosen = value;
        }
    }
    
    private void Awake()
    {
        instance = this;
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
            diceObjet[i] = PhotonNetwork.Instantiate("Dice", spawner,PlayerSetup.instance.transform.rotation,0);
        }
    }
    
    public void InitGaugeDiceMesh()
    {
        for (int i = 0; i < diceChoosen.Length; i++)
        {
            spawner = PlayerSetup.instance.GetGaugeSpawner()[i].position;
            diceGaugeObjet[i] = PhotonNetwork.Instantiate("GaugeDice", spawner,PlayerSetup.instance.transform.rotation,0);
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
            
            diceObjet[i].GetComponent<MeshRenderer>().material.mainTexture = diceListDeck.textureList[diceChoosen[i]];
            diceObjet[i].GetComponent<MeshRenderer>().enabled = true;
        }
        
        DeckManager.instance.CheckUnitWithRessources();
        RoundManager.instance.StateRound = 1;
        UiManager.instance.UpdateListCard();
    }

    public bool DeckEmpy()
    {
        for (int i = 0; i < 3; i++)
        {
            if (diceChoosen[i]!=0 || diceGauge[i]!=0)
            {
                return false;
            }
        }
        
        return true;
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
                    return true;
                }
            }
        }

        return false;
    }

    public void PutInGauge(int index, GameObject dice)
    {
        for (var i = 0; i < diceObjet.Length; i++)
        {
            if (diceObjet[i].Equals(dice))
            {
                view.RPC("RPC_SynchGaugeDice",RpcTarget.All, diceGaugeObjet[index].GetComponent<PhotonView>().ViewID, true, diceChoosen[i]);
                
                diceGauge[index] = diceChoosen[i];
                dicesNotDisponible[diceGauge.Length + index] = dicesNotDisponible[i];
                dicesNotDisponible[i] = null;
                diceChoosen[i] = 0;
                DeckManager.instance.CheckUnitWithRessources();
                UiManager.instance.UpdateListCard();
                return;
            }
        }
    }

    public void DeleteAllResources(List<int> resource)
    {
        for (int i = 0; i < resource.Count; i++)
        {
            DeleteResource(resource[i]);
        }

        DeckManager.instance.CheckUnitWithRessources();
        UiManager.instance.UpdateListCard();
    }
    
    public void DeleteAllResources(int[] resource)
    {
        for (int i = 0; i < resource.Length; i++)
        {
            DeleteResource(resource[i]);
        }

        DeckManager.instance.CheckUnitWithRessources();
        UiManager.instance.UpdateListCard();
    }

    public void DeleteResource(int i)
    {
        for (int j = 0; j < diceChoosen.Length+diceGauge.Length; j++)
        {
            if (j < diceChoosen.Length)
            {
                if (i.Equals(4))
                {
                    if (diceChoosen[j] != 0)
                    {
                        diceChoosen[j] = 0;
                        diceDeck.Add(dicesNotDisponible[j]);
                        dicesNotDisponible[j] = null;
                        diceObjet[j].GetComponent<MeshRenderer>().enabled = false;
                        return;
                    }
                }
                else if (i.Equals(diceChoosen[j]))
                {
                    diceChoosen[j] = 0;
                    if (dicesNotDisponible[j]!=null)
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
                if (i.Equals(4))
                {
                    if (diceGauge[j-diceGauge.Length] != 0)
                    {
                        diceGauge[j-diceGauge.Length] = 0;
                        diceDeck.Add(dicesNotDisponible[j]);
                        dicesNotDisponible[j] = null;
                        view.RPC("RPC_SynchGaugeDice",RpcTarget.All, diceGaugeObjet[j-diceGauge.Length].GetComponent<PhotonView>().ViewID, false, 0);
                        return;
                    }
                }
                else if (i.Equals(diceGauge[j-diceGauge.Length]))
                {
                    diceGauge[j-diceGauge.Length] = 0;
                    diceDeck.Add(dicesNotDisponible[j]);
                    dicesNotDisponible[j] = null;
                    view.RPC("RPC_SynchGaugeDice",RpcTarget.All, diceGaugeObjet[j-diceGauge.Length].GetComponent<PhotonView>().ViewID, false, 0);
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
    private void RPC_SynchGaugeDice(int id, bool b, int index)
    {
        PhotonView.Find(id).GetComponent<MeshRenderer>().material.mainTexture = diceListDeck.textureList[index];
        PhotonView.Find(id).GetComponent<MeshRenderer>().enabled = b;
    }
    
}
