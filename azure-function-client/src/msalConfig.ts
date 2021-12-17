import * as msal from "@azure/msal-browser";

export const msalConfig: msal.Configuration = {
  auth: {
    clientId: '562c2405-0be6-4dcd-9172-e9fc6c681d17',
    authority: 'https://login.microsoftonline.com/7c3aa68f-b3a1-415a-aa62-a6a97d4a12fc'
  }
};

export const loginRequest = {
  scopes: [
    "openid",
    "email",
    "profile",
    "562c2405-0be6-4dcd-9172-e9fc6c681d17/user_impersonation"
  ],
};