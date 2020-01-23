// vars
let domInputField, domBtn;
// Functions
const showResultFromApi = function(data) {
  if (data["id"] == "ERROR") {
    alert(data["error_message"]);
  } else {
    alert("Je ontvangt een mail met daaring een nieuw wachtwoord");
    domInputField.value = "";
  }
};
// Check if the inputfield is filled in
const checkInputField = function() {
  const mail = domInputField.value;
  if (mail.length > 0) {
    // Send the mail to the API
    getAPI(
      `user/password/${encodeURIComponent(mail)}`,
      showResultFromApi,
      "PUT"
    );
  } else {
    alert("Je moet een mailadres invullen");
  }
};
// Function that loads the dom-elements
const loadDom = function() {
  domInputField = document.querySelector(".js-input-mail");
  domBtn = document.querySelector(".js-btn");
  // Check if the user clicks on the btn
  domBtn.addEventListener("click", function() {
    checkInputField();
  });
};
document.addEventListener("DOMContentLoaded", function() {
  console.log("Wijzig je wachtwoord🤗");
  loadDom();
});
