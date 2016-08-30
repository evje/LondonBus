$(function livesearching() {
    $("[data-autocomplete-source]").each(function livesearching() {
        var target = $(this);
        target.autocomplete({ source: target.attr("data-autocomplete-source") });
    });
});