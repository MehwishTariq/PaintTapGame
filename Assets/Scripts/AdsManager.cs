using GoogleMobileAds.Api;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdsManager : MonoBehaviour
{
    public static AdsManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(instance);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public bool testAds;

    private string appId = "ca-app-pub-7418823270776132~9642597821";
    private string interstitialAdUnitId = "ca-app-pub-7418823270776132/5492471966";
    private string bannerAdUnitId = "ca-app-pub-7418823270776132/8552328691";

    public void Start()
    {
        if (testAds)
        {
            interstitialAdUnitId = "ca-app-pub-3940256099942544/1033173712";
            bannerAdUnitId = "ca-app-pub-3940256099942544/6300978111";

        }

        // Initialize the Google Mobile Ads SDK.
        //MobileAds.Initialize((InitializationStatus initStatus) =>
        //{
        //    // This callback is called once the MobileAds SDK is initialized.
            
        //});

        LoadInterstitialAd();
        LoadBannerAd();
        Invoke(nameof(Delay), 1f);
    }

    bool allowTimer;
    float clockTime = 60;

    private void Update()
    {
        if (allowTimer)
        {
            //Debug.Log("ClockTime: " + clockTime);
            clockTime -= Time.deltaTime;
            if(clockTime <= 0)
            {
                allowTimer = false;
                clockTime = 60;
                ShowInterstitialAd();
                
            }
        }
    }


    #region InterstitialAds
    private InterstitialAd _interstitialAd;

    /// <summary>
    /// Shows the interstitial ad.
    /// </summary>
    public void ShowInterstitialAd()
    {
        if (_interstitialAd != null && _interstitialAd.CanShowAd())
        {
            HideBanner();
            Debug.Log("Showing interstitial ad.");
            _interstitialAd.Show();
        }
        else
        {
            LoadInterstitialAd();
            Debug.LogError("Interstitial ad is not ready yet.");
        }
        Invoke(nameof(Delay), 1f);
        
    }
    void Delay()
    {
        allowTimer = true;
        Debug.Log("Show BANNER");
        ShowBannerAd();
    }
    /// <summary>
    /// Loads the interstitial ad.
    /// </summary>
    public void LoadInterstitialAd()
    {
        // Clean up the old ad before loading a new one.
        if (_interstitialAd != null)
        {
            _interstitialAd.Destroy();
            _interstitialAd = null;
        }

        Debug.Log("Loading the interstitial ad.");

        // create our request used to load the ad.
        var adRequest = new AdRequest();

        // send the request to load the ad.
        InterstitialAd.Load(interstitialAdUnitId, adRequest,
            (InterstitialAd ad, LoadAdError error) =>
            {
                  // if error is not null, the load request failed.
                if (error != null || ad == null)
                {
                    Debug.LogError("interstitial ad failed to load an ad " +
                                    "with error : " + error);
                    return;
                }

                Debug.Log("Interstitial ad loaded with response : "
                            + ad.GetResponseInfo());

                RegisterEventHandlers(ad);
                _interstitialAd = ad;
            });
    }
    private void RegisterEventHandlers(InterstitialAd interstitialAd)
    {
        // Raised when the ad is estimated to have earned money.
        interstitialAd.OnAdPaid += (AdValue adValue) =>
        {
            Debug.Log(String.Format("Interstitial ad paid {0} {1}.",
                adValue.Value,
                adValue.CurrencyCode));
        };
        // Raised when an impression is recorded for an ad.
        interstitialAd.OnAdImpressionRecorded += () =>
        {
            Debug.Log("Interstitial ad recorded an impression.");
        };
        // Raised when a click is recorded for an ad.
        interstitialAd.OnAdClicked += () =>
        {
            Debug.Log("Interstitial ad was clicked.");
        };
        // Raised when an ad opened full screen content.
        interstitialAd.OnAdFullScreenContentOpened += () =>
        {
            Debug.Log("Interstitial ad full screen content opened.");
        };
        // Raised when the ad closed full screen content.
        interstitialAd.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log("Interstitial ad full screen content closed.");
            LoadInterstitialAd();
            //allowTimer = true;
        };
        // Raised when the ad failed to open full screen content.
        interstitialAd.OnAdFullScreenContentFailed += (AdError error) =>
        {
            Debug.LogError("Interstitial ad failed to open full screen content " +
                           "with error : " + error);
           // allowTimer = true;
           // LoadInterstitialAd();
        };
    }
    #endregion

  
    #region Banner
    BannerView _bannerView;


    /// <summary>
    /// Shows the banner ad.
    /// </summary>
    public void ShowBannerAd()
    {
        if (_bannerView != null)
        {
            Debug.Log("Showing Banner ad.");
            _bannerView.Show();
        }
        else
        {
            LoadBannerAd();
            Debug.LogError("Banner ad is not ready yet.");
        }
    }

    /// <summary>
    /// Creates a 320x50 banner view at top of the screen.
    /// </summary>
    public void CreateBannerView()
    {
        Debug.Log("Creating banner view");

        // If we already have a banner, destroy the old one.
        if (_bannerView != null)
        {
            DestroyBannerView();
        }

        // Create a 320x50 banner at top of the screen
        _bannerView = new BannerView(bannerAdUnitId, AdSize.Banner, AdPosition.Top);
    }

    /// <summary>
    /// Creates the banner view and loads a banner ad.
    /// </summary>
    public void LoadBannerAd()
    {
        // create an instance of a banner view first.
        if (_bannerView == null)
        {
            CreateBannerView();
        }

        // create our request used to load the ad.
        var adRequest = new AdRequest();

        // send the request to load the ad.
        Debug.Log("Loading banner ad.");
        _bannerView.LoadAd(adRequest);
        ListenToAdEvents();
    }
    
    public void HideBanner()
    {
        if(_bannerView != null)
        {
            _bannerView.Hide();
        }
        LoadBannerAd();
    }

    /// <summary>
     /// Destroys the banner view.
     /// </summary>
    public void DestroyBannerView()
    {
        if (_bannerView != null)
        {
            Debug.Log("Destroying banner view.");
            _bannerView.Destroy();
            _bannerView = null;
        }
    }
    /// <summary>
    /// listen to events the banner view may raise.
    /// </summary>
    private void ListenToAdEvents()
    {
        // Raised when an ad is loaded into the banner view.
        _bannerView.OnBannerAdLoaded += () =>
        {
            Debug.Log("Banner view loaded an ad with response : "
                + _bannerView.GetResponseInfo());
        };
        // Raised when an ad fails to load into the banner view.
        _bannerView.OnBannerAdLoadFailed += (LoadAdError error) =>
        {
            Debug.LogError("Banner view failed to load an ad with error : "
                + error);
          
        };
        // Raised when the ad is estimated to have earned money.
        _bannerView.OnAdPaid += (AdValue adValue) =>
        {
            Debug.Log(String.Format("Banner view paid {0} {1}.",
                adValue.Value,
                adValue.CurrencyCode));
        };
        // Raised when an impression is recorded for an ad.
        _bannerView.OnAdImpressionRecorded += () =>
        {
            Debug.Log("Banner view recorded an impression.");
        };
        // Raised when a click is recorded for an ad.
        _bannerView.OnAdClicked += () =>
        {
            Debug.Log("Banner view was clicked.");
        };
        // Raised when an ad opened full screen content.
        _bannerView.OnAdFullScreenContentOpened += () =>
        {
            Debug.Log("Banner view full screen content opened.");
        };
        // Raised when the ad closed full screen content.
        _bannerView.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log("Banner view full screen content closed.");
        };
    }
    #endregion
}
