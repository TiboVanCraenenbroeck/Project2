let domUsers;
let cookieId;
const loadUsers = function(data) {
  console.log(data);
  let outputHTML = "";
  for (const user of data) {
    outputHTML += `<div><span>${user["surname"]}</span><span>${user["name"]}</span><span>${user["mail"]}</span></div>`;
  }
  domUsers.innerHTML = outputHTML;
};
const loadDom = function() {
  domUsers = document.querySelector(".js-users");
  // Load all the users form the API
  getAPI(`users?cookie_id=${cookieId}`, loadUsers);
};
document.addEventListener("DOMContentLoaded", function() {
  console.log("Script leerkrachten geladen😛");
  // Check if the user is logged in
  if (checkIfUserIsLoggedIn()) {
    cookieId = getCookie("id");
    // Load the DOM
    loadDom();
  } else {
    // Send the user to the main-page
    window.location.href = "./";
  }
});
