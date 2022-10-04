using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Photon.Pun;
using UnityEngine;


public class CastagneManager : MonoBehaviour
    {
        public static CastagneManager instance;

        [SerializeField] private PhotonView playerView;
        [SerializeField] private float range = 1;

        [SerializeField] private GameObject unitsSelected, unitTarget;

        [SerializeField] private List<Vector3> deadUnitCenters = new List<Vector3>();
        [SerializeField] private List<MeshRenderer> deadUnitMeshRenderers;
        [SerializeField] private List<Vector3> previsualisationUnitMore = new List<Vector3>();
        
        private GameObject unitFusion;

        private int motherUnitTiles;
        private MonstreData unitPivot;
        
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
        public int p_targetUnitAttack
        {
            get => targetUnitAttack;
        }

        public int p_result
        {
            get => result;
        }
        
        public GameObject p_unitTarget
        {
            get => unitTarget;
            set
            {
                unitTarget = value;
            }
        }

        public GameObject p_unitSelected
        {
            get => unitsSelected;
            set
            {
                unitsSelected = value;
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
            if (RoundManager.instance.p_roundState == RoundManager.enumRoundState.CastagnePhase)
            {
                if (Input.touchCount > 0)
                {
                    if (Input.GetTouch(0).phase == TouchPhase.Began)
                    {
                        Physics.Raycast(PlayerSetup.instance.GetCam().ScreenPointToRay(Input.GetTouch(0).position), out hit);
                        if (hit.collider != null)
                        {
                            foreach (var unit in PlacementManager.instance.p_board)
                            {
                                foreach (var cases in unit.emplacement)
                                {
                                    if (cases == new Vector2(Mathf.FloorToInt(hit.point.x) + 0.5f,
                                        Mathf.FloorToInt(hit.point.z) + 0.5f))
                                    {
                                        if (unitsSelected==null)
                                        {
                                            if (unit.monster.GetComponent<PhotonView>().AmOwner &&
                                                (!unit.monster.GetComponent<MonstreData>().p_attacked))
                                            {
                                                unitsSelected = unit.monster;
                                                unitsSelected.GetComponent<MonstreData>().BeChoosen();
                                                
                                                UiManager.instance.EnableDisableMenuNoChoice(true);
                                                
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
                                                    unitTarget.GetComponent<MonstreData>().InitColorsTiles();
                                                }
                                                
                                                unitTarget = unit.monster;
                                                UiManager.instance.p_instanceEnemyPointer.transform.position =
                                                    new Vector3(PlacementManager.instance.AverageCenterX(unitTarget), 3,PlacementManager.instance.AverageCenterZ(unitTarget));
                                                
                                                if (!UiManager.instance.p_instanceEnemyPointer.activeSelf)
                                                {
                                                    UiManager.instance.p_instanceEnemyPointer.SetActive(true);
                                                }

                                                unitTarget.GetComponent<MonstreData>().BeChoosen();
                                                PreCaculUnit();
                                                UiManager.instance.EnableDisableMenuYesChoice(true);
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
            targetUnitAttack = unitTarget.GetComponent<MonstreData>().p_atk;
            result = unitsSelected.GetComponent<MonstreData>().p_atk - targetUnitAttack;
            
            switch (result)
            {
                case >0:
                    TransformToVector3(unitTarget.GetComponent<MonstreData>().GetCenters(), deadUnitCenters);
                    deadUnitMeshRenderers = new List<MeshRenderer>(unitTarget.GetComponent<MonstreData>().GetMeshRenderers());
                    ShowAllUnitsExtension(unitsSelected);
                    break;
                case <0 :
                    TransformToVector3(unitsSelected.GetComponent<MonstreData>().GetCenters(), deadUnitCenters);
                    deadUnitMeshRenderers = new List<MeshRenderer>(unitsSelected.GetComponent<MonstreData>().GetMeshRenderers());
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
            unitsSelected.GetComponent<MonstreData>().p_attacked = true;
            
            if (unitsSelected.GetComponent<MonstreData>().p_animator != null)
            {
                unitsSelected.GetComponent<MonstreData>().p_animator.SetBool("ATK", true);
            }
            
            EffectManager.instance.View.RPC("RPC_PlayAnimation", RpcTarget.AllViaServer, 2, PlacementManager.instance.AverageCenterX(unitTarget),
                1f,
                PlacementManager.instance.AverageCenterZ(unitTarget)-PlayerSetup.instance.transform.forward.z, 3f);
            
            SoundManager.instance.PlaySFXSound(7, 0.07f);
            yield return new WaitForSeconds(0.5f);
            
            if (unitsSelected.GetComponent<MonstreData>().p_animator != null)
            {
                unitsSelected.GetComponent<MonstreData>().p_animator.SetBool("ATK", false);
            }
            
            if (unitTarget.GetComponent<MonstreData>().p_isMovable && !unitsSelected.GetComponent<MonstreData>().p_isChampion)
            {
                playerView.RPC("RPC_TakeDamageUnit", RpcTarget.AllViaServer,
                    unitsSelected.GetComponent<PhotonView>().ViewID, unitTarget.GetComponent<MonstreData>().p_atk,
                    unitTarget.GetComponent<PhotonView>().ViewID, unitsSelected.GetComponent<MonstreData>().p_atk);
                
                StartCoroutine(CoroutineAttackNormalUnit());
            }
            else
            {
                playerView.RPC("RPC_TakeDamageUnit", RpcTarget.AllViaServer,
                    unitsSelected.GetComponent<PhotonView>().ViewID, 0,
                    unitTarget.GetComponent<PhotonView>().ViewID, unitsSelected.GetComponent<MonstreData>().p_atk);

                StartCoroutine(CoroutineAttackStaticUnit());
            }
        }
        
        IEnumerator CoroutineAttackPlayer()
        {
            unitsSelected.GetComponent<MonstreData>().p_attacked = true;
            if (unitsSelected.GetComponent<MonstreData>().p_animator != null)
            {
                unitsSelected.GetComponent<MonstreData>().p_animator.SetBool("ATK", true);
            }

            yield return new WaitForSeconds(0.5f);
            
            if (unitsSelected.GetComponent<MonstreData>().p_animator != null)
            {
                unitsSelected.GetComponent<MonstreData>().p_animator.SetBool("ATK", false);
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
                    TransformToVector3(unitTarget.GetComponent<MonstreData>().GetCenters(), deadUnitCenters);
                    
                    LifeManager.instance.TakeDamageEnnemi(PlacementManager.instance.CenterMoreFar(unitsSelected));
                    StartCoroutine(AddAllExtension(unitsSelected, true));

                    if (unitsSelected.GetComponent<MonstreData>().HaveAnEffectThisPhase(EffectManager.enumEffectPhaseActivation.WhenThisUnitKill) && !unitsSelected.GetComponent<MonstreData>().p_effect.GetUsed())
                    {
                        EffectManager.instance.p_currentUnit = unitsSelected;
                        EffectManager.instance.UnitSelected(EffectManager.enumEffectPhaseActivation.WhenThisUnitKill);
                        yield return new WaitUntil(() => EffectManager.instance.EffectFinished());
                    }
                    
                    break;

                case <0:
                    TransformToVector3(unitsSelected.GetComponent<MonstreData>().GetCenters(), deadUnitCenters);
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
                    TransformToVector3(unitTarget.GetComponent<MonstreData>().GetCenters(), deadUnitCenters);
                    LifeManager.instance.TakeDamageEnnemi(PlacementManager.instance.CenterMoreFar(unitsSelected));
                    StartCoroutine(AddAllExtension(unitsSelected, true));

                    if (unitsSelected.GetComponent<MonstreData>().HaveAnEffectThisPhase(EffectManager.enumEffectPhaseActivation.WhenThisUnitKill) && !unitsSelected.GetComponent<MonstreData>().p_effect.GetUsed())
                    {
                        EffectManager.instance.p_currentUnit = unitsSelected;
                        EffectManager.instance.UnitSelected(EffectManager.enumEffectPhaseActivation.WhenThisUnitKill);
                        yield return new WaitUntil(() => EffectManager.instance.EffectFinished());
                    }
                    
                    EffectManager.instance.ActivateEffectWhenUnitDie();
                    break;
            }
            
            ClearUnits();
        }
        
        void ShowAllUnitsExtension(GameObject unitMore)
        {
            motherUnitTiles = unitMore.GetComponent<MonstreData>().p_id;
            TransformToVector3(unitMore.GetComponent<MonstreData>().GetCenters(), previsualisationUnitMore);
            
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
            numberTileUnit = unitMore.GetComponent<MonstreData>().p_extensions.Count;
            
            for (int i = 0; i < sizeListCentersDeadUnit; i++)
            {
                AddExtension(unitMore, owner);
                yield return new WaitUntil(() => unitMore.GetComponent<MonstreData>().p_extensions.Count == numberTileUnit + 1);
                numberTileUnit ++;
            }

            deadUnitCenters.Clear();
        }
        
        void AddExtension(GameObject unitMore, bool owner)
        {
            for (int j = 0; j < unitMore.GetComponent<MonstreData>().GetCenters().Count; j++)
            {
                for (int x = 0; x < deadUnitCenters.Count; x++)
                {
                    if (Vector3.Distance(deadUnitCenters[x],
                        unitMore.GetComponent<MonstreData>().GetCenters()[j].position).Equals(range))
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

            foreach (var center in unit.GetComponent<MonstreData>().GetCenters())
            {
                foreach (var targetCenter in v.GetComponent<MonstreData>().GetCenters())
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
                    foreach (var unit in PlacementManager.instance.p_board)
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
                    foreach (var unit in PlacementManager.instance.p_board)
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
                unitsSelected.GetComponent<MonstreData>().InitColorsTiles();
                unitsSelected = null;
            }

            if (unitTarget != null)
            {
                unitTarget.GetComponent<MonstreData>().InitColorsTiles();
                unitTarget = null;
            }

            targetUnitAttack = 0;
            
            UiManager.instance.EnableDisableMenuNoChoice(false);
            UiManager.instance.EnableDisableMenuYesChoice(false);
            EffectManager.instance.CheckAllHeroism();
            UiManager.instance.p_instanceEnemyPointer.SetActive(false);
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

