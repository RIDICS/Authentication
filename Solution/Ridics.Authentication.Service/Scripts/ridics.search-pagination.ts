namespace AuthorizationService {
    $(document.documentElement).ready(() => {
        const searchPagination = new SearchPagination();
        searchPagination.init();
    });

    class SearchPagination {
        private readonly numberOfItems: JQuery;
        private readonly searchForm: JQuery;
        private readonly resetForm: JQuery;
        private readonly searchCount: JQuery;
        private readonly resetCount: JQuery;

        constructor() {
            this.numberOfItems = $("#number-of-items");
            this.searchForm = $("#search-form");
            this.resetForm = $("#reset-form");
            this.searchCount = $("#search-count");
            this.resetCount = $("#reset-search-count");
        }

        public init() {
            this.searchForm.on("submit",
                () => {
                    this.setSearchCount();
                });

            this.resetForm.on("submit",
                () => {
                    this.setResetCount();
                });
        }

        private setSearchCount() {
            const items = this.numberOfItems.val();

            this.searchCount.val(items);
        }

        private setResetCount() {
            const items = this.numberOfItems.val();

            this.resetCount.val(items);
        }
    }
}
