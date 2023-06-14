using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class InterfaceControls : MonoBehaviour
{
    [Header("Scrtips")]
    public Sun Sun;

    [Header("CanvasGroup")]
    public CanvasGroup NavigationInput;
    public CanvasGroup Crosshair;
    [Space]

    [Header("KeyCodes")]
    [SerializeField] private KeyCode _cordinate;
    [Space]

    [Header("Sun Cordinates infor")]
    [SerializeField] private TMP_Dropdown _date;
    [SerializeField] private TMP_Dropdown _month;
    [SerializeField] private TMP_Dropdown _time;
    [SerializeField] private Slider _longitude;
    [SerializeField] private Slider _latitude;

    //Values From User
    public int dateValue { get; private set; }
    public int monthValue { get; private set; }
    public int timeValue { get; private set; }
    public float longitudeValue { get; private set; }
    public float latitudeValue { get; private set; }

    //Check
    private bool _hasKeyPress = false;
    private bool _toggleNav = false;


    #region UnityFunctions
    private void Awake()
    {
        Sun.enabled = false;
    }
    void Update()
    {
        if (Input.GetKeyDown(_cordinate))
        {
            _hasKeyPress = true;
            _toggleNav = !_toggleNav;
        }
        //check if any key has been pressed
        if (_hasKeyPress == false) return;
        OpenCor();
    }
    #endregion

    #region PublicFunctions
    public void OnCalculateButtonClicked()
    {
        GetValueFromUser();
        Sun.enabled = true;
    }

    public void OnCloseButtonClicked()
    {
        _toggleNav = false;
        CloseInterface(NavigationInput);
        ShowInterface(Crosshair);
    }
    #endregion

    #region PrivateFunctions

    //Interface
    private void OpenCor()
    {
        if (_toggleNav)
        {
            ShowInterface(NavigationInput);
            HideCrosshair(_toggleNav);
            _hasKeyPress = false;
        }
        else
        {
            CloseInterface(NavigationInput);
            HideCrosshair(_toggleNav);
            _hasKeyPress = false;
        }
    }
    private void HideCrosshair(bool isOpen)
    {
        if (isOpen)
        {
            CloseInterface(Crosshair);
        }
        else
        {
            ShowInterface(Crosshair);
        }
    }
    private void ShowInterface(CanvasGroup cG)
    {
        cG.alpha = 1;
        cG.blocksRaycasts = true;
    }
    private void CloseInterface(CanvasGroup cG)
    {
        cG.alpha = 0;
        cG.blocksRaycasts = false;
    }

    //Calculate Position
    private void GetValueFromUser()
    {
        dateValue = _date.value + 1;
        monthValue = _month.value + 1;
        timeValue = _time.value;
        longitudeValue = _longitude.value;
        latitudeValue = _latitude.value;

        DateValidate(dateValue, monthValue);
    }

    private void DateValidate(int date, int month)
    {
        try
        {
            DateTime d = new DateTime(2023,month,date);
        }
        catch (ArgumentOutOfRangeException e)
        {
            //Display bad day error window
            Debug.LogWarning(e.Message);
        }
    }
    #endregion












}
