// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

$( document ).ready(function() {
    $('.validation-summary-errors ul').addClass('govuk-list govuk-error-summary__list');

    $('.input-validation-error').closest("div.govuk-form-group").addClass('govuk-form-group--error');
    $('.input-validation-error').addClass('govuk-input--error');
    $('select.input-validation-error').addClass('govuk-select--error');
});
