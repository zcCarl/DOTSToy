using System;
// using Camera;
using State;
using Unity.Cinemachine;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class CameraController : MonoBehaviour , IContext
{
    // public Transform Patch, Zoom;
    
    public float panSpeed = 20f;
    public float zoomSpeed = 1f;
    public float minZoom = 20f;
    public float maxZoom = 100f;
    public float minDistance = 10f;
    public float maxDistance = 50f;
    
    public float minPatch = 35f;
    public float maxPatch = 90f;
    public Vector4 mapSize = new Vector4(1000, 1000, 1000, 1000);
    
    private InputActions actions;
    private Vector2 moveInput;
    private Vector2 zoomInput;
    
    
    // private CameraMachine cameraMachine;
    public CinemachineCamera c_cam;
    public Camera camera;
    
    private void Awake()
    {
        // c_cam = GetComponent<CinemachineCamera>();
        // camera = c_cam.camer;
        actions = new InputActions();
        actions.UI.Navigate.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        actions.UI.Navigate.canceled += ctx => moveInput = Vector2.zero;
        actions.UI.ScrollWheel.performed += ctx => zoomInput = ctx.ReadValue<Vector2>();
        actions.UI.ScrollWheel.canceled += ctx => zoomInput = Vector2.zero;
        actions.Enable();
        if (!cc)
        {
            cc = GetComponentInChildren<CinemachineCamera>();
        }
        
        if (!cpc)
        {
            cpc = cc.GetComponent<CinemachinePositionComposer>();
        }

        if (!cfz)
        {
            cfz = cc.GetComponent<CinemachineFollowZoom>();
        }
        mousePoint = c_cam.Follow.position;
    }
    
    private void Start()
    {
        // cameraMachine = new CameraMachine(this);
        // cameraMachine.ChangeState(new NormalState());
    }
    
    void Update()
    {
        var move = HandleMovement();
        var zoom = HandleZoom();
        UpdateProjection();
        if (move || zoom)
        {
            ClampCameraPosition();
            // if (zoom)
            // {
            //     transform.position -= (_center - _lastCenter);
            //     _lastCenter = _center;
            // }
        }
    }
    
    public bool HandleMovement()
    {
        Vector3 pos = transform.position;
        pos.x += moveInput.x * panSpeed * Time.deltaTime;
        pos.y = 0;
        pos.z += moveInput.y * panSpeed * Time.deltaTime;
        transform.position = pos;
        return moveInput.magnitude > 0;
    }
    
    private float _zoomPercentage = 0;
    private bool HandleZoom()
    {
        // var position = Zoom.position; 
        // Debug.Log(zoomInput+"---"+Mathf.Sign(zoomInput));
        if (Mathf.Abs(zoomInput.magnitude)>0.001f)
        {
            float percentage = _zoomPercentage - zoomInput.y * zoomSpeed * Time.deltaTime;
            percentage = Mathf.Lerp(_zoomPercentage, percentage, 0.2f);
            percentage = Mathf.Clamp(percentage, 0, 1);
            _zoomPercentage = percentage;
            cfz.Width = Mathf.Lerp(minZoom, maxZoom, percentage);
            c_cam.transform.localEulerAngles = new Vector3(Mathf.Lerp(minPatch, maxPatch, percentage), 0, 0);
            
            // Ray mouseRay = camera.ScreenPointToRay(actions.UI.Point.ReadValue<Vector2>());
            // if ((1 - percentage)>Mathf.Epsilon && percentage>Mathf.Epsilon)
            // {
            //     _groundPlane.Raycast(mouseRay, out float enter);
            //     mousePoint = mouseRay.GetPoint(enter);
            //     c_cam.Follow.position = Vector3.Lerp(c_cam.Follow.position,mousePoint,percentage);
            // }
            Debug.Log("2----"+mousePoint);
            return true; //math.abs(newDistance - lastDistance) > 0;
        }

        return false;
    }

    private Vector3 mousePoint;
    private Vector3 _center;
    private Vector3 _lastCenter;
    readonly Vector3[] _corners = new Vector3[4]; 
    private Plane _groundPlane = new Plane(Vector3.up, Vector3.zero);
    private void UpdateProjection()
    {
        
        Ray bottomLeftRay = camera.ViewportPointToRay(new Vector3(0, 0, 0));
        Ray topLeftRay = camera.ViewportPointToRay(new Vector3(0, 1, 0));
        Ray topRightRay = camera.ViewportPointToRay(new Vector3(1, 1, 0));
        Ray bottomRightRay = camera.ViewportPointToRay(new Vector3(1, 0, 0));
        // Debug.DrawRay(topLeftRay.origin, topLeftRay.direction * 1000, Color.red);
        // Debug.DrawRay(topRightRay.origin, topRightRay.direction * 1000, Color.red);
        // Debug.DrawRay(bottomLeftRay.origin, bottomLeftRay.direction * 1000, Color.red);
        // Debug.DrawRay(bottomRightRay.origin, bottomRightRay.direction * 1000, Color.red);
        float enter;
        _groundPlane.Raycast(bottomLeftRay, out enter);
        _corners[0] = bottomLeftRay.GetPoint(enter);
        _groundPlane.Raycast(topLeftRay, out enter);
        _corners[1] = topLeftRay.GetPoint(enter);
        _groundPlane.Raycast(topRightRay, out enter);
        _corners[2] = topRightRay.GetPoint(enter);
        _groundPlane.Raycast(bottomRightRay, out enter);
        _corners[3] = bottomRightRay.GetPoint(enter);
        
        // Debug.Log(mousePoint);
        Ray centerRay = camera.ViewportPointToRay(new Vector3(0.5f,0.5f,0));
        _groundPlane.Raycast(centerRay, out enter);
        _center = centerRay.GetPoint(enter);
    }

    private void OnDrawGizmos()
    {
        Debug.DrawLine(_corners[0],_corners[1], Color.green);
        Debug.DrawLine(_corners[1],_corners[2], Color.green);
        Debug.DrawLine(_corners[2],_corners[3], Color.green);
        Debug.DrawLine(_corners[3],_corners[0], Color.green);
    }

    private void ClampCameraPosition()
    {
        Vector3 pos = transform.position;
        var center = (_corners[0] + _corners[1] + _corners[2] + _corners[3]) / 4;
        if (_lastCenter == Vector3.zero)
        {
            _lastCenter = center;
        }
        float minX = Mathf.Max(_corners[0].x, _corners[1].x);
        float maxX = Mathf.Min( _corners[2].x, _corners[3].x);
        float minZ = Mathf.Max(_corners[0].z,  _corners[3].z);
        float maxZ = Mathf.Min( _corners[1].z, _corners[2].z);
        float width = maxX - minX;
        float height = maxZ - minZ;
        // Debug.Log("width: " + width + " height: " + height);
        // Debug.Log("Before minX: " + minX + " maxX: " + maxX + " minZ: " + minZ + " maxZ: " + maxZ);
        var should = center;
        if (minX < mapSize.x)
        {
            minX = math.lerp(minX,mapSize.x,0.1f);
            maxX = minX + width;
            should.x = (minX + maxX) / 2;
        }
        else if (maxX > mapSize.y)
        {
            maxX = math.lerp(maxX,mapSize.y,0.1f);
            minX = maxX - width;
            should.x = (minX + maxX) / 2;
        }
        if (minZ < mapSize.z)
        {
            minZ = math.lerp(minZ,mapSize.z,0.1f);
            maxZ = minZ + height;
            should.z = (minZ + maxZ) / 2;
        }
        else if (maxZ > mapSize.w)
        {
            maxZ = math.lerp(maxZ,mapSize.w,0.1f);
            minZ = maxZ - height;
            should.z = (minZ + maxZ) / 2;
        }


        transform.position = Vector3.Lerp(transform.position,_center,0.1f);
    }
    // private Camera camera;
    // private CinemachineConfiner3D cco;
    private CinemachineCamera cc;
    private CinemachinePositionComposer cpc;

    private CinemachineFollowZoom cfz;
    // private GameObject zone;
    // private BoxCollider zoneBox;
    // public Vector2 CenterPoint;
    // public Vector2 UnitLength;
    // public float WidthHeight;
    // public float WallHeight;
    // private Vector2 GetAxialRotation(Vector2 axial, float angle)
    // {
    //     var x = axial.x * Mathf.Cos(angle) + axial.y * Mathf.Sin(angle);
    //     var y = axial.y * Mathf.Cos(angle) + axial.x * Mathf.Sin(angle);
    //     return new Vector2(x, y);
    // }
    //
    // private void RotateYByPos(Vector2 r, Transform t, float angle)
    // {
    //     var tempv2 = new Vector2(t.position.x, t.position.z);
    //     var result = tempv2.RotateByPos(r, -angle);
    //     t.position = new Vector3(result.x, t.position.y, result.y);
    //     t.SetEulerAnglesY(angle * Mathf.Rad2Deg);
    // }
    // //计算的角度均为弧度值，传入纵向的（高）Fov的一半得到横向的（宽）Fov的一半
    // public float GetHorizontalFovHalf(float vhfov, float aspect)
    // {
    //     return Mathf.Atan(Mathf.Tan(vhfov) * aspect);
    // }
    //
    // //计算轴向偏移值
    // private float GetSizeOffse(float fbangel, float distance, float wh, float followy)
    // {
    //     //直角弧度值
    //     var rightangel = 90 * Mathf.Deg2Rad;
    //     //∠PAC
    //     var disangel = fbangel + rightangel;
    //     //求出正弦定理的比值
    //     var sin = distance / Mathf.Sin(disangel);
    //     //求∠APC的正弦值
    //     var angelo = (wh - followy) / sin;
    //     //三角形内角和求∠ACP
    //     var angel = rightangel * 2 - Mathf.Asin(angelo) - disangel;
    //     //计算AP利用α余弦返回AD
    //     return sin * angel * Mathf.Cos(fbangel);
    // }
    // //计算并生成透视摄像机的运动区域
    // public void GenZone()
    // {
    //     camera = Camera.main;
    //     //摄像机轴向旋转值
    //     var rotation = camera.transform.eulerAngles.x * Mathf.Deg2Rad;
    //     var sizeup = camera.transform.eulerAngles.y * Mathf.Deg2Rad;
    //
    //     //计算从地图中心到边缘的向量
    //     var toedge = WidthHeight * UnitLength * .5f;
    //
    //     //旋转后的大小值变化（添加内容）
    //     toedge= GetAxialRotation(new Vector2(toedge.x, toedge.y), sizeup);
    //
    //     //左后
    //     var lb = CenterPoint - toedge;
    //     //右前
    //     var rf = CenterPoint + toedge;
    //     //墙高
    //     var wh = WallHeight;
    //     if (!zone)
    //     {
    //         zone = GameObject.Find("CameraZone");
    //     }
    //
    //     if (!zone)
    //     {
    //         zone = new GameObject("CameraZone");
    //         zoneBox = zone.AddComponent<BoxCollider>();
    //     }
    //
    //     if (!zoneBox)
    //     {
    //         zoneBox = zone.GetComponent<BoxCollider>();
    //     }
    //
    //     if (!cc)
    //     {
    //         cc = GetComponent<CinemachineCamera>();
    //     }
    //
    //     if (!cpc)
    //     {
    //         cpc = cc.GetComponent<CinemachinePositionComposer>();
    //     }
    //
    //     if (!cco)
    //     {
    //         cco = cc.GetComponent<CinemachineConfiner3D>();
    //         cco.BoundingVolume = zoneBox;
    //     }
    //     
    //     var cvcs = cc.Lens;
    //     //摄像机跟踪目标的高度
    //     var followy = cc.Follow.position.y;
    //     //跟踪距离
    //     var distance = cpc.CameraDistance;
    //     //屏幕高对应的Fov一半（真实Fov）
    //     var hfov = cvcs.FieldOfView * .5f * Mathf.Deg2Rad;
    //     //摄像机视口宽高比
    //     var aspect = camera.aspect;
    //     var rightangle = 90 * Mathf.Deg2Rad;
    //     //屏幕宽对应的Fov一半（转化后的Fov）
    //     var whfov = GetHorizontalFovHalf(hfov, aspect);
    //
    //     //摄像机当前高度
    //     var height = Mathf.Sin(rotation) * distance + followy;
    //
    //     //计算左右偏移（对称）
    //     var lrangle = rightangle - whfov;
    //     var widthh = GetSizeOffse(lrangle, distance, wh, followy);
    //     var left = lb.x + widthh;
    //     var right = rf.x - widthh;
    //     var sizex = Mathf.Abs(left - right);
    //
    //     //计算前后偏移（带旋转值，非对称）
    //     var fangle = rotation - hfov;
    //     var front = rf.y - GetSizeOffse(fangle, distance, wh, followy);
    //
    //     var bangle = rotation + hfov;
    //     var back = lb.y - GetSizeOffse(bangle, distance, wh, followy);
    //
    //     var sizez = Mathf.Abs(front - back);
    //
    //     //设置摄像机运动范围的大小，因为在XZ平面上，盒子的高度可以为一个常量
    //     zoneBox.size = new Vector3(sizex, 5, sizez);
    //     zone.transform.position = new Vector3((left + right) * .5f, height, (front + back) * .5f);
    //     //位置值变化设置（添加内容）
    //     RotateYByPos(CenterPoint, zone.transform, sizeup);
    //
    // }
}
