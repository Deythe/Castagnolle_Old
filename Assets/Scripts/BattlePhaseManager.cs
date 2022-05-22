using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;


public class BattlePhaseManager : MonoBehaviour
    {
        public static BattlePhaseManager instance;

        [SerializeField] private PhotonView playerView;
        [SerializeField] private float range = 1;

        private GameObject unitsSelected;

        private List<Transform> deadUnitCenters;

        private GameObject unitTarget = null;
        private GameObject unitFusion;
        
        private int result;
        private bool isAttacking;
        private int numberView;
        private float sizeListCentersEnnemi;
        private int targetUnitAttack;
        private bool isWaiting;
        private RaycastHit hit;

        public bool IsWaiting
        {
            get => isWaiting;
            set
            {
                isWaiting = value;
            }
        }
        public int TargetUnitAttack
        {
            get => targetUnitAttack;
        }

        public int Result
        {
            get => result;
        }
        
        public GameObject TargetUnit
        {
            get => unitTarget;
        }

        public GameObject UnitSelected
        {
            get => unitsSelected;
        }
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

        void Update()
        {
            if (RoundManager.instance != null)
            {
                CheckBattle();
            }
        }

        void CheckBattle()
        {
            if (RoundManager.instance.StateRound == 3 || RoundManager.instance.StateRound == 4)
            {
                if (Input.touchCount > 0)
                {
                    if (Input.GetTouch(0).phase == TouchPhase.Began)
                    {
                        Physics.Raycast(PlayerSetup.instance.GetCam().ScreenPointToRay(Input.GetTouch(0).position), out hit);
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
                                                !unit.monster.GetComponent<Monster>().Attacked)
                                            {
                                                unitsSelected = unit.monster;
                                                unitsSelected.GetComponent<Monster>().BeChoosen();
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
                        if (unitsSelected != null)
                        {
                            RoundManager.instance.StateRound = 4;
                        }
                    }
                }
            }
        }

        IEnumerator ResultAttack(int result)
        {
            if (unitTarget.GetComponent<Monster>().Status != 1 && !unitsSelected.GetComponent<Monster>().IsChampion)
            {
                playerView.RPC("RPC_TakeDamageUnit", RpcTarget.AllViaServer,
                    unitsSelected.GetComponent<PhotonView>().ViewID, unitTarget.GetComponent<Monster>().Atk,
                    unitTarget.GetComponent<PhotonView>().ViewID, unitsSelected.GetComponent<Monster>().Atk);
            }
            else
            {
                playerView.RPC("RPC_TakeDamageUnit", RpcTarget.AllViaServer,
                    unitsSelected.GetComponent<PhotonView>().ViewID, 0,
                    unitTarget.GetComponent<PhotonView>().ViewID, unitsSelected.GetComponent<Monster>().Atk);
            }

            switch (result)
            {
                case 0:
                    if (unitTarget.GetComponent<Monster>().Status != 1)
                    {
                        if (unitTarget.GetComponent<Monster>().Status == 0)
                        {
                            LifeManager.instance.View.RPC("RPC_TakeDamageEnnemi", RpcTarget.Others, 1);
                            LifeManager.instance.EnnemiLife--;
                        }
                        
                        LifeManager.instance.TakeDamageHimself();
                    }
                    else
                    {
                        deadUnitCenters = unitTarget.GetComponent<Monster>().GetCenters();
                        if (unitTarget.GetComponent<Monster>().Status == 1)
                        {
                            LifeManager.instance.TakeDamageEnnemi(
                                PlacementManager.instance.CenterMoreFar(unitsSelected));
                        }

                        StartCoroutine(AddAllExtension(unitsSelected, true));
                    
                        if (unitsSelected.GetComponent<Monster>().HaveAnEffectThisTurn(1))
                        {
                            unitsSelected.GetComponent<Monster>().ActivateEffects(1);
                            yield return new WaitUntil(()=>unitsSelected.GetComponent<Monster>().ReturnUsedOfAnEffect(1));
                        }
                    }
                    break;

                case >0:
                    deadUnitCenters = unitTarget.GetComponent<Monster>().GetCenters();
                    
                    if (unitTarget.GetComponent<Monster>().Status == 0)
                    {
                        LifeManager.instance.TakeDamageEnnemi(PlacementManager.instance.CenterMoreFar(unitsSelected));
                    }

                    StartCoroutine(AddAllExtension(unitsSelected, true));
                    
                    if (unitsSelected.GetComponent<Monster>().HaveAnEffectThisTurn(1))
                    {
                        unitsSelected.GetComponent<Monster>().ActivateEffects(1);
                        yield return new WaitUntil(()=>unitsSelected.GetComponent<Monster>().ReturnUsedOfAnEffect(1));
                    }
                    
                    break;

                case <0:
                    if (unitTarget.GetComponent<Monster>().Status != 1)
                    {
                        LifeManager.instance.TakeDamageHimself();
                        deadUnitCenters = unitsSelected.GetComponent<Monster>()
                            .GetCenters();

                        StartCoroutine(AddAllExtension(unitTarget, false));
                    }

                    break;
            }

            if (unitTarget.GetComponent<Monster>().Status == 0 || unitTarget.GetComponent<Monster>().Status == 1 && result>0)
            {
                foreach (var cases in PlacementManager.instance.GetBoard())
                {
                    if (cases.monster.GetComponent<Monster>().HaveAnEffectThisTurn(5))
                    {
                        cases.monster.GetComponent<Monster>().ActivateEffects(5);
                    }
                }
            }

            RoundManager.instance.StateRound = 3;
            isAttacking = false;
            ClearUnits();
        }

        IEnumerator CoroutineAttack()
        {
            targetUnitAttack = unitTarget.GetComponent<Monster>().Atk;
            result = unitsSelected.GetComponent<Monster>().Atk - targetUnitAttack;
            unitsSelected.GetComponent<Monster>().Attacked = true;
            
            if (unitsSelected.GetComponent<Monster>().Animator != null)
            {
                unitsSelected.GetComponent<Monster>().Animator.SetBool("ATK", true);
            }

            yield return new WaitForSeconds(0.5f);
            
            if (unitsSelected.GetComponent<Monster>().Animator != null)
            {
                unitsSelected.GetComponent<Monster>().Animator.SetBool("ATK", false);
            }

            StartCoroutine(ResultAttack(result));
        }
        
        public void Attack()
        {
            StartCoroutine(CoroutineAttack());
        }
        
        void AddExtension(GameObject unitMore, bool owner)
        {
            for (int j = 0; j < unitMore.GetComponent<Monster>().GetCenters().Count; j++)
            {
                for (int x = 0; x < deadUnitCenters.Count; x++)
                {
                    if (Vector3.Distance(deadUnitCenters[x].position,
                        unitMore.GetComponent<Monster>().GetCenters()[j].position).Equals(range))
                    {
                        if (owner)
                        {
                            unitFusion = PhotonNetwork.Instantiate("Tile", deadUnitCenters[x].position,
                                PlayerSetup.instance.transform.rotation, 0);
                            playerView.RPC("RPC_SyncTiles", RpcTarget.All, unitFusion.GetComponent<PhotonView>().ViewID,unitMore.GetComponent<Monster>().ID);
                        }
                        else
                        {
                            playerView.RPC("RPC_InstantiateEnemyTiles", RpcTarget.Others,
                                unitMore.GetComponent<Monster>().ID, deadUnitCenters[x].position.x,
                                deadUnitCenters[x].position.z);
                        }
                        
                        deadUnitCenters.RemoveAt(x);

                        return;
                    }
                }
            }
        }

        IEnumerator AddAllExtension(GameObject unitMore, bool owner)
        {
            numberView = PhotonNetwork.ViewCount;
            sizeListCentersEnnemi = Mathf.Ceil(deadUnitCenters.Count / 2f);
            for (int i = 0; i < sizeListCentersEnnemi; i++)
            {
                AddExtension(unitMore, owner);
                yield return new WaitUntil((() => PhotonNetwork.ViewCount == numberView + 1));
                numberView = PhotonNetwork.ViewCount;
            }

            deadUnitCenters.Clear();
        }

        public bool CheckInRange(GameObject v)
        {
            bool unitCheck = false;

            foreach (var center in unitsSelected.GetComponent<Monster>().GetCenters())
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
            if (unitsSelected != null)
            {
                unitsSelected.GetComponent<Monster>().NotChossen();
                unitsSelected = null;
            }

            if (unitTarget != null)
            {
                unitTarget.GetComponent<Monster>().NotChossen();
                unitTarget = null;
            }

            targetUnitAttack = 0;
        }
        

        [PunRPC]
        private void RPC_InstantiateEnemyTiles(int idMore, float x, float z)
        {
            unitFusion = PhotonNetwork.Instantiate("Tile", new Vector3(x, 0.55f, z), PlayerSetup.instance.transform.rotation, 0);
            playerView.RPC("RPC_SyncTiles", RpcTarget.All, unitFusion.GetComponent<PhotonView>().ViewID, idMore);
        }
        
        [PunRPC]
        private void RPC_SyncTiles(int id, int idmore)
        {
            PhotonView.Find(id).GetComponent<UnitExtension>().Init(idmore);
        }

        [PunRPC]
        private void RPC_Atk(int id)
        {
            PhotonNetwork.Destroy(PhotonView.Find(id).gameObject);
        }

        [PunRPC]
        private void RPC_TakeDamageUnit(int unitSelected, int damage, int unitTarget, int damage2)
        {
            PlacementManager.instance.SearchMobWithID(unitSelected).Atk -= Mathf.Abs(damage);
            PlacementManager.instance.SearchMobWithID(unitTarget).Atk -= Mathf.Abs(damage2);
        }

    }

