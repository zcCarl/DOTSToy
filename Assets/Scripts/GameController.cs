using System;
using Common;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class GameController :MonoBehaviour, InputActions.IUIActions
{
    [FormerlySerializedAs("Camera")] public UnityEngine.Camera camera;
    // private Entity e;
    private InputActions actions;
    private Vector2 mousePosition;
    private Plane _groundPlane;
    private void Awake()
    {
        actions = new InputActions();
        // actions.UI.Click.performed += ctx =>  OnMouseDown();
        // actions.UI.Point.performed += ctx =>  mousePosition = ctx.ReadValue<Vector2>();
        
        _groundPlane = new Plane(Vector3.up, Vector3.zero);
        actions.UI.SetCallbacks(this);
        actions.Enable();
    }


    public void OnNavigate(InputAction.CallbackContext context)
    {
        
    }

    public void OnSubmit(InputAction.CallbackContext context)
    {
        
    }

    public void OnCancel(InputAction.CallbackContext context)
    {
        
    }

    public void OnPoint(InputAction.CallbackContext context)
    {
        mousePosition = context.ReadValue<Vector2>();
        // Debug.Log("Point:" + mousePosition);
    }

    public void OnClick(InputAction.CallbackContext context)
    {
        Debug.Log("OnClick:" + mousePosition);
        var screenRay = camera.ScreenPointToRay(mousePosition);
        _groundPlane.Raycast(screenRay, out float enter);
        var worldPos = screenRay.GetPoint(enter);
        var e = World.DefaultGameObjectInjectionWorld.EntityManager.CreateEntity();
        Debug.Log(mousePosition);
        World.DefaultGameObjectInjectionWorld.EntityManager.AddComponentData(e, new Controller()
        {
            targetPoint = new float3(worldPos.x, 0, worldPos.z),
        });
    }

    public void OnRightClick(InputAction.CallbackContext context)
    {
        var e = World.DefaultGameObjectInjectionWorld.EntityManager.CreateEntity();
        World.DefaultGameObjectInjectionWorld.EntityManager.AddComponentData(e, new SpawnerEnqueue()
        {
            Element = new CharacterConfig()
            {
                Id = 1,
                WaitTime = 1,
                Name = "Test",
            }
        });
        World.DefaultGameObjectInjectionWorld.EntityManager.DestroyEntity(e);
        Debug.Log("OnRightClick:" + mousePosition);
    }

    public void OnMiddleClick(InputAction.CallbackContext context)
    {
        Debug.Log("OnMiddleClick:" + mousePosition);
    }

    public void OnScrollWheel(InputAction.CallbackContext context)
    {
        
    }

    public void OnTrackedDevicePosition(InputAction.CallbackContext context)
    {
       
    }

    public void OnTrackedDeviceOrientation(InputAction.CallbackContext context)
    {
       
    }
}