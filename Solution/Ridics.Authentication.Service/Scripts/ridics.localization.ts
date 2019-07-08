namespace AuthorizationService {
    $(document.documentElement).ready(() => {
        const localizationInit = new LocalizationInitializer();
        localizationInit.init();
    });

    class LocalizationInitializer {
        private readonly languageSelector = $("#language-select");
        private readonly selectLanguageForm = $("#request-culture-form");

        public init() {
            this.languageSelector.on("change",
                () => {
                    this.selectLanguageForm.submit();
                });
        }
    }
}
