using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements.Experimental;

public class Sun : MonoBehaviour
{
    public InterfaceControls interfaceControls;

    public float Distance = 50f;
    public float Speed = 0.5f;

    [Header("The sun settings")]
    [SerializeField] private float _maxIntensity = 1.5f;
    [SerializeField] private float _minIntensity = 0.5f;
    private Light _light;

    private int _time;
    private int _date;
    private int _month;
    private float _latitude;
    private float _longitude;

    #region UnityFunctions
    private void Start()
    {
        _light = this.GetComponent<Light>();
    }
    private void Update()
    {
        GetUserInput();
        SunControl(2023, _time, _date, _month, _latitude, _longitude);
    }
    #endregion


    #region PrivateFunctions
    /// <summary>
    /// - First we get the nescessary info for the calculation: Time, Date, Month, Latitude, Longitude...
    /// - Then we apply calculation provided by NOAA. ( https://gml.noaa.gov/grad/solcalc/solareqns.PDF )
    /// Credit : https://gist.github.com/paulhayes/54a7aa2ee3cccad4d37bb65977eb19e2.
    /// </summary>
    private void GetUserInput()
    {
        _time = interfaceControls.timeValue;
        _date = interfaceControls.dateValue;
        _month = interfaceControls.monthValue;
        _latitude = interfaceControls.latitudeValue;
        _longitude = interfaceControls.longitudeValue;
    }
    private void SunControl(int y, int t,int d, int m, float la, float lo)
    {
       
        //Control distance (intensity)
        //_light.intensity = CalculateSunIntensity(2023,m,d,_minIntensity, _maxIntensity);

        // Orient the directional light (representing the Sun) in the calculated direction
        transform.position =
            Vector3.Lerp(transform.position, CalculatePosition(y, m, d, t, la, lo), Speed * Time.deltaTime);
    }
    //private float CalculateSunIntensity(int y = 2023, int m = 1, int d = 1, float min = 0.5f, float max = 1.5f)
    //{
    //    /*
    //     *  Refracture later
    //     */
    //    float intensity = min;
    //    int daysInMonth = DateTime.DaysInMonth(y, m);

    //    if (m >= 1 && m <= 3 || m >= 7 && m <= 9)
    //    {
            
    //        // Interpolate from 0.5 to 1.5 over the range of month 1 to 3 can still apply for month 7->9
    //        float t = Mathf.InverseLerp(1, 3, m);
    //        float targetIntensity = Mathf.Lerp(min, max, t);
    //        float adjustIntensity = targetIntensity + (0.5f * ((float)(d - 1) / (float)daysInMonth));
    //        if (m == 3 || m == 9)
    //        {
    //            adjustIntensity = max;
    //        }
    //        //intensity = Mathf.MoveTowards(intensity, adjustIntensity, Speed * Time.deltaTime);
    //        intensity = adjustIntensity;
    //    }
    //    else if (m >= 4 && m <= 6 || m >= 10 && m <= 12)
    //    {
            
    //        // Interpolate from 1.5 to 0.5 over the range of month 4 to 6 can still apply for month 10->12
    //        float t = Mathf.InverseLerp(4, 6, m);
    //        float targetIntensity = Mathf.Lerp(max, min, t);
    //        float adjustIntensity = targetIntensity - (0.5f * ((float)(d - 1) / (float)daysInMonth));
    //        if (m == 6 || m == 12)
    //        {
    //            adjustIntensity = min;
    //        }
    //        //intensity = Mathf.MoveTowards(intensity, adjustIntensity, Speed * 50 * Time.deltaTime);
    //        intensity = adjustIntensity;
    //    }
    //    return intensity;
    //}
    private Vector3 CalculatePosition(int y, int m, int d, int h, float la, float lo)
    {
        float jd = GetJulianDay(2023, m, d, h);
        // Calculate the Julian Century (JC) based on the Julian Day
        float jc = GetJulianCentury(jd);
        // Calculate the Sun's mean longitude
        float L = GetSunMeanLongitude(jc);
        // Calculate the Sun's mean anomaly
        float M = GetSunMeanAnomaly(jc);
        // Calculate the Sun's ecliptic longitude
        float lambda = GetSunEclipticLongitude(L, M);
        // Calculate the Sun's right ascension
        float alpha = GetSunRightAscension(lambda);
        // Calculate the Sun's declination
        float delta = GetSunDeclination(lambda);
        // Calculate the Sun's local hour angle 
        float H = GetCurrentHourAngle(h, lo, alpha);

        // Convert the latitude and declination to radians
        float latRad = Mathf.Deg2Rad * la;
        float deltaRad = Mathf.Deg2Rad * delta;

        // Calculate the altitude (angle above the horizon) of the Sun
        float altitude = 
            Mathf.Asin(Mathf.Sin(latRad) * Mathf.Sin(deltaRad) + Mathf.Cos(latRad) * Mathf.Cos(deltaRad) * Mathf.Cos(Mathf.Deg2Rad * H));

        // Set the position of the Sun GameObject based on the altitude
        Vector3 sunPosition = new Vector3(Distance * Mathf.Cos((float)altitude), Distance * Mathf.Sin((float)altitude), 0f);

        return sunPosition;
    }

    #region HelperFunctions

    //Calculate JulianDay
    private float GetJulianDay(int y, int m, int d, float hr)
    {
        if (m <= 2)
        {
            y -= 1;
            m += 12;
        }
        float A = y / 100;
        float B = 2 - A + A / 4;
        float C = (365.25f * (y + 4716));
        float D = (30.6f * (m + 1));
        float JD = B + C + D + d - 1542.5f + hr / 24.0f;
        return JD;
    }

    //Calculate JulianCentury
    private float GetJulianCentury(float jd)
    {
        float JC = (jd - 2451545) / 36525.0f;
        return JC;
    }

    //Calculate the Sun's mean longitud
    private float GetSunMeanLongitude(float jc)
    {
        float L = 280.5f + jc * (36000.8f + jc * 0.0003f);
        while (L > 360.0f)
            L -= 360.0f;
        while (L < 0.0f)
            L += 360.0f;
        return L;
    }

    //Calculate the Sun's mean anomaly
    private float GetSunMeanAnomaly(float jc)
    {
        float M = 357.53f + jc * (35999.05f - 0.00015f * jc);
        return M;
    }

    //Calculate the Sun's ecliptic longitude
    private float GetSunEclipticLongitude(float L, float M)
    {
        float lambda = L + 1.92f * Mathf.Sin((Mathf.Deg2Rad * M)) + 0.02f * Mathf.Sin((Mathf.Deg2Rad * 2 * M));
        return lambda;
    }

    //Calculate the Sun's right ascension
    private float GetSunRightAscension(float lambda)
    {
        float alpha = Mathf.Rad2Deg * Mathf.Atan2(Mathf.Sin(Mathf.Deg2Rad * lambda) * Mathf.Cos(Mathf.Deg2Rad * 23.44f), Mathf.Cos(Mathf.Deg2Rad * lambda));
        return alpha;
    }

    //Calculate the Sun's declination
    private float GetSunDeclination(float lambda)
    {
        float delta = Mathf.Rad2Deg * Mathf.Asin(Mathf.Sin(Mathf.Deg2Rad * lambda) * Mathf.Sin(Mathf.Deg2Rad * 23.44f));
        return delta;
    }

    //Calculate the current hour angle
    private float GetCurrentHourAngle(int hr, float longitude, float alpha)
    {
        float UT = hr;
        float LT = UT + longitude / 15.0f;
        float H = 15.0f * (LT - alpha / 15.0f);
        return H;
    }
    #endregion


    #endregion
}
