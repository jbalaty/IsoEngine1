using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public interface IGameLogicEntity : IEventSystemHandler {
    void GameTurnStart();
    void GameTurnEnd();
    int TakeDamage(int damage);
    int DealDamage(int damage); 
}
