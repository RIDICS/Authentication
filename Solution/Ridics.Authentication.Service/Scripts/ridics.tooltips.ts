$(document.documentElement).ready(() => {
    const elWithTooltips = $(".tooltip-help");
    elWithTooltips.tooltip({
        html: true,
        placement: "right",
    });
});
