using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private ParticleSystem _bloodSplashParticle;

    public static Enemy Instance;
    private void Awake()
    {
        Instance = this;
    }

    public void Hit()
    {
        _animator.Play("HitReceive");
        _bloodSplashParticle.Play();
    }

}
