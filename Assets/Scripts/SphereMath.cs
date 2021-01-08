using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereMath
{
    public static Vector3 SphericalToCartesian(Vector3 sphericalCoord)
    {
        return SphericalToCartesian(sphericalCoord.x, sphericalCoord.y, sphericalCoord.z);
    }

    /*
     * Returns new Vector3(x, y, z)
     */
    public static Vector3 SphericalToCartesian(float radius, float polar, float azimuthal)
    {
        float a = radius * Mathf.Cos(azimuthal);

        Vector3 result = new Vector3();
        result.x = a * Mathf.Cos(polar);
        result.y = radius * Mathf.Sin(azimuthal);
        result.z = a * Mathf.Sin(polar);

        return result;
    }

    /*
     * Returns new Vector3(radius, polar, azimuthal)
     */
    public static Vector3 CartesianToSpherical(Vector3 cartCoords)
    {
        float radius, polar, azimuthal;

        if (cartCoords.x == 0)
            cartCoords.x = Mathf.Epsilon;

        radius = Mathf.Sqrt((cartCoords.x * cartCoords.x) + (cartCoords.y * cartCoords.y) + (cartCoords.z * cartCoords.z));
        polar = Mathf.Atan(cartCoords.z / cartCoords.x);

        if (cartCoords.x < 0)
            polar += Mathf.PI;
        azimuthal = Mathf.Asin(cartCoords.y / radius);

        Vector3 result = new Vector3(radius, polar, azimuthal);
        return result;
    }
}