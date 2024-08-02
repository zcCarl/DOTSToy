using UnityEngine;

public static class Vector2Util
{
    public static Vector2 RotateByPos(this Vector2 pos, Vector2 rPos, float angle)
    {
        var x = (pos.x - rPos.x) * Mathf.Cos(angle) - (pos.y - rPos.y) * Mathf.Sin(angle) + rPos.x;
        var y = (pos.y - rPos.y) * Mathf.Cos(angle) + (pos.x - rPos.x) * Mathf.Sin(angle) + rPos.y;
        return new Vector2(x, y);
    }
    public static void SetEulerAnglesY(this Transform transform, float angle)
    {
        var localEulerAngles = transform.localEulerAngles;
        localEulerAngles = new Vector3(localEulerAngles.x, angle, localEulerAngles.z);
        transform.localEulerAngles = localEulerAngles;
    }
}