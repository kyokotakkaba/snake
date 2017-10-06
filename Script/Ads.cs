using UnityEngine;
using System.Collections;
using GoogleMobileAds.Api;

public class Ads : MonoBehaviour {
	private BannerView bannerViewTop;
	private BannerView bannerViewBottom;
	private InterstitialAd interstitial;
	private RewardBasedVideoAd rewardVideo;


	private string bannerID;
	private string interstitialID;
	private string rewardVideoID;
	private AdRequest bannerAdRequest;

	public int interstitialShowAfter;
	private int interstitialCounter;

	void Start () {
		bannerID = "ca-app-pub-3940256099942544/6300978111";
		interstitialID = "ca-app-pub-3940256099942544/1033173712";
		rewardVideoID = "ca-app-pub-3940256099942544/5224354917";

		//bannerAdRequest = new AdRequest.Builder().Build();

		interstitialCounter = 0;
		RequestInterstitial ();

		rewardVideo = RewardBasedVideoAd.Instance;
		rewardVideo.OnAdClosed += HandleOnAdClosed;
		RequestRewardVideo ();
	}

	public void showBannerBottom(){
		string adUnitId = bannerID;

		if (bannerViewTop!=null) {
			bannerViewTop.Hide ();
		}

		if (bannerViewBottom != null) {
			bannerViewBottom.Show ();
		} else {
			// Create a 320x50 banner at the top of the screen.
			bannerViewBottom = new BannerView(adUnitId, AdSize.SmartBanner, AdPosition.Bottom);
			// Load the banner with the request.
			bannerViewBottom.LoadAd(bannerAdRequest);
		}



	}

	public void showBannerTop(){
		string adUnitId = bannerID;

		if (bannerViewBottom != null) {
			bannerViewBottom.Hide ();
		
		}

		if (bannerViewTop != null) {
			bannerViewTop.Show ();
		} else {
			// Create a 320x50 banner at the top of the screen.
			bannerViewTop = new BannerView(adUnitId, AdSize.SmartBanner, AdPosition.Top);
			// Load the banner with the request.
			bannerViewTop.LoadAd(bannerAdRequest);
		}

	}

	private void RequestInterstitial(){
		if (interstitial != null) {
			interstitial.Destroy ();
		}
		string adUnitId = interstitialID;
		// Initialize an InterstitialAd.
		interstitial = new InterstitialAd(adUnitId);
		// Create an empty ad request.
		AdRequest request = new AdRequest.Builder().Build();
		// Load the interstitial with the request.
		interstitial.LoadAd(request);
	}

	public void showInterstitial(){
		if (interstitial.IsLoaded ()) {
			interstitial.Show ();
			RequestInterstitial ();
		} else {
			RequestInterstitial ();
		}
	}

	public void addInterstitialCounter(){
		interstitialCounter++;
		if (interstitialShowAfter<=interstitialCounter) {
			showInterstitial ();
			interstitialCounter = 0;
		}

	}

	private void RequestRewardVideo(){
		string adUnitId = rewardVideoID;

		AdRequest request = new AdRequest.Builder().Build();
		rewardVideo.LoadAd(request, adUnitId);
	}

	public void showRewardVideo(){
		if (rewardVideo.IsLoaded()) {
			rewardVideo.Show ();
		} else {
			RequestRewardVideo ();
		}
	}

	public void HandleOnAdClosed(object sender, System.EventArgs args){
		RequestRewardVideo ();
	}

}
