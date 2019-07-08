namespace AuthorizationService {
    $.validator.addMethod("customuri",
        (value, element, parameters) => {
            const pattern = /https?:\/\/(www\.)?[-a-zA-Z0-9@:%._\+~#=]{2,256}\b([-a-zA-Z0-9@:%_\+.~#?&//=]*)/g;
            return pattern.test(value);
        });

    $.validator.unobtrusive.adapters.add("customuri", [], (options) => {
        options.rules.customuri = {};
        options.messages.customuri = options.message;
    });
}
