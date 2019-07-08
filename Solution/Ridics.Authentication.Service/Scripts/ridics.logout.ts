namespace AuthorizationService {
    $(document.documentElement).ready(() => {
        const logout = new Logout();
        logout.init();
    });

    class Logout {
        private readonly signoutUrl: string;
        private readonly redirectUrl: string;
        private countdown: number;

        private readonly logoutFrame: JQuery;
        private readonly redirectCountdown: JQuery;
        private readonly redirectButton: JQuery;

        constructor() {
            const logoutScriptElement = $("#logout-script");

            this.signoutUrl = logoutScriptElement.attr("data-signout-url");
            this.redirectUrl = logoutScriptElement.attr("data-redirect-url");
            this.countdown = parseInt(logoutScriptElement.attr("data-redirect-seconds"), 10);

            this.logoutFrame = $("#logoutFrame");
            this.redirectCountdown = $("#redirect-countdown");
            this.redirectButton = $("#redirect-button");
        }

        /*Iframe has to be programatically added, otherwise the onload event can be fired before listener is attached*/
        public init() {
            if (this.signoutUrl === "") {
                this.onFrameLoad();
            } else {
                $("<iframe>")
                    .attr({
                        src: this.signoutUrl,
                    } as JQuery.PlainObject)
                    .on("load",
                    () => {
                        this.onFrameLoad();
                    }).appendTo(this.logoutFrame);
            }
        }

        private onFrameLoad() {
            this.redirectButton.removeClass("hidden");

            this.redirectCountdown.text(this.countdown);

            window.setInterval(() => {
                    this.tick();
                },
                1000);
        }

        private tick() {
            this.countdown--;

            if (this.countdown >= 0) {
                this.redirectCountdown.text(this.countdown);
            }
            if (this.countdown === 0) {
                window.location.replace(this.redirectUrl);
            }
        }
    }
}
