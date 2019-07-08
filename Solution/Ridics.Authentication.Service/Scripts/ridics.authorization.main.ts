namespace AuthorizationService {
    $(document.documentElement).ready(() => {
        const main = new Main(".modal.confirmModal");
        main.init();

        $(".history-back-btn").click((event) => {
            event.preventDefault();
            history.back();
        });
    });

    class Main {
        constructor(private readonly modalSelector: string) {
        }

        public init() {
            $(this.modalSelector).each(
                (index, value) => {
                    const $value = $(value);

                    $(".btn-ok", $value).on("click",
                        (event) => {
                            const data = $(event.currentTarget as Node as HTMLElement).data(); // TODO add interface
                            $(`#${data.formid}`).submit();
                        });

                    $value.on("show.bs.modal",
                        (event: any) => { // TODO find concrete type instead of any
                            const data =
                                $(event.relatedTarget as Node as HTMLElement).data() as any; // TODO add interface

                            $(".modal-body-info", $value).text(data.info);
                            $(".btn-ok", $value).data("formid", data.formid);
                        });
                },
            );
        }
    }
}
