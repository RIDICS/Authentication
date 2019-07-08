namespace AuthorizationService {
    $(document.documentElement).ready(() => {
        const cookieConsent = new CookieConsent();
        cookieConsent.init();
    });

    class CookieConsent {
        private acceptButton: JQuery;
        private alert: JQuery;
        private navbarContainer: JQuery;
        private learnMoreBtn: JQuery;
        private consentContainer: JQuery;
        private headerContainer: JQuery;
        constructor() {
            this.acceptButton = $("#cookieConsent button[data-cookie-string]");
            this.alert = this.acceptButton.closest(`.alert[role="alert"]`);
            this.navbarContainer = $(".navbar-container");
            this.learnMoreBtn = $(".cookie-more-btn");
            this.consentContainer = $(".cookie-consent-wrapper");
            this.headerContainer = $(".page-header");
        }

        public init() {
            if (this.consentContainer.length) {
                const jqWindow: JQuery<Window> = $(window as any);
                jqWindow.on("resize", () => {
                    const consentHeight = $(".cookie-consent-container").outerHeight();
                    this.navbarContainer.css("top", consentHeight);
                    this.headerContainer.css("margin-bottom", consentHeight);
                });
                jqWindow.resize();
            }

            this.acceptButton.on("click", () => {
                document.cookie = this.acceptButton.attr("data-cookie-string");
                this.slideNav();
            });
        }

        private slideNav() {
            this.navbarContainer.animate({
                top: 0,
            } as JQuery.PlainObject, "slow");

            this.headerContainer.animate({
                marginBottom: 0,
            } as JQuery.PlainObject, "slow");

            this.consentContainer.slideUp("slow");
        }
    }
}
