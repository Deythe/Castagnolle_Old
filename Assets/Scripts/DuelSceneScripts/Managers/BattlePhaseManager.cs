using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Photon.Pun;
using UnityEngine;


public class BattlePhaseManager : MonoBehaviour
    {
        public static BattlePhaseManager instance;

        [SerializeField] private PhotonView playerView;
        [SerializeField] private float range = 1;

        private GameObject unitsSelected;
        private GameObject  unitTarget = null;
        
        [SerializeField] private List<Vector3> deadUnitCenters = new List<Vector3>();
        [SerializeField] private List<MeshRenderer> deadUnitMeshRenderers;
        [SerializeField] private List<Vector3> previsualisationUnitMore = new List<Vector3>();
        
        private GameObject unitFusion;

        private int motherUnitTiles;
        private Monster unitPivot;
        
        private int result;

        private int numberTileUnit;
        private float sizeListCentersDeadUnit;
        
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
            if (RoundManager.instance.p_roundState == RoundManager.enumRoundState.CastagnePhase)
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
                                        if (unitsSelected!=null)
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
                                        else
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
                                                PreCaculUnit();
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        
        void PreCaculUnit()
        {
            targetUnitAttack = unitTarget.GetComponent<Monster>().p_atk;
            result = unitsSelected.GetComponent<Monster>().p_atk - targetUnitAttack;
            
            switch (result)
            {
                case >0:
                    TransformToVector3(unitTarget.GetComponent<Monster>().GetCenters(), deadUnitCenters);
                    deadUnitMeshRenderers = new List<MeshRenderer>(unitTarget.GetComponent<Monster>().GetMeshRenderers());
                    ShowAllUnitsExtension(unitsSelected);
                    break;
                case <0 :
                    TransformToVector3(unitsSelected.GetComponent<Monster>().GetCenters(), deadUnitCenters);
                    deadUnitMeshRenderers = new List<MeshRenderer>(unitsSelected.GetComponent<Monster>().GetMeshRenderers());
                    ShowAllUnitsExtension(unitTarget);
                    break;
            }
        }

        public void Attack()
        {
            StartCoroutine(CoroutineAttack());
        }
        
        IEnumerator CoroutineAttack()
        {
            unitsSelected.GetComponent<Monster>().p_attacked = true;
            
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
            switch (result)
            {
                case 0:
                    LifeManager.instance.View.RPC("RPC_TakeDamageEnnemi", RpcTarget.Others, 1);
                    LifeManager.instance.EnnemiLife--;
                    LifeManager.instance.TakeDamageHimself();
                    break;

                case >0:
                    TransformToVector3(unitTarget.GetComponent<Monster>().GetCenters(), deadUnitCenters);
                    
                    LifeManager.instance.TakeDamageEnnemi(PlacementManager.instance.CenterMoreFar(unitsSelected));
                    StartCoroutine(AddAllExtension(unitsSelected, true));

                    if (unitsSelected.GetComponent<Monster>().HaveAnEffectThisPhase(EffectManager.enumEffectPhaseActivation.WhenThisUnitKill))
                    {
                        unitsSelected.GetComponent<Monster>().ActivateEffects(EffectManager.enumEffectPhaseActivation.WhenThisUnitKill);
                        yield return new WaitUntil(() => unitsSelected.GetComponent<Monster>().ReturnUsedOfAnEffect());
                    }
                    
                    break;

                case <0:
                    TransformToVector3(unitsSelected.GetComponent<Monster>().GetCenters(), deadUnitCenters);
                    LifeManager.instance.TakeDamageHimself();
                    
                    StartCoroutine(AddAllExtension(unitTarget, false));
                    break;
            }
            
            EffectManager.instance.ActivateEffectWhenUnitDie();
            ClearUnits();
        }
        
        IEnumerator CoroutineAttackStaticUnit()
        {
            switch (result)
            {
                case 0:
                case >0:
                    TransformToVector3(unitTarget.GetComponent<Monster>().GetCenters(), deadUnitCenters);
                    LifeManager.instance.TakeDamageEnnemi(PlacementManager.instance.CenterMoreFar(unitsSelected));
                    StartCoroutine(AddAllExtension(unitsSelected, true));

                    if (unitsSelected.GetComponent<Monster>().HaveAnEffectThisPhase(EffectManager.enumEffectPhaseActivation.WhenThisUnitKill))
                    {
                        unitsSelected.GetComponent<Monster>().ActivateEffects(EffectManager.enumEffectPhaseActivation.WhenThisUnitKill);
                        yield return new WaitUntil(() => unitsSelected.GetComponent<Monster>().ReturnUsedOfAnEffect());
                    }
                    
                    EffectManager.instance.ActivateEffectWhenUnitDie();
                    break;
            }
            ClearUnits();
        }
        
        void ShowAllUnitsExtension(GameObject unitMore)
        {
            motherUnitTiles = unitMore.GetComponent<Monster>().p_id;
            TransformToVector3(unitMore.GetComponent<Monster>().GetCenters(), previsualisationUnitMore);
            
            sizeListCentersDeadUnit = Mathf.Ceil(deadUnitCenters.Count / 2f);

            for (int i = 0; i < sizeListCentersDeadUnit; i++)
            {
                ShowUnitExtension();
            }
            
            previsualisationUnitMore.Clear();
        }
        
        void ShowUnitExtension()
        {
            for (int j = 0; j < previsualisationUnitMore.Count; j++)
            {
                for (int x = 0; x < deadUnitCenters.Count; x++)
                {
                    if (Vector3.Distance(deadUnitCenters[x],
                        previsualisationUnitMore[j]).Equals(range))
                    {
                        ChangeColorShowUnit(x);

                        previsualisationUnitMore.Add(deadUnitCenters[x]);
                        deadUnitCenters.RemoveAt(x);
                        return;
                    }
                }
            }
        }

        void ChangeColorShowUnit(int x)
        {
            deadUnitMeshRenderers[x].material.DOColor(Color.red, 1).OnComplete(() =>
                deadUnitMeshRenderers[x].material.DOColor(Color.green, 1).OnComplete(()=>ChangeColorShowUnit(x)));
        }

        IEnumerator AddAllExtension(GameObject unitMore, bool owner)
        {
            numberTileUnit = unitMore.GetComponent<Monster>().p_extensions.Count;
            
            for (int i = 0; i < sizeListCentersDeadUnit; i++)
            {
                AddExtension(unitMore, owner);
                yield return new WaitUntil(() => unitMore.GetComponent<Monster>().p_extensions.Count == numberTileUnit + 1);
                numberTileUnit ++;
            }

            deadUnitCenters.Clear();
        }
        
        void AddExtension(GameObject unitMore, bool owner)
        {
            for (int j = 0; j < unitMore.GetComponent<Monster>().GetCenters().Count; j++)
            {
                for (int x = 0; x < deadUnitCenters.Count; x++)
                {
                    if (Vector3.Distance(deadUnitCenters[x],
                        unitMore.GetComponent<Monster>().GetCenters()[j].position).Equals(range))
                    {
                        if (owner)
                        {
                            unitFusion = PhotonNetwork.Instantiate("Tile", deadUnitCenters[x],
                                PlayerSetup.instance.transform.rotation, 0);
                            playerView.RPC("RPC_SyncTiles", RpcTarget.AllViaServer, unitFusion.GetComponent<PhotonView>().ViewID, motherUnitTiles);
                        }
                        else
                        {
                            playerView.RPC("RPC_InstantiateEnemyTiles", RpcTarget.Others,
                                motherUnitTiles, deadUnitCenters[x].x,
                                deadUnitCenters[x].z);
                        }
                        
                        deadUnitCenters.RemoveAt(x);
                        return;
                    }
                }
            }
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
            for (int i = 0; i < deadUnitMeshRenderers.Count; i++)
            {
                deadUnitMeshRenderers[i].material.DOKill(false);
            }
            ClearUnits();
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
        
        void TransformToVector3(List<Transform> transf, List<Vector3> vect)
        {
            vect.Clear();
            for (int i = 0; i < transf.Count; i++)
            {
               vect.Add(transf[i].position);
            }
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

