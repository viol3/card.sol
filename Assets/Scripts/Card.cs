using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
    [SerializeField] private string _cardName = "Homing Fire";
    [SerializeField] private float _idleScale = 1f;
    [SerializeField] private float _hoverShowScale = 1.1f;
    [SerializeField] private float _hoverShowOffsetY = 0.2f;
    [SerializeField] private float _offsetYSpeed = 1f;
    [SerializeField] private float _scaleSpeed = 1f;
    [Space]
    [SerializeField] private float _attackShowSourceScale = 1.2f;
    [SerializeField] private float _attackShowSourceScaleDuration = 1f;
    [SerializeField] private float _attackShowSourcePlaceDuration = 1f;
    [SerializeField] private float _attackShowTargetScale = 1.2f;
    [SerializeField] private float _attackShowTargetScaleDuration = 1f;
    [SerializeField] private float _attackShowTargetPlaceDuration = 1.2f;
    [Space]
    [SerializeField] private MeshRenderer[] _cardMeshes;
    [SerializeField] private Transform _inter;

    private bool _hovering = false;

    private int _idleOrder = 0;

    private static bool _attackShowInProgress;

    public void SetOrder(int order)
    {
        for (int i = 0; i < _cardMeshes.Length; i++)
        {
            _cardMeshes[i].material.renderQueue = 3000 + order;
        }
        _idleOrder = order;
    }

    void MouseHover()
    {
        if(_attackShowInProgress)
        {
            return;
        }
        _hovering = true;
        SetOutlineActive(true);
        for (int i = 0; i < _cardMeshes.Length; i++)
        {
            _cardMeshes[i].material.renderQueue = 3500;
            _inter.DOKill();
            _inter.DOScale(_hoverShowScale, _scaleSpeed).SetSpeedBased();
            _inter.DOLocalMove(_inter.up * _hoverShowOffsetY, _offsetYSpeed).SetSpeedBased();
        }
    }

    void MouseExit()
    {
        if (_attackShowInProgress)
        {
            return;
        }
        _hovering = false;
        SetOutlineActive(false);
        for (int i = 0; i < _cardMeshes.Length; i++)
        {
            _cardMeshes[i].material.renderQueue = 3000 + _idleOrder;
            _inter.DOKill();
            _inter.DOScale(Vector3.one, _scaleSpeed).SetSpeedBased();
            _inter.DOLocalMove(Vector3.zero, _offsetYSpeed).SetSpeedBased();
        }
    }

    void AttackShow()
    {
        if(!NFTManager.Instance.IsOwningNft(_cardName))
        {
            ShortMessagePanel.Instance.Show("You don't have this card.");
            return;
        }
        if (_attackShowInProgress)
        {
            return;
        }
        _attackShowInProgress = true;
        StartCoroutine(AttackShowProcess());
    }

    IEnumerator AttackShowProcess()
    {
        SetOutlineActive(false);
        transform.DORotate(Vector3.right * 28f, 0.1f, RotateMode.Fast);
        transform.DOMove(CardManager.Instance.GetAttackSourcePlace().position, _attackShowSourcePlaceDuration).SetEase(Ease.OutSine);
        transform.DOScale(_attackShowSourceScale, _attackShowSourceScaleDuration).SetEase(Ease.OutSine).WaitForCompletion();
        yield return new WaitForSeconds(_attackShowSourceScaleDuration);
        transform.DOKill();
        transform.DOMove(CardManager.Instance.GetAttackTargetPlace().position, _attackShowTargetPlaceDuration).SetEase(Ease.InSine);
        transform.DOScale(_attackShowTargetScale, _attackShowTargetScaleDuration).SetEase(Ease.InSine);
        yield return new WaitForSeconds(_attackShowTargetScaleDuration);
        _attackShowInProgress = false;
        gameObject.SetActive(false);
        Enemy.Instance.Hit();
        ShortMessagePanel.Instance.Show("You used the card.");
    }


    void SetOutlineActive(bool active)
    {
        GetOutline().gameObject.SetActive(active);
    }

    Renderer GetOutline()
    {
        return _cardMeshes[1];
    }

    private void OnMouseExit()
    {
        MouseExit();
    }

    private void OnMouseEnter()
    {
        MouseHover();
    }

    private void OnMouseUpAsButton()
    {
        Debug.Log("Clicked to : " + gameObject);
        AttackShow();
    }
}
