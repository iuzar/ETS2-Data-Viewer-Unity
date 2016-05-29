using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using System.Net;
using System.Text;
using System.Globalization;

public class FetchData : MonoBehaviour {

    public Text sourceText;
    public Text destinationText;
    public Text deadlineText;
    public Text etaText;
    public Text distanceText;
    public Text speed_limitText;
    public Text next_restText;
    public Text cargoText;
    public Text fuelText;
    public Text addressText;
    public Toggle NOK_EnabledToggle;

    public bool NOK_Enabled = false;
    public bool connected = false;

    public object stuff;
    public string jsonText = null;
    public bool cantConnectToTelemetryServer;
    public string ip;

    public Data data = new Data();

    private bool isFetching = false;
    private bool isUpdating = false;

    // Use this for initialization
    void Start () {
        ToggleNOK();
        Application.targetFrameRate = 10;
    }

    void FixedUpdate()
    {
        RepeatedUpdate();
    }
	
	// Update is called once per frame
	void RepeatedUpdate () {
	    if (connected && !cantConnectToTelemetryServer)
        {
            if (!isFetching)
            {
                Fetch(); 
            }
            if (!isUpdating)
            {
                UpdateData(); 
            }
        }
	}

    public void ToggleNOK ()
    {
        NOK_Enabled = NOK_EnabledToggle.isOn;
    }

    public void Connect()
    {
        ip = addressText.text;
        Fetch();
        if (!cantConnectToTelemetryServer)
        {
            connected = true;
            UpdateData(); 

            //InvokeRepeating("RepeatedUpdate", 0.1f, 0.1f);
        }
    }

    public void Fetch()
    {
        isFetching = true;
        cantConnectToTelemetryServer = false;
        WebClient wc = new WebClient();
        wc.Encoding = Encoding.UTF8;
        try
        {
            string address = "http://" + ip + ":25555/api/ets2/telemetry";
            //jsonText = wc.DownloadString("http://localhost:25555/api/ets2/telemetry");
            jsonText = wc.DownloadString(address);
        }
        catch (Exception ex)
        {
            cantConnectToTelemetryServer = true;

            if (IsInvoking("RepeatedUpdate"))
            {
                CancelInvoke("RepeatedUpdate"); 
            }

            sourceText.text = "CANT CONNECT TO TELEMETRY SERVER";
        }

        if (!cantConnectToTelemetryServer)
        {
            data = Newtonsoft.Json.JsonConvert.DeserializeObject<Data>(jsonText) as Data;
            
            string test;

            Debug.Log(data.Game.TimeScale);
        }
        isFetching = false;
    }

    public void UpdateData()
    {
        isUpdating = true;
        DateTime dateTime = data.Game.Time;
        string source = data.Job.SourceCity + "(" + data.Job.SourceCompany + ")";
        string destination = data.Job.DestinationCity + "(" + data.Job.DestinationCompany + ")";
        DateTime deadline = data.Job.DeadlineTime;
        float jobPay = data.Job.Income;
        DateTime eta = data.Navigation.EstimatedTime;
        int distance = data.Navigation.EstimatedDistance / 1000;
        int speedLimit = data.Navigation.SpeedLimit;
        DateTime mustRestIn = data.Game.NextRest;
        string cargo = data.Trailer.Cargo;
        float fuelLeft = data.Truck.Fuel;
        float maxFuel = data.Truck.FuelCapacity;
        float timeScale = data.Game.TimeScale;
        int velocity = (int) data.Truck.Speed;

        string deadlineHourString = (deadline.Hour > 0) ? (deadline.Hour + " hours ") : "";

        // ETA realtime:
        float etaRT = (eta.Minute * 1) / timeScale;
        string etaRTString;
        if ((float) eta.Hour / timeScale < 1)
        {
            float minutesToAdd = (float)(eta.Hour * 60) / timeScale;
            etaRT += minutesToAdd;
            etaRTString = "(</color><b><color=yellow>Real Time: " + (int)etaRT + " minutes</color></b><color=green>)</color>";
        }
        else
        {
            etaRTString = "(</color><b><color=yellow>Real Time: " + (int)((float)eta.Hour / timeScale) + " hours " + (int)etaRT + " minutes</color></b><color=green>)</color>";
        }

        int fuelLeftPercent = (int)((fuelLeft / maxFuel) * 100);
        string fuelPercentString = null;
        if (fuelLeftPercent <= 10)
        {
            fuelPercentString = "<color=red>" + fuelLeftPercent + "%</color>";
        }

        string jobPayString = null;
        if (NOK_Enabled)
        {
            jobPay *= 8.6f;
            jobPayString = jobPay.ToString("C0", CultureInfo.CreateSpecificCulture("nb-NO"));
        }
        else
        {
            jobPayString = jobPay.ToString("C0", CultureInfo.CreateSpecificCulture("fr-FR"));
        }

        string SourceRichString = "<b><color=yellow>Source: </color></b><color=green>" + source + "</color>";
        string DestinationRichString = "<b><color=yellow>Destination: </color></b><color=green>" + destination + "</color>";
        string DeadLineRichString =
            "<b><color=yellow>Deadline in: </color></b><color=green>" +
            deadlineHourString + deadline.Minute + " minutes " + "(</color><b><color=yellow>" + jobPayString + "</color></b><color=green>)</color>";
        string ETARichString = "<b><color=yellow>ETA: </color></b><color=green>" + eta.Hour + " hours " + eta.Minute + " minutes " + etaRTString;

        string DistanceRichString = "<b><color=yellow>Distance: </color></b><color=green>" + distance + " kilometers </color>";
        string CargoRichString = "<b><color=yellow>Cargo: </color></b><color=green>" + cargo + "</color>";
        string FuelRichString = "<b><color=yellow>Fuel: </color></b><color=green>" + (int) fuelLeft + " / " + (int) maxFuel+ " (</color><b><color=yellow>" + fuelLeftPercent + "%</color></b><color=green>)</color>";

        sourceText.text = SourceRichString;
        destinationText.text = DestinationRichString;
        deadlineText.text = DeadLineRichString;
        etaText.text = ETARichString;
        distanceText.text = DistanceRichString;
        cargoText.text = CargoRichString;
        fuelText.text = FuelRichString;

        speed_limitText.text = speedLimit.ToString();

        isUpdating = false;
    }
}
