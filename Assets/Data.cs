using UnityEngine;
using System.Collections;
using Newtonsoft.Json;
using System;

public class Data  {
    private Game game;
    private Truck truck;
    private Trailer trailer;
    private Job job;
    private Navigation navigation;

    [JsonProperty("game")]
    public Game Game
    {
        get { return game; }
        set { game = value; }
    }

    [JsonProperty("truck")]
    public Truck Truck
    {
        get
        {
            return truck;
        }

        set
        {
            truck = value;
        }
    }

    [JsonProperty("trailer")]
    public Trailer Trailer
    {
        get
        {
            return trailer;
        }

        set
        {
            trailer = value;
        }
    }

    [JsonProperty("job")]
    public Job Job
    {
        get
        {
            return job;
        }

        set
        {
            job = value;
        }
    }

    [JsonProperty("navigation")]
    public Navigation Navigation
    {
        get
        {
            return navigation;
        }

        set
        {
            navigation = value;
        }
    }
}

public class Game
{
    private bool connected;
    private bool paused;
    private DateTime time;
    private float timeScale;
    private DateTime nextRest;
    private string version;
    private string telemetryPluginVersion;

    [JsonProperty("paused")]
    public bool Paused
    {
        get
        {
            return paused;
        }

        set
        {
            paused = value;
        }
    }

    [JsonProperty("timeScale")]
    public float TimeScale
    {
        get { return timeScale; }
        set { timeScale = value; }
    }

    [JsonProperty("time")]
    public DateTime Time
    {
        get
        {
            return time;
        }

        set
        {
            time = value;
        }
    }

    [JsonProperty("nextRestStopTime")]
    public DateTime NextRest
    {
        get
        {
            return nextRest;
        }

        set
        {
            nextRest = value;
        }
    }

    [JsonProperty("version")]
    public string Version
    {
        get
        {
            return version;
        }

        set
        {
            version = value;
        }
    }

    [JsonProperty("telemetryPluginVersion")]
    public string TelemetryPluginVersion
    {
        get
        {
            return telemetryPluginVersion;
        }

        set
        {
            telemetryPluginVersion = value;
        }
    }
}
public class Truck
{
    private float speed;
    private float fuel;
    private float fuelCapacity;

    [JsonProperty("speed")]
    public float Speed
    {
        get
        {
            return speed;
        }

        set
        {
            speed = value;
        }
    }

    [JsonProperty("fuel")]
    public float Fuel
    {
        get
        {
            return fuel;
        }

        set
        {
            fuel = value;
        }
    }

    [JsonProperty("fuelCapacity")]
    public float FuelCapacity
    {
        get
        {
            return fuelCapacity;
        }

        set
        {
            fuelCapacity = value;
        }
    }
}
public class Trailer
{
    private string cargo;

    [JsonProperty("name")]
    public string Cargo
    {
        get
        {
            return cargo;
        }

        set
        {
            cargo = value;
        }
    }
}
public class Job
{
    private int income;
    private DateTime deadlineTime;
    private DateTime remainingTime;
    private string sourceCity;
    private string sourceCompany;
    private string destinationCity;
    private string destinationCompany;

    [JsonProperty("income")]
    public int Income
    {
        get
        {
            return income;
        }

        set
        {
            income = value;
        }
    }

    [JsonProperty("deadlineTime")]
    public DateTime DeadlineTime
    {
        get
        {
            return deadlineTime;
        }

        set
        {
            deadlineTime = value;
        }
    }

    [JsonProperty("remainingTime")]
    public DateTime RemainingTime
    {
        get
        {
            return remainingTime;
        }

        set
        {
            remainingTime = value;
        }
    }

    [JsonProperty("sourceCity")]
    public string SourceCity
    {
        get
        {
            return sourceCity;
        }

        set
        {
            sourceCity = value;
        }
    }

    [JsonProperty("sourceCompany")]
    public string SourceCompany
    {
        get
        {
            return sourceCompany;
        }

        set
        {
            sourceCompany = value;
        }
    }

    [JsonProperty("destinationCity")]
    public string DestinationCity
    {
        get
        {
            return destinationCity;
        }

        set
        {
            destinationCity = value;
        }
    }

    [JsonProperty("destinationCompany")]
    public string DestinationCompany
    {
        get
        {
            return destinationCompany;
        }

        set
        {
            destinationCompany = value;
        }
    }
}
public class Navigation
{
    private DateTime estimatedTime;
    private int estimatedDistance;
    private int speedLimit;

    [JsonProperty("estimatedTime")]
    public DateTime EstimatedTime
    {
        get
        {
            return estimatedTime;
        }

        set
        {
            estimatedTime = value;
        }
    }

    [JsonProperty("estimatedDistance")]
    public int EstimatedDistance
    {
        get
        {
            return estimatedDistance;
        }

        set
        {
            estimatedDistance = value;
        }
    }

    [JsonProperty("speedLimit")]
    public int SpeedLimit
    {
        get
        {
            return speedLimit;
        }

        set
        {
            speedLimit = value;
        }
    }
}
