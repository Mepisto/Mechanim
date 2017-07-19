using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillChainManager : MonoBehaviourSingleton<SkillChainManager>
{
    [SerializeField] List<int> _chainedSkillIds = new List<int>();

    [SerializeField] private Animator _actor;

    private bool _isAction = false;

    private int _currentChainIndex = -1;

    public void StartSkillEvent()
    {
        _actor.SetBool("Use", false);
        //_actor.SetInteger("Id", -1);

        //_currentChainIndex = -1;
    }

    public void FinishChainEvent()
    {
        _actor.SetBool("Use", false);
        _actor.SetInteger("Id", -1);                
        _currentChainIndex = -1;
    }

    public void OnClickButton()
    {
        //if (_isAction) return;

        if (0 < _chainedSkillIds.Count && _currentChainIndex < _chainedSkillIds.Count - 1)
        {
            _isAction = true;
            ++_currentChainIndex;

            Debug.LogError("_currentChainIndex : " + _currentChainIndex);
            _actor.SetBool("Use", true);
            _actor.SetInteger("Id", _chainedSkillIds[_currentChainIndex]);
        }
        else
        {
            Debug.LogError("SkillChain Last");

        }
    }    

    private void SetNexkSkill()
    {
        //_currentChainIndex++;

        //if (_currentChainIndex >= _chainedSkillIds.Count)
        //{
        //    _actor.SetBool("Use", false);
        //    return;
        //}

        //_actor.SetInteger("Id", _chainedSkillIds[_currentChainIndex]);
    }
}
