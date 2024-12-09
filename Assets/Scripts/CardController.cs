using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardController : MonoBehaviour
{
    public void FlipCard()
    {
        Debug.Log("TurnCard");
        // カードを回転させる
        transform.Rotate(180, 0, 0);
    }
}
