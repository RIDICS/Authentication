namespace AuthorizationService {
    export class AuthApiClient {

        public resendAuthCode(): JQueryPromise<any> {
            const url = `${Utils.getBaseUrl()}Account/ResendAuthorizationCode`;
            return $.post(url);
        }
    }
}
