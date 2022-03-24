using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using Random = UnityEngine.Random;

public class DiceManager : MonoBehaviour
{
    public static DiceManager instance;
    [SerializeField] private PhotonView view;
    
    [SerializeField] private List<DiceScriptable> diceDeck;

    [SerializeField] private GameObject[] diceObjet = new GameObject[3];
    [SerializeField] private GameObject[] diceGaugeObjet = new GameObject[3];
    
    [SerializeField] private int[] diceChoosen = new int[3];
    [SerializeField] private int[] diceGauge = new int[3];
    
    [SerializeField] private Vector3 spawner;
    private int random;
    
    private void Awake()
    {
        instance = this;
    }

    public void InitDice()
    {
        for (int i = 0; i < diceChoosen.Length; i++)
        {
            spawner = PlayerSetup.instance.GetSpawner()[i].position;
            diceObjet[i] = PhotonNetwork.Instantiate("Dice", spawner,PlayerSetup.instance.transform.rotation,0);
        }
    }
    
    public void InitGaugeDice()
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
            
            diceObjet[i].GetComponent<MeshRenderer>().material.mainTexture = PickDice(random).texture;
            diceObjet[i].GetComponent<MeshRenderer>().enabled = true;
        }
        
        DeckManager.instance.CheckUnitWithRessources();
        RoundManager.instance.SetStateRound(1);
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
                    Texture2D tex = diceTarget.GetComponent<MeshRenderer>().material.mainTexture as Texture2D;
                    byte[] bytes = tex.GetRawTextureData();
                    
                    view.RPC("RPC_SynchGaugeDice",RpcTarget.All, diceGaugeObjet[i].GetComponent<PhotonView>().ViewID, true, bytes);
                    view.RPC("RPC_SynchGaugeDice",RpcTarget.All, diceTarget.GetComponent<PhotonView>().ViewID, false, null);
                    
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
                diceGauge[index] = diceChoosen[i];
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

    public void DeleteResource(int i)
    {
        for (int j = 0; j < diceChoosen.Length+diceGauge.Length; j++)
        {
            if (j < diceChoosen.Length)
            {
                if (i.Equals(diceChoosen[j]))
                {
                    diceChoosen[j] = 0;
                    diceObjet[j].GetComponent<MeshRenderer>().enabled = false;
                    return;
                }
            }
            else
            {
                if (i.Equals(diceGauge[j-diceGauge.Length]))
                {
                    diceGauge[j-diceGauge.Length] = 0;
                    view.RPC("RPC_SynchGaugeDice",RpcTarget.All, diceGaugeObjet[j-diceGauge.Length].GetComponent<PhotonView>().ViewID, false, null);
                    return;
                }
            }
        }
    }

    private DiceScriptable PickDice(int random)
    {
        return diceDeck[random];
    }

    public int[] GetDiceChoosen()
    {
        return diceChoosen;
    }
    
    public int[] GetGauge()
    {
        return diceGauge;
    }

    [PunRPC]
    private void RPC_SynchGaugeDice(int i, bool b, byte[] array)
    {
        if (array != null)
        {
            Texture2D tex = new Texture2D(128, 128, TextureFormat.RGB24, false);
            tex.LoadRawTextureData(array);
            tex.Apply();
            PhotonView.Find(i).GetComponent<MeshRenderer>().material.mainTexture = tex;
        }
        PhotonView.Find(i).GetComponent<MeshRenderer>().enabled = b;
    }
    
}
