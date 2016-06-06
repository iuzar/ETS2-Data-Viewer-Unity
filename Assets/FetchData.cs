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
    public Text timeText;
    public Text gameText;    
    public bool connected = false;

    public object stuff;
    public string jsonText = null;
    public bool cantConnectToTelemetryServer;

    public bool monochrome = false;

    public Data data = new Data();

    private bool isFetching = false;
    private bool isUpdating = false;

    [System.Serializable]
    public class Settings
    {
        public bool NOK_Enabled = false;
        public string address = "localhost";
        public int port = 25555;
        public InputField portText;
        public Toggle NOK_EnabledToggle;
        public InputField addressText;
        public Text addressReadOnlyText;

        public void SaveSettings()
        {
            NOK_Enabled = NOK_EnabledToggle.isOn;
            address = addressText.text;

            PlayerPrefs.SetString("NOK_Enabled", NOK_Enabled.ToString());
            PlayerPrefs.SetString("address", address);
            PlayerPrefs.SetInt("port", port);
            PlayerPrefs.Save();

            UpdateTexts();
        }

        public void UpdateTexts()
        {
            if (NOK_EnabledToggle != null)
            {
                NOK_EnabledToggle.isOn = NOK_Enabled;
            }
            if (addressText != null)
            {
                addressText.text = address;
                addressReadOnlyText.text = address;
            }
            if (portText != null)
            {
                portText.text = port.ToString();
            }
        }

        public void LoadSettings()
        {
            if (PlayerPrefs.HasKey("NOK_Enabled"))
            {
                string nok_enabledString = PlayerPrefs.GetString("NOK_Enabled");
                Debug.Log("NOK Enabled string setting = " + nok_enabledString);
                NOK_Enabled = bool.Parse(nok_enabledString); 
            }
            if (PlayerPrefs.HasKey("address"))
            {
                address = PlayerPrefs.GetString("address");                 
            }
            if (PlayerPrefs.HasKey("port"))
            {
                port = PlayerPrefs.GetInt("port");                 
            }

            UpdateTexts();
        }
    }

    public Settings settings = new Settings();

    // Use this for initialization
    void Start ()
    {
        LoadSettings();
        settings.UpdateTexts();
        UpdateToggle();
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

    public void UpdateToggle()
    {
        if (settings.NOK_EnabledToggle != null)
        {
            settings.NOK_EnabledToggle.isOn = settings.NOK_Enabled; 
        }
    }

    public void ToggleNOK ()
    {
        if (settings.NOK_EnabledToggle != null)
        {
            settings.NOK_Enabled = settings.NOK_EnabledToggle.isOn; 
        }
    }

    public void Connect()
    {
        settings.address = settings.addressText.text;
        settings.addressReadOnlyText.text = settings.address;
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
            string address = "http://" + settings.address + ":" + settings.port + "/api/ets2/telemetry";
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

        string gameName = data.Game.GameName;
        string gameVersion = data.Game.Version;

        DateTime time = data.Game.Time;

        string timeString = time.ToString("ddd") +" " + time.Hour.ToString("0#") + ":" + time.Minute.ToString("0#");

        string deadlineHourString = (deadline.Hour > 0) ? (deadline.Hour + " hours ") : "";

        // ETA realtime:
        float etaRT = (eta.Minute * 1) / timeScale;
        string etaRTString;
        if ((float) eta.Hour / timeScale < 1)
        {
            float minutesToAdd = (float)(eta.Hour * 60) / timeScale;
            etaRT += minutesToAdd;
            etaRTString = "(<b>Real Time: " + (int)etaRT + " minutes</b>)";
        }
        else
        {
            etaRTString = "(Real Time: " + (int)((float)eta.Hour / timeScale) + " hours " + (int)etaRT + " minutes</b>)";
        }

        int fuelLeftPercent = (int)((fuelLeft / maxFuel) * 100);
        string fuelPercentString = null;
        if (fuelLeftPercent <= 10)
        {
            fuelPercentString = "<color=red>" + fuelLeftPercent + "%</color>";
        }

        string jobPayString = null;
        if (settings.NOK_Enabled)
        {
            jobPay *= 8.6f;
            jobPayString = jobPay.ToString("C0", CultureInfo.CreateSpecificCulture("nb-NO"));
        }
        else
        {
            jobPayString = jobPay.ToString("C0", CultureInfo.CreateSpecificCulture("fr-FR"));
        }

        string SourceRichString = "<b>Source: </b>" + source;
        string DestinationRichString = "<b>Destination: </b>" + destination;
        string DeadLineRichString =
            "<b>Deadline in: </b>" +
            deadlineHourString + deadline.Minute + " minutes " + "(<b>" + jobPayString + "</b>)";
        string ETARichString = "<b>ETA: </b>" + eta.Hour + " hours " + eta.Minute + " minutes " + etaRTString;

        string DistanceRichString = "<b>Distance: </b>" + distance + " kilometers ";
        string CargoRichString = "<b>Cargo: </b>" + cargo;
        string FuelRichString = "<b>Fuel: </b>" + (int) fuelLeft + " / " + (int) maxFuel+ " (<b>" + fuelLeftPercent + "%</b>)";

        string TimeRichString = "<b>" + timeString + "</b>";

        string GameRichString = "<b>" + gameName + " v" + gameVersion + "</b>";

        if (sourceText != null)
        {
            sourceText.text = SourceRichString; 
        }
        if (destinationText != null)
        {
            destinationText.text = DestinationRichString; 
        }
        if (deadlineText != null)
        {
            deadlineText.text = DeadLineRichString; 
        }
        if (etaText != null)
        {
            etaText.text = ETARichString; 
        }
        if (distanceText != null)
        {
            distanceText.text = DistanceRichString; 
        }
        if (cargoText != null)
        {
            cargoText.text = CargoRichString; 
        }
        if (fuelText != null)
        {
            fuelText.text = FuelRichString; 
        }

        if (speed_limitText != null)
        {
            speed_limitText.text = speedLimit.ToString(); 
        }

        if (timeText != null)
        {
            timeText.text = TimeRichString;
        }

        if (gameText != null)
        {
            gameText.text = GameRichString;
        }

        isUpdating = false;
    }

    public void SaveSettings()
    {
        settings.SaveSettings();
    }

    public void LoadSettings()
    {
        settings.LoadSettings();
    }
}
