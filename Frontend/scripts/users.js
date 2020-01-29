let domUsers, domBtnsDeleteUser;
let cookieId;
const showResponseFromServer = function(data) {
  /* console.log(data); */
  // Check if the user is deleted
  if (data["id"]) {
    alert("De gebruiker is succesvol verwijderd");
  } else {
    alert(data["error_message"]);
  }
  // Load the users from the API
  getAPI(`users?cookie_id=${encodeURIComponent(cookieId)}`, loadUsers);
};
const loadUsers = function(data) {
  let outputHTML = "";
  for (const user of data) {
    outputHTML += `<div class="c-user"><span>${user["surname"]}</span><span>${user["name"]}</span><span>${user["mail"]}</span><button class="c-btn c-btn__delete-user js-btn-deleteUser" data-userId="${user["id"]}">X</button></div>`;
  }
  domUsers.innerHTML = outputHTML;
  // Get all btns
  domBtnsDeleteUser = document.querySelectorAll(".js-btn-deleteUser");
  // Check if the user has clicked on the btn
  for (const domBtn of domBtnsDeleteUser) {
    domBtn.addEventListener("click", function() {
      // Delete the user in the database (API)
      getAPI(
        `user/${domBtn.getAttribute(
          "data-userId"
        )}?cookie_id=${encodeURIComponent(cookieId)}`,
        showResponseFromServer,
        "delete"
      );
    });
  }
};
const loadDom = function() {
  domUsers = document.querySelector(".js-users");
  // Load all the users form the API
  getAPI(`users?cookie_id=${encodeURIComponent(cookieId)}`, loadUsers);
};

const buttontoevoegen = function() {
  let btntvg = document.querySelector(".js-addUser");
  btntvg.addEventListener("click", function() {
    document.location.href = "AddUser.html";
  });
};

const buttonwijzigen = function(){
  let btnwijzig = document.querySelector('.js-changeuser');
  btnwijzig.addEventListener("click", function(){
    document.location.href = "usersettings.html";
  })
}

document.addEventListener("DOMContentLoaded", function() {
  console.log("Script leerkrachten geladen😛");
  // Check if the user is logged in
  if (checkIfUserIsLoggedIn()) {
    cookieId = getCookie("id");
    // Load the DOM
    loadDom();
    buttontoevoegen();
    buttonwijzigen();
  } else {
    // Send the user to the main-page
    window.location.href = "./";
  }
});
