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

        private int motherUnitTiles;
        private Monster unitPivot;
        
        private int result;
        private bool isAttacking;
        
        private int numberTileUnit;
        private float sizeListCentersEnnemi;
        
        private int targetUnitAttack;
        private bool isWaiting;
        
        private bool pivot;
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
            if (instance == null)
            {
                instance = this;
            }else
            {
                Destroy(gameObject);
            }
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
                                            if (unit.monster.GetComponent<PhotonView>().AmOwner &&
                                                (!unit.monster.GetComponent<Monster>().p_attacked))
                                            {
                                                unitsSelected = unit.monster;
                                                unitsSelected.GetComponent<Monster>().BeChoosen();

                                                if (CheckBackLaneEnemy(unitsSelected))
                                                {
                                                    StartCoroutine(CoroutineAttackPlayer());
                                                }
                                            }
                                        }
                                        else if (RoundManager.instance.StateRound == 4)
                                        {
                                            if (!unit.monster.GetComponent<PhotonView>().AmOwner &&
                                                CheckInRange(unitsSelected, unit.monster))
                                            {
                                                if (unitTarget != null)
                                                {
                                                    unitTarget.GetComponent<Monster>().InitColorsTiles();
                                                }
                                                
                                                unitTarget = unit.monster;
                                                UiManager.instance.p_instanceEnemyPointer.transform.position =
                                                    new Vector3(PlacementManager.instance.AverageCenterX(unitTarget), 3,PlacementManager.instance.AverageCenterZ(unitTarget));
                                                
                                                if (!UiManager.instance.p_instanceEnemyPointer.activeSelf)
                                                {
                                                    UiManager.instance.p_instanceEnemyPointer.SetActive(true);
                                                }

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
                        if (unitsSelected != null && !RoundManager.instance.StateRound.Equals(4))
                        {
                            RoundManager.instance.StateRound = 4;
                        }
                    }
                }
            }
        }
        
        public void Attack()
        {
            StartCoroutine(CoroutineAttack());
        }
        
        IEnumerator CoroutineAttack()
        {
            if (unitsSelected.GetComponent<Monster>().p_animator != null)
            {
                unitsSelected.GetComponent<Monster>().p_animator.SetBool("ATK", true);
            }
            
            EffectManager.instance.View.RPC("RPC_PlayAnimation", RpcTarget.AllViaServer, 2, PlacementManager.instance.AverageCenterX(unitTarget),
                1f,
                PlacementManager.instance.AverageCenterZ(unitTarget)-PlayerSetup.instance.transform.forward.z, 3f);
            SoundManager.instance.PlaySFXSound(7, 0.07f);
            yield return new WaitForSeconds(0.5f);
            
            if (unitsSelected.GetComponent<Monster>().p_animator != null)
            {
                unitsSelected.GetComponent<Monster>().p_animator.SetBool("ATK", false);
            }
            
            if (unitTarget.GetComponent<Monster>().p_isMovable && !unitsSelected.GetComponent<Monster>().p_isChampion)
            {
                playerView.RPC("RPC_TakeDamageUnit", RpcTarget.AllViaServer,
                    unitsSelected.GetComponent<PhotonView>().ViewID, unitTarget.GetComponent<Monster>().p_atk,
                    unitTarget.GetComponent<PhotonView>().ViewID, unitsSelected.GetComponent<Monster>().p_atk);
                
                StartCoroutine(CoroutineAttackNormalUnit());
            }
            else
            {
                playerView.RPC("RPC_TakeDamageUnit", RpcTarget.AllViaServer,
                    unitsSelected.GetComponent<PhotonView>().ViewID, 0,
                    unitTarget.GetComponent<PhotonView>().ViewID, unitsSelected.GetComponent<Monster>().p_atk);
                
                StartCoroutine(CoroutineAttackStaticUnit());
            }
        }
        

        IEnumerator CoroutineAttackPlayer()
        {
            unitsSelected.GetComponent<Monster>().p_attacked = true;
            if (unitsSelected.GetComponent<Monster>().p_animator != null)
            {
                unitsSelected.GetComponent<Monster>().p_animator.SetBool("ATK", true);
            }

            yield return new WaitForSeconds(0.5f);
            
            if (unitsSelected.GetComponent<Monster>().p_animator != null)
            {
                unitsSelected.GetComponent<Monster>().p_animator.SetBool("ATK", false);
            }
            
            LifeManager.instance.TakeDamageEnnemi(PlacementManager.instance.CenterMoreFar(unitsSelected));
            CancelSelection();
        }

        IEnumerator CoroutineAttackNormalUnit()
        {
            targetUnitAttack = unitTarget.GetComponent<Monster>().p_atk;
            result = unitsSelected.GetComponent<Monster>().p_atk - targetUnitAttack;
            unitsSelected.GetComponent<Monster>().p_attacked = true;

            switch (result)
            {
                case 0:
                    LifeManager.instance.View.RPC("RPC_TakeDamageEnnemi", RpcTarget.Others, 1);
                    LifeManager.instance.EnnemiLife--;
                    LifeManager.instance.TakeDamageHimself();
                    break;

                case >0:
                    deadUnitCenters = new List<Transform> (unitTarget.GetComponent<Monster>().GetCenters());
                    LifeManager.instance.TakeDamageEnnemi(PlacementManager.instance.CenterMoreFar(unitsSelected));
                    StartCoroutine(AddAllExtension(unitsSelected, true));

                    if (unitsSelected.GetComponent<Monster>().HaveAnEffectThisTurn(1))
                    {
                        unitsSelected.GetComponent<Monster>().ActivateEffects(1);
                        yield return new WaitUntil(() => unitsSelected.GetComponent<Monster>().ReturnUsedOfAnEffect(1));
                    }
                    
                    break;

                case <0:
                    LifeManager.instance.TakeDamageHimself();
                    deadUnitCenters = new List<Transform> (unitTarget.GetComponent<Monster>().GetCenters());


                    StartCoroutine(AddAllExtension(unitTarget, false));

                    break;
            }
            
            EffectManager.instance.ActivateEffectWhenUnitDie();
            RoundManager.instance.StateRound = 3;
            isAttacking = false;
            ClearUnits();
        }
        
        IEnumerator CoroutineAttackStaticUnit()
        {
            targetUnitAttack = unitTarget.GetComponent<Monster>().p_atk;
            result = unitsSelected.GetComponent<Monster>().p_atk - targetUnitAttack;
            unitsSelected.GetComponent<Monster>().p_attacked = true;

            switch (result)
            {
                case 0:
                case >0:
                    deadUnitCenters = new List<Transform> (unitTarget.GetComponent<Monster>().GetCenters());
                    LifeManager.instance.TakeDamageEnnemi(PlacementManager.instance.CenterMoreFar(unitsSelected));
                    StartCoroutine(AddAllExtension(unitsSelected, true));

                    if (unitsSelected.GetComponent<Monster>().HaveAnEffectThisTurn(1))
                    {
                        unitsSelected.GetComponent<Monster>().ActivateEffects(1);
                        yield return new WaitUntil(() => unitsSelected.GetComponent<Monster>().ReturnUsedOfAnEffect(1));
                    }
                    EffectManager.instance.ActivateEffectWhenUnitDie();
                    break;
            }
            
            RoundManager.instance.StateRound = 3;
            isAttacking = false;
            ClearUnits();
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
                            playerView.RPC("RPC_SyncTiles", RpcTarget.AllViaServer, unitFusion.GetComponent<PhotonView>().ViewID, motherUnitTiles);
                        }
                        else
                        {
                            playerView.RPC("RPC_InstantiateEnemyTiles", RpcTarget.Others,
                                motherUnitTiles, deadUnitCenters[x].position.x,
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
            motherUnitTiles = unitMore.GetComponent<Monster>().p_id;
            numberTileUnit = unitMore.GetComponent<Monster>().p_extensions.Count;

            sizeListCentersEnnemi = Mathf.Ceil(deadUnitCenters.Count / 2f);
            
            for (int i = 0; i < sizeListCentersEnnemi; i++)
            {
                AddExtension(unitMore, owner);
                yield return new WaitUntil((() => unitMore.GetComponent<Monster>().p_extensions.Count == numberTileUnit + 1));
                numberTileUnit = numberTileUnit + 1;
            }

            deadUnitCenters.Clear();
        }

        public bool CheckInRange(GameObject unit, GameObject v)
        {
            bool unitCheck = false;

            foreach (var center in unit.GetComponent<Monster>().GetCenters())
            {
                foreach (var targetCenter in v.GetComponent<Monster>().GetCenters())
                {
                    if (Vector2.Distance(new Vector2(center.position.x, center.position.z), new Vector2(targetCenter.position.x, targetCenter.position.z)).Equals(range))
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

        
        bool CheckBackLaneEnemy(GameObject unitSelected)
        {
            if (RoundManager.instance.p_localPlayerTurn == 1)
            {
                if (PlacementManager.instance.CenterMoreFar(unitSelected).Equals(4.5f))
                {
                    foreach (var unit in PlacementManager.instance.GetBoard())
                    {
                        if (!unit.monster.GetComponent<PhotonView>().AmOwner)
                        {
                            if (CheckInRange(unitSelected, unit.monster))
                            {
                                return false;
                            }
                        }
                    }

                    return true;
                }
            }
            else
            {
                if (PlacementManager.instance.CenterMoreFar(unitsSelected).Equals(-4.5f))
                {
                    foreach (var unit in PlacementManager.instance.GetBoard())
                    {
                        if (!unit.monster.GetComponent<PhotonView>().AmOwner)
                        {
                            if (CheckInRange(unitSelected, unit.monster))
                            {
                                return false;
                            }
                        }
                    }
                    
                    return true;
                }
            }
            
            return false;
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
                unitsSelected.GetComponent<Monster>().InitColorsTiles();
                unitsSelected = null;
            }

            if (unitTarget != null)
            {
                unitTarget.GetComponent<Monster>().InitColorsTiles();
                unitTarget = null;
            }

            targetUnitAttack = 0;
        }
        

        [PunRPC]
        private void RPC_InstantiateEnemyTiles(int idMore, float x, float z)
        {
            unitFusion = PhotonNetwork.Instantiate("Tile", new Vector3(x, 0.5f, z), PlayerSetup.instance.transform.rotation, 0);
            playerView.RPC("RPC_SyncTiles", RpcTarget.AllViaServer, unitFusion.GetComponent<PhotonView>().ViewID, idMore);
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
            unitPivot = PlacementManager.instance.SearchMobWithID(unitSelected);
            UiManager.instance.PlayHitMarker(unitPivot.transform.position, damage);
            unitPivot.p_atk -= Mathf.Abs(damage);
            
            unitPivot = PlacementManager.instance.SearchMobWithID(unitTarget);
            UiManager.instance.PlayHitMarker(unitPivot.transform.position, damage2);
            unitPivot.p_atk -= Mathf.Abs(damage2);
        }
    }

