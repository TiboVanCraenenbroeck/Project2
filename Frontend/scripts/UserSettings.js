let domUserInputs = {},
  domBtn;
let cookieId, userId;
// Functions
// Function that shows the response from the api tot the user
const showResponse = function(data) {
  console.log(data);
};
// Function that gets the data from the userinput
const getUserinfoFromInputfields = function() {
  const userInfo = {
    id: userId,
    surname: domUserInputs["surname"].value,
    name: domUserInputs["name"].value,
    mail: domUserInputs["mail"].value,
    password: domUserInputs["password"].value
  };
  // Check if all the fields are filled in
  if (
    userInfo["id"].length > 0 &&
    userInfo["surname"].length > 0 &&
    userInfo["name"].length > 0 &&
    userInfo["mail"].length > 0
  ) {
    // Send the userinfo to the database
    getAPI(
      `user?cookie_id=${encodeURIComponent(cookieId)}`,
      showResponse,
      "PUT",
      JSON.stringify(userInfo)
    );
    // IN BACKEND --> ERVOOR ZORGEN DAT HET WACHTWOORD NIET ELKE KEER INGEVULD MOET WORDEN!!!
  } else {
    alert("Je moet alle velden invullen");
  }
};
// Function that gets the userinfo of the current user
const showInfoOfTheCurrentUser = function(data) {
  // Set the data to the inputfields
  userId = data["id"];
  domUserInputs["surname"].value = data["surname"];
  domUserInputs["name"].value = data["name"];
  domUserInputs["mail"].value = data["mail"];
  // Check if the user clicked on the button
  domBtn.addEventListener("click", function() {
    getUserinfoFromInputfields();
  });
};
// Function that loads the DOM
const loadDom = function() {
  domUserInputs["surname"] = document.querySelector(".js-input--surname");
  domUserInputs["name"] = document.querySelector(".js-input--name");
  domUserInputs["mail"] = document.querySelector(".js-input--mail");
  domUserInputs["password"] = document.querySelector(".js-input--password");
  domBtn = document.querySelector(".js-btn--change-userinfo");
  getAPI(`user?cookie_id=${cookieId}`, showInfoOfTheCurrentUser);
};
document.addEventListener("DOMContentLoaded", function() {
  console.log("Wijzigen maar!!!😀");
  // Check if the user is logged in
  if (checkIfUserIsLoggedIn()) {
    // Get the cookieId
    cookieId = getCookie("id");
    loadDom();
  } else {
    // Send the user to the main-page
    window.location.href = "./";
  }
});
