// using State;
// using UnityEngine;
// using UnityEngine.InputSystem;
//
// namespace Camera
// {
//     public class CameraMachine : StateMachine<CameraController>
//     {
//         public CameraMachine(CameraController context) : base(context)
//         {
//             
//         }
//     }
//
//     public class NormalState : State<CameraController>
//     {
//         public override void Update(CameraController context)
//         {
//             Vector3 pos = context.transform.position;
//
//             if (Keyboard.current.wKey.isPressed || Mouse.current.position.y.ReadValue() >= Screen.height - 1)
//             {
//                 pos.z += context.panSpeed * Time.deltaTime;
//             }
//             if (Keyboard.current.sKey.isPressed || Mouse.current.position.y.ReadValue() <= 1)
//             {
//                 pos.z -= context.panSpeed * Time.deltaTime;
//             }
//             if (Keyboard.current.aKey.isPressed || Mouse.current.position.x.ReadValue() <= 1)
//             {
//                 pos.x -= context.panSpeed * Time.deltaTime;
//             }
//             if (Keyboard.current.dKey.isPressed || Mouse.current.position.x.ReadValue() >= Screen.width - 1)
//             {
//                 pos.x += context.panSpeed * Time.deltaTime;
//             }
//             context.transform.position = pos;
//         }
//     }
//
//     public class DraggingState : State<CameraController>
//     {
//         public override void Update(CameraController context)
//         {
//             Vector3 pos = context.transform.position;
//             Vector2 panDirection = Mouse.current.delta.ReadValue();
//             pos.x -= panDirection.x * Time.deltaTime * context.panSpeed * 0.1f;
//             pos.z -= panDirection.y * Time.deltaTime * context.panSpeed * 0.1f;
//             context.transform.position = pos;
//         }
//     }
//     
// }