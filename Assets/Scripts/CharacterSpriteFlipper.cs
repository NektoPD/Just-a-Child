using System;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class CharacterSpriteFlipper : MonoBehaviour
{
    [SerializeField] private MovementController _movementController;
    
    private SpriteRenderer _spriteRenderer;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        _movementController.MovedX += SetSpriteDirection;
    }

    private void OnDisable()
    {
        _movementController.MovedX -= SetSpriteDirection;
    }

    private void SetSpriteDirection(float movementDirection)
    {
        if(movementDirection == 0)
            return;
        
        _spriteRenderer.flipX = movementDirection < 0;
    }
}