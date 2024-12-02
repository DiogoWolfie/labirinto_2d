using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FloatingJoystick : Joystick
{
    protected override void Start()
    {
        base.Start();
        // Garanta que o background do joystick está ativo no início
        background.gameObject.SetActive(true);
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        // Remove a alteração de posição para deixar o joystick fixo
        base.OnPointerDown(eventData);
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        // Não desativa o joystick ao soltar o dedo
        base.OnPointerUp(eventData);
    }
}
