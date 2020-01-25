// Vars
let domUserInputs = {},
  domBtn,
  domSVGHome;
let cookieId, userId;
// Fucntions
// Function that clear the inputfields
const clearInputFields = function() {
  domUserInputs["surname"].value = "";
  domUserInputs["name"].value = "";
  domUserInputs["mail"].value = "";
  domUserInputs["password"].value = "";
  domUserInputs["password2"].value = "";
};
// Show the response from the server
const showResponse = function(data) {
  console.log(data);
  // Check if the changes were successful
  if (data["succeeded"]) {
    alert("Dit account is succesvol toegevoegd");
    // Clear the input-fields
    clearInputFields();
  } else {
    alert(data["message"]);
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
// Fucntion that gets the data from the inputfields
const getDataFromInputfields = function() {
  const userInfo = {
    surname: domUserInputs["surname"].value,
    name: domUserInputs["name"].value,
    mail: domUserInputs["mail"].value,
    password: domUserInputs["password"].value
  };
  // Check if all the fields are filled in
  if (
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
        "POST",
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
// Function that loads the DOM
const loadDom = function() {
  domUserInputs["surname"] = document.querySelector(".js-input--surname");
  domUserInputs["name"] = document.querySelector(".js-input--name");
  domUserInputs["mail"] = document.querySelector(".js-input--mail");
  domUserInputs["password"] = document.querySelector(".js-input--password");
  domUserInputs["password2"] = document.querySelector(".js-input--password2");
  domBtn = document.querySelector(".js-btn--change-userinfo");
  domSVGHome = document.querySelector(".js-svg--home");
  // Check if the user has clicked on the button
  domBtn.addEventListener("click", function() {
    getDataFromInputfields();
  });
  //Check if the user has clicked on the home-btn
  domSVGHome.addEventListener('click', function(){
    // Go to the main-page
    window.location.href = "./leerkrachtenvragen.html";
  });
};
document.addEventListener("DOMContentLoaded", function() {
  console.log("Je kan een gebruiker toevoegen😲");
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
