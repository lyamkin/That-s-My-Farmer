// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

$(document).ready(function () {
  // Confirmation code to create Farmer login
  // Display message and checkbox when user wants to create farmer login. Disable save button
  $("#isFarmerLoginTypeChecked").click(function () {
    $("#farmerRegistratioConfirmation").toggle(this.checked);
    if ($(this).is(":checked")) {
      $("#registerSaveBtn").prop("disabled", true);
    } else {
      $("#registerSaveBtn").prop("disabled", false);
      $("#farmLoginConfirmation").prop("checked", false);
    }
  });

  // Confiramtion when user wants to create farmer login. Enable Button
  $("#farmLoginConfirmation").click(function () {
    if ($(this).is(":checked")) {
      $("#registerSaveBtn").prop("disabled", false);
    } else {
      $("#registerSaveBtn").prop("disabled", true);
    }
  });
});
