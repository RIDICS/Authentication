namespace AuthorizationService {
    $(document.documentElement).ready(() => {
        const utils = new Utils();
        utils.configureJqueryAjax();
    });

    export class Utils {
        public static getBaseUrl(): string {
            const baseUrl = $("#base-url").data("path") as string;
            return baseUrl;
        }

        public getQueryParams(): { [name: string]: string; } {
            const result: { [name: string]: string; } = {};
            let hash: string[];

            let query = window.location.search;
            query = query.substring(1, query.length);

            const hashes = query.split("&");
            for (const queryHash of hashes) {
                hash = queryHash.split("=");
                result[hash[0]] = hash[1];
            }
            return result;
        }

        public configureJqueryAjax() {
            $(document as any).ajaxSend((event, request, settings) => {
                    this.addAntiforgeryTokenHeader(request, settings);
                });
        }

        private addAntiforgeryTokenHeader(request: JQuery.jqXHR, settings: JQuery.AjaxSettings) {
            const baseUrl = Utils.getBaseUrl();
            if (settings.url.lastIndexOf(baseUrl, 0) === 0) { // Modify only local requests
                request.setRequestHeader("RequestVerificationToken", this.getAntiForgeryToken());
            }
        }

        public getAntiForgeryToken(): string {
            return $("input[name='__RequestVerificationToken']").val() as string;
        }
    }
}
