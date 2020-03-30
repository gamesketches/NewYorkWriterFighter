using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArabelleSuper : MonoBehaviour
{
    public AnimationCurve xTravel;
    FighterController player;
    public float delay;
    public int[] frameChanges;
    HitBoxController hitBoxController;
    AttackData startingData;

    // Use this for initialization
    void Awake()
    {
        player = transform.parent.parent.gameObject.GetComponent<FighterController>();
        hitBoxController = player.GetComponentInChildren<HitBoxController>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnEnable()
    {
        StartCoroutine(SuperMovement());
        StartCoroutine(UpdateAttackData());
    }

    private void OnDisable()
    {
        hitBoxController.SetAttackData(startingData);
    }

    IEnumerator UpdateAttackData()
    {
        startingData = hitBoxController.GetAttackData();
        for(int i = 0; i < frameChanges.Length; i++)
        {
            yield return new WaitForSeconds(frameChanges[i] * CharacterAnimator.frameSpeed * Time.fixedDeltaTime);
            hitBoxController.UpdateAttackId();
        }
        //yield return new WaitForSeconds(frameChanges[frameChanges.Length - 1] * CharacterAnimator.frameSpeed * Time.fixedDeltaTime);
        hitBoxController.UpdateAttackId();
        hitBoxController.UpdateAttackDamage(200);
        hitBoxController.UpdateKnockdownState(true);
        hitBoxController.UpdateKnockBack(1.4f);
    }

    IEnumerator SuperMovement()
    {
        yield return new WaitForSeconds(delay);
        float travelTime = xTravel.keys[xTravel.length - 1].time;
        for (float t = 0; t < travelTime; t += Time.fixedDeltaTime)
        {
            if (player.leftSide)
            {
                player.MoveRight(xTravel.Evaluate(t));
            }
            else
            {
                player.MoveLeft(xTravel.Evaluate(t));
            }
            yield return null;
        }
    }
}
