namespace AuthorizationService {
    $(document.documentElement).ready(() => {
        const showSelect = new TwoFactorManagement();
        showSelect.init();
    });

    class TwoFactorManagement {
        public init() {
            if (!$("#twoFactorEnabled").is(":checked")) {
                $("#twoFactorProvidersSelect").hide();
            }
            $("#twoFactorEnabled").change(() => {
                if ($("#twoFactorEnabled").is(":checked")) {

                    $("#twoFactorProvidersSelect").fadeIn();
                    return;
                }
                $("#twoFactorProvidersSelect").fadeOut();
            });
        }
    }
}
