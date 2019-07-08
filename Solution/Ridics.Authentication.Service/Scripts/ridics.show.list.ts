namespace AuthorizationService {
    $(document.documentElement).ready(() => {
        const list = new List();
        list.init();
    });

    class List {
        private readonly numberOfItemsSelector = ".number-of-items-control";
        private readonly paginationContainerSelector = ".pagination-container";
        private readonly paginationSelector = ".pagination";
        private readonly listSelector = "#list";

        private readonly  notificationEl = $(".notification-selector");

        private pagination: Pagination;
        private readonly guiCommon: GuiCommon;
        private readonly utils: Utils;

        private $numberOfItems: JQuery;
        private selectedNumberOfItems: number;
        private paginationContainer: HTMLDivElement;
        private itemsCount: number;

        private url: string[];
        private controller: string;
        private actionUrl: string;
        private $list: JQuery;

        constructor() {
            this.guiCommon = new GuiCommon();
            this.utils = new Utils();
        }

        public init() {
            this.$list = $(this.listSelector);

            const $paginationContainer: JQuery = this.$list.nextAll(this.paginationContainerSelector);
            this.paginationContainer = $paginationContainer.children(this.paginationSelector)[0] as HTMLDivElement;

            if (typeof this.paginationContainer === "undefined") {
                return;
            }

            this.itemsCount = parseInt(this.paginationContainer.attributes.getNamedItem("data-items-count").value, 10);

            this.$numberOfItems = $paginationContainer.find(this.numberOfItemsSelector);

            this.pagination = new Pagination({
                container: this.paginationContainer,
                pageClickCallback: (pageNumber) => {
                    this.sendToController(this.actionUrl, pageNumber);
                },
            });

            let startPage = this.pagination.getCurrentPage();

            if (typeof startPage === "undefined") {

                const uriSearch = new URI(window.location.href).search(true);
                const startItem = uriSearch.start;
                startPage = this.computeInitPage(this.getItemsOnPage(), startItem);
            }

            this.renderPaginationContainer(startPage);
            this.handlePaginationEvent();
        }

        private sendToController(actionPath: string, pageNumber: number, searchValue: string = null) {

            const itemsOnPageCount = this.getItemsOnPage();
            const currentPage = pageNumber;
            const startItemNumber = this.computeStartItem(itemsOnPageCount, currentPage);

            let searchByName: string;
            if (searchValue != null) {
                searchByName = searchValue;
            } else {
                searchByName = this.utils.getQueryParams().searchByName;
            }

            this.renderPaginationContainer(currentPage);

            const drawDelay = 400;
            let notificationTimeout: number;
            let listRedrawEvent;

            const beforeSendNotification: INotificationConfiguration = {
                notificationType: NotificationType.Info,
                body: "Loading items...", // TODO localize
                isDismissible: false,
                hasSpinner: true,
                drawDelay,
                container: this.notificationEl,
            };

            const errorNotification: INotificationConfiguration = {
                notificationType: NotificationType.Danger,
                body: "Failed to load items.", // TODO localize
                isDismissible: true,
                container: this.notificationEl,
            };

            $.ajax({
                url: Utils.getBaseUrl() + actionPath,
                dataType: "text",
                method: "GET",
                data: { start: startItemNumber, count: itemsOnPageCount, partial: true, searchByName } as JQuery.PlainObject,
                beforeSend: () => {
                    listRedrawEvent = setTimeout(() => this.$list.hide(), drawDelay);

                    notificationTimeout = this.guiCommon.showNotification(beforeSendNotification).timeoutVariable;
                },
                success: (data) => {

                    const newUri = new URI(window.location.href).search((query) => {
                        query.start = startItemNumber;
                        query.count = itemsOnPageCount;
                        if (searchByName != null) {
                            query.searchByName = searchByName;
                        }
                    }).toString();

                    history.replaceState(null, null, newUri);

                    clearTimeout(listRedrawEvent);
                    listRedrawEvent = null;

                    this.guiCommon.destroyNotification(this.notificationEl, notificationTimeout);
                    notificationTimeout = null;

                    this.$list.html(data);
                    this.$list.show();
                },
                error: () => {
                    this.guiCommon.showNotification(errorNotification);
                },
            });
        }

        private computeStartItem(itemsPerPage: number, currentPage: number): number {
            return itemsPerPage * (currentPage - 1);
        }

        private computeInitPage(itemsPerPage: number, startItem: number): number {
            return Math.ceil(startItem / itemsPerPage + 1);
        }

        private getItemsOnPage(): number {
            return +this.$numberOfItems.val();
        }

        private handlePaginationEvent() {
            const baseUrl = Utils.getBaseUrl();
            const pathname = window.location.pathname.indexOf(baseUrl) === 0
                ? window.location.pathname.substr(baseUrl.length)
                : window.location.pathname.substr(1);
            this.url = pathname.split("/");
            this.controller = this.url[0];
            this.actionUrl = this.controller + "/Index";

            this.$numberOfItems.on("change",
                () => {
                    this.sendToController(this.actionUrl, 1);
                });
        }

        private renderPaginationContainer(activePage: number) {
            this.selectedNumberOfItems = +$(`${this.numberOfItemsSelector} option:selected`).val();
            this.pagination.make(this.itemsCount, +this.selectedNumberOfItems, activePage);
        }
    }
}
