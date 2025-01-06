using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    [SerializeField] private Transform _attackSourcePlace;
    [SerializeField] private Transform _attackTargetPlace;

    public static CardManager Instance;
    private void Awake()
    {
        Instance = this;
    }

    [SerializeField] private CardOrderer _cardOrderer;
    void Start()
    {
        _cardOrderer.Init();
    }

    public Transform GetAttackSourcePlace()
    {
        return _attackSourcePlace;
    }

    public Transform GetAttackTargetPlace()
    {
        return _attackTargetPlace;
    }

}
