using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

///--------------------------------
///   Author: Victor Alvarez, Ph.D.
///   University of Oviedo, Spain
///--------------------------------

#region GPS
/**
 * Class describing the GPS.
 *
 * MAIN CLASS.
 */
public class GPS : MonoBehaviour
{

    #region parameters

    public static GPS Instance { set; get; }

    private float timeElapsed = 0f;
    private bool gpsEnabled = false;
    public float refreshTime = 10f;

    private double latitude, longitude;
    private Location goalCoordinates;
    private double goalLatitude, goalLongitude;
    public GameObject pointOfInterest;
    private PointOfInterest goalPoI;
    public int distanceToGoal = 25;
    private bool goalReached = false;
    public Text gpsStatusText, latitudeText, longitudeText, goalText;
    public GameObject screen;
    private VideoPlayer videoPlayer;

    #endregion

    #region unityMainMethods

    /**
     * This method first launch the permissions to obtain the location.
     * 
     * This method works as:
     * Is the point of interest present?
     *     no:
     *          Show the error on screen.
     *     yes:
     *         We get the component of type 'PointOfInterest' in the point of interest.
     *         We get the longitude and latitude from the point of interest.
     *
     * Are the screen and goal point of interest present?
     *      yes:
     *          We get the VideoPlayer from the screen and set its url.
     * 
     * We save an instance of this.
     * We set this to not get destroid.
     * We start the location corrutine. 
     */
    private void Start()
    {
        if (pointOfInterest == null)
            // missing PoI, nowhere to go
            goalText.text = "Missing Point of Interest";
        else
        {
            goalPoI = pointOfInterest.GetComponent<PointOfInterest>();
            goalLatitude = goalPoI.getLatitude();
            goalLongitude = goalPoI.getLongitude();
        }

        if (screen != null && goalPoI != null)
        {
            videoPlayer = screen.GetComponent<VideoPlayer>();
            videoPlayer.url = goalPoI.getVideoURL();
        }   

        Instance = this;
        // DontDestroyOnLoad(gameObject); 
        StartCoroutine(StartLocationService());
    }

    /**
     * Checks if the 'refreshTime' has already pass, if so updates the GPS value.
     * 
     * Updates every tick in game.
     */
    private void Update()
    {

        timeElapsed += Time.deltaTime;

        if (gpsEnabled && timeElapsed >= refreshTime)
        {
            updateGPS();
            timeElapsed = 0f;
        }
    }

    #endregion

    #region addedPrivateMethods

    /**
     * It waits up to 20 seconds to obtain the location. 
     */
    private IEnumerator StartLocationService()
    {
        if (!Input.location.isEnabledByUser)
        {
            if (gpsStatusText.IsActive())
                gpsStatusText.text = "GPS is not enabled";
            yield break;
        }

        Input.location.Start(10, 0.1f);
        int maxWait = 20;

        while (Input.location.status == LocationServiceStatus.Initializing &&
            maxWait > 0)
        {
            yield return new WaitForSeconds(1);
            maxWait--;
        }

        if (maxWait <= 0)
        {
            if (gpsStatusText.IsActive())
                gpsStatusText.text = "GPS initialisation time out";
            yield break;
        }

        if (Input.location.status == LocationServiceStatus.Failed)
        {
            if (gpsStatusText.IsActive())
                gpsStatusText.text = "Unable to determine device location";
            yield break;
        }

        // All good, GPS is enabled!
        gpsEnabled = true;
        goalCoordinates = new Location(goalLatitude, goalLongitude);

        if (gpsStatusText.IsActive())
            gpsStatusText.text = "GPS " + Input.location.status.ToString();

        yield break;
    }

    /**
     * Updates the current values of latitude and longitude with the user's GPS one.
     */
    private void updateGPS()
    {
        latitude = Input.location.lastData.latitude;
        longitude = Input.location.lastData.longitude;

        if (latitudeText.IsActive())
            latitudeText.text = latitude.ToString("n6");

        if (longitudeText.IsActive())
            longitudeText.text = longitude.ToString("n6");

          checkGoal();
    }

