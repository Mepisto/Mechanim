using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillStateChacker : StateMachineBehaviour
{
    [SerializeField] private bool _isFinishChacker = false;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);

        if (_isFinishChacker) return;

        SkillChainManager.Instance.StartSkillEvent();
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateExit(animator, stateInfo, layerIndex);

        if (_isFinishChacker == false) return;

        SkillChainManager.Instance.FinishChainEvent();
    }
}
