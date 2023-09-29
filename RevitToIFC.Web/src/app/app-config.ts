//import { MsalAngularConfiguration } from '@azure/msal-angular';
//import { environment } from 'src/environments/environment';

//// this checks if the app is running on IE
//export const isIE = window.navigator.userAgent.indexOf('MSIE ') > -1 || window.navigator.userAgent.indexOf('Trident/') > -1;

///** =================== REGIONS ====================
// * 1) B2C policies and user flows
// * 2) Web API configuration parameters
// * 3) Authentication configuration parameters
// * 4) MSAL-Angular specific configuration parameters
// * ================================================= 
//*/ 

//// #region 1) B2C policies and user flows
///**
// * Enter here the user flows and custom policies for your B2C application,
// * To learn more about user flows, visit https://docs.microsoft.com/en-us/azure/active-directory-b2c/user-flow-overview
// * To learn more about custom policies, visit https://docs.microsoft.com/en-us/azure/active-directory-b2c/custom-policy-overview
// */
//export const b2cPolicies = {
//    names: {
//        signUpSignIn: 'B2C_1_RevitToIFCSignupsignin1',
//        resetPassword: 'B2C_1_passwordreset1',
//    },
//    authorities: {
//        signUpSignIn: {
//            authority: 'https://revittoifc.b2clogin.com/revittoifc.onmicrosoft.com/B2C_1_RevitToIFCSignupsignin1'
//        },
//        resetPassword: {
//            authority: 'https://revittoifc.b2clogin.com/revittoifc.onmicrosoft.com/B2C_1_passwordreset1'
//        }
//    }
//}
//// #endregion


//// #region 2) Web API Configuration
///** 
// * Enter here the coordinates of your Web API and scopes for access token request
// * The current application coordinates were pre-registered in a B2C tenant.
// */
//export const apiConfig: {b2cScopes: string[], webApi: string} = {
//    b2cScopes: ['https://revittoifc.onmicrosoft.com/helloapi/demo.read'],
//    webApi: 'https://fabrikamb2chello.azurewebsites.net/*'
//};
//// #endregion



//// #region 3) Authentication Configuration
///** 
// * Config object to be passed to Msal on creation. For a full list of msal.js configuration parameters,
// * visit https://azuread.github.io/microsoft-authentication-library-for-js/docs/msal/modules/_configuration_.html
// */
//export const msalConfig: Configuration = {
//    auth: {
//        clientId: 'cf4d263d-0c0b-4dc2-978b-bd3746416330',
//        authority: b2cPolicies.authorities.signUpSignIn.authority,
//        redirectUri: environment.localUri,
//        postLogoutRedirectUri: environment.localUri,
//        navigateToLoginRequestUrl: true,
//        validateAuthority: false,
//      },
//    cache: {
//        cacheLocation: 'localStorage',
//        storeAuthStateInCookie: isIE, // Set this to "true" to save cache in cookies to address trusted zones limitations in IE
//    },
//}

///** 
// * Scopes you enter here will be consented once you authenticate. For a full list of available authentication parameters, 
// * visit https://azuread.github.io/microsoft-authentication-library-for-js/docs/msal/modules/_authenticationparameters_.html
// */
//export const loginRequest: {scopes: string[]} = {
//    scopes: ['openid', 'profile'],
//};

//// Scopes you enter will be used for the access token request for your web API
//export const tokenRequest: {scopes: string[]} = {
//    scopes: apiConfig.b2cScopes // i.e. [https://revittoifc.onmicrosoft.com/helloapi/demo.read]
//};
//// #endregion



//// #region 4) MSAL-Angular Configuration
//// here you can define the coordinates and required permissions for your protected resources
//export const protectedResourceMap: [string, string[]][] = [
//    [apiConfig.webApi, apiConfig.b2cScopes]
//    // i.e. [https://revittoifchello.azurewebsites.net/hello, ['https://revittoifc.onmicrosoft.com/helloapi/demo.read']]
//];

///** 
// * MSAL-Angular specific authentication parameters. For a full list of available options,
// * visit https://github.com/AzureAD/microsoft-authentication-library-for-js/tree/dev/lib/msal-angular#config-options-for-msal-initialization
//*/
//export const msalAngularConfig: MsalAngularConfiguration = {
//    popUp: false,
//    consentScopes: [
//        ...loginRequest.scopes,
//        ...tokenRequest.scopes,
//    ],
//    protectedResourceMap,     // API calls to these coordinates will activate MSALGuard
//    extraQueryParameters: {}
//};
//// #endregion
