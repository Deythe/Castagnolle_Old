using System.Collections;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class Gauge : MonoBehaviour
{
    private Ray ray;
    private bool waiting;
    private bool mooving;
    private RaycastHit hit;
    private GameObject target;
    private Vector3 originalPosition;
    private void Update()
    {
        if (Input.touchCount > 0)
        {
            if (RoundManager.instance.p_roundState != RoundManager.enumRoundState.DragUnitPhase)
            {
                ray = PlayerSetup.instance.GetCam().ScreenPointToRay(Input.GetTouch(0).position);
                Physics.Raycast(ray, out hit);

                if (hit.collider != null)
                {
                    switch (Input.GetTouch(0).phase)
                    {
                        case TouchPhase.Began:
                            if (hit.collider.transform.CompareTag("Dice"))
                            {
                                target = hit.collider.gameObject;
                                originalPosition = target.transform.position;

                                mooving = true;
                            }

                            break;

                        case TouchPhase.Moved:
                            if (mooving)
                            {
                                target.transform.position = new Vector3(hit.point.x, 0.8f, hit.point.z) + PlayerSetup.instance.transform.forward;;
                            }

                            break;

                        case TouchPhase.Ended:
                            mooving = false;
                            if (target != null)
                            {
                                if (DiceManager.instance.CheckPositionDiceGauge(target))
                                {
                                    Debug.Log("Test");
                                }

                                target.transform.position = originalPosition;
                            }

                            target = null;
                            waiting = false;
                            break;
                    }
                }
            }
        }
    }
}

