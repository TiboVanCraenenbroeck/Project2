let domUserInputs = {},
  domBtn;
// Functions
// Function that gets the userinfo of the current user
const showInfoOfTheCurrentUser = function(data) {
  console.log(data);
};
// Function that loads the DOM
const loadDom = function() {
  domUserInputs["surname"] = document.querySelector(".js-input--surname");
  domUserInputs["name"] = document.querySelector(".js-input--name");
  domUserInputs["mail"] = document.querySelector(".js-input--mail");
  domUserInputs["password"] = document.querySelector(".js-input--password");
  domBtn = document.querySelector(".js-btn--change-userinfo");
};
document.addEventListener("DOMContentLoaded", function() {
  console.log("Wijzigen maar!!!😀");
  loadDom();
  
});
