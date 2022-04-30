using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon.StructWrapping;
using Photon.Pun;
using UnityEngine;


public class BattlePhaseManager : MonoBehaviour
    {
        public static BattlePhaseManager instance;

        [SerializeField] private PhotonView playerView;
        [SerializeField] private float range = 1;

        private Ray ray;
        private RaycastHit hit;

        private List<GameObject> unitsSelected;

        private List<Transform> deadUnitCenters;

        private GameObject unitTarget = null;
        private GameObject unitFusion;

        private int atkAlly;
        private bool isAttacking;
        private int numberView;
        private float sizeListCentersEnnemi;
        public bool IsAttacking
        {
            get => isAttacking;
            set
            {
                isAttacking = value;
            }
        }

        private void Awake()
        {
            instance = this;
        }

        private void Start()
        {
            unitsSelected = new List<GameObject>();
        }

        void Update()
        {
            CheckRaycast();
        }

        void CheckRaycast()
        {
            if (RoundManager.instance.StateRound == 3 || RoundManager.instance.StateRound == 4)
            {
                if (Input.touchCount > 0)
                {
                    ray = PlayerSetup.instance.GetCam().ScreenPointToRay(Input.GetTouch(0).position);
                    Physics.Raycast(ray, out hit);

                    if (Input.GetTouch(0).phase == TouchPhase.Began || Input.GetTouch(0).phase == TouchPhase.Moved)
                    {
                        if (hit.collider != null)
                        {
                            foreach (var unit in PlacementManager.instance.GetBoard())
                            {
                                foreach (var cases in unit.emplacement)
                                {
                                    if (cases == new Vector2(Mathf.FloorToInt(hit.point.x) + 0.5f,
                                        Mathf.FloorToInt(hit.point.z) + 0.5f))
                                    {
                                        if (RoundManager.instance.StateRound == 3)
                                        {
                                            if (unit.monster.GetComponent<Monster>().GetView().IsMine &&
                                                !unit.monster.GetComponent<Monster>().Attacked &&
                                                !unitsSelected.Contains(unit.monster))
                                            {
                                                GameObject current = unit.monster;
                                                current.GetComponent<Monster>().BeChoosen();
                                                unitsSelected.Add(current);
                                            }
                                        }
                                        else if (RoundManager.instance.StateRound == 4)
                                        {
                                            if (!unit.monster.GetComponent<Monster>().GetView().IsMine &&
                                                CheckInRange(unit.monster) && unitTarget == null)
                                            {
                                                unitTarget = unit.monster;
                                                unitTarget.GetComponent<Monster>().BeChoosen();
                                                isAttacking = true;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    else if (Input.GetTouch(0).phase == TouchPhase.Ended)
                    {
                        if (unitsSelected.Count != 0)
                        {
                            CalculAlliesAtk();
                            RoundManager.instance.StateRound = 4;
                        }
                    }
                }
            }
        }

        public void ResultAttack(int result)
        {
            switch (result)
            {
                case 0:
                    playerView.RPC("RPC_Atk", RpcTarget.Others, unitTarget.GetComponent<PhotonView>().ViewID);

                    LifeManager.instance.TakeDamageEnnemi(1);
                    LifeManager.instance.TakeDamageHimself();

                    foreach (var unit in unitsSelected)
                    {
                        PhotonNetwork.Destroy(unit);
                    }

                    break;

                case >0:
                    deadUnitCenters = unitTarget.GetComponent<Monster>().GetCenters();

                    playerView.RPC("RPC_Atk", RpcTarget.Others, unitTarget.GetComponent<PhotonView>().ViewID);

                    LifeManager.instance.TakeDamageEnnemi(AllCenterMoreFar());

                    AddAllExtension(StrongerMonster(), true);

                    ActivateAllEffectInUnitSelected(1);

                    break;

                case <0:
                    LifeManager.instance.TakeDamageHimself();

                    deadUnitCenters = unitsSelected[Random.Range(0, unitsSelected.Count)].GetComponent<Monster>()
                        .GetCenters();

                    foreach (var unit in unitsSelected)
                    {
                        PhotonNetwork.Destroy(unit);
                    }

                    AddAllExtension(unitTarget, false);

                    break;
            }

        }

        public void Attack()
        {
            int result = atkAlly - unitTarget.GetComponent<Monster>().Atk;
            AllMonsterAttacked(true);

            ResultAttack(result);

            RoundManager.instance.StateRound = 3;
            isAttacking = false;

            ClearUnits();
        }

        public void AllMonsterAttacked(bool b)
        {
            foreach (var unit in unitsSelected)
            {
                if (unit.GetComponent<Monster>().Status.Equals(0))
                {
                    unit.GetComponent<Monster>().Attacked = b;
                }
            }
        }

        private GameObject StrongerMonster()
        {
            GameObject unitSelected = unitsSelected[0];

            foreach (var unit in unitsSelected)
            {
                if (unit.GetComponent<Monster>().Atk > unitSelected.GetComponent<Monster>().Atk)
                {
                    unitSelected = unit;
                }
            }

            return unitSelected;
        }

        void AddExtension(GameObject unitMore, bool owner)
        {
            numberView = PhotonNetwork.ViewCount;
            
            for (int j = 0; j < unitMore.GetComponent<Monster>().GetCenters().Count; j++)
            {
                for (int x = 0; x < deadUnitCenters.Count; x++)
                {
                    if (Vector3.Distance(deadUnitCenters[x].position,
                        unitMore.GetComponent<Monster>().GetCenters()[j].position).Equals(range))
                    {
                        Debug.Log(unitMore.GetComponent<Monster>().GetCenters().Count);

                        if (owner)
                        {
                            Debug.Log("Loop");

                            object[] data = new object[] {unitMore.GetComponent<Monster>().GetView().ViewID};

                            PhotonNetwork.Instantiate("Tile", deadUnitCenters[x].position,
                                PlayerSetup.instance.transform.rotation, 0, data);
                        }
                        else
                        {
                            playerView.RPC("RPC_SyncUnit", RpcTarget.Others,
                                unitMore.GetComponent<Monster>().GetView().ViewID, deadUnitCenters[x].position.x,
                                deadUnitCenters[x].position.z);
                        }
                        
                        deadUnitCenters.RemoveAt(x);
                        
                        do
                        {
                        } while (numberView + 1 != PhotonNetwork.ViewCount);

                        return;
                    }
                }
            }
        }

        private void AddAllExtension(GameObject unitMore, bool owner)
        {
            sizeListCentersEnnemi = Mathf.Ceil(deadUnitCenters.Count / 2f);
            for (int i = 0; i < sizeListCentersEnnemi; i++)
            {
                AddExtension(unitMore, owner);
            }

            deadUnitCenters.Clear();
        }

        public float AllCenterMoreFar()
        {
            float farCenter = -10;
            foreach (var unit in unitsSelected)
            {
                if (PlacementManager.instance.CenterMoreFar(unit) > farCenter)
                {
                    farCenter = PlacementManager.instance.CenterMoreFar(unit);
                }
            }

            return farCenter;
        }

        public bool CheckInRange(GameObject v)
        {
            foreach (var unit in unitsSelected)
            {
                bool unitCheck = false;

                foreach (var center in unit.GetComponent<Monster>().GetCenters())
                {
                    foreach (var targetCenter in v.GetComponent<Monster>().GetCenters())
                    {
                        if (Vector3.Distance(center.position, targetCenter.position).Equals(range))
                        {
                            unitCheck = true;
                        }
                    }
                }

                if (!unitCheck)
                {
                    return false;
                }
            }

            return true;
        }

        public void CancelSelection()
        {
            ClearUnits();
            isAttacking = false;
            RoundManager.instance.StateRound =3;
        }

        public void ClearUnits()
        {
            for (int i = 0; i < unitsSelected.Count; i++)
            {
                unitsSelected[i].GetComponent<Monster>().NotChossen();
            }

            unitsSelected.Clear();

            if (unitTarget != null)
            {
                unitTarget.GetComponent<Monster>().NotChossen();
                unitTarget = null;
            }
        }

        public void CalculAlliesAtk()
        {
            atkAlly = 0;
            foreach (var unit in unitsSelected)
            {
                atkAlly += unit.GetComponent<Monster>().Atk;
            }
        }

        private void ActivateAllEffectInUnitSelected(int i)
        {
            foreach (var unit in unitsSelected)
            {
                unit.GetComponent<Monster>().ActivateEffects(i);
            }
        }

        [PunRPC]
        private void RPC_SyncUnit(int idMore, float x, float z)
        {
            object[] data = new object[] {idMore};
            PhotonNetwork.Instantiate("Tile", new Vector3(x, 0.55f, z), PlayerSetup.instance.transform.rotation, 0,
                data);
            
        }

        [PunRPC]
        private void RPC_Atk(int id)
        {
            PhotonView ph = PhotonView.Find(id);
            if (ph.IsMine)
            {
                PhotonNetwork.Destroy(ph.gameObject);
            }
        }

    }

