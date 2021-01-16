using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PlanetUtils
{
    /*
     * Align a transform to planets surface and rotate towards a specific target on that planet surface.
     */
    public static Vector3 AlignToPlanetSurface(Transform transform, Transform planet, Vector3 target)
    {
        // Rotate towards the target on the y axis whilst maintaining a standing rotation on the surface of the planet
        var gravityUp = (transform.position - planet.position).normalized;
        var forward = Vector3.ProjectOnPlane(target - transform.position, gravityUp);
        if (forward != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(forward, gravityUp);

        // Snap back to planets surface
        var planetRadius = planet.GetComponent<Planet>().radius;
        return gravityUp * (planetRadius + 1);
    }

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