    /**
     * Checks whether we got to the goal or not, if so shows the screen and updates the point of interest.
     */
    private void checkGoal()
    {
        var currentCoordinates = new Location(GPS.Instance.latitude, GPS.Instance.longitude);
        var distance = DistanceBetweenCoordinates.DistanceTo(goalCoordinates, currentCoordinates);
        if (!goalReached)
        {
            if (distance <= distanceToGoal)
                GoalReached();
            else if (goalText.IsActive())
                goalText.text = "Distance to goal: " + distance.ToString("n0") + " metres";
        }    
        else if (goalReached && distance > distanceToGoal)
        {
            goalReached = false;
            videoPlayer.Stop();
            screen.SetActive(false);
            // next point of interest
            goalPoI = goalPoI.nextPoI;
            goalLatitude = goalPoI.getLatitude();
            goalLongitude = goalPoI.getLongitude();
            goalCoordinates = new Location(goalLatitude, goalLongitude);
            videoPlayer.url = goalPoI.getVideoURL();
        }

        
    }

    /**
     * Plays the video as the goal has been reached.
     */
    private void GoalReached()
    {
        if (goalText.IsActive())
            goalText.text = "Goal Reached !!";

        screen.SetActive(true);
        videoPlayer.Play();

        goalReached = true;
    }

    #endregion
}

#endregion 

#region Location

/**
 * Class containing the data for a location (latitude, longitude).
 */
public class Location : IComparable
{
    
    #region parameters

    public double latitude { get; set; }
    public double longitude { get; set; }

    public double distance { get; set; }
    
    #endregion

    #region basicMethods

    /**
    * Basic constructor for the location.
    */
    public Location(double latitude, double longitude)
    {
        this.latitude = latitude;
        this.longitude = longitude;
    }

    /**
     * Compares the location to another one given.
     */
    public int CompareTo(object location)
    {
        return Convert.ToInt32(((Location)location).distance - this.distance);
    }

    #endregion
}

#endregion

#region DistanceBetweenCoordinates
/**
 * Class containing the data of the distance between two given locations.
 */
public static class DistanceBetweenCoordinates
{

    #region constructors

    /**
     * Computes the distance between two given Location objects.
     */
    public static double DistanceTo(this Location baseCoordinates, Location targetCoordinates)
    {
        return DistanceTo(baseCoordinates, targetCoordinates, UnitOfLength.Kilometre);
    }

    /**
     * Computes the distance between two given Location objects in regard to a specified unitOfLength.
     */
    public static double DistanceTo(this Location baseCoordinates, Location targetCoordinates, UnitOfLength unitOfLength)
    {
        var baseRad = Math.PI * baseCoordinates.latitude / 180;
        var targetRad = Math.PI * targetCoordinates.latitude / 180;
        var theta = baseCoordinates.longitude - targetCoordinates.longitude;
        var thetaRad = Math.PI * theta / 180;

        double dist =
            Math.Sin(baseRad) * Math.Sin(targetRad) + Math.Cos(baseRad) *
            Math.Cos(targetRad) * Math.Cos(thetaRad);
        dist = Math.Acos(dist);

        dist = dist * 180 / Math.PI;
        dist = dist * 60 * 1.1515;

        return unitOfLength.ConvertFromMiles(dist) * 1000; //To meters
    }

    #endregion

}

#endregion

#region UnitOfLength

/**
 * Class specifying a unit of length (Km, m...)
 */
public class UnitOfLength
{
    #region CONSTANTS

    public static UnitOfLength Kilometre = new UnitOfLength(1.609344);
    public static UnitOfLength NauticalMile = new UnitOfLength(0.8684);
    public static UnitOfLength Mile = new UnitOfLength(1);

    #endregion

    #region parameters

    private readonly double _fromMilesFactor;

    #endregion

    #region contructor

    /**
     * Basic contructor for the unit of length given a factor from miles.
     */
    private UnitOfLength(double fromMilesFactor)
    {
        _fromMilesFactor = fromMilesFactor;
    }

    #endregion

    #region methods

    /**
     * Converts a given value in Miles to the unit of length of this specific object.
     */
    public double ConvertFromMiles(double input)
    {
        return input * _fromMilesFactor;
    }

    #endregion
}

#endregion

