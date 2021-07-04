using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    [SerializeField] private Animator _playerAnimator;
    [SerializeField] private string _idleAnimation;
    [SerializeField] private string _wallSlideAnimation;
    [SerializeField] private string _launchAnimation;
    [SerializeField] private string _surfAnimation;

    private readonly Vector2 scaleRight = Vector2.one;
    private readonly Vector2 scaleLeft = new Vector2(-1,1);

    private EAnimState _currentAnimation;

    //TODO use hashes for efficiency
    public void SwapToAnim(EAnimState animstate, bool facingRight)
    {
        transform.localScale = facingRight ? scaleRight : scaleLeft;

        //No need to play an already playing animation
        if(_currentAnimation == animstate) { return; }

        switch (animstate)
        {
            case EAnimState.idle:
                _playerAnimator.Play(_idleAnimation);
                break;
            case EAnimState.wallSlide:
                _playerAnimator.Play(_wallSlideAnimation);
                break;
            case EAnimState.launch:
                _playerAnimator.Play(_launchAnimation);
                break;
            case EAnimState.surf:
                _playerAnimator.Play(_surfAnimation);
                break;
            default:
                break;
        }
        _currentAnimation = animstate;

    }
}

public enum EAnimState { idle, surf, wallSlide, launch };