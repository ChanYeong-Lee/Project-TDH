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
                Vector3 direction = model.attack.mainTarget.model.transform.position - transform.position;
                direction.y = 0.0f;
                direction.Normalize();
                model.move.RotateWithoutNotify(direction);

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
            case CharacterState.Skill:
                break;
            case CharacterState.Upgrade:
                break;
        }
    }
}
