using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///--------------------------------
///   Author: Victor Alvarez, Ph.D.
///   University of Oviedo, Spain
///--------------------------------

public class PointOfInterest : MonoBehaviour
{
    public double latitude = 43.354668f;
    public double longitude = -5.851981f;
    // PoIs EII
    // Pol0-> 43.354668 / -5.851981
    // Pol1-> 43.354267 / -5.851508
    public string videoURL = "";
    // Video streaming using Dropbox > replace www with dl
    // T Rex https://dl.dropbox.com/s/ktn4wi61far1azp/T-Rex.mp4
    // Velociraptors https://dl.dropbox.com/s/dcepr9b7yfcdpkm/Velociraptors.mp4

    public PointOfInterest nextPoI;

    public double getLatitude()
    { 
        return latitude;
    }

    public double getLongitude()
    { 
        return longitude;
    }

    public string getVideoURL()
    {
        return videoURL;
    }
}
