using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterBrain : MonoBehaviour
{
    public CharacterModel model;

    private void Awake()
    {
        model = GetComponent<CharacterModel>();
    }

    private void Update()
    {
        switch (model.state)
        {
            case CharacterState.Idle:
                if (model.attack.canAttack)
                {
                    if (model.skill.CheckAttackSkill(out Skill activatedSkill))
                    {
                        activatedSkill.Execute();
                    }
                    else
                    {
                        model.attack.StartAttack();
                    }
                }
                break;
            case CharacterState.Move:
                break;
            case CharacterState.Attack:

                if (model.attack.mainTarget.model != null)
                {
                    Vector3 direction = model.attack.mainTarget.model.transform.position - transform.position;
                    direction.y = 0.0f;
                    direction.Normalize();
                    model.move.RotateWithoutNotify(direction);

                    if (model.attack.isAttacking &&model.attack.mainTarget.model.gameObject.activeSelf == false)
                    {
                        model.attack.StopAttack();
                    }
                }
                
                if (model.attack.canAttack)
                {
                    if (model.skill.CheckAttackSkill(out Skill activatedSkill))
                    {
                        activatedSkill.Execute();
                    }
                    else
                    {
                        model.attack.StartAttack();
                    }
                }

                if (model.move.tryMoving)
                {
                    model.attack.StopAttack();
                }
                break;
            case CharacterState.Skill:
                break;
            case CharacterState.Upgrade:
                break;
        }
    }
}
