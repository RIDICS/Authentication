namespace AuthorizationService {
    $(document.documentElement).ready(() => {
        const verifyUser = new ProvidersSection();
        verifyUser.init();
    });

    class ProvidersSection {
        public init() {
            $(".external-identity-card-header-button").click((event) => {
                const clickedButton = $(event.target).closest("button");
                const parentHeader = clickedButton.parent(".external-identity-card-header");
                parentHeader.next(".external-identity-card-body").slideToggle();
                clickedButton.find("i").toggleClass("fa-rotate-180");
            });
        }
    }
}
