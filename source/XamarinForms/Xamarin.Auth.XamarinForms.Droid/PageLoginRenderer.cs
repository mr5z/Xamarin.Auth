using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;

using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Xamarin.Auth.XamarinForms;
using Xamarin.Auth;
using Xamarin.Auth.Helpers;


[assembly: 
	Xamarin.Forms.ExportRenderer
			(
			// ViewElement to be rendered (from Portable/Shared)
			typeof(Xamarin.Auth.XamarinForms.PageLogin),
			// platform specific Renderer : global::Xamarin.Forms.Platform.XamarinIOS.PageRenderer
			typeof(Xamarin.Auth.XamarinForms.XamarinAndroid.PageLoginRenderer)
			)
]
namespace Xamarin.Auth.XamarinForms.XamarinAndroid
{
	public partial class PageLoginRenderer : global::Xamarin.Forms.Platform.Android.PageRenderer
	{
		bool IsShown;

		protected override void OnElementChanged (ElementChangedEventArgs<Page> e)
		{
			base.OnElementChanged (e);

			// OnElementChanged is fired before ViewDidAppear, using it to pass data

			PageLogin e_new = e.NewElement as PageLogin;

			// PageRenderer is a ViewGroup - so should be able to load an AXML file and FindView<>
			activity = this.Context as Activity;

			if (!IsShown)
			{

				IsShown = true;

				if (null != e_new.OAuth)
				{
					this.Authenticate(e_new.OAuth);
					return;
				}
			}
			return;
		}

		Android.App.Activity activity = null;

		private void Authenticate(Xamarin.Auth.Helpers.OAuth1 oauth1)
		{
			OAuth1Authenticator auth = new OAuth1Authenticator 
				(
					consumerKey: oauth1.OAuth_IdApplication_IdAPI_KeyAPI_IdClient_IdCustomer,
					consumerSecret: oauth1.OAuth1_SecretKey_ConsumerSecret_APISecret,
					requestTokenUrl: oauth1.OAuth1_UriRequestToken,
					authorizeUrl: oauth1.OAuth_UriAuthorization,
					accessTokenUrl: oauth1.OAuth1_UriAccessToken,
					callbackUrl: oauth1.OAuth_UriCallbackAKARedirect
				);

			auth.AllowCancel = oauth1.AllowCancel;

			// If authorization succeeds or is canceled, .Completed will be fired.
			auth.Completed += Auth_Completed;

			activity.StartActivity (auth.GetUI(activity));

			return;
		}



		private void Authenticate(Xamarin.Auth.Helpers.OAuth2 oauth2)
		{
			OAuth2Authenticator auth = new OAuth2Authenticator 
				(
					clientId: oauth2.OAuth_IdApplication_IdAPI_KeyAPI_IdClient_IdCustomer,
					scope: oauth2.OAuth2_Scope,
					authorizeUrl: oauth2.OAuth_UriAuthorization,
					redirectUrl: oauth2.OAuth_UriCallbackAKARedirect
				);

			auth.AllowCancel = oauth2.AllowCancel;

			// If authorization succeeds or is canceled, .Completed will be fired.
			auth.Completed += Auth_Completed;

			activity.StartActivity (auth.GetUI(activity));

			return;
		}


		private void Auth_Completed(object sender, global::Xamarin.Auth.AuthenticatorCompletedEventArgs e)
		{
			if (e.IsAuthenticated)
			{
				// e.Account contains info:
				//		e.AccountProperties[""]
				//
				// use access tokenmore detailed user info from the API

				this.AccountProperties = e.Account.Properties;
			}
			else
			{
				// The user cancelled
			}

			// dismiss UI on iOS, because it was manually created
			// IOS
			// 			DismissViewController(true, null);
			// Android

			// possibly do something to dismiss THIS viewcontroller, 
			// or else login screen does not disappear             

			return;
		}

		public OAuth Oauth
		{
			get;
			set;
		}

		protected Dictionary<string, string> account_properties;

		public Dictionary<string, string> AccountProperties
		{
			protected get
			{
				return account_properties;
			}
			set
			{
				this.OAuth.AccountProperties = account_properties = value;
			}
		}
	}
}