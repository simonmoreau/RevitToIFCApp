import { Component, OnInit } from '@angular/core';
import {
  Event,
  NavigationCancel,
  NavigationEnd,
  NavigationError,
  NavigationStart,
  Router,
} from '@angular/router';
import { BroadcastService, MsalService } from '@azure/msal-angular';
import { Logger, CryptoUtils } from 'msal';
import { isIE, b2cPolicies } from './app-config';
import { UserService } from './services/user.service';
import { NgcCookieConsentService } from 'ngx-cookieconsent';

// declare gtag as a function to set and sent the events
declare let gtag: Function;

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss'],
})
export class AppComponent implements OnInit {
  isIframe = false;
  loggedIn = false;
  userName: any;
  loading: boolean;

  constructor(
    private broadcastService: BroadcastService,
    private authService: MsalService,
    private userService: UserService,
    private router: Router,
    private ccService: NgcCookieConsentService
  ) {
    this.router.events.subscribe((event: Event) => {
      if (event instanceof NavigationEnd) {
        gtag('config', 'G-2S7M7HEKL6', {
          page_path: event.urlAfterRedirects,
        });
      }

      switch (true) {
        case event instanceof NavigationStart: {
          this.loading = true;
          break;
        }

        case event instanceof NavigationEnd:
        case event instanceof NavigationCancel:
        case event instanceof NavigationError: {
          this.loading = false;
          break;
        }
        default: {
          break;
        }
      }
    });
  }

  ngOnInit() {
    this.isIframe = window !== window.parent && !window.opener;
    this.checkAccount();

    // event listeners for authentication status
    this.broadcastService.subscribe('msal:loginSuccess', (success) => {
      // We need to reject id tokens that were not issued with the default sign-in policy.
      // "acr" claim in the token tells us what policy is used (NOTE: for new policies (v2.0), use "tfp" instead of "acr")
      // To learn more about b2c tokens, visit https://docs.microsoft.com/en-us/azure/active-directory-b2c/tokens-overview
      if (success.idToken.claims['tfp'] !== b2cPolicies.names.signUpSignIn) {
        window.alert(
          'Password has been reset successfully. \nPlease sign-in with your new password'
        );
        return this.authService.logout();
      }

      console.log(
        'login succeeded. id token acquired at: ' + new Date().toString()
      );
      console.log(success);

      this.userService.refreshToken().subscribe((t) => {
        console.log('Get a Forge Token');
        console.log('this.router.navigateByUrl');
        // this.router.navigateByUrl('/upload');
      });

      this.checkAccount();
    });

    this.broadcastService.subscribe('msal:loginFailure', (error) => {
      console.log('login failed');
      console.log(error);

      // Check for forgot password error
      // Learn more about AAD error codes at https://docs.microsoft.com/en-us/azure/active-directory/develop/reference-aadsts-error-codes
      if (error.errorMessage.indexOf('AADB2C90118') > -1) {
        this.authService.loginRedirect(b2cPolicies.authorities.resetPassword);
        if (isIE) {
          this.authService.loginRedirect(b2cPolicies.authorities.resetPassword);
        } else {
          this.authService.loginPopup(b2cPolicies.authorities.resetPassword);
        }
      }
    });

    // redirect callback for redirect flow (IE)
    this.authService.handleRedirectCallback((authError, response) => {
      if (authError) {
        console.error('Redirect Error: ', authError.errorMessage);
        return;
      }

      console.log('Redirect Success: ', response);
    });

    this.authService.setLogger(
      new Logger(
        (logLevel, message, piiEnabled) => {
          // console.log('MSAL Logging: ', message);
        },
        {
          correlationId: CryptoUtils.createNewGuid(),
          piiLoggingEnabled: false,
        }
      )
    );
  }

  // other methods
  checkAccount() {
    const account = this.authService.getAccount();
    this.loggedIn = !!account;
    if (account) {
      this.userName = account.name;
      // this.router.navigateByUrl('/upload');
    }
  }

  login() {
    this.authService.loginRedirect();
    if (isIE) {
      this.authService.loginRedirect();
    } else {
      this.authService.loginPopup();
    }
  }

  logout() {
    this.authService.logout();
  }
}
