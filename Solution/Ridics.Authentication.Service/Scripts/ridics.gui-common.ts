namespace AuthorizationService {
    export class GuiCommon {
        public showNotification(notificationConfiguration: INotificationConfiguration): INotification {
            const notificationContainer = notificationConfiguration.container;
            const drawDelay = notificationConfiguration.drawDelay ? notificationConfiguration.drawDelay : null;
            const spinnerIsDefined = (typeof notificationConfiguration.hasSpinner !== "undefined" &&
                notificationConfiguration.hasSpinner !== null);
            const hasSpinner = spinnerIsDefined ? notificationConfiguration.hasSpinner : false;
            const isDismissableIsDefined = (typeof notificationConfiguration.isDismissible !== "undefined" &&
                notificationConfiguration.isDismissible !== null);
            const isDismissable = isDismissableIsDefined ? notificationConfiguration.isDismissible : true;
            let body = notificationConfiguration.body;
            const notificationType = notificationConfiguration.notificationType;

            this.destroyNotification(notificationContainer);

            notificationContainer.show();

            if (hasSpinner) {
                const spinner = `<i class="fas fa-circle-notch fa-lg fa-spin"></i> <b>`;
                body = spinner + body;
            }
            let notificationClass: string;
            switch (notificationType) {
                case NotificationType.Success:
                    notificationClass = "alert-success";
                    break;
                case NotificationType.Info:
                    notificationClass = "alert-info";
                    break;
                case NotificationType.Warning:
                    notificationClass = "alert-warning";
                    break;
                case NotificationType.Danger:
                    notificationClass = "alert-danger";
                    break;

                default:
                    notificationClass = "alert-info";
            }
            if (isDismissable) {
                notificationClass += " alert-dismissible fade show";
            }
            let opening = `<div class="alert ${notificationClass}">`;
            if (isDismissable) {
                opening +=
                    `<button type="button" class="close" data-dismiss="alert" aria-label="Close"><span aria-hidden="true">&times;</span></button>`;
            }
            if (notificationConfiguration.retryCallback) {
                body +=
                    `<button type="button" class="btn notification-alert-refresh-button retry-notificatoin-selector" aria-label="Retry">
                    <span aria-hidden="true"><i class="fas fa-sync"></i>${notificationConfiguration.retryText}</span>
                    </button>`;
            }
            const closing = `</b></div>`;
            const notificationHtml = $(opening + body + closing);

            let timeoutVariable = null;
            if (drawDelay !== null) {
                timeoutVariable = setTimeout(() => {
                    notificationContainer.append(notificationHtml);
                },
                    drawDelay);
            } else {
                notificationContainer.append(notificationHtml);
            }

            if (notificationConfiguration.retryCallback) {
                notificationContainer.find(".retry-notificatoin-selector").on("click",
                    () => {
                        notificationConfiguration.retryCallback();
                    });
            }

            const notificationResult: INotification = {
                timeoutVariable,
                notificationHtml,
            };

            return notificationResult;
        }

        public destroyNotification(notificationContainer: JQuery, notificationTimeout: number = null) {
            if (notificationTimeout !== null) {
                clearTimeout(notificationTimeout);
            }

            const alertBox = notificationContainer.find(".alert");
            alertBox.remove();
        }
    }

    export interface INotificationConfiguration {
        notificationType: NotificationType;
        body: string;
        isDismissible?: boolean;
        hasSpinner?: boolean;
        container?: JQuery;
        retryCallback?: () => void;
        retryText?: string;
        drawDelay?: number;
    }

    export interface INotification {
        timeoutVariable: number;
        notificationHtml: JQuery;
    }

    export enum NotificationType {
        Success,
        Info,
        Warning,
        Danger,
    }
}
