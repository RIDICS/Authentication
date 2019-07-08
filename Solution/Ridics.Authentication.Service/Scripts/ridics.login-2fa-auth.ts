namespace AuthorizationService {
    $(document.documentElement).ready(() => {
        const verifyCode = new CodeAuthentication();
        verifyCode.init();

    });

    class CodeAuthentication {
        private readonly authApiClient: AuthApiClient;

        private readonly loginForm: JQuery;

        constructor() {
            this.authApiClient = new AuthApiClient();
            this.loginForm = $("#login-form");
        }

        public init() {
            if ($(".form-errors li:visible").length === 0) {
                $(".form-errors").css("display", "none");
            }

            $("#login-form").submit((event: JQuery.SubmitEvent) => {
                this.verifyAuthCode(event);
            });

            $("#resend-code-button").click((event: JQuery.ClickEvent) => {
                this.resendAuthCode(event);
            });
        }

        private verifyAuthCode(event: JQuery.SubmitEvent) {
            this.showLoading(this.loginForm);

            let code: string = "";
            $(".verification-code").each((index, element) => {
                code += $(element as Node as HTMLElement).val();
            });

            $("#TwoFactorCodeHidden").val(code);

            this.removeLoading(this.loginForm);
        }

        private resendAuthCode(event: JQuery.ClickEvent) {
            event.preventDefault();

            $("#resend-message").addClass("hidden");
            $("#resend-message-error").addClass("hidden");

            this.showLoading(this.loginForm);

            const requestPromise = this.authApiClient.resendAuthCode();

            requestPromise.done(() => {
                $("#resend-message").removeClass("hidden");
            });

            requestPromise.fail(() => {
                $("#resend-message-error").removeClass("hidden");
            });

            requestPromise.always(() => {
                this.removeLoading(this.loginForm);
            });
        }

        private showLoading(element: JQuery) {
            const loadingDiv = document.createElement("div");
            $(loadingDiv).addClass("loading");

            const loadingIcon = document.createElement("i");
            $(loadingIcon).addClass("fa fa-spinner fa-pulse fa-3x fa-fw");
            $(loadingDiv).append(loadingIcon);

            element.append(loadingDiv);
            element.css("position", "relative");
        }

        private removeLoading(element: JQuery) {
            const loadingDiv = element.find(".loading");
            element.removeAttr("style");
            loadingDiv.remove();
        }
    }
}
