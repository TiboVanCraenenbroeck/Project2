let domUserInputs = {},
  domBtn;
let cookieId, userId;
// Functions
// Function that shows the response from the api tot the user
const showResponse = function(data) {
  // Check if the changes were successful
  if (data["id"]) {
    alert("Je gegevens zijn succesvolg gewijzigd");
  } else {
    alert(data["error_message"]);
  }
  // Reset the password-field
  domUserInputs["password"].value = "";
  domUserInputs["password2"].value = "";
};
// Function that checks of the password is filled in + of both passwords are the same
const checkIfPasswordIsValid = function() {
  if (domUserInputs["password"].value.length > 0) {
    if (domUserInputs["password"].value == domUserInputs["password2"].value) {
      // Check if the password math with the regex
      if (
        domUserInputs["password"].value.match(
          "^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#.?!@$%^&*-]).{8,}$"
        )
      ) {
        return true;
      } else {
        alert(
          "Je wachtwoord moet minstens 8 karakters, 1 nummer, 1 hoofdletter, 1 gewone letter en een speciaal teken (bijvoorbeeld: .?) bevatten"
        );
        return false;
      }
    } else {
      alert("Beide wachtwoorden komen niet overeen");
      return false;
    }
  } else {
    return true;
  }
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
    // Check if the password is filled in
    if (checkIfPasswordIsValid()) {
      // Send the userinfo to the database
      getAPI(
        `user?cookie_id=${encodeURIComponent(cookieId)}`,
        showResponse,
        "PUT",
        JSON.stringify(userInfo)
      );
    } else {
      domUserInputs["password"].value = "";
      domUserInputs["password2"].value = "";
    }
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
  domUserInputs["password2"] = document.querySelector(".js-input--password2");
  domBtn = document.querySelector(".js-btn--change-userinfo");
  getAPI(
    `user?cookie_id=${encodeURIComponent(cookieId)}`,
    showInfoOfTheCurrentUser
  );
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
