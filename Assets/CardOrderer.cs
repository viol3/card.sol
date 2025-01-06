using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardOrderer : MonoBehaviour
{
    [SerializeField] private Card[] _cards;
    public void Init()
    {
        for (int i = 0; i < _cards.Length; i++)
        {
            _cards[_cards.Length - i - 1].SetOrder(i * 10);
        }
    }

}
