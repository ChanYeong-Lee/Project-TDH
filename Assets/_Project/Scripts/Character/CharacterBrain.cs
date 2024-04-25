using Photon.Pun;
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
                        activatedSkill.StartSkill();
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

                if (model.attack.mainTarget != null && model.attack.mainTarget.gameObject.activeSelf)
                {
                    Vector3 direction = model.attack.mainTarget.transform.position - transform.position;
                    direction.y = 0.0f;
                    direction.Normalize();
                    print(Vector3.Dot(direction, transform.forward));
                    model.move.Rotate(direction);
                    //transform.forward = direction;
                }
                else
                {
                    if (model.attack.isAttacking)
                    {
                        model.attack.CancelAttack();
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
